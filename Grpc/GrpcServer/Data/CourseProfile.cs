using AutoMapper;
using Common.Models;
using GrpcServer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Data
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDTO>().ConvertUsing(new CourseConverter());
        }
    }

    public class CourseConverter : ITypeConverter<Course, CourseDTO>
    {
        public CourseDTO Convert(Course source, CourseDTO destination, ResolutionContext context)
        {
            return new CourseDTO
            {
                ID = source.CourseID,
                Name = source.Name,
                Type = source.Type,
                Day = (int)source.Day,
                From = source.From.ToString(),
                To = source.To.ToString()
            };
        }
    }
}
