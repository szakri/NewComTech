using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Measurements
{
    public class Root
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public Students students { get; set; }
    }

    public class Students
    {
        public List<object> nodes { get; set; }
        public PageInfo pageInfo { get; set; }
    }

    public class PageInfo
    {
        public string endCursor { get; set; }
    }

    /*public class Measurement
    {
        public double responseTime;
        public int size;

        public override string ToString()
        {
            return $"Response time: {responseTime}, Size: {size}";
        }
    }*/

    class Program
    {
        private const string REST_BASE_URL = "https://localhost:44337/api/";
        private const string ODATA_BASE_URL = "https://localhost:44349/odata/";
        private const string GRAPHQL_BASE_URL = "https://localhost:44378/graphql/";

        static async Task Main(string[] args)
        {
            string[] restURLs = new string[]
            {
                "students/1/QR",
                "students/withCourses?pageSize=50&orderBy=name desc&filterBy=name.Contains(\"á\") or name.Contains(\"é\")"
            };
            string[] odataURLs = new string[]
            {
                "students/GetQRCode(studentId=1)",
                "students?$top=50&$orderby=name desc&$select=name&$filter=contains( name,'á') or contains( name,'é')&$expand=Courses($select=name)"
            };
            string[] graphQLQueries = new string[]
            {
                @"query{
	                student(id: 1){
		                qRCode
	                }
                }",
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
                }"
            };

            /*long time = await measureHTTPGet(REST_BASE_URL + restURLs[0]);
            Console.WriteLine(time);*/

            long time = await measureHTTPGet(ODATA_BASE_URL + odataURLs[0]);
            Console.WriteLine(time);

            /*time = await measureHTTPPost(GRAPHQL_BASE_URL, graphQLQueryToJSON(graphQLQueries[0]));
            Console.WriteLine(time);*/

            /*Console.WriteLine("REST");
            foreach (var item in restTimes)
            {
                Console.WriteLine($"Response time: {item}");
            }
            double average = restTimes.Average();
            double sum = restTimes.Sum(d => Math.Pow(d - average, 2));
            double stdDev = Math.Sqrt((sum) / (restTimes.Count() - 1));
            Console.WriteLine($"Avg: {average}");
            Console.WriteLine($"Standard deviation: {stdDev}");
            Console.WriteLine();

            Console.WriteLine("OData");
            foreach (var item in odataTimes)
            {
                Console.WriteLine($"Response time: {item}");
            }
            average = odataTimes.Average();
            sum = odataTimes.Sum(d => Math.Pow(d - average, 2));
            stdDev = Math.Sqrt((sum) / (odataTimes.Count() - 1));
            Console.WriteLine($"Avg: {average}");
            Console.WriteLine($"Standard deviation: {stdDev}");
            Console.WriteLine();

            Console.WriteLine("GraphQL");
            foreach (var item in graphqlTimes)
            {
                Console.WriteLine($"Response time: {item}");
            }
            average = graphqlTimes.Average();
            sum = graphqlTimes.Sum(d => Math.Pow(d - average, 2));
            stdDev = Math.Sqrt((sum) / (graphqlTimes.Count() - 1));
            Console.WriteLine($"Avg: {average}");
            Console.WriteLine($"Standard deviation: {stdDev}");*/
        }

        static string graphQLQueryToJSON(string query)
        {
            return "{ \"query\" : \"" + query + "\" }";
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
