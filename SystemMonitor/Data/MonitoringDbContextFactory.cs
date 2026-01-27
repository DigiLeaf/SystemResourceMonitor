using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemMonitor.Data
{
    public class MonitoringDbContextFactory : IDesignTimeDbContextFactory<MonitoringDbContext> {
        public MonitoringDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MonitoringDbContext>();
            optionsBuilder.UseSqlServer("Server=tcp:systemmonitorserver.database.windows.net,1433;Initial Catalog=SystemMonitorDB;Persist Security Info=False;User ID={YourId};Password={YourPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", options =>
            {
                options.EnableRetryOnFailure();
            });
            return new MonitoringDbContext(optionsBuilder.Options);
        }
    }

}
