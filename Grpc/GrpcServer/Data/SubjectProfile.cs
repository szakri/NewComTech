using AutoMapper;
using Common.Models;
using GrpcServer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Data
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<Subject, SubjectDTO>().ConvertUsing(new SubjectConverter());
        }
    }

    public class SubjectConverter : ITypeConverter<Subject, SubjectDTO>
    {
        public SubjectDTO Convert(Subject source, SubjectDTO destination, ResolutionContext context)
        {
            return new SubjectDTO
            {
                ID = source.SubjectID,
                Name = source.Name
            };
        }
    }
}
