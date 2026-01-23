using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SystemMonitor.Monitoring
{
    internal class CpuMonitor
    {
        
        private readonly PerformanceCounter cpuCounter;
        private bool isWarmedUp = false;
        public CpuMonitor() {
                                            //Category Name     Counter Name       Instance Name
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        }

        public float CpuUsage()
        {
            //for initial read
            if (!isWarmedUp)
            {
                //initializing cpu performance counter.
                float firstRead = cpuCounter.NextValue();
                Thread.Sleep(200);
                isWarmedUp = true;
            }

            return cpuCounter.NextValue();
        }



    }
}
