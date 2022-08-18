using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIMS.Reporting.WorkerService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIMS.Reporting.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private NLog.Logger _nlogger;
        private readonly IServiceProvider _provider;
        private AppDbContext _dbContext;
        Event _event;
        DateTime _startDate;
        DateTime _endDate;


        public Worker(ILogger<Worker> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _nlogger = NLog.LogManager.GetCurrentClassLogger();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker Reporting service running at: {time}", DateTimeOffset.Now);
                _nlogger.Info("Worker Reporting service running at: {time}", DateTimeOffset.Now);

                using var scope = _provider.CreateScope();
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                _logger.LogInformation($"Records count = " + _dbContext.Hazris.Count());

                // fetch data from the AIMSDB and Push data to the DODB
                _event = _dbContext.Events.FirstOrDefault(x => x.IsActive);
                Hazri hazri;
                AttendanceTanzeem attendance;
                for (int i = 1; i <= 4; i++)
                {
                    attendance = GetAttendance((Days)i);
                    hazri = _dbContext.Hazris.Find(i);
                    hazri.Ansar = attendance.Ansar;
                    hazri.Khuddam = attendance.Khuddam;
                    hazri.Itfal = attendance.Itfal;
                    hazri.Lajna = attendance.Lajna;
                    hazri.Nasrat = attendance.Nasrat;
                    hazri.Boys = attendance.Boys;
                    hazri.Girls = attendance.Girls;
                    hazri.Visitors = attendance.Visitors;
                    hazri.CreatedAtUTC = DateTime.Now;

                    _dbContext.SaveChanges();

                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private AttendanceTanzeem GetAttendance(Days i)
        {
            SetDates(i);
            throw new NotImplementedException();




        }

        private void SetDates(Days i)
        {            

            switch (i)
            {
                case Days.FirstDay:
                    _startDate = _event.StartDate.Date;
                    _endDate = _startDate.AddDays(1).AddSeconds(-1);
                    break;
                case Days.SecondDay:
                    _startDate = _event.StartDate.AddDays(1).Date;
                    _endDate = _startDate.AddDays(1).AddSeconds(-1);
                    break;
                case Days.ThirdDay:
                    _startDate = _event.StartDate.AddDays(2).Date;
                    _endDate = _startDate.AddDays(1).AddSeconds(-1);
                    break;
                default:
                    _startDate = _event.StartDate.Date;
                    _endDate = _startDate.AddDays(2).AddSeconds(-1);
                    break;
            }
        }
    }
}

