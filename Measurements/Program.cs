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
using Microsoft.OData.Client;



namespace Measurements
{
    class Response
    {
        public string Body { get; set; }
        public long Time { get; set; }
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
            string currentDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;
            string file = currentDirectory + "\\Measurements.txt";
            bool runREST = true;
            bool runOData = true;
            bool runGraphQL = true;
            bool runGRPC = true;
            int iterationNumber = 10;
            int queryNum = 1;
            string filter = "";

            if (runREST)
            {
                // The first request is dropped because of 'warm up'
                await measureHTTPGet(REST_BASE_URL + "students/1");
                write("REST\n", file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(REST_BASE_URL + "students?pageSize=100", iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(REST_BASE_URL + "students/1/QR", iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(REST_BASE_URL +
                    "students/withCourses?pageSize=50&orderBy=name desc&filterBy=name.Contains(\"á\") or name.Contains(\"é\")", iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                for (int i = 0; i < iterationNumber; i++)
                {
                    filter = "";
                    Response res = await measureHTTPGet(REST_BASE_URL + "students/withCourses?pageSize=50&orderBy=name desc&filterBy=name.Contains(\"á\") or name.Contains(\"é\")");
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
                        Response res2 = await measureHTTPGet(REST_BASE_URL + "courses?pageSize=100&filterBy=" + filter);
                        res.Time += res2.Time;
                        res.Body += res2.Body;
                    }
                    write($"{res.Time} ms ", file);
                }
                write("\n", file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                for (int i = 0; i < iterationNumber; i++)
                {
                    filter = "";
                    Response res = await measureHTTPGet(REST_BASE_URL + "attendances?pageSize=100&filterBy=checkInTime>\"10:00:00\" and checkInTime<\"11:30:00\"");
                    List<Attendance> attendances = JsonConvert.DeserializeObject<List<Attendance>>(res.Body);
                    for (int j = 0; j < attendances.Count; j++)
                    {
                        filter += $"courseId={attendances[j].course.courseId}";
                        if (j < attendances.Count - 1)
                        {
                            filter += " or ";
                        }
                    }
                    Response res2 = await measureHTTPGet(REST_BASE_URL + "courses/withSubject?pageSize=100&filterBy=" + filter);
                    res.Time += res2.Time;
                    res.Body += res2.Body;
                    write($"{res.Time} ms ", file);
                }
                write("\n", file);
            }

            if (runOData)
            {
                // The first request is dropped because of 'warm up'
                await measureHTTPGet(ODATA_BASE_URL + "students(1)");

                write("OData\n", file);
                queryNum = 1;

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(ODATA_BASE_URL + "students?$top=100", iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(ODATA_BASE_URL + "students/GetQRCode(studentId=1)", iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(ODATA_BASE_URL +
                    "students?$top=50&$orderby=name desc&$select=name&$filter=contains( name,'á') or contains( name,'é')&$expand=Courses($select=name)", iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(ODATA_BASE_URL +
                    "students?$top=50&$orderby=name desc&$select=name&$filter=contains( name,'á') or contains( name,'é')&$expand=Courses($select=name;$expand=Subject($select=name))", iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                await runMultipleTimes(ODATA_BASE_URL +
                    "attendances?$top=100&$expand=Student($select=name),Course($expand=Subject;$select=Subject)&$select=Student,Course", iterationNumber, file);
            }

            if (runGraphQL)
            {
                // The first request is dropped because of 'warm up'
                await measureHTTPPost(GRAPHQL_BASE_URL, graphQLQueryToJSON("query{ student(id: 1){ name } }"));

                write("GraphQL\n", file);
                queryNum = 1;
                string body;

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                body = @"
                query{
                    students(first: 100){
                        nodes{
                            studentId,
                            name,
                            dayOfBirth,
                            neptun
                        }
                    }
                }";
                await runMultipleTimes(GRAPHQL_BASE_URL, graphQLQueryToJSON(body), iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                body = @"
                query{
                    student(id: 1){
                        qRCode
                    }
                }";
                await runMultipleTimes(GRAPHQL_BASE_URL, graphQLQueryToJSON(body), iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                body = @"
                query{
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
                }";
                await runMultipleTimes(GRAPHQL_BASE_URL, graphQLQueryToJSON(body), iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                body = @"
                query{
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
                }";
                await runMultipleTimes(GRAPHQL_BASE_URL, graphQLQueryToJSON(body), iterationNumber, file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                body = @"
                query{
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
                }";
                await runMultipleTimes(GRAPHQL_BASE_URL, graphQLQueryToJSON(body), iterationNumber, file);
            }

            if (runGRPC)
            {
                GrpcChannel channel = GrpcChannel.ForAddress(GRPC_BASE_URL);
                var studentClient = new Students.StudentsClient(channel);
                var courseClient = new Courses.CoursesClient(channel);
                var attendanceClient = new Attendances.AttendancesClient(channel);
                Stopwatch stopWatch = new Stopwatch();

                // The first request is dropped because of 'warm up'
                await studentClient.GetStudentAsync(new ID { Value = 1 });

                write("gRPC\n", file);
                queryNum = 1;

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                for (int i = 0; i < iterationNumber; i++)
                {
                    stopWatch.Restart();
                    using (var call = studentClient.GetStudents(new QueryParams { PageSize = 100 }))
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var _ = call.ResponseStream.Current;
                        }
                    }
                    stopWatch.Stop();
                    write($"{stopWatch.ElapsedMilliseconds} ms ", file);
                }
                write("\n", file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                for (int i = 0; i < iterationNumber; i++)
                {
                    stopWatch.Restart();
                    var _ = await studentClient.GetStudentQRCodeAsync(new ID { Value = 1 });
                    stopWatch.Stop();
                    write($"{stopWatch.ElapsedMilliseconds} ms ", file);
                }
                write("\n", file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                for (int i = 0; i < iterationNumber; i++)
                {
                    stopWatch.Restart();
                    using (var call = studentClient.GetStudentsWithCourses(new QueryParams { PageSize = 5, OrderBy = "name desc", FilterBy = "name.Contains(\"á\") or name.Contains(\"é\")" }))
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var a = call.ResponseStream.Current;
                        }
                    }
                    stopWatch.Stop();
                    write($"{stopWatch.ElapsedMilliseconds} ms ", file);
                }
                write("\n", file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                for (int i = 0; i < iterationNumber; i++)
                {
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
                    for (int j = 0; j < ids.Count; j++)
                    {
                        filter += $"courseId={ids[j]}";
                        if (j < ids.Count - 1)
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
                    write($"{stopWatch.ElapsedMilliseconds} ms ", file);
                }
                write("\n", file);

                write($"\tQuery {queryNum++} {DateTime.Now}: ", file);
                for (int i = 0; i < iterationNumber; i++)
                {
                    List<int> ids = new List<int>();
                    stopWatch.Restart();
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
                    for (int j = 0; j < ids.Count; j++)
                    {
                        filter += $"courseId={ids[j]}";
                        if (j < ids.Count - 1)
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
                    write($"{stopWatch.ElapsedMilliseconds} ms ", file);
                }
                write("\n", file);
            }
        }

        static async Task runMultipleTimes(string url, int iterationNumber, string file)
        {
            for (int i = 0; i < iterationNumber; i++)
            {
                Response res = await measureHTTPGet(url);
                write($"{res.Time} ms ", file);
            }
            write("\n", file);
        }

        static async Task runMultipleTimes(string url, string body, int iterationNumber, string file)
        {
            for (int i = 0; i < iterationNumber; i++)
            {
                Response res = await measureHTTPPost(url, body);
                write($"{res.Time} ms ", file);
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