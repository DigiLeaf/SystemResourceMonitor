using Microsoft.Identity.Client.Utils;
using System.Diagnostics;
using System.IO;
using SystemMonitor.Monitoring;

class Program
{

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World! This is the beginning of my System Resource Monitor");

        SystemSnapshot systemSnapshot = new SystemSnapshot();
        FullMetricSnapShot metricSnapShot;

        while (true)
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
            }
             catch (Exception ex){
                 Console.Error.WriteLine($"An error has occured: {ex.Message}");
             }
                //Pause between system readings
                Thread.Sleep(2000);

         }
        

    }
}
