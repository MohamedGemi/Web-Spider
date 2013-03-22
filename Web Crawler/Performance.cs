using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace mission_impossible_code_assassin
{
    class Performance
    {
        public static void check_state()
        {
            while (true)
            {
                if (Data.Download_Thread != null) // Check If The Download Thread is Null Or Not
                {
                    if (Data.Download_Thread.ThreadState != System.Threading.ThreadState.Running)
                    {
                        Data.Download_Thread.Abort();
                        Data.downloading = false;
                    }
                }

                for (int i = 0; i < Data.nThreads; i++)
                {
                    if (Data.threads_parse[i] != null)
                    {
                        if (Data.threads_parse[i].ThreadState == System.Threading.ThreadState.Stopped)
                        {
                            
                            Data.threads_parse[i].Abort();
                            if (Data.threads_parse[i] != null)
                            if (Data.threads_parse[i].ThreadState == System.Threading.ThreadState.Running || Data.threads_parse[i].ThreadState == System.Threading.ThreadState.Suspended)
                                Data.threads_parse[i].Abort();
                            Data.threads_parse[i] = null;
                            Data.nThreads--;
                        }
                    }
                }
                Data._AREvt.WaitOne(10, true);
            }
        }
        public static void check_internet()
        {
            Monitor.Enter(Data.is_there_internet);
            while (true)
            {
                try
                {
                    IPHostEntry i = Dns.GetHostEntry("www.google.com"); // Send Request To Google
                    Data.is_there_internet = true;
                }
                catch
                {
                    Data.is_there_internet = false;
                }
            }
            Monitor.Exit(Data.is_there_internet);
        }
        public static void getramusage()
        {
            PerformanceCounter ram;
            ram = new PerformanceCounter("Memory", "Available MBytes", true);
            Data.available_ram = ram.NextValue().ToString();
        }
        public static void getCPUCOunter()
        {

            PerformanceCounter cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            // will always start at 0
            dynamic firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            string secondValue = cpuCounter.NextValue().ToString();

            Data.cpuUsage = secondValue;

        }
        public static void update_Usage()
        {
            Data.CpuRamStatus = new Thread(() => getCPUCOunter());
            Data.CpuRamStatus.Priority = ThreadPriority.Lowest;
            Data.CpuRamStatus.IsBackground = true;
            Data.CpuRamStatus.Start();
            Data.CpuRamStatus = new Thread(() => getramusage());
            Data.CpuRamStatus.Priority = ThreadPriority.Lowest;
            Data.CpuRamStatus.IsBackground = true;
            Data.CpuRamStatus.Start();
        }
    }
}
