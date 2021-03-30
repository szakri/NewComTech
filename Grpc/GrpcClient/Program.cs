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

            try
            {
                StudentDTO student = client.GetStudent(new Id { Id_ = 1 });
                Console.WriteLine(student);
                student = client.GetStudent(new Id { Id_ = 100000 });
                Console.WriteLine(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

            try
            {
                CourseDTO course = client.GetCourse(new Id { Id_ = 1 });
                Console.WriteLine(course);
                course = client.GetCourse(new Id { Id_ = 100000 });
                Console.WriteLine(course);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("---------------------------------------");

            using (var call = client.GetCoursesWithSubject(new EmptyMessage()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    CourseSubjectDTO s = call.ResponseStream.Current;
                    Console.WriteLine(s.ToString());
                }
            }

            Console.WriteLine("---------------------------------------");

            try
            {
                CourseSubjectDTO course = client.GetCourseWithSubject(new Id { Id_ = 1 });
                Console.WriteLine(course);
                course = client.GetCourseWithSubject(new Id { Id_ = 100000 });
                Console.WriteLine(course);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            
            Console.WriteLine("---------------------------------------");

            try
            {
                SubjectDTO subject = client.GetSubject(new Id { Id_ = 1 });
                Console.WriteLine(subject);
                subject = client.GetSubject(new Id { Id_ = 100000 });
                Console.WriteLine(subject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
