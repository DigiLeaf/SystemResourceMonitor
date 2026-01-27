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

class Program
{
    public class Config
    {
          public int MonitoringIntervalMilSeconds { get; set; }
          public int CPUThresholdPercent { get; set; }
          public string DefaultConnection { get; set; }

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

        //Database Connection
        ServiceCollection services = new ServiceCollection();

        services.AddDbContext<MonitoringDbContext>(options => options.UseSqlServer(configinfo.DefaultConnection));



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
                using (var db = serviceProvider.GetRequiredService<MonitoringDbContext>())
                {
                    // save your SystemMetrics object here
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
