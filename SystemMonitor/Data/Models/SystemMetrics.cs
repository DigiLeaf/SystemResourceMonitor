using System;
using System.Collections.Generic;
using System.Text;

namespace SystemMonitor.Data.Models
{
    public class SystemMetrics
    {

        public int Id { get; set; }
        public DateTime RecordedAt { get; set; }
        public double CpuUsagePercent {  get; set; }
        public double MemoryUsageMB {  get; set; }
        public double DiskUsagePercent { get; set; }
        public int CollectionIntervalMs { get; set; }
    }

}
