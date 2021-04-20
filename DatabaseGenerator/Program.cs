using Common.Data;
using DatabaseGenerator.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace DatabaseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
            contextOptionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=selflab;Trusted_Connection=True;MultipleActiveResultSets=true");
            var context = new SchoolContext(contextOptionsBuilder.Options);
            DbInitializer.Initialize(context);
        }
    }
}
