using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient.Protos;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new School.SchoolClient(channel);

            using (var call = client.GetStudents(new EmptyMessage()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    StudentDTO s = call.ResponseStream.Current;
                    Console.WriteLine(s.ToString());
                }
            }

            Console.WriteLine("---------------------------------------");

            using (var call = client.GetCourses(new EmptyMessage()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    CourseDTO s = call.ResponseStream.Current;
                    Console.WriteLine(s.ToString());
                }
            }

            Console.WriteLine("---------------------------------------");

            using (var call = client.GetSubjects(new EmptyMessage()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    SubjectDTO s = call.ResponseStream.Current;
                    Console.WriteLine(s.ToString());
                }
            }
        }
    }
}
