using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Perfbase;

using System.Runtime.Serialization.Formatters.Binary; //For serializing to bytes.
using System.IO;

namespace Perfbase_Windows_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            GetReport();
            

            //test();

        }



        void GetReport()
        {
            Console.WriteLine("Getting Report");
            OpenHardwareMonitor.Hardware.Computer computer = new OpenHardwareMonitor.Hardware.Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.FanControllerEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.MainboardEnabled = true;
            computer.RAMEnabled = true;
            List<Perfbase.HardwareStats> stats = computer.GetPerfReport();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(stats);
            Console.WriteLine(" --- ");
            Console.Write(json);

        }

        /// <summary>
        /// Some test stuff I was working with 4 months ago.
        /// Leaving this here for now, method is not called anymore.
        /// 
        /// </summary>
        void test()
        {
            
            List<PerfProcess> processes = new List<PerfProcess>(); //Create a list to store our process info
            List<Process> localAll = Process.GetProcesses().ToList<Process>(); // Get all processes running on the local computer.
            
            //remove idle
            localAll.Remove(localAll.Single(s => s.ProcessName == "Idle"));

            //Create performance counters and counter samples for each process and save them in a list
            List<PCCS> pccs = new List<PCCS>();
            foreach (Process process in localAll)
            {
                PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", process.ProcessName);
                CounterSample cs1 = cpuCounter.NextSample();

                pccs.Add(new PCCS(cpuCounter, cs1));
            }

            //Sleep for a bit so we can get the process cpu consumption values
            System.Threading.Thread.Sleep(100);

            //Save all processes to list
            int i = 0; //track which process we're on
            foreach (Process process in localAll)
            {
                CounterSample cs2 = pccs[i].performanceCounter.NextSample();
                //Add values to processes list.
                PerfProcess newProcess = new PerfProcess();
                newProcess.name = process.ProcessName;
                newProcess.cpuUsage = CounterSample.Calculate(pccs[i].counterSample, cs2); //the final CPU counter
                newProcess.memUsage = ProcessMemoryUsage(process);
                processes.Add(newProcess);

                //increment index
                i++;
            }

            //Sort by name
            processes.Sort((a, b) => a.name.CompareTo(b.name));

            //display
            float total = 0;
            foreach (PerfProcess proc in processes)
            {
                Console.WriteLine(proc.name + " | " + proc.cpuUsage + " | " + proc.memUsage);
                total += proc.cpuUsage;
            }
            Console.WriteLine(total);
        }

        private float ProcessMemoryUsage(Process process)
        {
            var counter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
            return counter.NextValue() / 1024 / 1024;
        }

        class PCCS
        {
            public PerformanceCounter performanceCounter;
            public CounterSample counterSample;

            public PCCS(PerformanceCounter pc, CounterSample cs)
            {
                performanceCounter = pc;
                counterSample = cs;
            }
        }

    }
}
