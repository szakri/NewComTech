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
        private const string REST_URL = "https://localhost:44337/api/";
        private const string ODATA_URL = "https://localhost:44349/odata/";
        private const string GRAPHQL_URL = "https://localhost:44378/graphql/";
        private static HttpClient client;

        static async Task Main(string[] args)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            List<double> restTimes = new List<double>();
            List<double> odataTimes = new List<double>();
            List<double> graphqlTimes = new List<double>();
            string cursor = "";
            int pageSize = 10;
            for (int i = 0; i < 2; i++)
            {
                Stopwatch stopWatch = new Stopwatch();
                string responseAsString;

                stopWatch.Restart();
                responseAsString = await client.GetAsync($"{REST_URL}students/withCourses?pageNumber={i + 1}").Result.Content.ReadAsStringAsync();
                stopWatch.Stop();
                restTimes.Add(stopWatch.Elapsed.TotalMilliseconds);
                /*Console.WriteLine("REST:");
                Console.WriteLine($"\t Response time: {stopWatch.Elapsed}");
                Console.WriteLine($"\t Size: {Encoding.Unicode.GetByteCount(responseAsString)}");*/

                stopWatch.Restart();
                responseAsString = await client
                    .GetAsync($"{ODATA_URL}students?$select=StudentId,%20name&$expand=Courses($select=name)&$skip={i * pageSize}")
                    .Result.Content.ReadAsStringAsync();
                stopWatch.Stop();
                odataTimes.Add(stopWatch.Elapsed.TotalMilliseconds);
                /*Console.WriteLine("ODATA:");
                Console.WriteLine($"\t Response time: {stopWatch.Elapsed}");
                Console.WriteLine($"\t Size: {Encoding.Unicode.GetByteCount(responseAsString)}");*/

                string GraphQLQuery;
                if (cursor == "")
                {
                    GraphQLQuery = "query{ students{ nodes{ studentId, name, courses{ name } }, pageInfo { endCursor } } }";
                }
                else
                {
                    GraphQLQuery = "query{ students(after: \\\"" + cursor + "\\\"){ nodes{ studentId, name, courses{ name } }, pageInfo { endCursor } } }";
                }
                string query = "{ \"query\" : \"" + GraphQLQuery + "\" }";
                stopWatch.Restart();
                HttpResponseMessage response = await client.PostAsync(GRAPHQL_URL, new StringContent(
                    query, Encoding.UTF8, "application/json"));
                responseAsString = await response.Content.ReadAsStringAsync();
                stopWatch.Stop();
                graphqlTimes.Add(stopWatch.Elapsed.TotalMilliseconds);
                /*Console.WriteLine("GRAPHQL:");
                Console.WriteLine($"\t Response time: {stopWatch.Elapsed}");
                Console.WriteLine($"\t Size: Sent: {Encoding.Unicode.GetByteCount(query)} " +
                    $"Received: {Encoding.Unicode.GetByteCount(responseAsString)}");*/

                var parsed = JsonConvert.DeserializeObject<Root>(responseAsString);
                cursor = parsed.data.students.pageInfo.endCursor;
            }

            client.Dispose();

            Console.WriteLine("REST");
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
            average = restTimes.Average();
            sum = restTimes.Sum(d => Math.Pow(d - average, 2));
            stdDev = Math.Sqrt((sum) / (odataTimes.Count() - 1));
            Console.WriteLine($"Avg: {average}");
            Console.WriteLine($"Standard deviation: {stdDev}");
            Console.WriteLine();

            Console.WriteLine("GraphQL");
            foreach (var item in graphqlTimes)
            {
                Console.WriteLine($"Response time: {item}");
            }
            average = restTimes.Average();
            sum = restTimes.Sum(d => Math.Pow(d - average, 2));
            stdDev = Math.Sqrt((sum) / (graphqlTimes.Count() - 1));
            Console.WriteLine($"Avg: {average}");
            Console.WriteLine($"Standard deviation: {stdDev}");
        }
    }
}
