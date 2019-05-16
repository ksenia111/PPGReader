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
using OfficeOpenXml;

namespace PPGReader
{
    public partial class Form1 : Form
    {
        //вынесла в глобальные, чтобы можно было обращатся к ней из событий контекстного меню
        PPG ppg;
        // добавила хранение для производной
        DPPG dppg;
        // внесла их для запоминания координат, при клике правой кнопкой мыши (тогда появляется контестное меню, мышь двигается и все координаты сбиваются)
        double x;
        double y;
        // при клике правой кнопкой - был ли курсор на графике
        bool СursorOnChart = false;
        bool IsDrawn = false;
        bool IsFoundDerivative = false;
        int[] duplicatePoints;
        int increaseScaleClick = 0;
        int decreaseScaleClick = 0;
        int increaseClick = 0;
        int decreaseClick = 0;
        int PeriodClick = 0;
        const int CountPeriods = 10;
        PeriodPPG[] Periods = new PeriodPPG[CountPeriods];
        double beginSmoothingPeriod = 0;
        double endSmoothingPeriod = 0;
        int clickTagSmoothingPeriod = 0;
        bool drowChartClick = false;
        bool fixSmoothing = false;
        bool smoothingClick = false;
        bool tagSmoothingPeriod = false;
        int[] forDrawing;

        public Form1()
        {
            InitializeComponent();
            // создаем элементы меню
            ToolStripMenuItem PeriobMenuItem = new ToolStripMenuItem("Period beginning/end");
            ToolStripMenuItem aMenuItem = new ToolStripMenuItem("A");
            ToolStripMenuItem bMenuItem = new ToolStripMenuItem("B");
            ToolStripMenuItem cMenuItem = new ToolStripMenuItem("C");
            ToolStripMenuItem dMenuItem = new ToolStripMenuItem("D");
            ToolStripMenuItem eMenuItem = new ToolStripMenuItem("E");
            // добавляем элементы в меню
            contextMenuStrip1.Items.AddRange(new[] { PeriobMenuItem, aMenuItem,
                                                     bMenuItem, cMenuItem, dMenuItem, eMenuItem});
            // ассоциируем контекстное меню с чартом
            chart1.ContextMenuStrip = contextMenuStrip1;
            // устанавливаем обработчики событий для меню
            PeriobMenuItem.Click += PeriobMenuItem_Click;
            aMenuItem.Click += aMenuItem_Click;
            bMenuItem.Click += bMenuItem_Click;
            cMenuItem.Click += cMenuItem_Click;
            dMenuItem.Click += dMenuItem_Click;
            eMenuItem.Click += eMenuItem_Click;
            contextMenuStrip1.Enabled = true;

            for(int i=0; i < Periods.Length;i++)
            {
                Periods[i] = new PeriodPPG();
            }
        }
      
        private int[] Read()
        {
            string filePath = @"J:\Documents\8 семестр\Диплом\plz\набор1\ГУЗЕЛЬ.plz";//@"D:\ВУЗ\4 курс\Диплом\ФПГ\plz\набор1\ГУЗЕЛЬ.plz"; //textBoxPath.Text; // //
            int w = int.Parse(textBoxW.Text);
            PeriodClick = 0;
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
            if (p % 2 == 0)
            {
                p++; //если окно четное, увеличиваем на 1 для симметрии
                label6.Text = Convert.ToString(p);
            }
            int m = (p - 1) / 2; //размах окна влево и вправо от текущей позиции
            int n = points.Length;// - m * 2;
            int[] SmoothedData = new int[n];

            SmoothedData[0] = points[0];
            int sum = 0;  
            int k1 = 0;  //начало окна
            int k2 = 0;  //конец окна
            int z = 0;   //размер окна

            for (int i=1;i<n;i++)
            {
                sum = 0;
                if (i < m) //в начале 
                {
                    k1 = 0;
                    k2 = 2 * i;
                    z = k2 + 1;
                }
                else if ((i + m) > (n - 1))
                {
                    k1 = i - n + i + 1;
                    k2 = n - 1;
                    z = k2 - k1 + 1;
                }
                else
                {
                    k1 = i - m;
                    k2 = i + m;
                    z = p;
                }

                for (int j = k1; j <= k2; j++)
                {
                    sum += points[j];
                }
                SmoothedData[i] = sum / z;
            }

            return SmoothedData;
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            drowChartClick = true;

            //чтение
            int[] points = Read();

            //прореживание
            int[] singlingPoints = Singling(points);

            duplicatePoints = singlingPoints;  // дубликат прореженных точек (я использую в сглаживании)

            forDrawing = CopyPoints(duplicatePoints);

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
            
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 400;
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chart1.Series[0].Points.DataBindXY(ppg.GetX(), ppg.GetY()); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            chart1.Series[0].Color = Color.Blue;
            chart1.Series[0].MarkerSize = 1;
            IsDrawn = true;
        }

        //Рисование ДФПГ
        private void Draw(DPPG dppg)
        {
            //chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            ////chart1.Series[2].ChartType = SeriesChartType.Line;
            //chart1.ChartAreas[0].AxisX.ScaleView.Size = 1000;
            //chart1.Series[2].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            //chart1.Series[2].ToolTip = "X = #VALX, Y = #VALY";
            //chart1.Series[2].Points.DataBindXY(dppg.GetX(), dppg.GetY()); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            //chart1.Series[2].Color = Color.Red;
            //chart1.Series[2].MarkerSize = 1;
            chart2.ChartAreas[0].AxisX.MaximumAutoSize = 20;//chart1.ChartAreas[0].AxisX.Interval;

            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //chart1.Series[2].ChartType = SeriesChartType.Line;
            chart2.ChartAreas[0].AxisX.ScaleView.Size = 400;
            chart2.Series[0].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            chart2.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chart2.Series[0].Points.DataBindXY(dppg.GetX(), dppg.GetY()); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            chart2.Series[0].Color = Color.ForestGreen;
            chart2.Series[0].MarkerSize = 1;

        }

        //Рисование массива х и у
        private void Draw(int[] x,int[] y)
        {
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 400;
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chart1.Series[0].Points.DataBindXY(x, y); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            chart1.Series[0].Color = Color.Blue;
            chart1.Series[0].MarkerSize = 1;
        }


        
        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            var result = chart1.HitTest(e.X, e.Y);
            
            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double pX = chart1.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double pY = chart1.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor
            richTextBox1.Text = Convert.ToString(pX) + "  " + Convert.ToString(pY); //вывод координат т-ки, на которую кликнули

            if (tagSmoothingPeriod == true)
            {
                clickTagSmoothingPeriod++;
                СursorOnChart = result.ChartElementType == ChartElementType.DataPoint;

                if (СursorOnChart == false)
                {
                    MessageBox.Show(
                                    "Курсор находился не на графике, попробуйте еще раз",
                                    "Сообщение",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    clickTagSmoothingPeriod--;
                }
                else
                {
                    if (clickTagSmoothingPeriod == 1)
                    {
                        beginSmoothingPeriod = chart1.ChartAreas[0].CursorX.Position;
                        textBox2.Text = "Начало: " + Convert.ToString(beginSmoothingPeriod);
                    }

                    if (clickTagSmoothingPeriod == 2)
                    {
                        endSmoothingPeriod = chart1.ChartAreas[0].CursorX.Position;
                        textBox2.Text += ". Конец: " + Convert.ToString(endSmoothingPeriod);
                        clickTagSmoothingPeriod = 0;
                        tagSmoothingPeriod = false;
                    }
                }
            }
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double pX = chart1.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double pY = chart1.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor

            var result = chart1.HitTest(e.X, e.Y);
            Cursor = result.ChartElementType == ChartElementType.DataPoint ? Cursors.Hand : Cursors.Default;
            //chart1.Series[0].Color = result.ChartElementType == ChartElementType.DataPoint ? Color.Coral : Color.Blue; //у всего графика меняется цвет, как у одной точки заменить, пока не придумала
            chart1.Series[0].Color = result.Series == chart1.Series[0] ? Color.Coral : Color.Blue;

            chart2.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chart2.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double dpX = chart2.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double dpY = chart2.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor

            var dresult = chart2.HitTest(e.X, e.Y);
            Cursor = dresult.ChartElementType == ChartElementType.DataPoint ? Cursors.Hand : Cursors.Default;
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
            chart2.ChartAreas[0].AxisX.ScaleView.Size = chart2.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
            increaseScaleClick++;
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * 1.5;
            chart2.ChartAreas[0].AxisX.ScaleView.Size = chart2.ChartAreas[0].AxisX.ScaleView.Size * 1.5;
            decreaseScaleClick++;
        }

        //подписи к первым 10 периодам, Юля, тебе нужно использовать этот метод, а не для 1 периода
        private void WriteLabelForChart()
        {
            for (int i = 0; i < CountPeriods; i++)
            {
                if (Periods[i].Begin != 0) chart1.Series[0].Points[Periods[i].Begin].Label = "T";
                if (Periods[i].End != 0) chart1.Series[0].Points[Periods[i].End].Label = "T";
                if (Periods[i].A != 0) chart1.Series[0].Points[Periods[i].A].Label = "A";
                if (Periods[i].B != 0) chart1.Series[0].Points[Periods[i].B].Label = "B";
                if (Periods[i].C != 0) chart1.Series[0].Points[Periods[i].C].Label = "C";
                if (Periods[i].D != 0) chart1.Series[0].Points[Periods[i].D].Label = "D";
                if (Periods[i].E != 0) chart1.Series[0].Points[Periods[i].E].Label = "E";
            }
        }

        //подписи к 1 периоду
        private void WriteLabelForChart(PeriodPPG periodPPG)
        {
           if (periodPPG.Begin != 0) chart1.Series[0].Points[periodPPG.Begin].Label = "T";
           if (periodPPG.End != 0) chart1.Series[0].Points[periodPPG.End].Label = "T";
           if (periodPPG.A != 0) chart1.Series[0].Points[periodPPG.A].Label = "A";
           if (periodPPG.B != 0) chart1.Series[0].Points[periodPPG.B].Label = "B";
           if (periodPPG.C != 0) chart1.Series[0].Points[periodPPG.C].Label = "C";
           if (periodPPG.D != 0) chart1.Series[0].Points[periodPPG.D].Label = "D";
           if (periodPPG.E != 0) chart1.Series[0].Points[periodPPG.E].Label = "E";
        }

        void ResetValue()
        {
            x = 0;
            y = 0;
            СursorOnChart = false;
        }

        private void PeriobMenuItem_Click(object sender, EventArgs e)
        {
            if(PeriodClick == 0)
            {
                Periods[PeriodClick].Begin = (int)x;
            }
            else if(PeriodClick != CountPeriods)
            {
                Periods[PeriodClick - 1].End = (int)x;
                Periods[PeriodClick].Begin = (int)x;
            }
            else
            {
                Periods[CountPeriods - 1].End = (int)x;
            }
            
            if (CheckCursorOnChart())
            {
                chart1.Series[0].Points[(int)x].Label = "T";
                PeriodClick++;
            }
            if (PeriodClick == CountPeriods + 1)
            {
                MessageBox.Show(
                                "10 периодов отмечены. Отметьте на них характерные точки и нажмите на кнопку \"Найти характеристики\" " +
                                "для нахождения следующих точек",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                
            }
            ResetValue();
        }

       
      

        private void MarkCharacteristic(int idxPeriod, ref int point, string label)
        {
            if (CheckCursorOnChart())
            {
                if (BelongPeriod((int)x, idxPeriod) && BelongPeriod(point, idxPeriod))
                {
                    MessageBox.Show(
                                    "На этом периоде уже отмечена точка " + label + ", отметьте другую точку",
                                    "Сообщение",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    point = (int)x;
                    chart1.Series[0].Points[(int)x].Label = label;
                    ResetValue();

                }
            }
        }

        private bool BelongPeriod(int point, int idxPeriod)
        {
            if ((point > Periods[idxPeriod].Begin) && (point < Periods[idxPeriod].End))
            {
                return true;
            }
            return false;
        }

        private bool CheckCursorOnChart()
        {
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return false;

            }else
             return true;
        }

        private bool CheckMarkPeriod(int idxPeriod)
        {
            if (idxPeriod == -1)
            {
                MessageBox.Show(
                                "Сначала отметьте границы периода",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return false;

            }
            else
                return true;
        }

        private void aMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].A, "A");
            }
        }

        private void bMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].B, "B");
            }
        }

        private void cMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
               MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].C, "C");
            }
        }

        private void dMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod,  ref Periods[idxPeriod].D, "D");
            }
        }

        private void eMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].E, "E");
            }
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

        //увеличение окна
        private void button1_Click_1(object sender, EventArgs e)
        {
            int window = Convert.ToInt32(label6.Text);
            increaseClick = window;
            increaseClick += 2;
            label6.Text = Convert.ToString(increaseClick);
        }

        //уменьшение окна
        private void button2_Click_1(object sender, EventArgs e)
        {
            int window = Convert.ToInt32(label6.Text);
            
            if (window > 3)
            {
                decreaseClick = window;
                decreaseClick -= 2;
                label6.Text = Convert.ToString(decreaseClick);
            }
            if (window <= 3) MessageBox.Show("Невозможно уменьшить окно сглаживания", 
                                             "Ошибка", 
                                             MessageBoxButtons.OK, 
                                             MessageBoxIcon.Hand);

        }

        //"отметить период сглаживания"
        private void button4_Click(object sender, EventArgs e)
        {
            tagSmoothingPeriod = true;
            textBox2.Text = "";
            beginSmoothingPeriod = 0;
            endSmoothingPeriod = 0;
        }

        
        //сглаживание
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (endSmoothingPeriod == 0) MessageBox.Show("Отметьте период сглаживания",
                                             "Ошибка",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Hand);
            else
            {
                int beginPeriod = Convert.ToInt32(beginSmoothingPeriod);                             
                int endPeriod = Convert.ToInt32(endSmoothingPeriod);                              
                int Period = endPeriod - beginPeriod;
                //int[] points = duplicatePoints;                   //взяли дубликат точек, уже прореженных
                int[] pointsForSmoothing = new int[Period];
                fixSmoothing = false;
                smoothingClick = true;

                for (int i = 0; i < Period; i++)
                {
                    pointsForSmoothing[i] = forDrawing[beginPeriod + i];         //берем только точки внутри периода
                }

                int window = Convert.ToInt32(label6.Text);

                int[] smoothedData = new int[Period];
                smoothedData = MovingAverageMethod(beginPeriod, endPeriod, window, pointsForSmoothing);

                int j = 0;
                for (int i = beginPeriod; i < endPeriod; i++)
                {
                    //duplicatePoints[i] = smoothedData[j];
                    forDrawing[i] = smoothedData[j];
                    //points[i] = smoothedData[j];                //в дубликате заменяем точки в выбранном периоде на сглаженные
                    j++;
                }

                int n = forDrawing.Length;
                int[] x = new int[n];
                for (int i = 0; i < n; i++) 
                {
                    x[i] = i;
                }

                Draw(x, forDrawing);

                //если до сглаживания был изменен масштаб, то пытаемся такой же  масштаб сделать после сглаживания
                if (increaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / (1.5 * increaseScaleClick);
                if (decreaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * (1.5 * decreaseScaleClick);

                ////создание массива точекФПГ и ФПГ
                //int n = points.Length;

                //PointPPG[] pointPPGs = new PointPPG[n];

                //for (int i = 0; i < n; i++)
                //{
                //    pointPPGs[i] = new PointPPG(i, points[i]);
                //}

                //ppg = new PPG(pointPPGs, n);
                //Draw(ppg);
                ////если до сглаживания был изменен масштаб, то пытаемся такой же  масштаб сделать после сглаживания
                //if (increaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / (1.5 * increaseScaleClick);
                //if (decreaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * (1.5 * decreaseScaleClick);
            }
        }

        //отменить сглаживание
        private void button6_Click(object sender, EventArgs e)
        {
            if (fixSmoothing == true) MessageBox.Show("Невозможно отменить сглаживание, т.к. вы нажали кнопку \"Применить\"",
                                               "Ошибка",
                                               MessageBoxButtons.OK,
                                               MessageBoxIcon.Hand);
            else
            {
                if (smoothingClick == false) MessageBox.Show("Невозможно отменить сглаживание, т.к. оно не было выполнено, или вы нажали кнопку \"Применить\"",
                                               "Ошибка",
                                               MessageBoxButtons.OK,
                                               MessageBoxIcon.Hand);
                else
                { 
                    int beginPeriod = Convert.ToInt32(beginSmoothingPeriod);
                    int endPeriod = Convert.ToInt32(endSmoothingPeriod);
                    int Period = endPeriod - beginPeriod;

                    for (int i = beginPeriod; i < endPeriod; i++)
                    {
                        forDrawing[i] = duplicatePoints[i];
                    }

                    int n = duplicatePoints.Length;
                    int[] x = new int[n];
                    for (int i = 0; i < n; i++) 
                    {
                        x[i] = i;
                    }

                    Draw(x, forDrawing);
                    if (increaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / (1.5 * increaseScaleClick);
                    if (decreaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * (1.5 * decreaseScaleClick);
                }
            }
        }


        
        //применить сглаживание
        private void button7_Click(object sender, EventArgs e)
        {
            if (smoothingClick == false) MessageBox.Show("Невозможно применить сглаживание, т.к. оно не было выполнено. Выберите период сглаживания и нажмите \"сглдить период\". После этого можете применить сглаживание",
                                               "Ошибка",
                                               MessageBoxButtons.OK,
                                               MessageBoxIcon.Exclamation);
            else
            {
                fixSmoothing = true;
                smoothingClick = false;

                int n = duplicatePoints.Length;
                for (int i = 0; i < n; i++)
                {
                    duplicatePoints[i] = forDrawing[i];
                }

                //создание массива точекФПГ и ФПГ

                PointPPG[] pointPPGs = new PointPPG[n];

                for (int i = 0; i < n; i++)
                {
                    pointPPGs[i] = new PointPPG(i, duplicatePoints[i]);
                }

                ppg = new PPG(pointPPGs, n);
                Draw(ppg);
                //если до сглаживания был изменен масштаб, то пытаемся такой же  масштаб сделать после сглаживания
                if (increaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / (1.5 * increaseScaleClick);
                if (decreaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * (1.5 * decreaseScaleClick);
            }
        }

        private void FullSearchCharacteristics_Click(object sender, EventArgs e)
        {
            DialogResult mesageResult = CheckData();
            if (mesageResult == DialogResult.OK)
            {
                ReadStartCharacteristics();
                contextMenuStrip1.Enabled = false;
                int averageLengthPeriod = 0, A_RelativePosition = 0, B_RelativePosition = 0, C_RelativePosition = 0, D_RelativePosition = 0, E_RelativePosition = 0;
                UpdateRelativePosition(ref averageLengthPeriod, ref A_RelativePosition, ref B_RelativePosition,
                                       ref C_RelativePosition, ref D_RelativePosition, ref E_RelativePosition);

                int previousIdxP = CountPeriods-1;
                int currentIdxP = CountPeriods;
                List<PeriodPPG> periodPPGs = new List<PeriodPPG>();
                
                AddMarkedPeriods(ref periodPPGs);
                int updateData = CountPeriods;

                while (periodPPGs.Last().End + averageLengthPeriod < ppg.points.Length)
                {
                    // добавить проверку на критические точки и сглаживание, обновление через 10 периодов для с,d,e
                    PeriodPPG currentPeriod = new PeriodPPG();
                    updateData = currentIdxP % CountPeriods;
                    if (updateData == 0)
                    {
                        UpdateRelativePosition(ref averageLengthPeriod, ref A_RelativePosition, ref B_RelativePosition,
                                       ref C_RelativePosition, ref D_RelativePosition, ref E_RelativePosition);
                    }

                    /* if(CalcCountExtr(Periods[currentIdxP].Begin, Periods[currentIdxP].End)>5)
                     {

                     }*/

                    currentPeriod = CalcCurrentPeriod(averageLengthPeriod, D_RelativePosition, C_RelativePosition,50, 5,10,5,10,5,periodPPGs);
                    WriteLabelForChart(currentPeriod);
                    periodPPGs.Add(currentPeriod);

                    previousIdxP = currentIdxP;
                    currentIdxP++;
                }
            }
        }

        private PeriodPPG CalcCurrentPeriod(int averageLengthPeriod, int D_RelativePosition, int С_RelativePosition,
                                            int end_percent, int end_percentD, int c_percent, int c_percentD, int d_percent, int d_percentD, List<PeriodPPG> periodPPGs)
        {
            PeriodPPG currentPeriod = new PeriodPPG();
            currentPeriod.Begin = periodPPGs.Last().End;
            currentPeriod.End = CalcEndPeriod(currentPeriod.Begin, averageLengthPeriod, end_percent, end_percentD);
            currentPeriod.B = CalcB(currentPeriod.Begin, currentPeriod.End);
            currentPeriod.A = CalcA(currentPeriod.Begin, currentPeriod.B);
            currentPeriod.C = CalcС(С_RelativePosition, currentPeriod.Length(), c_percent, c_percentD, currentPeriod.Begin, currentPeriod.B, currentPeriod.End);
            if (currentPeriod.C == -1)
            {
                currentPeriod.C = 0;
            }
            else
            {
                currentPeriod.D = CalcD(D_RelativePosition, currentPeriod.Length(), d_percent, d_percentD, currentPeriod.Begin, currentPeriod.C, currentPeriod.End);
            }
            return currentPeriod;
        }

        private void AddMarkedPeriods(ref List<PeriodPPG> periodPPGs)
        {
            for(int i = 0; i < CountPeriods;i++)
            {
                periodPPGs.Add(Periods[i]);
            }
        }

        private DialogResult CheckData()
        {
            DialogResult mesageResult = DialogResult.None;
            if (IsDrawn == false)
            {
                MessageBox.Show(
                               "Нарисуйте график ФПГ",
                               "Сообщение",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
            } else if(IsFoundDerivative == false)
            {
                MessageBox.Show(
                               "Нажмите на кнопку \"Найти производную\"",
                               "Сообщение",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
            }
            else if (PeriodClick != CountPeriods+1)
            {
                mesageResult = MessageBox.Show(
                                 "Отметьте характерные точки на 10 периодах," +
                                 "или если точки заданы в файле Excel, нажмите ОК",
                                 "Сообщение",
                                 MessageBoxButtons.OKCancel,
                                 MessageBoxIcon.Information);
            }

            return mesageResult;
        }

        private void UpdateRelativePosition(ref int averageLengthPeriod, ref int A_RelativePosition, ref int B_RelativePosition, 
                                            ref int C_RelativePosition, ref int D_RelativePosition, ref int E_RelativePosition)
        {
            averageLengthPeriod = CalcAverageLengthPeriod();
            A_RelativePosition = CalcRelativePosition(Periods.Select(p => p.A).ToArray(), CountPeriods);
            B_RelativePosition = CalcRelativePosition(Periods.Select(p => p.B).ToArray(), CountPeriods);
            C_RelativePosition = CalcRelativePosition(Periods.Select(p => p.B).ToArray(), CountPeriods);
            D_RelativePosition = CalcRelativePosition(Periods.Select(p => p.B).ToArray(), CountPeriods);
            E_RelativePosition = CalcRelativePosition(Periods.Select(p => p.B).ToArray(), CountPeriods);
        }

        private int CalcCountExtr(int begin, int end)
        {
            int count = 0;
            for (int i =begin; i < end; i++)
            {
                if (ppg.points[i].y == 0)
                {
                    count++;
                }
            }

            return count;
        }

        private int CalcB( int begin, int  end)
        {
            int idxB = -1;
            int max = int.MinValue;
            for (int i = begin; i < end; i++)
            {
                if (ppg.points[i].y > max)
                {
                    max = ppg.points[i].y;
                    idxB = i;
                }
            }

            return idxB;
        }

        //не правильно работает
        private int CalcD(int D_RelativePosition, int periodLength, int percent, int percentD, int beginPeriod, int begin, int end)
        {
            int predictionD = (int)Math.Round((Convert.ToDouble(D_RelativePosition) / 100) * periodLength);
            int idxD = beginPeriod + predictionD;
            int searchInterval = percent * periodLength / 100;
            
            for (int i = begin; i < end; i++)
            {
                if (IsMax(dppg.points[i].y, dppg.points[i - 1].y, dppg.points[i + 1].y))
                {
                    return i;
                }
            }

            idxD = CheckDerivativeMax(periodLength, percentD, idxD);


            return idxD;
        }

        //не правильно находит
        private int CalcС(int С_RelativePosition, int periodLength, int percent, int percentD, int beginPeriod, int begin, int end)
        {
            int predictionС = (int)Math.Round((Convert.ToDouble(С_RelativePosition) / 100) * periodLength);
            int idxC =-1;
            int searchInterval = percent * periodLength / 100;
            for (int i = begin; i < end; i++)
            {
                if (IsMin(dppg.points[i].y, dppg.points[i - 1].y, dppg.points[i + 1].y))
                {
                    return i;
                }
            }

            return idxC;
        }


        private int CalcA(int begin, int end)
        {
            int idxA = -1;
            double maxDPPG = int.MinValue;
            for (int i = begin; i < end; i++)
            {
                if(dppg.points[i].y > maxDPPG)
                {
                    maxDPPG = dppg.points[i].y;
                    idxA = i;
                }
            }
            return idxA;
        }

        private int CalcEndPeriod(int begin, int averageLength, int percent, int percentD)
        {
            int searchInterval = percent * averageLength / 100;
            int min = int.MaxValue;
            int idxEnd = -1;
            int idxSearchEnd = begin + averageLength + searchInterval;
            if (idxSearchEnd > ppg.points.Length)
            {
                idxSearchEnd = ppg.points.Length;
            }
            for (int i = begin + searchInterval; i < idxSearchEnd; i++)
            {
                if (ppg.points[i].y < min)
                {
                    min = ppg.points[i].y;
                    idxEnd = i;
                }
            }
            
            idxEnd = CheckDerivativeMax(averageLength, percentD,  idxEnd);

            return idxEnd;

        }

        bool IsMax(double Derivative, double previousDerivative, double nextDerivative)
        {
            return ((Derivative == 0) && (previousDerivative < 0) && (nextDerivative > 0));
        }

        bool IsMin(double Derivative, double previousDerivative, double nextDerivative)
        {
            return ((Derivative == 0) && (previousDerivative > 0) && (nextDerivative < 0));
        }

        private int CheckDerivativeMax(int lengthPeriod, int percentD, int idx)
        {
            if (IsMax(dppg.points[idx].y,dppg.points[idx-1].y ,dppg.points[idx + 1].y))
            {
                return idx;
            }

            int searchInterval = percentD * lengthPeriod / 100;
            int idxSearchEnd = idx + searchInterval;

            if (idxSearchEnd > ppg.points.Length)
            {
                idxSearchEnd = ppg.points.Length;
            }

            for (int i = idx - searchInterval; i < idxSearchEnd; i++)
            {
                if (IsMax(dppg.points[idx].y, dppg.points[idx - 1].y, dppg.points[idx + 1].y))
                {
                    idx = i;
                }
            }

            return idx;
        }

        private int CheckDerivativeMin(int lengthPeriod, int percentD, int idx)
        {
            if (IsMin(dppg.points[idx].y, dppg.points[idx - 1].y, dppg.points[idx + 1].y))
            {
                return idx;
            }

            int searchInterval = percentD * lengthPeriod / 100;
            int idxSearchEnd = idx + searchInterval;

            if (idxSearchEnd > ppg.points.Length)
            {
                idxSearchEnd = ppg.points.Length;
            }

            for (int i = idx - searchInterval; i < idxSearchEnd; i++)
            {
                if (IsMin(dppg.points[idx].y, dppg.points[idx - 1].y, dppg.points[idx + 1].y))
                {
                    idx = i;
                }
            }

            return idx;
        }


        private int CalcAverageLengthPeriod()
        {
            int sum = 0;
            for (int i = 0; i < CountPeriods; i++)
            {
                sum += Periods[i].Length();
            }
             
            return sum / CountPeriods;
        }

        private int CalcRelativePosition(int[] point, int lengthForCalc)
        {
            int[] RelativePosition = new int[lengthForCalc];
            int idxPeriod = 0;
            for (int i=0;i< lengthForCalc; i++)
            {
                idxPeriod = CalcPeriod(point[i]);
                RelativePosition[i] = (int)Math.Round((Convert.ToDouble(point[i] - Periods[idxPeriod].Begin) / 
                                        Periods[idxPeriod].Length()) * 100);
            }

            int sum = 0;
            for (int i = 0; i < lengthForCalc; i++)
            {
                sum += RelativePosition[i];
            }

            return sum / lengthForCalc;
        }

        private int CalcPeriod(int point)
        {
            for(int i = 0; i < CountPeriods;i++)
            {
                if ((point > Periods[i].Begin)&&(point < Periods[i].End))
                {
                    return i;
                }
            }
            return -1;
        }

        public double[] differentiation1orderAccuracy(int[] points)
        {
            int n = points.Length;
            double[] derivativePoints = new double[n];
            int h = 1;

            derivativePoints[0] = (points[1] - points[0]) / h;
            derivativePoints[n - 1] = (points[n - 1] - points[n - 2]) / h;
            for (int i = 1; i < n - 1; i++)
            {
                derivativePoints[i] = (points[i + 1] - points[i]) / (2 * h);
            }

            return derivativePoints;
        }

        public double[] differentiation2orderAccuracy(int[] points)
        {
            int n = points.Length;
            double[] derivativePoints = new double[n];
            int h = 1;

            for (int i = n - 2; i > 0; i--)
            {
                derivativePoints[i + 1] = (points[i - 1] - 4 * points[i] + 3 * points[i + 1]) / (2 * h);
                //if (derivativePoints[i + 1] == 0)
                //{
                //    richTextBox1.Text += Convert.ToString(i + 1) + '\n';
                //}
            }

            return derivativePoints;
        }

        public double[] differentiation4points(int[] points)
        {
            int n = points.Length;
            double[] derivativePoints = new double[n];
            int h = 1;

            for (int i = 2; i < n - 2; i++)
            {
                derivativePoints[i] = (points[i - 2] - 8 * points[i - 1] + 8 * points[i + 1] - points[i + 2]) / (12 * h);
                //if (derivativePoints[i + 1] == 0)
                //{
                //    richTextBox1.Text += Convert.ToString(i + 1) + '\n';
                //}
            }

            return derivativePoints;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (drowChartClick == false)
            {
                MessageBox.Show("Для нахождения производной сначала нужно нарисовать график ФПГ. Нажмите \" Нарисовать гравик\"",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            else
            {
                richTextBox1.Text = "";
                int n = duplicatePoints.Length;
                PointDPPG[] pointDPPG = new PointDPPG[n];
                double[] derivativePoints = new double[n];
                //derivativePoints = differentiation1orderAccuracy(duplicatePoints);
                //derivativePoints = differentiation2orderAccuracy(duplicatePoints);
                derivativePoints = differentiation4points(duplicatePoints);

                for (int i = 0; i < n; i++)
                {
                    pointDPPG[i] = new PointDPPG(i, derivativePoints[i]);
                }
                dppg = new DPPG(pointDPPG, n);
                IsFoundDerivative = true;
                Draw(dppg);
                if (increaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / (1.5 * increaseScaleClick); 
                if (decreaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * (1.5 * decreaseScaleClick);
            }
        }

        private void WriteExcel(string [] nameСolumns, int [,] valueColumns, int countColumns, int countValues, 
                                string nameFile, string nameSheet)
        {
            //Create a new ExcelPackage
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "PPGReader";
                excelPackage.Workbook.Properties.Title = "Characteristics";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(nameSheet);

                for(int j = 1;j <= countColumns;j++)
                {
                    worksheet.Cells[1, j].Value = nameСolumns[j-1];
                }
                for (int j = 1; j <= countColumns; j++)
                {
                    for (int i = 2; i <= countValues+1; i++)
                    {
                        worksheet.Cells[i, j].Value = valueColumns[j - 1, i - 2];
                    }
                }

                //Save your file
                FileInfo file = new FileInfo(nameFile);//"D:\ВУЗ\4 курс\Диплом\DATA\Characteristics.xlsx"
                excelPackage.SaveAs(file);
            }

        }

        private void ReadStartCharacteristics()
        {
            string namefile = @"D:\ВУЗ\4 курс\Диплом\DATA\StartCharacteristics.xlsx";
            string[,] excelData = ReadExcelSheet(namefile);
            int rows = excelData.GetUpperBound(0) + 1;
            int columns = excelData.Length / rows;
            for (int j = 1; j < columns; j++)
            {
                for(int i = 1; i < rows; i++)
                {
                    switch (i)
                    {
                        case 1:
                            Periods[j-1].Begin = Convert.ToInt32(excelData[i,j]);
                            break;
                        case 2:
                            Periods[j-1].End = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 3:
                            Periods[j-1].A = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 4:
                            Periods[j-1].B = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 5:
                            Periods[j-1].C = Convert.ToInt32(excelData[i, j]);
                            break;
                        case 6:
                            Periods[j-1].D = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 7:
                            Periods[j-1].E = Convert.ToInt32(excelData[i, j]); ;
                            break;
                    }
                }
            }

            WriteLabelForChart();
        }

       

        private string [,] ReadExcelSheet(string namefile)
        {
            //read the Excel file as byte array
            byte[] bin = File.ReadAllBytes(namefile);
            string[,] excelData=null;
            
            //create a new Excel package in a memorystream
            using (MemoryStream stream = new MemoryStream(bin))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //loop all worksheets
                foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
                {
                    excelData = new string[worksheet.Dimension.End.Column, worksheet.Dimension.End.Row];
                    //loop all rows
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        //loop all columns in a row
                        for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                        {
                            //add the cell data to the List
                            if (worksheet.Cells[i, j].Value != null)
                            {
                                excelData[j-1,i-1]= worksheet.Cells[i, j].Value.ToString();
                            }
                        }
                    }
                }
                
            }
            return excelData;
        }

        private void WriteCharacteristics_Click(object sender, EventArgs e)
        {
            if (PeriodClick != CountPeriods+1)
            {
                MessageBox.Show(
                                 "Нарисуйте график ФПГ и отметьте характерные точки на 10 периодах",
                                 "Сообщение",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
            }
            else
            {
                const int countColumns = 8;
                string[] nameСolumns = new string[countColumns] { "Период", "Начало", "Конец", "A", "B", "C", "D", "E" };
                int[,] valueColumns = new int[countColumns,CountPeriods];
                for (int i = 0; i < countColumns; i++)
                {
                    for (int j = 0; j < CountPeriods; j++)
                    {
                        switch (i)
                        {
                            case 0:
                                valueColumns[i, j] = j+1;
                                break;
                            case 1:
                                valueColumns[i, j] = Periods[j].Begin;
                                break;
                            case 2:
                                valueColumns[i, j] = Periods[j].End;
                                break;
                            case 3:
                                valueColumns[i, j] = Periods[j].A;
                                break;
                            case 4:
                                valueColumns[i, j] = Periods[j].B;
                                break;
                            case 5:
                                valueColumns[i, j] = Periods[j].C;
                                break;
                            case 6:
                                valueColumns[i, j] = Periods[j].D;
                                break;
                            case 7:
                                valueColumns[i, j] = Periods[j].E;
                                break;
                        }
                    }
                }
                string nameFile = @"D:\ВУЗ\4 курс\Диплом\DATA\Characteristics.xlsx";
                string nameSheet = "Characteristics";
                WriteExcel(nameСolumns, valueColumns, countColumns, CountPeriods, nameFile, nameSheet);
            }
        }
    }
}
