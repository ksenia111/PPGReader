using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PPGReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = 1000;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i * Math.PI / 4.0;
                y[i] = Math.Sin(x[i]);
            }

            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 100;
            chart1.Series[0].Points.DataBindXY(x, y);
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
