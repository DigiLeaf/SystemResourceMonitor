using Microsoft.Identity.Client.Utils;
using System.Diagnostics;
using System.IO;

class Program
{

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World! This is the beginning of my System Resource Monitor");
       //Counter Assignment
                                                            //Category Name     Counter Name       Instance Name
       PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
       //initializing cpu performance counter.
       cpuCounter.NextValue();
       Thread.Sleep(500);

                                                            //Category Name    Counter Name 
        PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available Mbytes");

        PerformanceCounter diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

        //Get data of a known drive drive available
        DriveInfo mainDrive = new DriveInfo("C");

        long totalDriveSpace;
        long freeDriveSpace;
        while (true)
        {
             try
             {
                 //Drive Data
                 totalDriveSpace = mainDrive.TotalSize;
                 freeDriveSpace = mainDrive.AvailableFreeSpace;


                 Console.WriteLine("------------------------------------");
                 Console.WriteLine($"CPU Usage:    {cpuCounter.NextValue():0.##} %");
                 Console.WriteLine($"Availabe Ram: {ramCounter.NextValue():0.##} MB");
                 Console.WriteLine($"Disk Usage:   {diskCounter.NextValue():0.##} %");
                 Console.WriteLine($"Total Disk:   {totalDriveSpace / 1024 / 1024 / 1024} GB");
                 Console.WriteLine($"Free Disk:    {freeDriveSpace / 1024 / 1024 / 1024} GB");
             }
             catch (Exception ex){
                 Console.Error.WriteLine($"An error has occured: {ex.Message}");
             }
                //Pause between system readings
                Thread.Sleep(2000);

         }
        

    }
}
