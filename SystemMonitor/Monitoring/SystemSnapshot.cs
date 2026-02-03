using Hardware.Info;
using NickStrupat;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;

namespace SystemMonitor.Monitoring
{
    internal class FullMetricSnapShot
    {
        public DateTime TimeStamp { get; set; }
        public double CpuUsage { get; set; }
        public double MemUsage { get; set; }
        public double TotalRAM { get; set; }
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
        double totalRamMB;
        ComputerInfo computerInfo;

        public SystemSnapshot() {
            //System Metrics
            memMonitor = new SystemMonitor.Monitoring.MemoryMonitor();

            diskCounter = new SystemMonitor.Monitoring.DiskMonitor();
            cpuMonitor = new SystemMonitor.Monitoring.CpuMonitor();

            computerInfo = new ComputerInfo();
           
        }

        public FullMetricSnapShot GetSystemSnapshot()
        {
            diskCapacitySnapshot = diskCounter.GetDiskCapacitySnapshot();
            totalRamMB = computerInfo.TotalPhysicalMemory / 1024 / 1024.0;

            return new FullMetricSnapShot
            {
                TimeStamp = DateTime.Now,
                CpuUsage = cpuMonitor.CpuUsage(),
                MemUsage = totalRamMB - memMonitor.RamAvaMBytes(), //total ram - available mb gives how much is actually in use.
                TotalRAM = totalRamMB,
                DiskIoUsage = diskCounter.DiskIoUsagePercent(),
                DiskTotal = diskCapacitySnapshot.TotalGB,
                DiskFree = diskCapacitySnapshot.FreeGB,
                DiskUsed = diskCapacitySnapshot.UsedGB,
                DiskusedPercent = diskCapacitySnapshot.PercentUsed
            };
        }
    }
}
