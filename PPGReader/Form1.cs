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
        //вынесла в глобальные, чтобы можно было обращатся к ней из событий контекстного меню
        PPG ppg;
        // внесла их для запоминания координат, при клике правой кнопкой мыши (тогда появляется контестное меню, мышь двигается и все координаты сбиваются)
        double x;
        double y;
        // при клике правой кнопкой - был ли курсор на графике
        bool СursorOnChart = false;

        public Form1()
        {
            InitializeComponent();
            // создаем элементы меню
            ToolStripMenuItem beginPeriobMenuItem = new ToolStripMenuItem("Beginning of period");
            ToolStripMenuItem endPeriodMenuItem = new ToolStripMenuItem("End of period");
            ToolStripMenuItem aMenuItem = new ToolStripMenuItem("A");
            ToolStripMenuItem bMenuItem = new ToolStripMenuItem("B");
            ToolStripMenuItem cMenuItem = new ToolStripMenuItem("C");
            ToolStripMenuItem dMenuItem = new ToolStripMenuItem("D");
            ToolStripMenuItem eMenuItem = new ToolStripMenuItem("E");
            // добавляем элементы в меню
            contextMenuStrip1.Items.AddRange(new[] { beginPeriobMenuItem, endPeriodMenuItem, aMenuItem,
                                                     bMenuItem, cMenuItem, dMenuItem, eMenuItem});
            // ассоциируем контекстное меню с текстовым полем
            chart1.ContextMenuStrip = contextMenuStrip1;
            // устанавливаем обработчики событий для меню
            beginPeriobMenuItem.Click += beginPeriobMenuItem_Click;
            endPeriodMenuItem.Click += endPeriodMenuItem_Click;
            aMenuItem.Click += aMenuItem_Click;
            bMenuItem.Click += bMenuItem_Click;
            cMenuItem.Click += cMenuItem_Click;
            dMenuItem.Click += dMenuItem_Click;
            eMenuItem.Click += eMenuItem_Click;
            contextMenuStrip1.Enabled = true;
        }
      
        private int[] Read()
        {
            string filePath = @"J:\Documents\8 семестр\Диплом\plz\набор1\ГУЗЕЛЬ.plz"; //textBoxPath.Text; //@"D:\ВУЗ\4 курс\Диплом\ФПГ\plz\набор1\ГУЗЕЛЬ.plz";
            int w = int.Parse(textBoxW.Text);
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            int[] points = new int[stream.Length];
            stream.Close();

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int i = 0;
                while (reader.PeekChar() > -1)
                {
                    byte number = reader.ReadByte();
                    points[i] = Convert.ToInt16(Convert.ToString(number, 10))*w;
                    i++;
                }
            }

            return points;
        }

        public int[] CopyPoints(int[] points)  //для создания дубликата точек
        {
            int n = points.Length;
            int[] duplicatePoints = new int[n];

            for (int i = 0; i < n; i++)
            {
                duplicatePoints[i] = points[i];
            }

            return duplicatePoints;
        }

        /// <summary>
        /// прореживание
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private int[] Singling(int[] points)
        {
           int rate = int.Parse(textBoxSinglingRate.Text); //частота прореживания
           int n = points.Length / rate;
           int[] singlingPoints = new int[n];

           for (int i=0; i < n; i++)
            {

                singlingPoints[i] = points[(i+1) * rate - 1];
            }
            return singlingPoints;
        }

        /// <summary>
        /// Сглаживание
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private int[] MovingAverageMethod(int beginPeriod, int endPeriod, int window, int[] points) //метод скользящей средней
        {
            int p = window; //окно
            int m = (p - 1) / 2;
            int n = points.Length;// - m * 2;
            int[] SmoothedData = new int[n];

            for (int i = 0; i < m; i++)
            {
                SmoothedData[i] = points[i];
            }

            for (int i = n - m; i < n; i++)
            {
                SmoothedData[i] = points[i];
            }

            for (int i = m; i < n-m; i++) 
            {
                for (int j = i - m; j < i + m; j++) 
                SmoothedData[i] += points[j];
                SmoothedData[i] = SmoothedData[i] / p;
            }

            return SmoothedData;
        }

        int[] duplicatePoints;
        private void button1_Click(object sender, EventArgs e)
        {
            //чтение
            int[] points = Read();

            //прореживание
            int[] singlingPoints = Singling(points);

            duplicatePoints = singlingPoints;  // дубликат прореженных точек (я использую в сглаживании)
            
            //создание массива точекФПГ и ФПГ
            int n = singlingPoints.Length;

            PointPPG[] pointPPGs = new PointPPG[n];
            
            for (int i = 0; i < n; i++)
            {
                pointPPGs[i] = new PointPPG(i, singlingPoints[i]);
            }

            ppg = new PPG(pointPPGs, n);
            Draw(ppg);
        }

        //Рисование ФПГ
        private void Draw(PPG ppg)
        {
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 1000;
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chart1.Series[0].Points.DataBindXY(ppg.GetX(), ppg.GetY()); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            chart1.Series[0].Color = Color.Blue;
            chart1.Series[0].MarkerSize = 1;
        }

        private void Mark10Periods()
        {

        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            var result = chart1.HitTest(e.X, e.Y);
            
            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double pX = chart1.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double pY = chart1.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor
            richTextBox1.Text = Convert.ToString(pX) + "  " + Convert.ToString(pY); //вывод координат т-ки, на которую кликнули
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double pX = chart1.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double pY = chart1.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor

            var result = chart1.HitTest(e.X, e.Y);
            Cursor = result.ChartElementType == ChartElementType.DataPoint ? Cursors.Hand : Cursors.Default;
            chart1.Series[0].Color = result.ChartElementType == ChartElementType.DataPoint ? Color.Coral : Color.Blue; //у всего графика меняется цвет, как у одной точки заменить, пока не придумала
        }

        int b2click = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
            b2click++;
        }

        int b3click = 0;
        private void button3_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * 1.5;
            b3click++;
        }

        void ResetValue()
        {
            x = 0;
            y = 0;
            СursorOnChart = false;
        }

        private void beginPeriobMenuItem_Click(object sender, EventArgs e)
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                ppg.points[(int)x].IsBeginPeriod = true;
                chart1.Series[0].Points[(int)x].Label = "T1";
            }
            ResetValue();
        }

        private void endPeriodMenuItem_Click(object sender, EventArgs e)
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                ppg.points[(int)x].IsEndPeriod = true;
                chart1.Series[0].Points[(int)x].Label = "T2";
            }
            ResetValue();
        }

        private void aMenuItem_Click(object sender, EventArgs e)
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                ppg.points[(int)x].IsA = true;
                chart1.Series[0].Points[(int)x].Label = "A";
            }
            ResetValue();
        }

        private void bMenuItem_Click(object sender, EventArgs e)
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                ppg.points[(int)x].IsB = true;
                chart1.Series[0].Points[(int)x].Label = "B";
            }
            ResetValue();
        }

        private void cMenuItem_Click(object sender, EventArgs e)
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                ppg.points[(int)x].IsС = true;
                chart1.Series[0].Points[(int)x].Label = "C";
            }
            ResetValue();
        }

        private void dMenuItem_Click(object sender, EventArgs e)
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                ppg.points[(int)x].IsD = true;
                chart1.Series[0].Points[(int)x].Label = "D";
            }
            ResetValue();
        }

        private void eMenuItem_Click(object sender, EventArgs e)
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                ppg.points[(int)x].IsE = true;
                chart1.Series[0].Points[(int)x].Label = "E";
            }
            ResetValue();
        }

        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var result = chart1.HitTest(e.X, e.Y);
                СursorOnChart = result.ChartElementType == ChartElementType.DataPoint;
                x = chart1.ChartAreas[0].CursorX.Position;
                y =  chart1.ChartAreas[0].CursorY.Position;
            }
        }

        int increaseClick = 0;
        private void button1_Click_1(object sender, EventArgs e)
        {
            int window = 2;
            if (textBox1.Text != "") window = Convert.ToInt32(textBox1.Text);
            increaseClick = window;
            increaseClick += 1;
            textBox1.Text = Convert.ToString(increaseClick);
        }

        int decreaseClick = 0;
        private void button2_Click_1(object sender, EventArgs e)
        {
            int window = 2;
            if (textBox1.Text != "") window = Convert.ToInt32(textBox1.Text);
            
            if (window >= 3)
            {
                decreaseClick = window;
                decreaseClick -= 1;
                textBox1.Text = Convert.ToString(decreaseClick);
            }
            if (window < 3) MessageBox.Show("Невозможно уменьшить окно сглаживания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Hand);

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            int beginPeriod = 98;                             //нужно как-то будет брать отмеченные на графике 
            int endPeriod = 270;                              //нужно как-то будет брать отмеченные на графике 
            int Period = endPeriod - beginPeriod;
            int[] points = duplicatePoints;                   //взяли дубликат точек, уже прореженных
            int[] pointsForSmoothing = new int[Period];

            for (int i = 0; i < Period; i++)
            {
                pointsForSmoothing[i] = points[beginPeriod + i];         //берем только точки внутри периода
            }

            int window = Convert.ToInt32(textBox1.Text); 

            int[] smoothedData = new int[Period];
            smoothedData = MovingAverageMethod(beginPeriod, endPeriod, window, pointsForSmoothing);

            int j = 0;
            for (int i = 98; i < 270; i++)
            {
                points[i] = smoothedData[j];                //в дубликате заменяем точки в выбранном периоде на сглаженные
                j++;
            }

            //создание массива точекФПГ и ФПГ
            int n = points.Length;

            PointPPG[] pointPPGs = new PointPPG[n];

            for (int i = 0; i < n; i++)
            {
                pointPPGs[i] = new PointPPG(i, points[i]);
            }

            ppg = new PPG(pointPPGs, n);
            Draw(ppg);
            //если до сглаживания был изменен масштаб, то пытаемся такой же  масштаб сделать после сглаживания
            if (b2click != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / (1.5 * b2click);
            if (b3click != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * (1.5 * b3click);
        }

        
    }
}
