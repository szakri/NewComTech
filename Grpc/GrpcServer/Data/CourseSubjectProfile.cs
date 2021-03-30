using AutoMapper;
using Common.Models;
using GrpcServer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Data
{
    public class CourseSubjectProfile : Profile
    {
        public CourseSubjectProfile()
        {
            CreateMap<Course, CourseSubjectDTO>().ConvertUsing(new CourseSubjectConverter());
        }
    }

    public class CourseSubjectConverter : ITypeConverter<Course, CourseSubjectDTO>
    {
        public CourseSubjectDTO Convert(Course source, CourseSubjectDTO destination, ResolutionContext context)
        {
            return new CourseSubjectDTO
            {
                CourseID = source.CourseID,
                Name = source.Name,
                Type = source.Type,
                Day = (int)source.Day,
                From = source.From.ToString(),
                To = source.To.ToString(),
                Subject = new SubjectDTO { Name = source.Subject.Name }
            };
        }
    }
}
