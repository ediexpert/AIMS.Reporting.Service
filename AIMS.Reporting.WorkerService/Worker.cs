using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIMS.Dashboards.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private SqlConnection sqlcon;
        private string _aimsConnString;
        private Dictionary<string, int> _dbData;
        private readonly IConfiguration _config;
        public Worker(ILogger<Worker> logger, IServiceProvider provider, IConfiguration config)
        {
            _logger = logger;
            _provider = provider;
            _nlogger = NLog.LogManager.GetCurrentClassLogger();
            _dbData = new Dictionary<string, int>();
            _config = config;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker Reporting service running at: {time}", DateTimeOffset.Now);
                _nlogger.Info("Worker Reporting service running at: {time}", DateTimeOffset.Now);

                using var scope = _provider.CreateScope();                
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                _aimsConnString = _config.GetConnectionString("AIMSConnection");
                sqlcon = new SqlConnection();
                sqlcon.ConnectionString = _aimsConnString; //"Data Source=192.168.3.100;Initial Catalog=JSMSDB;User id=khadimeaala;Password=AMJJ$@120820!8;";

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
            AttendanceTanzeem at = new AttendanceTanzeem();
            try
            {
                // require date in format 2022-08-15 00:00:00
                string query = $"SELECT count (distinct j.barcode) total, m.tanzeem FROM dbo.members_local m, dbo.attendance_a j where j.barcode = m.barcode AND j.entrytime BETWEEN '{_startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{_endDate.ToString("yyyy-MM-dd HH:mm:ss")}' group by tanzeem ";
                _logger.LogInformation($"Fetching data(Local members) from AIMS server....");
                _logger.LogInformation(query);
                Execute(query);
                at.Ansar = Convert.ToInt32( _dbData.FirstOrDefault(x => x.Key == "A").Value);
                at.Khuddam = Convert.ToInt32( _dbData.FirstOrDefault(x => x.Key == "K").Value);
                at.Lajna = Convert.ToInt32( _dbData.FirstOrDefault(x => x.Key == "L").Value);
                at.Itfal = Convert.ToInt32( _dbData.FirstOrDefault(x => x.Key == "T").Value);
                at.Nasrat = Convert.ToInt32( _dbData.FirstOrDefault(x => x.Key == "N").Value);
                at.Boys = Convert.ToInt32( _dbData.FirstOrDefault(x => x.Key == "B").Value);
                at.Girls = Convert.ToInt32( _dbData.FirstOrDefault(x => x.Key == "G").Value);

                // Visitor

                query = query.Replace("dbo.members_local", "dbo.visitors");                
                _logger.LogInformation($"Fetching data(Visitors) from AIMS server....");
                _logger.LogInformation(query);
                Execute(query);
                at.Ansar += Convert.ToInt32(_dbData.FirstOrDefault(x => x.Key == "A").Value);
                at.Khuddam += Convert.ToInt32(_dbData.FirstOrDefault(x => x.Key == "K").Value);
                at.Lajna += Convert.ToInt32(_dbData.FirstOrDefault(x => x.Key == "L").Value);
                at.Itfal += Convert.ToInt32(_dbData.FirstOrDefault(x => x.Key == "T").Value);
                at.Nasrat += Convert.ToInt32(_dbData.FirstOrDefault(x => x.Key == "N").Value);
                at.Boys += Convert.ToInt32(_dbData.FirstOrDefault(x => x.Key == "B").Value);
                at.Girls += Convert.ToInt32(_dbData.FirstOrDefault(x => x.Key == "G").Value);

                return at;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, exception);
                _nlogger.Error(exception);
                throw;
            }
            
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
        private void Execute(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(
                       _aimsConnString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                // Call Read before accessing data.
                _dbData?.Clear();
                while (reader.Read())
                {
                    ReadSingleRow((IDataRecord)reader);
                }

                // Call Close when done reading.
                reader.Close();
            }
        }
        private void ReadSingleRow(IDataRecord dataRecord)
        {
            _dbData.Add(((string)dataRecord[1]).Trim(), (int)dataRecord[0]);
            Console.WriteLine(String.Format("{0}, {1}", dataRecord[0], dataRecord[1]));
        }
    }
}

