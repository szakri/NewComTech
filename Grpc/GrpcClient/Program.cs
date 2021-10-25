using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient.Protos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        private static GrpcChannel channel;

        static async Task Main(string[] args)
        {
            channel = GrpcChannel.ForAddress("https://localhost:5001");

            using (channel)
            {
                await checkStudents();
                Console.WriteLine();
                await checkCourses();
                Console.WriteLine();
                await checkSubjects();
                Console.WriteLine();
                await checkAttendances();
                Console.WriteLine();
                await checkExceptions();
                Console.WriteLine();
                await measureTimeAsync();
                Console.ReadLine();
            }
        }

        private static async Task checkStudents()
        {
            var studentClient = new Students.StudentsClient(channel);

            Console.WriteLine("GetStudents PageSize = 5, OrderBy = \"name\"");
            List<StudentDTO> students = new List<StudentDTO>();
            using (var call = studentClient.GetStudents(new QueryParams { PageSize = 5, OrderBy = "name" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    students.Add(call.ResponseStream.Current);
                }
            }

            Assert.IsNotNull(students);
            Assert.IsTrue(students.Count == 5); // Check page size
            Assert.IsNotNull(students[0]);
            Assert.IsNotNull(students[0].StudentId);
            Assert.IsNotNull(students[0].Name);
            Assert.IsNotNull(students[0].Neptun);
            Assert.IsNotNull(students[0].DayOfBirth);
            // Check ordering
            for (int i = 1; i < students.Count; i++)
            {
                Assert.IsTrue(string.Compare(students[i - 1].Name, students[i].Name) <= 0);
            }
            foreach (var s in students)
            {
                printStudent(s);
            }

            Console.WriteLine("\nGetStudentsWithCourses PageSize = 5, OrderBy = \"name\"");
            List<StudentCoursesDTO> studentsWithCourses = new List<StudentCoursesDTO>();
            using (var call = studentClient.GetStudentsWithCourses(new QueryParams { PageSize = 5, OrderBy = "name" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    studentsWithCourses.Add(call.ResponseStream.Current);
                }
            }

            Assert.IsTrue(studentsWithCourses.Count == 5); // Check page size
            Assert.IsNotNull(studentsWithCourses[0]);
            Assert.IsNotNull(studentsWithCourses[0].StudentId);
            Assert.IsNotNull(studentsWithCourses[0].Name);
            Assert.IsNotNull(studentsWithCourses[0].Neptun);
            Assert.IsNotNull(studentsWithCourses[0].DayOfBirth);
            Assert.IsNotNull(studentsWithCourses[0].Courses);
            Assert.IsTrue(studentsWithCourses[0].Courses.Count > 0);
            Assert.IsNotNull(studentsWithCourses[0].Courses[0].CourseId);
            Assert.IsNotNull(studentsWithCourses[0].Courses[0].Name);
            Assert.IsNotNull(studentsWithCourses[0].Courses[0].Type);
            Assert.IsNotNull(studentsWithCourses[0].Courses[0].Day);
            Assert.IsNotNull(studentsWithCourses[0].Courses[0].From);
            Assert.IsNotNull(studentsWithCourses[0].Courses[0].To);
            // Check ordering
            for (int i = 1; i < studentsWithCourses.Count; i++)
            {
                Assert.IsTrue(string.Compare(studentsWithCourses[i - 1].Name, studentsWithCourses[i].Name) <= 0);
            }
            foreach (var s in studentsWithCourses)
            {
                printStudentWithCourses(s);
            }

            Console.WriteLine("\nGetStudent");

            StudentDTO student = await studentClient.GetStudentAsync(new ID { Value = 1 });

            Assert.IsNotNull(student);
            Assert.IsNotNull(student.StudentId);
            Assert.IsNotNull(student.Name);
            Assert.IsNotNull(student.Neptun);
            Assert.IsNotNull(student.DayOfBirth);

            printStudent(student);

            Console.WriteLine("\nGetStudentWithCourses");

            StudentCoursesDTO studentWithCourse = await studentClient.GetStudentWithCoursesAsync(new ID { Value = 1 });

            Assert.IsNotNull(studentWithCourse);
            Assert.IsNotNull(studentWithCourse);
            Assert.IsNotNull(studentWithCourse.StudentId);
            Assert.IsNotNull(studentWithCourse.Name);
            Assert.IsNotNull(studentWithCourse.Neptun);
            Assert.IsNotNull(studentWithCourse.DayOfBirth);
            Assert.IsNotNull(studentWithCourse.Courses);
            Assert.IsTrue(studentWithCourse.Courses.Count > 0);
            Assert.IsNotNull(studentWithCourse.Courses[0].CourseId);
            Assert.IsNotNull(studentWithCourse.Courses[0].Name);
            Assert.IsNotNull(studentWithCourse.Courses[0].Type);
            Assert.IsNotNull(studentWithCourse.Courses[0].Day);
            Assert.IsNotNull(studentWithCourse.Courses[0].From);
            Assert.IsNotNull(studentWithCourse.Courses[0].To);

            printStudentWithCourses(studentWithCourse);

            Console.WriteLine("\nGetStudentQRCode");

            StudentQRCodeDTO studentQR = await studentClient.GetStudentQRCodeAsync(new ID { Value = 1 });

            Assert.IsNotNull(studentQR);
            Assert.IsNotNull(studentQR.StudentId);
            Assert.IsNotNull(studentQR.QRCode);
            Assert.IsTrue(studentQR.QRCode.Length == 8949); // Check file size

            Console.WriteLine("The file size is: " + studentQR.QRCode.Length);

            Console.WriteLine("\nAddStudent");

            StudentDTO newStudent = new StudentDTO
            {
                Name = "Teszt Elek",
                Neptun = "B4TM4N",
                DayOfBirth = "2000.01.01"
            };

            StudentDTO createdStudent = await studentClient.AddStudentAsync(newStudent);

            Assert.IsNotNull(createdStudent);
            Assert.IsNotNull(createdStudent.StudentId);
            Assert.IsNotNull(createdStudent.Name);
            Assert.IsNotNull(createdStudent.Neptun);
            Assert.IsNotNull(createdStudent.DayOfBirth);
            Assert.IsTrue(newStudent.Name == createdStudent.Name);
            Assert.IsTrue(newStudent.Neptun == createdStudent.Neptun);
            Assert.IsTrue(newStudent.DayOfBirth == createdStudent.DayOfBirth);

            printStudent(createdStudent);

            Console.WriteLine("\nModifyStudent");

            StudentDTO modifiedStudent = createdStudent;
            modifiedStudent.Name = "Teszt Elek 2";
            modifiedStudent = await studentClient.ModifyStudentAsync(new ChangeStudentDTO
            {
                StudentId = createdStudent.StudentId,
                Student = modifiedStudent
            });

            Assert.IsNotNull(modifiedStudent);
            Assert.IsNotNull(modifiedStudent.StudentId);
            Assert.IsNotNull(modifiedStudent.Name);
            Assert.IsNotNull(modifiedStudent.Neptun);
            Assert.IsNotNull(modifiedStudent.DayOfBirth);
            Assert.IsTrue(modifiedStudent.StudentId == createdStudent.StudentId);
            Assert.IsTrue(modifiedStudent.Name == createdStudent.Name);
            Assert.IsTrue(modifiedStudent.Neptun == createdStudent.Neptun);
            Assert.IsTrue(modifiedStudent.DayOfBirth == createdStudent.DayOfBirth);

            printStudent(modifiedStudent);

            Console.WriteLine("\nDeleteStudent");

            StudentDTO deletedStudent = await studentClient.DeleteStudentAsync(new ID { Value = createdStudent.StudentId });

            Assert.IsNotNull(deletedStudent);
            Assert.IsNotNull(deletedStudent.StudentId);
            Assert.IsNotNull(deletedStudent.Name);
            Assert.IsNotNull(deletedStudent.Neptun);
            Assert.IsNotNull(deletedStudent.DayOfBirth);
            Assert.IsTrue(deletedStudent.StudentId == createdStudent.StudentId);
            Assert.IsTrue(deletedStudent.Name == createdStudent.Name);
            Assert.IsTrue(deletedStudent.Neptun == createdStudent.Neptun);
            Assert.IsTrue(deletedStudent.DayOfBirth == createdStudent.DayOfBirth);

            printStudent(deletedStudent);
        }

        private static async Task checkCourses()
        {
            var courseClient = new Courses.CoursesClient(channel);

            Console.WriteLine("GetCourses PageSize = 5, OrderBy = \"day\"");

            List<CourseDTO> courses = new List<CourseDTO>();
            using (var call = courseClient.GetCourses(new QueryParams { PageSize = 5, OrderBy = "day" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    courses.Add(call.ResponseStream.Current);
                }
            }

            Assert.IsNotNull(courses);
            Assert.IsTrue(courses.Count == 5); // Check page size
            Assert.IsNotNull(courses[0]);
            Assert.IsNotNull(courses[0].Name);
            Assert.IsNotNull(courses[0].Type);
            Assert.IsNotNull(courses[0].Day);
            Assert.IsTrue(courses[0].Day >= 1);
            Assert.IsTrue(courses[0].Day <= 7);
            Assert.IsNotNull(courses[0].From);
            Assert.IsNotNull(courses[0].To);
            // Check ordering
            for (int i = 1; i < courses.Count; i++)
            {
                Assert.IsTrue(courses[i - 1].Day <= courses[i].Day);
            }

            foreach (var c in courses)
            {
                printCourse(c);
            }

            Console.WriteLine("\nGetCoursesWithSubject PageSize = 5, OrderBy = \"day\"");

            List<CourseSubjectDTO> coursesWithSubject = new List<CourseSubjectDTO>();
            using (var call = courseClient.GetCoursesWithSubject(new QueryParams { PageSize = 5, OrderBy = "day" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    coursesWithSubject.Add(call.ResponseStream.Current);
                }
            }

            Assert.IsNotNull(coursesWithSubject);
            Assert.IsTrue(coursesWithSubject.Count == 5); // Check page size
            Assert.IsNotNull(coursesWithSubject[0]);
            Assert.IsNotNull(coursesWithSubject[0].Name);
            Assert.IsNotNull(coursesWithSubject[0].Type);
            Assert.IsNotNull(coursesWithSubject[0].Day);
            Assert.IsTrue(coursesWithSubject[0].Day >= 1);
            Assert.IsTrue(coursesWithSubject[0].Day <= 7);
            Assert.IsNotNull(coursesWithSubject[0].From);
            Assert.IsNotNull(coursesWithSubject[0].To);
            Assert.IsNotNull(coursesWithSubject[0].Subject);
            Assert.IsNotNull(coursesWithSubject[0].Subject.SubjectId);
            Assert.IsNotNull(coursesWithSubject[0].Subject.Name);
            // Check ordering
            for (int i = 1; i < coursesWithSubject.Count; i++)
            {
                Assert.IsTrue(coursesWithSubject[i - 1].Day <= coursesWithSubject[i].Day);
            }

            foreach (var c in coursesWithSubject)
            {
                printCourseWithSubject(c);
            }

            Console.WriteLine("\nGetCourse");

            CourseDTO course = await courseClient.GetCourseAsync(new ID { Value = 1 });
            
            Assert.IsNotNull(course);
            Assert.IsNotNull(course.Name);
            Assert.IsNotNull(course.Type);
            Assert.IsNotNull(course.Day);
            Assert.IsTrue(course.Day >= 1);
            Assert.IsTrue(course.Day <= 7);
            Assert.IsNotNull(course.From);
            Assert.IsNotNull(course.To);

            printCourse(course);

            Console.WriteLine("\nGetCourseWithSubject");

            CourseSubjectDTO courseWithSubject = await courseClient.GetCourseWithSubjectAsync(new ID { Value = 1 });

            Assert.IsNotNull(courseWithSubject);
            Assert.IsNotNull(courseWithSubject.Name);
            Assert.IsNotNull(courseWithSubject.Type);
            Assert.IsNotNull(courseWithSubject.Day);
            Assert.IsTrue(courseWithSubject.Day >= 1);
            Assert.IsTrue(courseWithSubject.Day <= 7);
            Assert.IsNotNull(courseWithSubject.From);
            Assert.IsNotNull(courseWithSubject.To);
            Assert.IsNotNull(courseWithSubject.Subject);
            Assert.IsNotNull(courseWithSubject.Subject.SubjectId);
            Assert.IsNotNull(courseWithSubject.Subject.Name);

            printCourseWithSubject(courseWithSubject);

            Console.WriteLine("\nAddCourse");

            CourseDTO newCourse = new CourseDTO
            {
                Name = "Teszt",
                Type = "T",
                Day = 1,
                From = "08:15:00",
                To = "09:45:00"
            };
            CourseDTO createdCourse = await courseClient.AddCourseAsync(newCourse);

            Assert.IsNotNull(createdCourse);
            Assert.IsNotNull(createdCourse.CourseId);
            Assert.IsNotNull(createdCourse.Name);
            Assert.IsNotNull(createdCourse.Type);
            Assert.IsNotNull(createdCourse.Day);
            Assert.IsTrue(createdCourse.Day >= 1);
            Assert.IsTrue(createdCourse.Day <= 7);
            Assert.IsNotNull(createdCourse.From);
            Assert.IsNotNull(createdCourse.To);
            Assert.IsTrue(createdCourse.Name == newCourse.Name);
            Assert.IsTrue(createdCourse.Type == newCourse.Type);
            Assert.IsTrue(createdCourse.Day == newCourse.Day);
            Assert.IsTrue(createdCourse.From == newCourse.From);
            Assert.IsTrue(createdCourse.To == newCourse.To);

            printCourse(createdCourse);

            Console.WriteLine("\nModifyCourse");

            CourseDTO modifiedCourse = createdCourse;
            modifiedCourse.Name = "Teszt 2";
            modifiedCourse = await courseClient.ModifyCourseAsync(new ChangeCourseDTO
            {
                CourseId = createdCourse.CourseId,
                Course = modifiedCourse
            });

            Assert.IsNotNull(modifiedCourse);
            Assert.IsNotNull(modifiedCourse.CourseId);
            Assert.IsNotNull(modifiedCourse.Name);
            Assert.IsNotNull(modifiedCourse.Type);
            Assert.IsNotNull(modifiedCourse.Day);
            Assert.IsTrue(modifiedCourse.Day >= 1);
            Assert.IsTrue(modifiedCourse.Day <= 7);
            Assert.IsNotNull(modifiedCourse.From);
            Assert.IsNotNull(modifiedCourse.To);
            Assert.IsTrue(createdCourse.CourseId == modifiedCourse.CourseId);
            Assert.IsTrue(createdCourse.Type == modifiedCourse.Type);
            Assert.IsTrue(createdCourse.Day == modifiedCourse.Day);
            Assert.IsTrue(createdCourse.From == modifiedCourse.From);
            Assert.IsTrue(createdCourse.To == modifiedCourse.To);

            printCourse(modifiedCourse);

            Console.WriteLine("\nDeleteCourse");

            CourseDTO deletedCourse = await courseClient.DeleteCourseAsync(new ID { Value = createdCourse.CourseId });

            Assert.IsNotNull(deletedCourse);
            Assert.IsNotNull(deletedCourse.CourseId);
            Assert.IsNotNull(deletedCourse.Name);
            Assert.IsNotNull(deletedCourse.Type);
            Assert.IsNotNull(deletedCourse.Day);
            Assert.IsTrue(deletedCourse.Day >= 1);
            Assert.IsTrue(deletedCourse.Day <= 7);
            Assert.IsNotNull(deletedCourse.From);
            Assert.IsNotNull(deletedCourse.To);
            Assert.IsTrue(deletedCourse.CourseId == modifiedCourse.CourseId);
            Assert.IsTrue(deletedCourse.Name == modifiedCourse.Name);
            Assert.IsTrue(deletedCourse.Type == modifiedCourse.Type);
            Assert.IsTrue(deletedCourse.Day == modifiedCourse.Day);
            Assert.IsTrue(deletedCourse.From == modifiedCourse.From);
            Assert.IsTrue(deletedCourse.To == modifiedCourse.To);

            printCourse(deletedCourse);
        }

        private static async Task checkSubjects()
        {
            var subjectClient = new Subjects.SubjectsClient(channel);

            Console.WriteLine("GetSubjects PageSize = 5, OrderBy = \"name\"");

            List<SubjectDTO> subjects = new List<SubjectDTO>();
            using (var call = subjectClient.GetSubjects(new QueryParams { PageSize = 5, OrderBy = "name" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    subjects.Add(call.ResponseStream.Current);
                }
            }

            Assert.IsNotNull(subjects);
            Assert.IsTrue(subjects.Count == 5); // Check page size
            Assert.IsNotNull(subjects[0]);
            Assert.IsNotNull(subjects[0].SubjectId);
            Assert.IsNotNull(subjects[0].Name);
            // Check ordering
            for (int i = 1; i < subjects.Count; i++)
            {
                Assert.IsTrue(string.Compare(subjects[i - 1].Name, subjects[i].Name) <= 0);
            }

            foreach (var s in subjects)
            {
                printSubject(s);
            }

            Console.WriteLine("\nGetSubject");

            SubjectDTO subject = await subjectClient.GetSubjectAsync(new ID { Value = 1 });

            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.SubjectId);
            Assert.IsNotNull(subject.Name);

            printSubject(subject);

            Console.WriteLine("\nAddSubject");

            SubjectDTO newSubject = new SubjectDTO
            {
                Name = "Teszt"
            };
            SubjectDTO createdSubject = await subjectClient.AddSubjectAsync(newSubject);

            Assert.IsNotNull(createdSubject);
            Assert.IsNotNull(createdSubject.SubjectId);
            Assert.IsNotNull(createdSubject.Name);

            printSubject(createdSubject);

            Console.WriteLine("\nModifySubject");

            SubjectDTO modifiedSubect = createdSubject;
            modifiedSubect.Name = "Teszt 2";
            modifiedSubect = await subjectClient.ModifySubjectAsync(new ChangeSubjectDTO
            {
                SubjectId = createdSubject.SubjectId,
                Subject = modifiedSubect
            });

            Assert.IsNotNull(modifiedSubect);
            Assert.IsNotNull(modifiedSubect.SubjectId);
            Assert.IsNotNull(modifiedSubect.Name);
            Assert.IsTrue(modifiedSubect.SubjectId == createdSubject.SubjectId);

            printSubject(modifiedSubect);

            Console.WriteLine("\nDeleteSubject");

            SubjectDTO deletedSubject = await subjectClient.DeleteSubjectAsync(new ID { Value = createdSubject.SubjectId });

            Assert.IsNotNull(deletedSubject);
            Assert.IsNotNull(deletedSubject.SubjectId);
            Assert.IsNotNull(deletedSubject.Name);
            Assert.IsTrue(deletedSubject.SubjectId == modifiedSubect.SubjectId);
            Assert.IsTrue(deletedSubject.Name == modifiedSubect.Name);

            printSubject(deletedSubject);
        }

        private static async Task checkAttendances()
        {
            var attendanceClient = new Attendances.AttendancesClient(channel);

            Console.WriteLine("GetAttendances PageSize = 5, OrderBy = \"date\"");

            List<AttendanceDTO> attendances = new List<AttendanceDTO>();
            using (var call = attendanceClient.GetAttendances(new QueryParams { PageSize = 5, OrderBy = "date" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    attendances.Add(call.ResponseStream.Current);
                }
            }

            Assert.IsNotNull(attendances);
            Assert.IsTrue(attendances.Count == 5); // Check page size
            Assert.IsNotNull(attendances[0]);
            Assert.IsNotNull(attendances[0].AttendanceId);
            Assert.IsNotNull(attendances[0].Date);
            Assert.IsNotNull(attendances[0].CheckInTime);
            Assert.IsNotNull(attendances[0].CheckOutTime);
            Assert.IsNotNull(attendances[0].Course);
            Assert.IsNotNull(attendances[0].Course.Name);
            Assert.IsNotNull(attendances[0].Course.Type);
            Assert.IsNotNull(attendances[0].Course.Day);
            Assert.IsTrue(attendances[0].Course.Day >= 1);
            Assert.IsTrue(attendances[0].Course.Day <= 7);
            Assert.IsNotNull(attendances[0].Course.From);
            Assert.IsNotNull(attendances[0].Course.To);
            Assert.IsNotNull(attendances[0].Student);
            Assert.IsNotNull(attendances[0].Student.StudentId);
            Assert.IsNotNull(attendances[0].Student.Name);
            Assert.IsNotNull(attendances[0].Student.Neptun);
            Assert.IsNotNull(attendances[0].Student.DayOfBirth);
            // Check ordering
            for (int i = 1; i < attendances.Count; i++)
            {
                Assert.IsTrue(string.Compare(attendances[i - 1].Date, attendances[i].Date) <= 0);
            }

            foreach (var a in attendances)
            {
                printAttendance(a);
            }

            Console.WriteLine("\nGetAttendance");

            AttendanceDTO attendance = await attendanceClient.GetAttendanceAsync(new ID { Value = 1 });

            Assert.IsNotNull(attendance);
            Assert.IsNotNull(attendance.AttendanceId);
            Assert.IsNotNull(attendance.Date);
            Assert.IsNotNull(attendance.CheckInTime);
            Assert.IsNotNull(attendance.CheckOutTime);
            Assert.IsNotNull(attendance.Course);
            Assert.IsNotNull(attendance.Course.Name);
            Assert.IsNotNull(attendance.Course.Type);
            Assert.IsNotNull(attendance.Course.Day);
            Assert.IsTrue(attendance.Course.Day >= 1);
            Assert.IsTrue(attendance.Course.Day <= 7);
            Assert.IsNotNull(attendance.Course.From);
            Assert.IsNotNull(attendance.Course.To);
            Assert.IsNotNull(attendance.Student);
            Assert.IsNotNull(attendance.Student.StudentId);
            Assert.IsNotNull(attendance.Student.Name);
            Assert.IsNotNull(attendance.Student.Neptun);
            Assert.IsNotNull(attendance.Student.DayOfBirth);

            printAttendance(attendance);

            Console.WriteLine("\nAddAttendance");

            AttendanceDTO newAttendance = new AttendanceDTO
            {
                Date = "2000.01.01",
                CheckInTime = "08:15:00",
                CheckOutTime = "09:45:00"
            };
            AttendanceDTO createdAttendance = await attendanceClient.AddAttendanceAsync(newAttendance);

            Assert.IsNotNull(createdAttendance);
            Assert.IsNotNull(createdAttendance.AttendanceId);
            Assert.IsNotNull(createdAttendance.Date);
            Assert.IsNotNull(createdAttendance.CheckInTime);
            Assert.IsNotNull(createdAttendance.CheckOutTime);
            Assert.IsTrue(createdAttendance.Date == newAttendance.Date);
            Assert.IsTrue(createdAttendance.CheckInTime == newAttendance.CheckInTime);
            Assert.IsTrue(createdAttendance.CheckOutTime == newAttendance.CheckOutTime);

            printAttendance(createdAttendance);

            Console.WriteLine("\nModifyAttendance");

            AttendanceDTO modifiedAttendance = createdAttendance;
            modifiedAttendance.Date = "2020.01.01.";
            modifiedAttendance = await attendanceClient.ModifyAttendanceAsync(new ChangeAttendanceDTO
            {
                AttendanceId = createdAttendance.AttendanceId,
                Attendance = modifiedAttendance
            });

            Assert.IsNotNull(modifiedAttendance);
            Assert.IsNotNull(modifiedAttendance.AttendanceId);
            Assert.IsNotNull(modifiedAttendance.Date);
            Assert.IsNotNull(modifiedAttendance.CheckInTime);
            Assert.IsNotNull(modifiedAttendance.CheckOutTime);
            Assert.IsTrue(createdAttendance.AttendanceId == modifiedAttendance.AttendanceId);
            Assert.IsTrue(createdAttendance.Date == modifiedAttendance.Date);
            Assert.IsTrue(createdAttendance.CheckInTime == modifiedAttendance.CheckInTime);
            Assert.IsTrue(createdAttendance.CheckOutTime == modifiedAttendance.CheckOutTime);

            printAttendance(modifiedAttendance);

            Console.WriteLine("\nDeleteAttendance");

            AttendanceDTO deletedAttendance = await attendanceClient.DeleteAttendanceAsync(new ID { Value = createdAttendance.AttendanceId });

            Assert.IsNotNull(deletedAttendance);
            Assert.IsNotNull(deletedAttendance.AttendanceId);
            Assert.IsNotNull(deletedAttendance.Date);
            Assert.IsNotNull(deletedAttendance.CheckInTime);
            Assert.IsNotNull(deletedAttendance.CheckOutTime);
            Assert.IsTrue(deletedAttendance.AttendanceId == modifiedAttendance.AttendanceId);
            Assert.IsTrue(deletedAttendance.Date == modifiedAttendance.Date);
            Assert.IsTrue(deletedAttendance.CheckInTime == modifiedAttendance.CheckInTime);
            Assert.IsTrue(deletedAttendance.CheckOutTime == modifiedAttendance.CheckOutTime);

            printAttendance(deletedAttendance);
        }

        private static TimeSpan cumulateTime(TimeSpan[] times)
        {
            TimeSpan t = times[0];
            for (int i = 1; i < times.Length; i++)
            {
                t = t.Add(times[i]);
            }
            return t;
        }

        private static async Task checkExceptions()
        {
            var studentClient = new Students.StudentsClient(channel);

            List<StudentDTO> students = new List<StudentDTO>();
            using (var call = studentClient.GetStudents(new QueryParams { PageSize = 1000 }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    students.Add(call.ResponseStream.Current);
                }
            }
            Assert.IsTrue(students.Count <= 100);

            List<StudentCoursesDTO> studentsWithCourses = new List<StudentCoursesDTO>();
            using (var call = studentClient.GetStudentsWithCourses(new QueryParams { PageSize = 1000 }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    studentsWithCourses.Add(call.ResponseStream.Current);
                }
            }
            Assert.IsTrue(studentsWithCourses.Count <= 100);

            try
            {
                await studentClient.GetStudentAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await studentClient.GetStudentWithCoursesAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await studentClient.ModifyStudentAsync(new ChangeStudentDTO
                {
                    StudentId = 1000,
                    Student = new StudentDTO { StudentId = 1 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await studentClient.ModifyStudentAsync(new ChangeStudentDTO
                {
                    StudentId = 1000,
                    Student = new StudentDTO { StudentId = 1000 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await studentClient.DeleteStudentAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            var courseClient = new Courses.CoursesClient(channel);
            List<CourseDTO> courses = new List<CourseDTO>();
            using (var call = courseClient.GetCourses(new QueryParams { PageSize = 1000 }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    courses.Add(call.ResponseStream.Current);
                }
            }
            Assert.IsTrue(courses.Count <= 100);

            List<CourseSubjectDTO> coursesWithSubject = new List<CourseSubjectDTO>();
            using (var call = courseClient.GetCoursesWithSubject(new QueryParams { PageSize = 5, OrderBy = "day" }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    coursesWithSubject.Add(call.ResponseStream.Current);
                }
            }
            Assert.IsTrue(coursesWithSubject.Count <= 100);

            try
            {
                await courseClient.GetCourseAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await courseClient.GetCourseWithSubjectAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await courseClient.ModifyCourseAsync(new ChangeCourseDTO
                {
                    CourseId = 1000,
                    Course = new CourseDTO { CourseId = 1 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await courseClient.ModifyCourseAsync(new ChangeCourseDTO
                {
                    CourseId = 1000,
                    Course = new CourseDTO { CourseId = 1000 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await courseClient.DeleteCourseAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            var subjectClient = new Subjects.SubjectsClient(channel);
            List<SubjectDTO> subjects = new List<SubjectDTO>();
            using (var call = subjectClient.GetSubjects(new QueryParams { PageSize = 1000 }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    subjects.Add(call.ResponseStream.Current);
                }
            }
            Assert.IsTrue(subjects.Count <= 100);

            try
            {
                await subjectClient.GetSubjectAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await subjectClient.ModifySubjectAsync(new ChangeSubjectDTO
                {
                    SubjectId = 1000,
                    Subject = new SubjectDTO { SubjectId = 1 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await subjectClient.ModifySubjectAsync(new ChangeSubjectDTO
                {
                    SubjectId = 1000,
                    Subject = new SubjectDTO { SubjectId = 1000 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await subjectClient.DeleteSubjectAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            var attendanceClient = new Attendances.AttendancesClient(channel);
            List<AttendanceDTO> attendances = new List<AttendanceDTO>();
            using (var call = attendanceClient.GetAttendances(new QueryParams { PageSize = 1000 }))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    attendances.Add(call.ResponseStream.Current);
                }
            }
            Assert.IsTrue(subjects.Count <= 100);

            try
            {
                await attendanceClient.GetAttendanceAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await attendanceClient.ModifyAttendanceAsync(new ChangeAttendanceDTO
                {
                    AttendanceId = 1000,
                    Attendance = new AttendanceDTO { AttendanceId = 1 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await attendanceClient.ModifyAttendanceAsync(new ChangeAttendanceDTO
                {
                    AttendanceId = 1000,
                    Attendance = new AttendanceDTO { AttendanceId = 1000 }
                });
                Assert.Fail();
            }
            catch (RpcException) { }

            try
            {
                await attendanceClient.DeleteAttendanceAsync(new ID { Value = 1000 });
                Assert.Fail();
            }
            catch (RpcException) { }
        }

        private static void printTime(TimeSpan[] times)
        {
            TimeSpan cumulativeTime = cumulateTime(times);
            Console.WriteLine("Cumulative time:\t" + cumulateTime(times));
            Console.WriteLine("Average time:\t\t" + cumulativeTime.Divide(times.Length));
        }

        private static async Task measureTimeAsync()
        {
            var studentClient = new Students.StudentsClient(channel);
            var attendanceClient = new Attendances.AttendancesClient(channel);
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan[] times = new TimeSpan[1000];

            Console.WriteLine("1000 simple query");
            for (int i = 0; i < 1000; i++)
            {
                stopWatch.Restart();
                await studentClient.GetStudentAsync(new ID { Value = 1 });
                stopWatch.Stop();
                times[i] = stopWatch.Elapsed;
            }
            printTime(times);

            Console.WriteLine("\n1000 complex query");
            for (int i = 0; i < 1000; i++)
            {
                stopWatch.Restart();
                await attendanceClient.GetAttendanceAsync(new ID { Value = 1 });
                stopWatch.Stop();
                times[i] = stopWatch.Elapsed;
            }
            printTime(times);

            List<Thread> threads = new List<Thread>();
            Console.WriteLine("\n10 simple query with 100 threads");
            stopWatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                Thread t = new Thread(new ThreadStart(RunSimpeQueryAsync));
                t.Start();
                threads.Add(t);
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);

            threads = new List<Thread>();
            Console.WriteLine("\n10 complex query with 100 threads");
            stopWatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                Thread t = new Thread(new ThreadStart(RunComplexQuery));
                t.Start();
                threads.Add(t);
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);

            Console.WriteLine("\nDownload file");
            stopWatch.Restart();
            await studentClient.GetStudentQRCodeAsync(new ID { Value = 1 });
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
        }

        private static void RunSimpeQueryAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                Task t = GetStudent(i + 1);
                t.Wait();
            }
        }

        private static async Task GetStudent(int i)
        {
            var studentClient = new Students.StudentsClient(channel);
            await studentClient.GetStudentAsync(new ID { Value = i % 100 });
        }

        private static void RunComplexQuery()
        {
            for (int i = 0; i < 10; i++)
            {
                Task t = GetAttendance(i + 1);
                t.Wait();
            }
        }

        private static async Task GetAttendance(int i)
        {
            var attendanceClient = new Attendances.AttendancesClient(channel);
            await attendanceClient.GetAttendanceAsync(new ID { Value = i % 100 });
        }

        private static void printStudent(StudentDTO student)
        {
            Console.WriteLine($"StudentId: {student.StudentId}, Neptun: {student.Neptun}, Name: {student.Name}, DayOfBirth: {student.DayOfBirth}");
        }

        private static void printStudentWithCourses(StudentCoursesDTO student)
        {
            Console.WriteLine($"StudentId: {student.StudentId}, Neptun: {student.Neptun}, Name: {student.Name}, DayOfBirth: {student.DayOfBirth}");
            foreach (var c in student.Courses)
            {
                Console.Write("\t");
                printCourse(c);
            }
        }

        private static void printCourse(CourseDTO course)
        {
            Console.WriteLine($"CourseId: {course.CourseId}, Name: {course.Name}, Type: {course.Type}, Day: {course.Day}, From: {course.From}, To: {course.To}");
        }

        private static void printCourseWithSubject(CourseSubjectDTO course)
        {
            Console.WriteLine($"CourseId: {course.CourseId}, Name: {course.Name}, Type: {course.Type}, Day: {course.Day}, From: {course.From}, To: {course.To}");
            Console.Write("\t");
            printSubject(course.Subject);
        }

        private static void printSubject(SubjectDTO subject)
        {
            Console.WriteLine($"SubjectId: {subject.SubjectId}, Name: {subject.Name}");
        }

        private static void printAttendance(AttendanceDTO attendance)
        {
            Console.WriteLine($"AttendanceId: {attendance.AttendanceId}, Date: {attendance.Date}, CheckInTime: {attendance.CheckInTime}, CheckOutTime: {attendance.CheckOutTime}");
            if (attendance.Course != null)
            {
                Console.Write($"\tCourse: ");
                printCourse(attendance.Course);
            }
            if (attendance.Student != null)
            {
                Console.Write($"\tStudent: ");
                printStudent(attendance.Student);
            }
            Console.WriteLine();
        }
    }
}
