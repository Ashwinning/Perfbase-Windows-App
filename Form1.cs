using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            
            OpenHardwareMonitor.Hardware.Computer computer = new OpenHardwareMonitor.Hardware.Computer();
            computer.Open();
            Console.WriteLine(computer.Test());
            computer.CPUEnabled = true;
            List<Perfbase.HardwareStats> stats = computer.GetPerfReport();
            Console.WriteLine(computer.Test());
            Console.WriteLine("Length = " + stats.Count);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(stats);
            //Console.Write(json);
        }
    }
}
