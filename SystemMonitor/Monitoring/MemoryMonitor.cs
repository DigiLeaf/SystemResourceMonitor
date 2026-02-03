using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SystemMonitor.Monitoring
{
    internal class MemoryMonitor
    {
        private readonly PerformanceCounter ramCounter;
        public MemoryMonitor() {
                                             //Category Name    Counter Name 
            ramCounter = new PerformanceCounter("Memory", "Available Mbytes");
        }

        public float RamAvaMBytes()
        {
            return ramCounter.NextValue();
        }

    }
}
