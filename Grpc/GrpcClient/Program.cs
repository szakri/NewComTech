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

            var studentClient = new Students.StudentsClient(channel);
            var courseClient = new Courses.CoursesClient(channel);
            var subjectClient = new Subjects.SubjectsClient(channel);
            var attendanceClient = new Attendances.AttendancesClient(channel);

            Console.WriteLine("GetStudents---------------------------------------");

            using (var call = studentClient.GetStudents(new QueryParams { PageSize = 5, OrderBy = "name desc" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    StudentDTO s = call.ResponseStream.Current;
                    Console.WriteLine(s.ToString());
                }
            }

            Console.WriteLine("\nGetStudent---------------------------------------");

            StudentDTO student = await studentClient.GetStudentAsync(new ID { Value = 1 });
            Console.WriteLine(student);

            Console.WriteLine("\nGetStudentsWithCourses---------------------------------------");

            using (var call = studentClient.GetStudentsWithCourses(new QueryParams { PageSize = 5, OrderBy = "name desc" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    StudentCoursesDTO s = call.ResponseStream.Current;
                    Console.WriteLine(s.ToString());
                }
            }

            Console.WriteLine("\nGetStudentWithCourses---------------------------------------");

            StudentCoursesDTO studentCourses = await studentClient.GetStudentWithCoursesAsync(new ID { Value = 1 });
            Console.WriteLine(studentCourses);

            Console.WriteLine("\nGetStudentQRCode---------------------------------------");

            StudentQRCodeDTO studentQR = await studentClient.GetStudentQRCodeAsync(new ID { Value = 1 });
            Console.WriteLine(studentQR);

            Console.WriteLine("\nAddStudent---------------------------------------");

            StudentDTO addedStudent = await studentClient.AddStudentAsync(new StudentDTO { Name = "Test", DayOfBirth = "2000.0.10.", Neptun = "asdfge" });
            Console.WriteLine("Added " + addedStudent.ToString());

            Console.WriteLine("\nModifyStudent---------------------------------------");

            StudentDTO modifiedStudent = addedStudent;
            modifiedStudent.Name = "Test2";
            modifiedStudent = await studentClient.ModifyStudentAsync(new ChangeStudentDTO
            {
                StudentId = addedStudent.StudentId,
                Student = modifiedStudent
            });
            Console.WriteLine("Modified " + modifiedStudent.ToString());

            Console.WriteLine("\nDeleteStudent---------------------------------------");

            StudentDTO deletedStudent = await studentClient.DeleteStudentAsync(new ID { Value = addedStudent.StudentId });
            Console.WriteLine("Deleted " + deletedStudent);

            Console.WriteLine("\nGetCourses---------------------------------------");

            using (var call = courseClient.GetCourses(new QueryParams { PageSize = 5, OrderBy = "day desc" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    CourseDTO c = call.ResponseStream.Current;
                    Console.WriteLine(c.ToString());
                }
            }

            Console.WriteLine("\nGetCourse---------------------------------------");

            CourseDTO course = await courseClient.GetCourseAsync(new ID { Value = 1 });
            Console.WriteLine(course);

            Console.WriteLine("\nGetCoursesWithSubject---------------------------------------");

            using (var call = courseClient.GetCoursesWithSubject(new QueryParams { PageSize = 5, OrderBy = "day desc" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    CourseSubjectDTO c = call.ResponseStream.Current;
                    Console.WriteLine(c.ToString());
                }
            }

            Console.WriteLine("\nGetCourseWithSubject---------------------------------------");

            CourseSubjectDTO courseSubjects = await courseClient.GetCourseWithSubjectAsync(new ID { Value = 1 });
            Console.WriteLine(courseSubjects);

            Console.WriteLine("\nAddCourse---------------------------------------");

            CourseDTO addedCourse = await courseClient.AddCourseAsync(new CourseDTO { Name = "Test", Type = "Test" });
            Console.WriteLine("Added " + addedCourse.ToString());

            Console.WriteLine("\nModifyCourse---------------------------------------");

            CourseDTO modifiedCourse = addedCourse;
            modifiedCourse.Name = "Test2";
            modifiedCourse = await courseClient.ModifyCourseAsync(new ChangeCourseDTO
            {
                CourseId = addedCourse.CourseId,
                Course = modifiedCourse
            });
            Console.WriteLine("Modified " + modifiedCourse.ToString());

            Console.WriteLine("\nDeleteCourse---------------------------------------");

            CourseDTO deletedCourse = await courseClient.DeleteCourseAsync(new ID { Value = addedCourse.CourseId });
            Console.WriteLine("Deleted " + deletedCourse);

            Console.WriteLine("\nGetSubjects---------------------------------------");

            using (var call = subjectClient.GetSubjects(new QueryParams { PageSize = 5, OrderBy = "name desc" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    SubjectDTO s = call.ResponseStream.Current;
                    Console.WriteLine(s.ToString());
                }
            }

            Console.WriteLine("\nGetSubject---------------------------------------");

            SubjectDTO subject = await subjectClient.GetSubjectAsync(new ID { Value = 1 });
            Console.WriteLine(subject);

            Console.WriteLine("\nAddSubject---------------------------------------");

            SubjectDTO addedSubject = await subjectClient.AddSubjectAsync(new SubjectDTO { Name = "Test" });
            Console.WriteLine("Added " + addedSubject.ToString());

            Console.WriteLine("\nModifySubject---------------------------------------");

            SubjectDTO modifiedSubect = addedSubject;
            modifiedSubect.Name = "Test2";
            modifiedSubect = await subjectClient.ModifySubjectAsync(new ChangeSubjectDTO
            {
                SubjectId = addedSubject.SubjectId,
                Subject = modifiedSubect
            });
            Console.WriteLine("Modified " + modifiedSubect.ToString());

            Console.WriteLine("\nDeleteSubject---------------------------------------");

            SubjectDTO deletedSubject = await subjectClient.DeleteSubjectAsync(new ID { Value = addedSubject.SubjectId });
            Console.WriteLine("Deleted " + deletedSubject);

            Console.WriteLine("\nGetAttendances---------------------------------------");

            using (var call = attendanceClient.GetAttendances(new QueryParams { PageSize = 5, OrderBy = "CheckInTime desc" }))
            {
                try
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        AttendanceDTO a = call.ResponseStream.Current;
                        Console.WriteLine(a.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            Console.WriteLine("\nGetAttendance---------------------------------------");

            AttendanceDTO attendance = await attendanceClient.GetAttendanceAsync(new ID { Value = 1 });
            Console.WriteLine(attendance);

            Console.WriteLine("\nAddAttendance---------------------------------------");

            AttendanceDTO addedAttendance = await attendanceClient.AddAttendanceAsync(new AttendanceDTO { CheckInTime = "Test" });
            Console.WriteLine("Added " + addedAttendance.ToString());

            Console.WriteLine("\nModifyAttendance---------------------------------------");

            AttendanceDTO modifiedAttendance = addedAttendance;
            modifiedAttendance.CheckInTime = "Test2";
            modifiedAttendance = await attendanceClient.ModifyAttendanceAsync(new ChangeAttendanceDTO
            {
                AttendanceId = addedAttendance.AttendanceId,
                Attendance = modifiedAttendance
            });
            Console.WriteLine("Modified " + modifiedAttendance.ToString());

            Console.WriteLine("\nDeleteAttendance---------------------------------------");

            AttendanceDTO deletedAttendance = await attendanceClient.DeleteAttendanceAsync(new ID { Value = addedAttendance.AttendanceId });
            Console.WriteLine("Deleted " + deletedAttendance);
        }
    }
}
