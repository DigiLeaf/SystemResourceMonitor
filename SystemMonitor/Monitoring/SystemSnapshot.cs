using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Text;

namespace SystemMonitor.Monitoring
{
    internal class FullMetricSnapShot
    {
        public DateTime TimeStamp { get; set; }
        public double CpuUsage { get; set; }
        public double MemUsage { get; set; }
        public double DiskIoUsage { get; set; }
        public double DiskTotal { get; set; }
        public double DiskUsed { get; set; }
        public double DiskFree { get; set; }
        public double DiskusedPercent { get; set; }
        
    }
    internal class SystemSnapshot
    {
        SystemMonitor.Monitoring.CpuMonitor cpuMonitor;
        SystemMonitor.Monitoring.MemoryMonitor memMonitor;

        SystemMonitor.Monitoring.DiskMonitor diskCounter;
        SystemMonitor.Monitoring.DiskCapacitySnapshot diskCapacitySnapshot;


        public SystemSnapshot() {
            //System Metrics
            memMonitor = new SystemMonitor.Monitoring.MemoryMonitor();

            diskCounter = new SystemMonitor.Monitoring.DiskMonitor();
            cpuMonitor = new SystemMonitor.Monitoring.CpuMonitor();
        }

        public FullMetricSnapShot GetSystemSnapshot()
        {
            diskCapacitySnapshot = diskCounter.GetDiskCapacitySnapshot();


            return new FullMetricSnapShot
            {
                TimeStamp = DateTime.Now,
                CpuUsage = cpuMonitor.CpuUsage(),
                MemUsage = memMonitor.RamUsage(),
                DiskIoUsage = diskCounter.DiskIoUsagePercent(),
                DiskTotal = diskCapacitySnapshot.TotalGB,
                DiskFree = diskCapacitySnapshot.FreeGB,
                DiskUsed = diskCapacitySnapshot.UsedGB,
                DiskusedPercent = diskCapacitySnapshot.PercentUsed
            };
        }
    }
}
