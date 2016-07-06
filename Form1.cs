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
            /*
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
            */

            test();

        }

        void test()
        {
            // Get all processes running on the local computer.
            List<PerfProcess> processes = new List<PerfProcess>();
            Process[] localAll = Process.GetProcesses();
            foreach (Process process in localAll)
            {
                PerformanceCounter theCPUCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                //PerformanceCounter theMemCounter = new PerformanceCounter("Process", "Working Set", process.ProcessName);

                PerfProcess newProcess = new PerfProcess();
                newProcess.name = process.ProcessName;
                //newProcess.cpuUsage = theCPUCounter.NextValue();
                newProcess.memUsage = PerfProcessMemoryUsage(process);
                processes.Add(newProcess);
            }
            processes.Sort((a, b) => a.name.CompareTo(b.name));

            foreach (PerfProcess proc in processes)
            {
                Console.WriteLine(proc.name + " | " + proc.cpuUsage + " | " + proc.memUsage);
            }
        }

        private float PerfProcessMemoryUsage(Process process)
        {
            var counter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
            return counter.NextValue() / 1024 / 1024;
        }

    }
}
