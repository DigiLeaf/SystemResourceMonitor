using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SystemMonitor.Monitoring
{    internal class DiskCapacitySnapshot
    {
        public double TotalGB { get; set; }
        public double FreeGB { get; set; }
        public double UsedGB { get; set; }
        public double PercentUsed { get; set; }
    }
    internal class DiskMonitor
    {
        private readonly PerformanceCounter diskIoCounter;
        private readonly DriveInfo systemDrive;
        public DiskMonitor() {
                                                //Category name       Counter Name    Instance Name
            diskIoCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

            systemDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));


        }

        public float DiskIoUsagePercent()
        {
            return diskIoCounter.NextValue();
        }

        public DiskCapacitySnapshot GetDiskCapacitySnapshot()
        {
            //Getting disk size information
            long totalBytes = systemDrive.TotalSize;
            long freeBytes = systemDrive.AvailableFreeSpace;
            long usedBytes = totalBytes - freeBytes;
            double totalGB = totalBytes / (1024.0 * 1024 * 1024);
            double freeGB = freeBytes / (1024.0 * 1024 * 1024);
            double usedGB = usedBytes / (1024.0 * 1024 * 1024);

            double percentUsed = ((double)usedBytes / totalBytes) * 100;

            return new DiskCapacitySnapshot
            {
                TotalGB = totalGB,
                FreeGB = freeGB,
                UsedGB = usedGB,
                PercentUsed = percentUsed
            };
        }



    }


}
