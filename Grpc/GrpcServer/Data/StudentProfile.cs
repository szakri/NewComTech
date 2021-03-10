using AutoMapper;
using Common.Models;
using GrpcServer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Data
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentDTO>().ConvertUsing(new StudentConverter());
        }
    }

    public class StudentConverter : ITypeConverter<Student, StudentDTO>
    {
        public StudentDTO Convert(Student source, StudentDTO destination, ResolutionContext context)
        {
            return new StudentDTO
            {
                Name = source.Name,
                Neptun = source.Neptun,
                DayOfBirth = String.Format("{0:yyyy.MM.dd}", source.DayOfBirth.Date)
            };
        }
    }
}
