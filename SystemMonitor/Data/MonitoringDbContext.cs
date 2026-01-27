using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SystemMonitor.Data.Models;

namespace SystemMonitor.Data
{
    public class MonitoringDbContext : DbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options) : base(options)
        {
        }
        public DbSet<SystemMetrics> SystemMetrics {get; set;}
    }
}
