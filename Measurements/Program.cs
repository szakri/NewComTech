using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Measurements
{
    class Measurement
    {
        public string URL { get; set; }
        public int IterationNumber { get; set; }
        public int? IterationFrom { get; set; }
        public bool IsPOST { get; set; }
        public string Body { get; set; }

        public Measurement(string URL, int iterationNumber = 1, int? iterationFrom = null)
        {
            this.URL = URL;
            this.IterationNumber = iterationNumber;
            this.IterationFrom = iterationFrom;
            this.IsPOST = false;
        }

        public Measurement(string URL, string body, int iterationNumber = 1, int? iterationFrom = null)
        {
            this.URL = URL;
            this.IterationNumber = iterationNumber;
            this.IterationFrom = iterationFrom;
            this.IsPOST = true;
            this.Body = body;
        }
    }

    class Program
    {
        private const string REST_BASE_URL = "https://localhost:44337/api/";
        private const string ODATA_BASE_URL = "https://localhost:44349/odata/";
        private const string GRAPHQL_BASE_URL = "https://localhost:44378/graphql/";
        static List<long> restTimes = new List<long>();
        static List<long> odataTimes = new List<long>();
        static List<long> graphQLTimes = new List<long>();

        static async Task Main(string[] args)
        {
            Measurement[] rest = new Measurement[] {
                new Measurement(REST_BASE_URL + "students/1/QR"),
                new Measurement(REST_BASE_URL + "students/withCourses?pageSize=50&orderBy=name desc&filterBy=name.Contains(\"á\") or name.Contains(\"é\")"),
                new Measurement(REST_BASE_URL + "courses/withSubject?pageSize=5&pageNumber=#", 5, 1)
            };
            Measurement[] odata = new Measurement[] {
                new Measurement(ODATA_BASE_URL + "students/GetQRCode(studentId=1)"),
                new Measurement(ODATA_BASE_URL + "students?$top=50&$orderby=name desc&$select=name&$filter=contains( name,'á') or contains( name,'é')&$expand=Courses($select=name)"),
                //new Measurement(ODATA_BASE_URL + "students?$top=50&$orderby=name desc&$select=name&$filter=contains( name,'á') or contains( name,'é')&$expand=Courses($select=name;$expand=Subject($select=name))"),
                new Measurement(ODATA_BASE_URL + "courses?top=5&$select=name&$expand=Subject($select=name)&$skip=#", 5, 0)
            };
            Measurement[] graphql = new Measurement[]
            {
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
                                    { name: { contains: ""é"" } },
                                    { name: { contains: ""á"" } }
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
                /*new Measurement(GRAPHQL_BASE_URL,
                    @"query{
                        students(order: { name: DESC},
                            where: {
                                or: [
                                    { name: { contains: ""é"" } },
                                    { name: { contains: ""á"" } }
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
                    }"),*/
                new Measurement(GRAPHQL_BASE_URL,
                    @"query{
	                    courses(first: 5, offset:#){
			                name
			                subject{
				                name
			                }
	                    }
                    }", 5, 0)
            };
            foreach (var item in rest)
            {
                for (int i = 0; i < item.IterationNumber; i++)
                {
                    long time;
                    if (item.IterationFrom != null)
                    {
                        time = await measureHTTPGet(item.URL.Replace("#", (item.IterationFrom + i).ToString()));
                    }
                    else
                    {
                        time = await measureHTTPGet(item.URL);
                    }
                    restTimes.Add(time);
                    Console.WriteLine(time);
                }
            }
            foreach (var item in odata)
            {
                for (int i = 0; i < item.IterationNumber; i++)
                {
                    long time;
                    if (item.IterationFrom != null)
                    {
                        time = await measureHTTPGet(item.URL.Replace("#", (item.IterationFrom + i).ToString()));
                    }
                    else
                    {
                        time = await measureHTTPGet(item.URL);
                    }
                    odataTimes.Add(time);
                    Console.WriteLine(time);
                }
            }
            foreach (var item in graphql)
            {
                for (int i = 0; i < item.IterationNumber; i++)
                {
                    long time;
                    if (item.IterationFrom != null)
                    {
                        time = await measureHTTPPost(item.URL, graphQLQueryToJSON(item.Body.Replace("#", (item.IterationFrom + i).ToString())));
                    }
                    else
                    {
                        time = await measureHTTPPost(item.URL, graphQLQueryToJSON(item.Body));
                    }
                    graphQLTimes.Add(time);
                    Console.WriteLine(time);
                }
            }

            /*average = graphQLTimes.Average();
            sum = graphQLTimes.Sum(d => Math.Pow(d - average, 2));
            stdDev = Math.Sqrt((sum) / (graphQLTimes.Count() - 1));*/
        }

        static string graphQLQueryToJSON(string query)
        {
            return Regex.Replace("{ \"query\" : \"" + query.Replace("\r", "").Replace("\n", "") + "\" }", @"\s+", " ");
        }

        static async Task<long> measureHTTPGet(string url)
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
            return stopWatch.ElapsedMilliseconds;
        }

        static async Task<long> measureHTTPPost(string url, string body)
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
            return stopWatch.ElapsedMilliseconds;
        }
    }
}
