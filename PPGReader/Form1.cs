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
        // добавила хранение для производной
        DPPG dppg;
        // внесла их для запоминания координат, при клике правой кнопкой мыши (тогда появляется контестное меню, мышь двигается и все координаты сбиваются)
        double x;
        double y;
        // при клике правой кнопкой - был ли курсор на графике
        bool СursorOnChart = false;
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
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 1000;
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chart1.Series[0].Points.DataBindXY(ppg.GetX(), ppg.GetY()); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            chart1.Series[0].Color = Color.Blue;
            chart1.Series[0].MarkerSize = 1;
        }

        //Рисование ДФПГ
        private void Draw(DPPG dppg)
        {
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            //chart1.Series[2].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 1000;
            chart1.Series[2].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            chart1.Series[2].ToolTip = "X = #VALX, Y = #VALY";
            chart1.Series[2].Points.DataBindXY(dppg.GetX(), dppg.GetY()); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            chart1.Series[2].Color = Color.Red;
            //chart1.Series[2].MarkerSize = 1;
        }

        //Рисование массива х и у
        private void Draw(int[] x,int[] y)
        {
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 1000;
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
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
            increaseScaleClick++;
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * 1.5;
            decreaseScaleClick++;
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
                Periods[PeriodClick].MarkBegin = true;
                ppg.points[(int)x].IsBeginPeriod = true;
            }
            else if(PeriodClick != CountPeriods)
            {
                Periods[PeriodClick - 1].End = (int)x;
                Periods[PeriodClick - 1].MarkEnd = true;
                Periods[PeriodClick].Begin = (int)x;
                Periods[PeriodClick].MarkBegin = true;
                ppg.points[(int)x].IsBeginPeriod = true;
                ppg.points[(int)x].IsEndPeriod = true;

            }
            else
            {
                Periods[CountPeriods - 1].End = (int)x;
                Periods[CountPeriods - 1].MarkEnd = true;
                ppg.points[(int)x].IsEndPeriod = true;
            }
            PeriodClick++;
            if (CheckCursorOnChart())
            {
                chart1.Series[0].Points[(int)x].Label = "T";
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

       
      

        private void MarkCharacteristic(int idxPeriod, ref int point, ref bool isPoint, string label)
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
                    isPoint = true;
                    chart1.Series[0].Points[(int)x].Label = label;
                    ResetValue();

                }
            }
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
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].A, ref ppg.points[(int)x].IsA, "A");
            }
        }

        private void bMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].B, ref ppg.points[(int)x].IsB, "B");
            }
        }

        private void cMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
               MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].C, ref ppg.points[(int)x].IsC, "C");
            }
        }

        private void dMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod,  ref Periods[idxPeriod].D, ref ppg.points[(int)x].IsD, "D");
            }
        }

        private void eMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod((int)x);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].E, ref ppg.points[(int)x].IsE, "E");
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

        private void FindCharacteristics_Click(object sender, EventArgs e)
        {
            if (PeriodClick != 11)
            { MessageBox.Show(
                               "Нарисуйте график ФПГ и отметьте характерные точки на 10 периодах",
                               "Сообщение",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
            }
            else
            {
                contextMenuStrip1.Enabled = false;
                int averageLengthPeriod = CalcAverageLengthPeriod();
                int A_RelativePosition = CalcRelativePositionA();
                int B_RelativePosition = CalcRelativePositionB();
                //int C_RelativePosition = CalcRelativePosition(ppg.idxCPoints.ToArray(), ppg.idxCPoints.ToArray().Length);
                //int D_RelativePosition = CalcRelativePosition(ppg.idxDPoints.ToArray(), ppg.idxDPoints.ToArray().Length);
                //int E_RelativePosition = CalcRelativePosition(ppg.idxEPoints.ToArray(), ppg.idxEPoints.ToArray().Length);

                int previousIdxP = 10;
                int currentIdxP = 0;

                while (Periods[currentIdxP].End + averageLengthPeriod < ppg.points.Length)
                {
                    // добавить проверку на критические точки и сглаживание, обновление через 10 периодов для с,d,e
                    currentIdxP = previousIdxP % 10;
                    if (currentIdxP == 0)
                    {
                        averageLengthPeriod = CalcAverageLengthPeriod();
                        A_RelativePosition = CalcRelativePositionA();
                        B_RelativePosition = CalcRelativePositionB();
                        Periods[currentIdxP].Begin = Periods[9].End;
                        ppg.points[Periods[9].End].IsBeginPeriod = true;
                    }
                    else
                    {
                        Periods[currentIdxP].Begin = Periods[currentIdxP - 1].End;
                    }
                    Periods[currentIdxP].End = CalcEndPeriod(Periods[currentIdxP].Begin, averageLengthPeriod, currentIdxP,20);
                    ppg.points[Periods[currentIdxP].End].IsEndPeriod = true;
                    chart1.Series[0].Points[Periods[currentIdxP].End].Label = "T";

                    //Periods[currentIdxP].A = CalcA(A_RelativePosition, currentIdxP,10);
                    //ppg.points[Periods[currentIdxP].A].IsA = true;
                    //chart1.Series[0].Points[Periods[currentIdxP].A].Label = "A";

                    Periods[currentIdxP].B = CalcB(currentIdxP);
                    
                    ppg.points[Periods[currentIdxP].B].IsB = true;
                    chart1.Series[0].Points[Periods[currentIdxP].B].Label = "B";
                    
                    previousIdxP = currentIdxP + 1;

                }
            }


        }

        private int CalcB( int currentIdxPeriod)
        {
            int idxB = -1;
            int max = int.MinValue;
            for (int i = Periods[currentIdxPeriod].Begin; i < Periods[currentIdxPeriod].End; i++)
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
            int periodBegin = Periods[currentIdxPeriod].Begin;
            int predictionA = (int)Math.Round((Convert.ToDouble(A_RelativePosition) / 100) * periodLength);
            int searchInterval =(int) Math.Round(percent * Convert.ToDouble(predictionA)/ 100);
            int idxA = -1;
            double maxDPPG = int.MinValue;
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

        private int CalcEndPeriod(int begin, int averageLength, int currentIdxPeriod, int percent)
        {
            int searchInterval = percent * averageLength / 100;
            int min = int.MaxValue;
            int idxEnd = -1;
            for (int i = begin + searchInterval; i < begin+averageLength + searchInterval;i++)
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
                RelativePosition[i] = (int)Math.Round((Convert.ToDouble(point[i] - Periods[idxPeriod].Begin) / 
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
                RelativePosition[i] = (int)Math.Round((Convert.ToDouble(Periods[i].A - Periods[i].Begin) /
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
                RelativePosition[i] = (int)Math.Round((Convert.ToDouble(Periods[i].B - Periods[i].Begin) /
                                        Periods[i].Length()) * 100);
            }

            int sum = 0;
            for (int i = 0; i < CountPeriods; i++)
            {
                sum += RelativePosition[i];
            }

            return sum / CountPeriods;
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

                Draw(dppg);
                if (increaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size / (1.5 * increaseScaleClick); 
                if (decreaseScaleClick != 0) chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.ScaleView.Size * (1.5 * decreaseScaleClick);
            }
        }
    }
}
