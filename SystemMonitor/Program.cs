using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.Utils;
using NickStrupat;
using System.Configuration;
using System.Configuration.Internal;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Timers;
using SystemMonitor.Data;
using SystemMonitor.Data.Models;
using SystemMonitor.Monitoring;
class Program
{
    public class Config
    {
          public int MonitoringIntervalMilSeconds { get; set; }
          public int CPUThresholdPercent { get; set; }
          public ConnectionStringsConfig ConnectionStrings { get; set; }

    }

    public class ConnectionStringsConfig
    {
        public string DefaultConnection {  set; get; }
    }
    static void Main(string[] args)
    {

        //create logger
        SystemMonitor.Logging.FileLogger logger = new SystemMonitor.Logging.FileLogger("Logs");
        logger.Log(SystemMonitor.Logging.LogLevel.Info, "Startup", "Application Started");


        Console.WriteLine("##############################");
        Console.WriteLine("   System Resource Monitor   ");
        Console.WriteLine("##############################");

        //read config file
        string json;
        string basePath = AppContext.BaseDirectory;
        string configPath = Path.Combine(basePath,"Config","appsettings.json");
        json = File.ReadAllText(configPath);
        Config configinfo = JsonSerializer.Deserialize<Config>(json);

        //Console.WriteLine("DB Connection String:");
        //Console.WriteLine(configinfo.ConnectionStrings.DefaultConnection);

        //Database Connection
        ServiceCollection services = new ServiceCollection();

        services.AddDbContext<MonitoringDbContext>(options => 
            options.UseSqlServer(
                configinfo.ConnectionStrings.DefaultConnection, 
                sqlOptions =>{
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3, 
                        maxRetryDelay: TimeSpan.FromSeconds(5), 
                        errorNumbersToAdd: null);
                }
            ));


        //instantiate objects needed for metric collection
        SystemSnapshot systemSnapshot = new SystemSnapshot();
        FullMetricSnapShot metricSnapShot;

        bool isRunning = true;


        Console.WriteLine("Monitoring Interval: " + configinfo.MonitoringIntervalMilSeconds+" ms");
        Console.WriteLine("CPU Alert Threshold: " + configinfo.CPUThresholdPercent +" %");
        
        Console.WriteLine("----------------------");
        Console.WriteLine("   Begin Monitoring  ");
        Console.WriteLine("----------------------");

        //Handling Ctrl C to shutdown
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nShutdown Requested... stopping monitor.");
            logger.Log(SystemMonitor.Logging.LogLevel.Info, "Shutdown", "Application Shutdown");
            isRunning = false;
            e.Cancel = true;
        };

        var serviceProvider = services.BuildServiceProvider();


        while (isRunning)
        {
            try
            {
                
                try
                {
                    metricSnapShot = systemSnapshot.GetSystemSnapshot();
                }
                catch (Exception ex) {
                    logger.Log(SystemMonitor.Logging.LogLevel.Error, "SnapshotCollector", ex.Message);

                    //sleep for a moment if error because otherwise it will bloat logs
                    Thread.Sleep(configinfo.MonitoringIntervalMilSeconds / 2);
                    continue;
                }
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Date: {System.DateTime.Now}");
                Console.WriteLine($"Drive: {Environment.SystemDirectory.Substring(0, 1)}:");
                Console.WriteLine($"CPU Usage:       {metricSnapShot.CpuUsage:0.##} %");
                Console.WriteLine($"Ram Usage:    {metricSnapShot.MemUsage:0.##} / {metricSnapShot.TotalRAM:0.##} MB");
                Console.WriteLine($"Disk I/O Usage:  {metricSnapShot.DiskIoUsage:0.##} %");
                Console.WriteLine($"Total Disk:      {metricSnapShot.DiskTotal:0.##} GB");
                Console.WriteLine($"Free Disk:       {metricSnapShot.DiskFree:0.##} GB");
                Console.WriteLine($"Used Disk:       {metricSnapShot.DiskUsed:0.##} GB");
                Console.WriteLine($"Disk Used %:     {metricSnapShot.DiskusedPercent:0.##} %");
                //creating new scope each iteration
                using (var scope = serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MonitoringDbContext>();
                    //metric validation
                    if ((metricSnapShot.CpuUsage < 0) || (metricSnapShot.CpuUsage > 100 )){
                        logger.Log(SystemMonitor.Logging.LogLevel.Warning, "Validation", $"Invalid CPU usage:{metricSnapShot.CpuUsage}");
                        Thread.Sleep(configinfo.MonitoringIntervalMilSeconds/2);
                        continue;
                    }
                    if ((metricSnapShot.MemUsage < 0) || (metricSnapShot.MemUsage > metricSnapShot.TotalRAM))
                    {
                        logger.Log(SystemMonitor.Logging.LogLevel.Warning, "Validation", $"Invalid Memory usage:{metricSnapShot.MemUsage}");
                        Thread.Sleep(configinfo.MonitoringIntervalMilSeconds/2);

                        continue;
                    }
                    if ((metricSnapShot.DiskUsed < 0) || (metricSnapShot.DiskUsed > metricSnapShot.DiskTotal ))
                    {
                        logger.Log(SystemMonitor.Logging.LogLevel.Warning, "Validation", $"Invalid Disk usage:{metricSnapShot.DiskUsed}");
                        Thread.Sleep(configinfo.MonitoringIntervalMilSeconds/2);
                        continue;
                    }
                    

                    //metrics object to be saved to db
                    SystemMetrics metrics = new SystemMetrics
                    {
                        RecordedAt = DateTime.Now,
                        CpuUsagePercent = metricSnapShot.CpuUsage,
                        MemoryUsageMB = metricSnapShot.MemUsage,
                        DiskUsagePercent = metricSnapShot.DiskusedPercent,
                        CollectionIntervalMs = configinfo.MonitoringIntervalMilSeconds
                    };
                    try
                    {
                        db.SystemMetrics.Add(metrics);
                        db.SaveChanges();
                    }
                    catch (Exception ex) {
                        logger.Log(SystemMonitor.Logging.LogLevel.Error, "DatabaseSaver", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error has occured: {ex.Message}");
            }
            //Pause between system readings
            Thread.Sleep(configinfo.MonitoringIntervalMilSeconds);
        }
        Console.WriteLine("System Resource Monitor stopped.");
    }

}
