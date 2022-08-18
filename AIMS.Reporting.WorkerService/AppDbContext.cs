using System;
using AIMS.Reporting.WorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AIMS.Reporting.WorkerService
{
    public class AppDbContext: DbContext
    {
        //private string _connString;
        //public AppDbContext(string connString)
        //{
        //    _connString = connString;
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySql(_connString);
        //}


        static DbContextOptions<AppDbContext> _options;
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            _options = options;
        }


        public DbSet<Hazri> Hazris { get; set; }
        public DbSet<Event> Events { get; set; }

    }
}

