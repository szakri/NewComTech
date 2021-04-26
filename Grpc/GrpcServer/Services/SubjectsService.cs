using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcServer.Protos;
using System.Linq.Dynamic.Core;
using Common.Data;
using AutoMapper;
using Grpc.Core;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Services
{
    public class SubjectsService : Subjects.SubjectsBase
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public SubjectsService(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetSubjects(QueryParams request, IServerStreamWriter<SubjectDTO> responseStream, ServerCallContext context)
        {
            if (request.OrderBy == "") request.OrderBy = "subjectId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var subjects = _context.Subjects.OrderBy(request.OrderBy);
            List<Subject> responses = await PaginatedList<Subject>.CreateAsync(subjects, request.PageNumber, request.PageSize);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<SubjectDTO>(response));
            }
        }

        public override async Task<SubjectDTO> GetSubject(ID request, ServerCallContext context)
        {
            var subject = await _context.Subjects.FindAsync(request.Value);
            if (subject == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such subject exists (ID = " + request.Value + ")!"));
            }
            return _mapper.Map<SubjectDTO>(subject);
        }

        public override async Task<SubjectDTO> AddSubject(SubjectDTO request, ServerCallContext context)
        {
            Subject subjectEntity = _mapper.Map<Subject>(request);
            _context.Subjects.Add(subjectEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<SubjectDTO>(subjectEntity);
        }

        public override async Task<SubjectDTO> ModifySubject(ChangeSubjectDTO request, ServerCallContext context)
        {
            if (request.SubjectId != request.Subject.SubjectId)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "The specify IDs don't match (StudentId = " + request.SubjectId +
                    ", request.Student.StudentId = " + request.Subject.SubjectId + ")!"));
            }

            _context.Entry(_mapper.Map<Subject>(request.Subject)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subjects.Any(e => e.SubjectId == request.SubjectId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (SubjectId = " + request.SubjectId + ")!"));
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<SubjectDTO>(await _context.Subjects.FindAsync(request.SubjectId));
        }

        public override async Task<SubjectDTO> DeleteSubject(ID request, ServerCallContext context)
        {
            var subject = await _context.Subjects.FindAsync(request.Value);
            if (subject == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (SubjectId = " + request.Value + ")!"));
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubjectDTO>(subject);
        }
    }
}
