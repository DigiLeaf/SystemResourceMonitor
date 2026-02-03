using Microsoft.Identity.Client.Utils;
using System.Configuration.Internal;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using SystemMonitor.Monitoring;
using System.Timers;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Data;
using Microsoft.EntityFrameworkCore;
using SystemMonitor.Data.Models;

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

        Console.WriteLine("##############################");
        Console.WriteLine("   System Resource Monitor   ");
        Console.WriteLine("##############################");

        //read config file
        string json;
        string basePath = AppContext.BaseDirectory;
        string configPath = Path.Combine(basePath,"Config","appsettings.json");
        json = File.ReadAllText(configPath);
        Config configinfo = JsonSerializer.Deserialize<Config>(json);

        Console.WriteLine("DB Connection String:");
        Console.WriteLine(configinfo.ConnectionStrings.DefaultConnection);

        //Database Connection
        ServiceCollection services = new ServiceCollection();

        services.AddDbContext<MonitoringDbContext>(options => options.UseSqlServer(configinfo.ConnectionStrings.DefaultConnection));


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
            isRunning = false;
            e.Cancel = true;
        };

        var serviceProvider = services.BuildServiceProvider();


        while (isRunning)
        {
            try
            {

                metricSnapShot = systemSnapshot.GetSystemSnapshot();
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Date: {System.DateTime.Now}");
                Console.WriteLine($"Drive: {Environment.SystemDirectory.Substring(0, 1)}:");
                Console.WriteLine($"CPU Usage:       {metricSnapShot.CpuUsage:0.##} %");
                Console.WriteLine($"Availabe Ram:    {metricSnapShot.MemUsage:0.##} MB");
                Console.WriteLine($"Disk I/O Usage:  {metricSnapShot.DiskIoUsage:0.##} %");
                Console.WriteLine($"Total Disk:      {metricSnapShot.DiskTotal:0.##} GB");
                Console.WriteLine($"Free Disk:       {metricSnapShot.DiskFree:0.##} GB");
                Console.WriteLine($"Used Disk:       {metricSnapShot.DiskUsed:0.##} GB");
                Console.WriteLine($"Disk Used %:     {metricSnapShot.DiskusedPercent:0.##} %");
                //creating new scope each iteration
                using (var scope = serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MonitoringDbContext>();

                    //metrics object to be saved to db
                    SystemMetrics metrics = new SystemMetrics
                    {
                        RecordedAt = DateTime.Now,
                        CpuUsagePercent = metricSnapShot.CpuUsage,
                        MemoryUsageMB = metricSnapShot.MemUsage,
                        DiskUsagePercent = metricSnapShot.DiskusedPercent,
                        CollectionIntervalMs = configinfo.MonitoringIntervalMilSeconds
                    };

                    db.SystemMetrics.Add(metrics);
                    db.SaveChanges();
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
