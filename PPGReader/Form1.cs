﻿using System;
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
        // добавила хранение для производной
        DPPG dppg;
        // внесла их для запоминания координат, при клике правой кнопкой мыши (тогда появляется контестное меню, мышь двигается и все координаты сбиваются)
        double x;
        double y;
        // при клике правой кнопкой - был ли курсор на графике
        bool СursorOnChart = false;
        int[] duplicatePoints;
        int b2click = 0;
        int b3click = 0;
        int increaseClick = 0;
        int decreaseClick = 0;
        int PeriodClick = 0;
        int AClick = 0;
        int BClick = 0;
        int CClick = 0;
        int DClick = 0;
        int EClick = 0;
        const int CountPeriods = 10;
        PeriodPPG[] Periods = new PeriodPPG[CountPeriods];
        

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
            string filePath = @"D:\ВУЗ\4 курс\Диплом\ФПГ\plz\набор1\ГУЗЕЛЬ.plz"; //textBoxPath.Text; //@"J:\Documents\8 семестр\Диплом\plz\набор1\ГУЗЕЛЬ.plz" //
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

        
        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
            b2click++;
        }

        
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

        private void PeriobMenuItem_Click(object sender, EventArgs e)
        {
            if(PeriodClick == 0)
            {
                Periods[PeriodClick].begin = (int)x;
                ppg.points[(int)x].IsBeginPeriod = true;
            }
            else if(PeriodClick != 10)
            {
                Periods[PeriodClick-1].end = (int)x;
                Periods[PeriodClick].begin = (int)x;
                ppg.points[(int)x].IsBeginPeriod = true;
                ppg.points[(int)x].IsEndPeriod = true;

            }
            else
            {
                Periods[9].end = (int)x;
                ppg.points[(int)x].IsEndPeriod = true;
            }
            PeriodClick++;
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                
                chart1.Series[0].Points[(int)x].Label = "T";
            }
            if (PeriodClick == 11)
            {
                MessageBox.Show(
                                "10 периодов отмечены, нажмите на кнопку \"Найти характеристики\" " +
                                "для нахождения следующих характеристик",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                contextMenuStrip1.Enabled = false;
            }
            ResetValue();
        }

       
        private void aMenuItem_Click(object sender, EventArgs e)
        {
            Periods[AClick].A = (int)x;
            AClick++;
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                ppg.points[(int)x].IsA = true;
                ppg.idxAPoints.Add((int)x);
                chart1.Series[0].Points[(int)x].Label = "A";
            }
            ResetValue();
            
        }

        private void bMenuItem_Click(object sender, EventArgs e)
        {
            Periods[BClick].B = (int)x;
            BClick++;
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                ppg.points[(int)x].IsB = true;
                ppg.idxBPoints.Add((int)x);
                chart1.Series[0].Points[(int)x].Label = "B";
            }
            ResetValue();
        }

        private void cMenuItem_Click(object sender, EventArgs e)
        {
            Periods[CClick].C = (int)x;
            CClick++;
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                ppg.points[(int)x].IsС = true;
                ppg.idxCPoints.Add((int)x);
                chart1.Series[0].Points[(int)x].Label = "C";
            }
            ResetValue();
        }

        private void dMenuItem_Click(object sender, EventArgs e)
        {
            Periods[DClick].D = (int)x;
            DClick++;
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                ppg.points[(int)x].IsD = true;
                ppg.idxDPoints.Add((int)x);
                chart1.Series[0].Points[(int)x].Label = "D";
            }
            ResetValue();
        }

        private void eMenuItem_Click(object sender, EventArgs e)
        {
            Periods[EClick].E = (int)x;
            EClick++;
            if (СursorOnChart == false)
            {
                MessageBox.Show(
                                "Курсор находился не на графике, попробуйте еще раз",
                                "Сообщение",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                ppg.points[(int)x].IsE = true;
                ppg.idxEPoints.Add((int)x);
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

        
        private void button1_Click_1(object sender, EventArgs e)
        {
            int window = 2;
            if (textBox1.Text != "") window = Convert.ToInt32(textBox1.Text);
            increaseClick = window;
            increaseClick += 1;
            textBox1.Text = Convert.ToString(increaseClick);
        }

        
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
            if (window < 3) MessageBox.Show("Невозможно уменьшить окно сглаживания", 
                                             "Ошибка", 
                                             MessageBoxButtons.OK, 
                                             MessageBoxIcon.Hand);

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

        private void FindCharacteristics_Click(object sender, EventArgs e)
        {
            if (PeriodClick != 11)
            { MessageBox.Show(
                               "Отметьте характерные точки на 10 периодах",
                               "Сообщение",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
            }
            else
            {
                int averageLengthPeriod = CalcAverageLengthPeriod();
                int A_RelativePosition = CalcRelativePosition(ppg.idxAPoints.ToArray(), ppg.idxAPoints.ToArray().Length);
                int B_RelativePosition = CalcRelativePosition(ppg.idxBPoints.ToArray(), ppg.idxBPoints.ToArray().Length);
                //int C_RelativePosition = CalcRelativePosition(ppg.idxCPoints.ToArray(), ppg.idxCPoints.ToArray().Length);
                //int D_RelativePosition = CalcRelativePosition(ppg.idxDPoints.ToArray(), ppg.idxDPoints.ToArray().Length);
                //int E_RelativePosition = CalcRelativePosition(ppg.idxEPoints.ToArray(), ppg.idxEPoints.ToArray().Length);

                int previousIdxP = 10;
                int currentIdxP = 0;

                while (Periods[currentIdxP].end + averageLengthPeriod < ppg.points.Length)
                {
                    // добавить проверку на критические точки и сглаживание, обновление через 10 периодов для с,d,e
                    currentIdxP = previousIdxP % 10;
                    if (currentIdxP == 0)
                    {
                        averageLengthPeriod = CalcAverageLengthPeriod();
                        A_RelativePosition = CalcRelativePositionA();
                        B_RelativePosition = CalcRelativePositionB();
                        Periods[currentIdxP].begin = Periods[9].end;
                        ppg.points[Periods[9].end].IsBeginPeriod = true;
                    }
                    else
                    {
                        Periods[currentIdxP].begin = Periods[currentIdxP - 1].end;
                    }
                    Periods[currentIdxP].end = CalcEndPeriod(Periods[currentIdxP].begin, averageLengthPeriod, currentIdxP);
                    ppg.points[Periods[currentIdxP].end].IsEndPeriod = true;
                    chart1.Series[0].Points[Periods[currentIdxP].end].Label = "T";

                    //Periods[currentIdxP].A = CalcA(A_RelativePosition, currentIdxP,10);
                    //ppg.points[Periods[currentIdxP].A].IsA = true;
                    //chart1.Series[0].Points[Periods[currentIdxP].A].Label = "A";

                    Periods[currentIdxP].B = CalcB(B_RelativePosition, currentIdxP, 20);
                    
                    ppg.points[Periods[currentIdxP].B].IsB = true;
                    chart1.Series[0].Points[Periods[currentIdxP].B].Label = "B";
                    
                    previousIdxP = currentIdxP + 1;

                }
            }


        }

        private int CalcB(int B_RelativePosition, int currentIdxPeriod, int percent)
        {
            int periodLength = Periods[currentIdxPeriod].Length();
            int periodBegin = Periods[currentIdxPeriod].begin;
            int predictionB =(int)Math.Round((Convert.ToDouble(B_RelativePosition) / 100) * periodLength);
            int searchInterval = (int)Math.Round(percent * Convert.ToDouble(predictionB) / 100);
            int idxB = -1;
            int max = int.MinValue;
            int iEnd = periodBegin + predictionB + searchInterval;
            if (periodBegin + predictionB + searchInterval > ppg.points.Length)
            {
                iEnd = ppg.points.Length;
            }
            for (int i = periodBegin + predictionB - searchInterval; i < iEnd; i++)
            {
                if (ppg.points[i].y > max)
                {
                    max = ppg.points[i].y;
                    idxB = i;
                }
            }

            return idxB;
        }

        //пока не работает, тк нет производной
        private int CalcA(int A_RelativePosition, int currentIdxPeriod, int percent)
        {
            int periodLength = Periods[currentIdxPeriod].Length();
            int periodBegin = Periods[currentIdxPeriod].begin;
            int predictionA = (int)Math.Round((Convert.ToDouble(A_RelativePosition) / 100) * periodLength);
            int searchInterval =(int) Math.Round(percent * Convert.ToDouble(predictionA)/ 100);
            int idxA = -1;
            int maxDPPG = int.MinValue;
            for (int i = periodBegin + predictionA - searchInterval; i < periodBegin + predictionA + searchInterval; i++)
            {
                if(dppg.points[i].y > maxDPPG)
                {
                    maxDPPG = dppg.points[i].y;
                    idxA = i;
                }
            }

            return idxA;
        }

        private int CalcEndPeriod(int begin, int averageLength, int currentIdxPeriod)
        {
            int searchInterval = 10 * averageLength / 100;
            int min = int.MaxValue;
            int idxEnd = -1;
            for (int i = begin + averageLength - searchInterval; i< begin+averageLength + searchInterval;i++)
            {
                if(ppg.points[i].y <min)
                {
                    min = ppg.points[i].y;
                    idxEnd = i;
                }
            }

            return idxEnd;
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

        private int CalcRelativePosition(int[] point, int n)
        {
            int[] RelativePosition = new int[n];
            int idxPeriod = 0;
            for (int i=0;i<n;i++)
            {
                idxPeriod = CalcPeriod(point[i]);
                RelativePosition[i] = (int)Math.Round((Convert.ToDouble(point[i] - Periods[idxPeriod].begin) / 
                                        Periods[idxPeriod].Length()) * 100);
            }

            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += RelativePosition[i];
            }

            return sum / n;
        }

        private int CalcRelativePositionA()
        {
            int[] RelativePosition = new int[CountPeriods];
            for (int i = 0; i < CountPeriods; i++)
            {
                RelativePosition[i] = (int)Math.Round((Convert.ToDouble(Periods[i].A - Periods[i].begin) /
                                        Periods[i].Length()) * 100);
            }

            int sum = 0;
            for (int i = 0; i < CountPeriods; i++)
            {
                sum += RelativePosition[i];
            }

            return sum / CountPeriods;
        }

        private int CalcRelativePositionB()
        {
            int[] RelativePosition = new int[CountPeriods];
            for (int i = 0; i < CountPeriods; i++)
            {
                RelativePosition[i] = (int)Math.Round((Convert.ToDouble(Periods[i].B - Periods[i].begin) /
                                        Periods[i].Length()) * 100);
            }

            int sum = 0;
            for (int i = 0; i < CountPeriods; i++)
            {
                sum += RelativePosition[i];
            }

            return sum / CountPeriods;
        }
        private int CalcPeriod(int k)
        {
            for(int i = 0; i < CountPeriods;i++)
            {
                if ((k>Periods[i].begin)&&(k<Periods[i].end))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
