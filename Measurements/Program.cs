using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient.Protos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Measurements
{
    class Measurement
    {
        public List<string> URLs { get; }
        public int IterationNumber { get; }
        public int? IterationFrom { get; }
        public bool IsPOST { get; }
        public List<string> Bodies { get; }
        public List<long> ResponseTimes { get; }
        public List<int> ResponseSizes { get; }

        public Measurement(List<string> URLs, int iterationNumber = 1, int? iterationFrom = null)
        {
            this.URLs = URLs;
            this.IterationNumber = iterationNumber;
            this.IterationFrom = iterationFrom;
            this.IsPOST = false;
            this.ResponseTimes = new List<long>();
            this.ResponseSizes = new List<int>();
        }

        public Measurement(string URL, int iterationNumber = 1, int? iterationFrom = null)
        {
            this.URLs = new List<string>();
            this.URLs.Add(URL);
            this.IterationNumber = iterationNumber;
            this.IterationFrom = iterationFrom;
            this.IsPOST = false;
            this.ResponseTimes = new List<long>();
            this.ResponseSizes = new List<int>();
        }

        public Measurement(List<string> URLs, List<string> bodies, int iterationNumber = 1, int? iterationFrom = null)
        {
            this.URLs = URLs;
            this.IterationNumber = iterationNumber;
            this.IterationFrom = iterationFrom;
            this.IsPOST = true;
            this.Bodies = bodies;
            this.ResponseTimes = new List<long>();
            this.ResponseSizes = new List<int>();
        }

        public Measurement(string URL, string body, int iterationNumber = 1, int? iterationFrom = null)
        {
            this.URLs = new List<string>();
            this.URLs.Add(URL);
            this.IterationNumber = iterationNumber;
            this.IterationFrom = iterationFrom;
            this.IsPOST = true;
            this.Bodies = new List<string>();
            this.Bodies.Add(body);
            this.ResponseTimes = new List<long>();
            this.ResponseSizes = new List<int>();
        }

        public double GetAverageTime()
        {
            return ResponseTimes.Average();
        }

        public double GetStdDevTime()
        {
            double sum = ResponseTimes.Sum(d => Math.Pow(d - GetAverageTime(), 2));
            return Math.Sqrt((sum) / (ResponseTimes.Count - 1));
        }
    }

    class Response
    {
        public string Body { get; set; }
        public long Time { get; set; }

        public long GetSize()
        {
            return string.IsNullOrEmpty(Body) ? 0: Encoding.Unicode.GetByteCount(Body);
        }
    }

    public class Course
    {
        public int courseId { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int day { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Student
    {
        public int studentId { get; set; }
        public string neptun { get; set; }
        public string name { get; set; }
        public string dayOfBirth { get; set; }
        public List<Course> courses { get; set; }
    }

    public class Attendance
    {
        public int attendanceId { get; set; }
        public Course course { get; set; }
        public Student student { get; set; }
        public string date { get; set; }
        public string checkInTime { get; set; }
        public string checkOutTime { get; set; }
    }

    class Program
    {
        private const string REST_BASE_URL = "https://localhost:44337/api/";
        private const string ODATA_BASE_URL = "https://localhost:44349/odata/";
        private const string GRPC_BASE_URL = "https://localhost:5001";
        private const string GRAPHQL_BASE_URL = "https://localhost:44378/graphql/";

        static async Task Main(string[] args)
        {
            int queryNum = 1;
            string filter = "";
            Response res;
            Response res2;
            string currentDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;
            string file = currentDirectory + "\\Measurements.txt";
            File.AppendAllText(file, DateTime.UtcNow.ToString() + "\n");

            // The first request is dropped because of 'warm up'
            await measureHTTPGet(REST_BASE_URL + "students/1");
            write("REST\n", file);

            write($"\tQuery {queryNum++}: ", file);
            Measurement m1 = new Measurement(REST_BASE_URL + "students?pageSize=100");
            res = await measureHTTPGet(m1.URLs[0]);
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            Measurement m2 = new Measurement(REST_BASE_URL + "students/1/QR");
            res = await measureHTTPGet(m2.URLs[0]);
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            Measurement m3 = new Measurement(REST_BASE_URL + "students/withCourses?pageSize=50&orderBy=name desc&filterBy=name.Contains(\"á\") or name.Contains(\"é\")");
            res = await measureHTTPGet(m3.URLs[0]);
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            Measurement m4 = new Measurement(
                new List<string> {
                    REST_BASE_URL + "students/withCourses?pageSize=50&orderBy=name desc&filterBy=name.Contains(\"á\") or name.Contains(\"é\")",
                    REST_BASE_URL + "courses?pageSize=100&filterBy="
                }
            );
            res = await measureHTTPGet(m4.URLs[0]);
            List<Student> students = JsonConvert.DeserializeObject<List<Student>>(res.Body);
            // The query string would be too long, so we have to split the query into multiple
            foreach (var student in students)
            {
                filter = "";
                for (int j = 0; j < student.courses.Count; j++)
                {
                    filter += $"courseId={student.courses[j].courseId}";
                    if (j < student.courses.Count - 1)
                    {
                        filter += " or ";
                    }
                }
                res2 = await measureHTTPGet(m4.URLs[1] + filter);
                res.Time += res2.Time;
                res.Body += res2.Body;
            }
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            filter = "";
            Measurement m5 = new Measurement(
                    new List<string> {
                        REST_BASE_URL + "attendances?pageSize=100&filterBy=checkInTime>\"10:00:00\" and checkInTime<\"11:30:00\"",
                        REST_BASE_URL + "courses/withSubject?pageSize=100&filterBy="
                    }
                );
            res = await measureHTTPGet(m5.URLs[0]);
            List<Attendance> attendances = JsonConvert.DeserializeObject<List<Attendance>>(res.Body);
            for (int i = 0; i < attendances.Count; i++)
            {
                filter += $"courseId={attendances[i].course.courseId}";
                if (i < attendances.Count - 1)
                {
                    filter += " or ";
                }
            }
            res2 = await measureHTTPGet(m5.URLs[1] + filter);
            res.Time += res2.Time;
            res.Body += res2.Body;
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            Measurement m6 = new Measurement(REST_BASE_URL + "courses/withSubject?pageSize=5&pageNumber=#", 5, 1);
            for (int i = 0; i < m6.IterationNumber; i++)
            {
                res = await measureHTTPGet(m3.URLs[0]);
                write($"({res.Time} ms, {res.GetSize()} B) ", file);
            }
            write("\n", file);


            Measurement[] odata = new Measurement[] {
                new Measurement(ODATA_BASE_URL + "students?$top=100"),
                new Measurement(ODATA_BASE_URL + "students/GetQRCode(studentId=1)"),
                new Measurement(ODATA_BASE_URL + "students?$top=50&$orderby=name desc&$select=name&$filter=contains( name,'á') or contains( name,'é')&$expand=Courses($select=name)"),
                new Measurement(ODATA_BASE_URL + "students?$top=50&$orderby=name desc&$select=name&$filter=contains( name,'á') or contains( name,'é')&$expand=Courses($select=name;$expand=Subject($select=name))"),
                new Measurement(ODATA_BASE_URL + "attendances?$top=100&$expand=Student($select=name),Course($expand=Subject;$select=Subject)&$select=Student,Course"),
                new Measurement(ODATA_BASE_URL + "courses?top=5&$select=name&$expand=Subject($select=name)&$skip=#", 5, 1)
            };
            // The first request is dropped because of 'warm up'
            await measureHTTPGet(ODATA_BASE_URL + "students(1)");

            write("OData\n", file);
            queryNum = 1;
            foreach (var item in odata)
            {
                write($"\tQuery {queryNum}: ", file);
                queryNum++;
                for (int i = 0; i < item.IterationNumber; i++)
                {
                    if (item.IterationFrom != null)
                    {
                        foreach (var url in item.URLs)
                        {
                            res = await measureHTTPGet(url.Replace("#", (item.IterationFrom + i).ToString()));
                            write($"({res.Time} ms, {res.GetSize()} B) ", file);
                        }
                    }
                    else
                    {
                        res = await measureHTTPGet(item.URLs[0]);
                        write($"({res.Time} ms, {res.GetSize()} B)", file);
                    }
                }
                write("\n", file);
            }


            Measurement[] graphql = new Measurement[]
            {
                new Measurement(GRAPHQL_BASE_URL,
                @"query{
                    students(first: 100){
                        nodes{
                          studentId,
                            name,
                            dayOfBirth,
                            neptun
                        }
                    }
                }"),
                new Measurement(GRAPHQL_BASE_URL,
                    @"query{
                        student(id: 1){
                            qRCode
                        }
                    }"),
                new Measurement(GRAPHQL_BASE_URL,
                    @"query{
                        students(order: { name: DESC},
                            where: {
                                or: [
                                    { name: { contains: \""é\"" } },
                                    { name: { contains: \""á\"" } }
                                ]
                            },
                            first: 50
                        ){
                            nodes{
                                name,
                                courses{
                                    name
                                }
                            }
                        }
                    }"),
                new Measurement(GRAPHQL_BASE_URL,
                    @"query{
                        students(order: { name: DESC},
                            where: {
                                or: [
                                    { name: { contains: \""é\"" } },
                                    { name: { contains: \""á\"" } }
                                ]
                            },
                            first: 50
                        ){
                            nodes{
                                name,
                                courses{
                                    name,
                                    subject{
                                      name
                                    }
                                }
                            }
                        }
                    }"),
                new Measurement(GRAPHQL_BASE_URL,
                    @"query{
                        attendances{
                            nodes{
                                student{
                                    name
                                },
                                course{
                                    subject{
                                        name
                                    }
                                }
                            }
                        }
                    }")
                /*new Measurement(GRAPHQL_BASE_URL,
                    @"query{
                        courses(first: 5, offset:#){
                            name
                            subject{
                                name
                            }
                        }
                    }", 5, 0)*/
            };

            // The first request is dropped because of 'warm up'
            await measureHTTPPost(GRAPHQL_BASE_URL, graphQLQueryToJSON("query{ student(id: 1){ name } }"));

            write("GraphQL\n", file);
            queryNum = 1;
            foreach (var item in graphql)
            {
                write($"\tQuery {queryNum}: ", file);
                queryNum++;
                for (int i = 0; i < item.IterationNumber; i++)
                {
                    if (item.IterationFrom != null)
                    {
                        foreach (var body in item.Bodies)
                        {
                            res = await measureHTTPPost(GRAPHQL_BASE_URL, graphQLQueryToJSON(body.Replace("#", (item.IterationFrom + i).ToString())));
                            write($"({res.Time} ms, {res.GetSize()} B) ", file);
                        }
                    }
                    else
                    {
                        res = await measureHTTPPost(GRAPHQL_BASE_URL, graphQLQueryToJSON(item.Bodies[0]));
                        write($"({res.Time} ms, {res.GetSize()} B)", file);
                    }
                }
                write("\n", file);
            }


            GrpcChannel channel = GrpcChannel.ForAddress(GRPC_BASE_URL);
            var studentClient = new Students.StudentsClient(channel);
            var courseClient = new Courses.CoursesClient(channel);
            var attendanceClient = new Attendances.AttendancesClient(channel);
            Stopwatch stopWatch = new Stopwatch();

            // The first request is dropped because of 'warm up'
            await studentClient.GetStudentAsync(new ID { Value = 1 });
            
            write("gRPC\n", file);
            queryNum = 1;

            write($"\tQuery {queryNum++}: ", file);
            stopWatch.Restart();
            var studentQR = await studentClient.GetStudentQRCodeAsync(new ID { Value = 1 });
            stopWatch.Stop();
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            stopWatch.Restart();
            using (var call = studentClient.GetStudents(new QueryParams { PageSize = 100 }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var a = call.ResponseStream.Current;
                }
            }
            stopWatch.Stop();
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            stopWatch.Restart();
            using (var call = studentClient.GetStudentsWithCourses(new QueryParams { PageSize = 5, OrderBy = "name desc", FilterBy = "name.Contains(\"á\") or name.Contains(\"é\")" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var a = call.ResponseStream.Current;
                }
            }
            stopWatch.Stop();
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            List<int> ids = new List<int>();
            stopWatch.Restart();
            using (var call = studentClient.GetStudentsWithCourses(new QueryParams { PageSize = 5, OrderBy = "name desc", FilterBy = "name.Contains(\"á\") or name.Contains(\"é\")" }))
            {
                
                while (await call.ResponseStream.MoveNext())
                {
                    var a = call.ResponseStream.Current;
                    stopWatch.Stop();
                    ids.AddRange(a.Courses.Select(c => c.CourseId).ToList());
                    stopWatch.Start();
                }
            }
            stopWatch.Stop();
            filter = "";
            for (int i = 0; i < ids.Count; i++)
            {
                filter += $"courseId={ids[i]}";
                if (i < ids.Count - 1)
                {
                    filter += " or ";
                }
            }
            stopWatch.Start();
            using (var call = courseClient.GetCoursesWithSubject(new QueryParams { PageSize = 100, FilterBy = filter }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var a = call.ResponseStream.Current;
                }
            }
            stopWatch.Stop();
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            stopWatch.Restart();
            ids = new List<int>();
            using (var call = attendanceClient.GetAttendances(
                new QueryParams { PageSize = 100, FilterBy = "checkInTime>\"10:00:00\" and checkInTime<\"11:30:00\"" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var a = call.ResponseStream.Current;
                    stopWatch.Stop();
                    ids.Add(a.Course.CourseId);
                    stopWatch.Start();
                }
            }
            stopWatch.Stop();
            filter = "";
            for (int i = 0; i < ids.Count; i++)
            {
                filter += $"courseId={ids[i]}";
                if (i < ids.Count - 1)
                {
                    filter += " or ";
                }
            }
            stopWatch.Start();
            using (var call = courseClient.GetCoursesWithSubject(new QueryParams { PageSize = 100, FilterBy = filter }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var a = call.ResponseStream.Current;
                }
            }
            stopWatch.Stop();
            write($"({res.Time} ms, {res.GetSize()} B)\n", file);

            write($"\tQuery {queryNum++}: ", file);
            for (int i = 0; i < 5; i++)
            {
                stopWatch.Restart();
                using (var call = courseClient.GetCoursesWithSubject(new QueryParams { PageSize = 5, PageNumber = i + 1 }))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var a = call.ResponseStream.Current;
                    }   
                }
                stopWatch.Stop();
                write($"({res.Time} ms, {res.GetSize()} B) ", file);
            }
            write("\n", file);
        }

        static void write(string text, string file)
        {
            Console.Write(text);
            File.AppendAllText(file, text);
        }
        
        static string graphQLQueryToJSON(string query)
        {
            return Regex.Replace("{ \"query\" : \"" + query.Replace("\r", "").Replace("\n", "") + "\" }", @"\s+", " ");
        }

        static async Task<Response> measureHTTPGet(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            Stopwatch stopWatch = new Stopwatch();
            string responseAsString;

            stopWatch.Restart();
            responseAsString = await client.GetAsync(url).Result.Content.ReadAsStringAsync();
            stopWatch.Stop();

            client.Dispose();

            return new Response { Body = responseAsString, Time = stopWatch.ElapsedMilliseconds };
        }

        static async Task<Response> measureHTTPPost(string url, string body)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            Stopwatch stopWatch = new Stopwatch();
            string responseAsString;

            stopWatch.Restart();
            HttpResponseMessage response = await client.PostAsync(url, new StringContent(
                body, Encoding.UTF8, "application/json"));
            responseAsString = await response.Content.ReadAsStringAsync();
            stopWatch.Stop();

            client.Dispose();

            return new Response { Body = responseAsString, Time = stopWatch.ElapsedMilliseconds };
        }
    }
}