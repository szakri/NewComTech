using AutoMapper;
using Common.Models;
using Google.Protobuf;
using GrpcServer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Data
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Course, CourseDTO>();
            CreateMap<CourseDTO, Course>();

            CreateMap<Course, CourseSubjectDTO>();

            CreateMap<Student, StudentCoursesDTO>();

            CreateMap<Student, StudentDTO>();
            CreateMap<StudentDTO, Student>();

            CreateMap<Student, StudentQRCodeDTO>().ConvertUsing(s => toStudentQRCodeDTO(s));
            CreateMap<StudentQRCodeDTO, Student>();

            CreateMap<Subject, SubjectDTO>();
            CreateMap<SubjectDTO, Subject>();

            CreateMap<Attendance, AttendanceDTO>();
            CreateMap<AttendanceDTO, Attendance>();
        }

        private StudentQRCodeDTO toStudentQRCodeDTO(Student s)
        {
            return new StudentQRCodeDTO
            {
                StudentId = s.StudentId,
                QRCode = ByteString.CopyFrom(s.QRCode, 0, s.QRCode.Length),
            };                
        }
    }
}
