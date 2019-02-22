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
using System.IO;

namespace PPGReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private int[] read()
        {
            string filePath = textBox1.Text;
            int w = int.Parse(textBox2.Text);
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            int[] points = new int[stream.Length];
            stream.Close();

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int i = 0;
                while (reader.PeekChar() > -1)
                {
                    byte number = reader.ReadByte();
                    points[i] = Convert.ToInt16(Convert.ToString(number, 10));
                    i++;
                }
            }

            return points;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] points = read();
            int n = points.Length;
            double[] x = new double[n];
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = points[i];
            }

            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 1000;
            chart1.Series[0].Points.DataBindXY(x, y);
           
        }
    }
}
