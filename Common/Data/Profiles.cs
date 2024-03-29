﻿using AutoMapper;
using Common.Models;

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

            CreateMap<Student, StudentQRCodeDTO>();
            CreateMap<StudentQRCodeDTO, Student>();

            CreateMap<Subject, SubjectDTO>();
            CreateMap<SubjectDTO, Subject>();

            CreateMap<Attendance, AttendanceDTO>();
            CreateMap<AttendanceDTO, Attendance>();
        }
    }
}
