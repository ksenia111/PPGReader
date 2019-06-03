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
        DDPPG ddppg;
        // внесла их для запоминания координат, при клике правой кнопкой мыши (тогда появляется контестное меню, мышь двигается и все координаты сбиваются)
        double x;
        double y;
        // при клике правой кнопкой - был ли курсор на графике
        bool СursorOnChart = false;
        bool IsDrawn = false;
        bool IsFoundDerivative = false;
        bool IsEndWatch = true;
        string selectedStateFindCharacteristic;
        int[] duplicatePoints;
        int increaseClick = 0;
        int decreaseClick = 0;
        int PeriodClick = 0;
        const int CountPeriods = 10;
        PeriodPPG[] Periods = new PeriodPPG[CountPeriods];
        double beginSmoothingPeriod = 0;
        double endSmoothingPeriod = 0;
        int clickTagSmoothingPeriod = 0;
        bool drawChartClick = false;
        bool fixSmoothing = false;
        bool smoothingClick = false;
        bool tagSmoothingPeriod = false;
        int[] forDrawing;
        string selectedState;
        string selectedSmoothingMethod;
        bool IsAllPPGSmoothed;

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
            chartPPG.ContextMenuStrip = contextMenuStrip1;
            // устанавливаем обработчики событий для меню
            PeriobMenuItem.Click += PeriobMenuItem_Click;
            aMenuItem.Click += aMenuItem_Click;
            bMenuItem.Click += bMenuItem_Click;
            cMenuItem.Click += cMenuItem_Click;
            dMenuItem.Click += dMenuItem_Click;
            eMenuItem.Click += eMenuItem_Click;
            contextMenuStrip1.Enabled = true;
            for (int i = 0; i < Periods.Length; i++)
            {
                Periods[i] = new PeriodPPG();
            }
        }

        private int[] Read()
        {
            string filePath = @"J:\Documents\8 семестр\Диплом\plz\набор1\ГУЗЕЛЬ.plz"; //textBoxPath.Text; //@"D:\ВУЗ\4 курс\Диплом\ФПГ\plz\набор1\ГУЗЕЛЬ.plz";  //
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
                    points[i] = Convert.ToInt16(Convert.ToString(number, 10)) * w;
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

            for (int i = 0; i < n; i++)
            {

                singlingPoints[i] = points[(i + 1) * rate - 1];
            }
            return singlingPoints;
        }

        int endOfSmoothedData;
        /// <summary>
        /// Скользящее среднее
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private int[] MovingAverageMethod(int beginPeriod, int endPeriod, int window, int[] points) //метод скользящей средней
        {
            int p = window; //окно
            if (p % 2 == 0)
            {
                p++; //если окно четное, увеличиваем на 1 для симметрии
                labelValueSmoothingWindow.Text = Convert.ToString(p);
            }
            int m = (p - 1) / 2; //размах окна влево и вправо от текущей позиции
            int n = points.Length;// - m * 2;
            int[] SmoothedData = new int[n];

            SmoothedData[0] = points[0];
            int sum = 0;
            int k1 = 0;  //начало окна
            int k2 = 0;  //конец окна
            int z = 0;   //размер окна

            for (int i = 1; i < n; i++)
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
                    SmoothedData[i] = points[i];
                    //k1 = i - n + i + 1;
                    //k2 = n - 1;
                    //z = k2 - k1 + 1;
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

            endOfSmoothedData = n - m;
            return SmoothedData;
        }

        private int[] NonlinearSmoothing(int beginPeriod, int endPeriod, int[] points)
        {
            int n = points.Length;
            int[] SmoothedData = new int[n];

            SmoothedData[0] = (39 * points[0] + 8 * points[1] - 4 * (points[2] + points[3] - points[4]) + points[5] - 2 * points[6]) / 42;
            SmoothedData[1] = (8 * points[0] + 19 * points[1] + 16 * points[2] + 6 * points[3] - 4 * points[4] - 7 * points[5] + 4 * points[6]) / 42;
            SmoothedData[2] = (-4 * points[0] + 16 * points[1] + 19 * points[2] + 12 * points[3] + 2 * points[4] - 4 * points[5] + points[6]) / 42;

            for (int i = 3; i < n - 3; i++)
            {
                SmoothedData[i] = (7 * points[i] + 6 * (points[i + 1] + points[i - 1]) + 3 * (points[i + 2] + points[i - 2]) - 2 * (points[i + 3] + points[i - 3])) / 21;
            }

            SmoothedData[n - 3] = points[n - 3];
            SmoothedData[n - 2] = points[n - 2];
            SmoothedData[n - 1] = points[n - 1];

            endOfSmoothedData = n - 4;
            //SmoothedData[n - 3] = (points[n - 7] - 4 * points[n - 6] + 2 * points[n - 5] + 12 * points[n - 4] + 19 * points[n - 3] + 16 * points[n - 2] - 4 * points[n - 1]) / 42;
            //SmoothedData[n - 2] = (4 * points[n - 7] - 7 * points[n - 6] - 4 * points[n - 5] + 6 * points[n - 4] + 16 * points[n - 3] + 19 * points[n - 2] + 8 * points[n - 1]) / 42;
            //SmoothedData[n - 1] = (-2 * points[n - 7] + 4 * points[n - 6] + points[n - 5] - 4 * points[n - 4] - 4 * points[n - 3] + 8 * points[n - 2] + 39 * points[n - 1]) / 42;

            return SmoothedData;
        }

        
        /// Сглаживание
        private void Smoothing(int beginPeriod, int endPeriod)
        {
            
            int Period = endPeriod - beginPeriod;
            int[] pointsForSmoothing = new int[Period];
            fixSmoothing = false;
            smoothingClick = true;

            for (int i = 0; i < Period; i++)
            {
                pointsForSmoothing[i] = forDrawing[beginPeriod + i];         //берем только точки внутри периода
            }

            int[] smoothedData = new int[Period];
            if (selectedSmoothingMethod == null || selectedSmoothingMethod == "Сглаживание методом скользящего среднего")
            {
                int window = Convert.ToInt32(labelValueSmoothingWindow.Text);
                smoothedData = MovingAverageMethod(beginPeriod, endPeriod, window, pointsForSmoothing);

            }
            if (selectedSmoothingMethod == "Сглаживание полиномами 2 порядка по 7 точкам")
            {
                smoothedData = NonlinearSmoothing(beginPeriod, endPeriod, pointsForSmoothing);
            }

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

            double size = chartPPG.ChartAreas[0].AxisX.ScaleView.Size;

            Draw(x, forDrawing);

            chartPPG.ChartAreas[0].AxisX.ScaleView.Size = size;
        }

        /// <summary>
        /// Отмена сглаживания
        /// </summary>
        /// <param name="beginPeriod"></param>
        /// <param name="endPeriod"></param>
        /// <param name="SmoothedData"></param>
        /// <param name="DuplicatePoints"></param>
        /// <returns></returns>
        private void CancelSmoothing(int beginPeriod, int endPeriod, int[] SmoothedData, int[] DuplicatePoints)
        {
            int n = DuplicatePoints.Length;
            int[] dataAfterCancel = new int[n];

            for (int i = beginPeriod; i <= endPeriod; i++) 
            {
                SmoothedData[i] = DuplicatePoints[i];
            }
            dataAfterCancel = SmoothedData;

            int[] x = new int[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = i;
            }

            double size = chartPPG.ChartAreas[0].AxisX.ScaleView.Size;

            Draw(x, forDrawing);

            chartPPG.ChartAreas[0].AxisX.ScaleView.Size = size;
        }

        /// <summary>
        /// Применить сглаживание
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplySmoothing()
        {
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

            double size = chartPPG.ChartAreas[0].AxisX.ScaleView.Size;

            ppg = new PPG(pointPPGs, n);
            Draw(ppg);

            chartPPG.ChartAreas[0].AxisX.ScaleView.Size = size;
        }

        int[] duplicatePoints1;
        int[] duplicatePoints2;
        int[] duplicatePoints3;
        int[] duplicatePoints4;
        private void buttonDrawPPG_Click(object sender, EventArgs e)
        {
            drawChartClick = true;

            //чтение
            int[] points = Read();

            //прореживание
            int[] singlingPoints = Singling(points);

            duplicatePoints = singlingPoints;  // дубликат прореженных точек (я использую в сглаживании)

            if (int.Parse(textBoxSinglingRate.Text) == 1) duplicatePoints1 = CopyPoints(singlingPoints);
            else if (int.Parse(textBoxSinglingRate.Text) == 2) duplicatePoints2 = CopyPoints(singlingPoints);
            else if (int.Parse(textBoxSinglingRate.Text) == 3) duplicatePoints3 = CopyPoints(singlingPoints);
            else if (int.Parse(textBoxSinglingRate.Text) == 4) duplicatePoints4 = CopyPoints(singlingPoints);

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
            chartDPPG.Series[0].Points.Clear();
            chartDDPPG.Series[0].Points.Clear();
            chartDPPG.ChartAreas[0].AxisX.ScaleView.Size = 400;
            buttonSmoothingPeriod.Text = "Сгладить";
        }

        //Рисование ФПГ
        private void Draw(PPG ppg)
        { 
            chartPPG.ChartAreas[0].AxisX.Minimum = 0;
            chartPPG.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartPPG.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chartPPG.Series[0].ChartType = SeriesChartType.Line;
            chartPPG.ChartAreas[0].AxisX.ScaleView.Size = 400;
            chartPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chartPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chartPPG.ChartAreas[0].AxisX.MajorGrid.Interval = 50;
            chartPPG.ChartAreas[0].Position.Height = 85;
            chartPPG.ChartAreas[0].Position.Width = 80;
            chartPPG.ChartAreas[0].InnerPlotPosition.X = 5;
            chartPPG.ChartAreas[0].AxisX.LineWidth = 2;
            chartPPG.ChartAreas[0].AxisX.LineColor = Color.Gray;
            chartPPG.ChartAreas[0].AxisY.LineWidth = 2;
            chartPPG.ChartAreas[0].AxisY.LineColor = Color.Gray;

            chartPPG.ChartAreas[0].AxisX.Crossing = 0;
            chartPPG.ChartAreas[0].AxisY.Crossing = 50;
            chartPPG.Series[0].Points.DataBindXY(ppg.GetX(), ppg.GetY()); 
            chartPPG.Series[0].Color = Color.Blue;
            chartPPG.Series[0].MarkerSize = 1;
            IsDrawn = true;
            
        }

        //Рисование ДФПГ
        private void Draw(DPPG dppg)
        {
            
            chartDPPG.ChartAreas[0].AxisX.Minimum = 0;
            chartDPPG.ChartAreas[0].InnerPlotPosition.X = 5;
            chartDPPG.ChartAreas[0].AxisX.Crossing = 0;
            chartDPPG.ChartAreas[0].AxisX.LineWidth = 2;
            chartDPPG.ChartAreas[0].AxisX.LineColor = Color.Gray;
            chartDPPG.ChartAreas[0].AxisY.LineWidth = 2;
            chartDPPG.ChartAreas[0].AxisY.LineColor = Color.Gray;
            chartDPPG.ChartAreas[0].AxisY.Crossing = 0;
            chartDPPG.ChartAreas[0].AxisX.MajorGrid.Interval = 50;
            chartDPPG.ChartAreas[0].Position.Height = 85;
            chartDPPG.ChartAreas[0].Position.Width = 80;
            
            chartDPPG.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDPPG.ChartAreas[0].AxisX.ScaleView.Size = 400;
            chartDPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY"; 
            chartDPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chartDPPG.Series[0].Points.DataBindXY(dppg.GetX(), dppg.GetY());
            chartDPPG.Series[0].BorderWidth = 2;
            chartDPPG.Series[0].Color = Color.Green;
            chartDPPG.Series[0].MarkerSize = 1;

        }

        private void Draw(DDPPG ddppg)
        {

            chartDDPPG.ChartAreas[0].AxisX.Minimum = 0;
            chartDDPPG.ChartAreas[0].InnerPlotPosition.X = 5;
            chartDDPPG.ChartAreas[0].AxisX.Crossing = 0;
            chartDDPPG.ChartAreas[0].AxisX.LineWidth = 2;
            chartDDPPG.ChartAreas[0].AxisX.LineColor = Color.Gray;
            chartDDPPG.ChartAreas[0].AxisY.LineWidth = 2;
            chartDDPPG.ChartAreas[0].AxisY.LineColor = Color.Gray;
            chartDDPPG.ChartAreas[0].AxisY.Crossing = 0;
            chartDDPPG.ChartAreas[0].AxisX.MajorGrid.Interval = 50;
            chartDDPPG.ChartAreas[0].Position.Height = 85;
            chartDDPPG.ChartAreas[0].Position.Width = 80;

            chartDDPPG.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartDDPPG.ChartAreas[0].AxisX.ScaleView.Size = 400;
            chartDDPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chartDDPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chartDDPPG.Series[0].Points.DataBindXY(ddppg.GetX(), ddppg.GetY());
            chartDDPPG.Series[0].BorderWidth = 2;
            chartDDPPG.Series[0].Color = Color.Green;
            chartDDPPG.Series[0].MarkerSize = 1;

        }

        //Рисование массива х и у
        private void Draw(int[] x, int[] y)
        {
            chartPPG.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chartPPG.Series[0].ChartType = SeriesChartType.Line;
            chartPPG.ChartAreas[0].AxisX.ScaleView.Size = 400;
            chartPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY"; // выдает подсказки в виде координат Х и У(ниже) при наведении мыши на точку
            chartPPG.Series[0].ToolTip = "X = #VALX, Y = #VALY";
            chartPPG.Series[0].Points.DataBindXY(x, y); //если отрисовывать просто точки, то график все равно выглядит как линия при не очень большом увеличении
            chartPPG.Series[0].Color = Color.Blue;
            chartPPG.Series[0].MarkerSize = 1;
        }

        private void Draw(int[] x, int[] y, int idxSeries)
        {
            chartPPG.ChartAreas[idxSeries].AxisX.ScrollBar.Enabled = true;
            chartPPG.Series[idxSeries].ChartType = SeriesChartType.Line;
            chartPPG.ChartAreas[idxSeries].AxisX.ScaleView.Size = 400;
            chartPPG.Series[idxSeries].ToolTip = "X = #VALX, Y = #VALY"; 
            chartPPG.Series[idxSeries].ToolTip = "X = #VALX, Y = #VALY";
            chartPPG.Series[idxSeries].Points.DataBindXY(x, y); 
            chartPPG.Series[idxSeries].Color = Color.Blue;
            chartPPG.Series[idxSeries].MarkerSize = 1;
        }


        private void chartPPG_MouseClick(object sender, MouseEventArgs e)
        {
            var result = chartPPG.HitTest(e.X, e.Y);

            chartPPG.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chartPPG.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double pX = chartPPG.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double pY = chartPPG.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor
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
                        beginSmoothingPeriod = chartPPG.ChartAreas[0].CursorX.Position;
                        textBoxSmoothingPeriod.Text = "Начало: " + Convert.ToString(beginSmoothingPeriod);
                    }

                    if (clickTagSmoothingPeriod == 2)
                    {
                        endSmoothingPeriod = chartPPG.ChartAreas[0].CursorX.Position;
                        textBoxSmoothingPeriod.Text += ". Конец: " + Convert.ToString(endSmoothingPeriod);
                        clickTagSmoothingPeriod = 0;
                        tagSmoothingPeriod = false;
                    }
                }
            }
        }

        private void chartPPG_MouseMove(object sender, MouseEventArgs e)
        {
            chartPPG.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chartPPG.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double pX = chartPPG.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double pY = chartPPG.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor

            var result = chartPPG.HitTest(e.X, e.Y);
            Cursor = result.ChartElementType == ChartElementType.DataPoint ? Cursors.Hand : Cursors.Default;
            //chart1.Series[0].Color = result.ChartElementType == ChartElementType.DataPoint ? Color.Coral : Color.Blue; //у всего графика меняется цвет, как у одной точки заменить, пока не придумала
            chartPPG.Series[0].Color = result.Series == chartPPG.Series[0] ? Color.Green : Color.Blue;

            chartDPPG.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chartDPPG.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double dpX = chartDPPG.ChartAreas[0].CursorX.Position; //X Axis Coordinate of your mouse cursor
            double dpY = chartDPPG.ChartAreas[0].CursorY.Position; //Y Axis Coordinate of your mouse cursor

            var dresult = chartDPPG.HitTest(e.X, e.Y);
            Cursor = dresult.ChartElementType == ChartElementType.DataPoint ? Cursors.Hand : Cursors.Default;
        }


        private void buttonIncreaseScale_Click(object sender, EventArgs e)
        {
            chartPPG.ChartAreas[0].AxisX.ScaleView.Size = chartPPG.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
            chartDPPG.ChartAreas[0].AxisX.ScaleView.Size = chartDPPG.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
            chartDDPPG.ChartAreas[0].AxisX.ScaleView.Size = chartDDPPG.ChartAreas[0].AxisX.ScaleView.Size / 1.5;
        }


        private void buttonDecreaseScale_Click(object sender, EventArgs e)
        {
            chartPPG.ChartAreas[0].AxisX.ScaleView.Size = chartPPG.ChartAreas[0].AxisX.ScaleView.Size * 1.5;
            chartDPPG.ChartAreas[0].AxisX.ScaleView.Size = chartDPPG.ChartAreas[0].AxisX.ScaleView.Size * 1.5;
            chartDDPPG.ChartAreas[0].AxisX.ScaleView.Size = chartDDPPG.ChartAreas[0].AxisX.ScaleView.Size * 1.5;
        }

        //подписи к первым 10 отмеченным периодам
        private void WriteLabelForChart()
        {
            for (int i = 0; i < CountPeriods; i++)
            {
                if (Periods[i].Begin != 0) chartPPG.Series[0].Points[Periods[i].Begin].Label = "T";
                if (Periods[i].End != 0) chartPPG.Series[0].Points[Periods[i].End].Label = "T";
                if (Periods[i].A != 0) chartPPG.Series[0].Points[Periods[i].A].Label = "A";
                if (Periods[i].B != 0) chartPPG.Series[0].Points[Periods[i].B].Label = "B";
                if (Periods[i].C != 0) chartPPG.Series[0].Points[Periods[i].C].Label = "C";
                if (Periods[i].D != 0) chartPPG.Series[0].Points[Periods[i].D].Label = "D";
                if (Periods[i].E != 0) chartPPG.Series[0].Points[Periods[i].E].Label = "E";
            }
        }


        //подписи к 1 периоду
        private void WriteLabelForChart(PeriodPPG periodPPG)
        {
            if (periodPPG.Begin != 0) chartPPG.Series[0].Points[periodPPG.Begin].Label = "T";
            if (periodPPG.End != 0) chartPPG.Series[0].Points[periodPPG.End].Label = "T";
            if (periodPPG.A != 0) chartPPG.Series[0].Points[periodPPG.A].Label = "A";
            if (periodPPG.B != 0) chartPPG.Series[0].Points[periodPPG.B].Label = "B";
            if (periodPPG.C != 0) chartPPG.Series[0].Points[periodPPG.C].Label = "C";
            if (periodPPG.D != 0) chartPPG.Series[0].Points[periodPPG.D].Label = "D";
            if (periodPPG.E != 0) chartPPG.Series[0].Points[periodPPG.E].Label = "E";
        }

        private void WriteLabelForChartSpecimen(PeriodPPG periodPPG, int idxSeries)
        {
            chartPPG.Series[1].Points[periodPPG.Begin].LabelForeColor = Color.Green;
            if (periodPPG.Begin != 0) chartPPG.Series[idxSeries].Points[periodPPG.Begin].Label = "T";
            if (periodPPG.End != 0) chartPPG.Series[idxSeries].Points[periodPPG.End].Label = "T";
            if (periodPPG.A != 0) chartPPG.Series[idxSeries].Points[periodPPG.A].Label = "A";
            if (periodPPG.B != 0) chartPPG.Series[idxSeries].Points[periodPPG.B].Label = "B";
            if (periodPPG.C != 0) chartPPG.Series[idxSeries].Points[periodPPG.C].Label = "C";
            if (periodPPG.D != 0) chartPPG.Series[idxSeries].Points[periodPPG.D].Label = "D";
            if (periodPPG.E != 0) chartPPG.Series[idxSeries].Points[periodPPG.E].Label = "E";
        }

        //подписи ко всем периодам
        private void WriteLabelForChart(PeriodPPG[] periodPPG)
        {
            for (int i = 0; i < periodPPG.Length; i++)
            {
                WriteLabelForChart(periodPPG[i]);
            }
        }

        void ResetValue()
        {
            x = 0;
            y = 0;
            СursorOnChart = false;
        }

        private void PeriobMenuItem_Click(object sender, EventArgs e)
        {
            if (PeriodClick == 0)
            {
                Periods[PeriodClick].Begin = (int)x;
            }
            else if (PeriodClick != CountPeriods)
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
                chartPPG.Series[0].Points[(int)x].Label = "T";
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
                    chartPPG.Series[0].Points[(int)x].Label = label;
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

            } else
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
            int idxPeriod = CalcPeriod(Periods, (int)x, CountPeriods);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].A, "A");
            }
        }

        private void bMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod(Periods, (int)x, CountPeriods);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].B, "B");
            }
        }

        private void cMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod(Periods, (int)x, CountPeriods);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].C, "C");
            }
        }

        private void dMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod(Periods, (int)x, CountPeriods);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].D, "D");
            }
        }

        private void eMenuItem_Click(object sender, EventArgs e)
        {
            int idxPeriod = CalcPeriod(Periods, (int)x, CountPeriods);
            if (CheckMarkPeriod(idxPeriod))
            {
                MarkCharacteristic(idxPeriod, ref Periods[idxPeriod].E, "E");
            }
        }

        private void chartPPG_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var result = chartPPG.HitTest(e.X, e.Y);
                СursorOnChart = result.ChartElementType == ChartElementType.DataPoint;
                x = chartPPG.ChartAreas[0].CursorX.Position;
                y = chartPPG.ChartAreas[0].CursorY.Position;
            }
        }

        //увеличение окна
        private void buttonIncreaseWindow_Click_1(object sender, EventArgs e)
        {
            int window = Convert.ToInt32(labelValueSmoothingWindow.Text);
            increaseClick = window;
            increaseClick += 2;
            labelValueSmoothingWindow.Text = Convert.ToString(increaseClick);
        }

        //уменьшение окна
        private void buttonDecreaseWindow_Click_1(object sender, EventArgs e)
        {
            int window = Convert.ToInt32(labelValueSmoothingWindow.Text);

            if (window > 3)
            {
                decreaseClick = window;
                decreaseClick -= 2;
                labelValueSmoothingWindow.Text = Convert.ToString(decreaseClick);
            }
            if (window <= 3) MessageBox.Show("Невозможно уменьшить окно сглаживания",
                                             "Ошибка",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Hand);

        }

        //"отметить период сглаживания"
        private void buttonTagSmoothingPeriod_Click(object sender, EventArgs e)
        {
            tagSmoothingPeriod = true;
            textBoxSmoothingPeriod.Text = "";
            beginSmoothingPeriod = 0;
            endSmoothingPeriod = 0;
            buttonSmoothingPeriod.Text = "Сгладить период";
        }

        //сглаживание
        private void buttonSmoothingPeriod_Click_1(object sender, EventArgs e)
        {
            IsAllPPGSmoothed = false;
            if (drawChartClick == false) MessageBox.Show("Сначала нарисуйте график",
                                             "Ошибка",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Error);
            else
            {
                if (endSmoothingPeriod == 0)
                {
                    DialogResult result = MessageBox.Show("При нажатии кнопки \"ОК\" сгладится весь график. Если хотите сгладить определенный период, то нажмите \"Отмена\" и отметьте начало и конец периода",
                       "Сообщение",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Information);
                    if (result == DialogResult.Cancel) { }
                    //MessageBox.Show("Отметьте начало и конец периода и нажмите \"Сгладить\"",
                    //                             "Сообщение",
                    //                             MessageBoxButtons.OK,
                    //                             MessageBoxIcon.Information);
                    else
                    {
                        IsAllPPGSmoothed = true;
                        int beginPeriod = 0;
                        int endPeriod = ppg.points.Length;
                        int Period = endPeriod - beginPeriod;

                        Smoothing(beginPeriod, endPeriod);
                    }
                }
                else
                {
                    int beginPeriod = Convert.ToInt32(beginSmoothingPeriod);
                    int endPeriod = Convert.ToInt32(endSmoothingPeriod);
                    int Period = endPeriod - beginPeriod;
                    //int[] points = duplicatePoints;                   //взяли дубликат точек, уже прореженных

                    Smoothing(beginPeriod, endPeriod);
                }
            }
        }

        //отменить сглаживание
        private void buttonCancelSmoothingPeriod_Click(object sender, EventArgs e)
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
                    if (IsAllPPGSmoothed == false)
                    {
                        int beginPeriod = Convert.ToInt32(beginSmoothingPeriod);
                        int endPeriod = Convert.ToInt32(endSmoothingPeriod);
                        int Period = endPeriod - beginPeriod;
                        CancelSmoothing(beginPeriod, endPeriod, forDrawing, duplicatePoints);
                    }
                    else
                    {
                        int beginPeriod = 0;
                        int endPeriod = ppg.points.Length - 1;
                        int Period = endPeriod - beginPeriod;
                        CancelSmoothing(beginPeriod, endPeriod, forDrawing, duplicatePoints);
                    }
                }
            }
        }



        //применить сглаживание
        private void buttonApplySmoothing_Click(object sender, EventArgs e)
        {
            if (smoothingClick == false) MessageBox.Show("Невозможно применить сглаживание, т.к. оно не было выполнено. Выберите период сглаживания и нажмите \"сглдить период\". После этого можете применить сглаживание",
                                               "Ошибка",
                                               MessageBoxButtons.OK,
                                               MessageBoxIcon.Exclamation);
            else
            {
                fixSmoothing = true;
                smoothingClick = false;

                ApplySmoothing();
                textBoxSmoothingPeriod.Text = "";
                beginSmoothingPeriod = 0;
                endSmoothingPeriod = 0;
                buttonSmoothingPeriod.Text = "Сгладить";
            }
        }

        private void FullSearchCharacteristics_Click(object sender, EventArgs e)
        {
            DialogResult mesageResult = CheckData();
            if (mesageResult == DialogResult.OK)
            {
                FindCharacteristicsExcel();
            }
            else if (PeriodClick == CountPeriods + 1)
            {
                FindCharacteristics();
            }
        }

        private PeriodPPG[] FindCharacteristicsSpecimen(int countStartPeriods)
        {
            string namefile = @"D:\ВУЗ\4 курс\Диплом\DATA\StartCharacteristics_2.xlsx";
            PeriodPPG[] periods = ReadStartCharacteristics(countStartPeriods, namefile);
            contextMenuStrip1.Enabled = false;
            int averageLengthPeriod = 0;
            int A_RelativePosition = 0;
            int B_RelativePosition = 0;
            int C_RelativePosition = 0;
            int D_RelativePosition = 0;
            int E_RelativePosition = 0;
            int previousIdxP = countStartPeriods - 1;
            int currentIdxP = countStartPeriods;
            List<PeriodPPG> periodPPGs = new List<PeriodPPG>();

            int idxSeries = 1;
            Draw(ppg.GetX(), ppg.GetY(), idxSeries);

            AddMarkedPeriods(ref periodPPGs, countStartPeriods, periods);
            int updateData = countStartPeriods;
            int updateFrequency = 10;
            int previousLengthPeriod = periodPPGs.Last().Length();
            UpdateRelativePosition(periodPPGs, countStartPeriods, ref averageLengthPeriod, ref A_RelativePosition, ref B_RelativePosition,
                                 ref C_RelativePosition, ref D_RelativePosition, ref E_RelativePosition);

            while (periodPPGs.Last().End + averageLengthPeriod < ppg.points.Length)
            {
                PeriodPPG currentPeriod = new PeriodPPG();
                updateData = currentIdxP % updateFrequency;
                if (updateData == 0)
                {
                    UpdateRelativePosition(periodPPGs, countStartPeriods, ref averageLengthPeriod, ref A_RelativePosition, ref B_RelativePosition,
                                  ref C_RelativePosition, ref D_RelativePosition, ref E_RelativePosition);
                }
                
                int end_percent, end_percentD, c_percent, c_percentD, c_percentC, d_percent, d_percentD;
                Initialization(out end_percent, out end_percentD, out c_percent, out c_percentD, out c_percentC, out d_percent, out d_percentD);
                
                currentPeriod = CalcCurrentPeriod(averageLengthPeriod, D_RelativePosition, C_RelativePosition, end_percent,
                 end_percentD, c_percent, c_percentD, c_percentC, d_percent, d_percentD, periodPPGs);

                WriteLabelForChartSpecimen(currentPeriod, idxSeries);
                periodPPGs.Add(currentPeriod);
                previousLengthPeriod = periodPPGs.Last().Length();

                previousIdxP = currentIdxP;
                currentIdxP++;
            }
           
            return periodPPGs.ToArray();
        }

        private void FindCharacteristicsExcel()
        {
            Statistics statistics = new Statistics();
            int countStartPeriods;
            string namefile = @"D:\ВУЗ\4 курс\Диплом\DATA\StartCharacteristics_2.xlsx";
            int startWrite = 1;
            string metod;
            int numberSheet;
            if (selectedStateFindCharacteristic == null)
            {
                metod = "Полный перебор значений";
                numberSheet = 1;
            }
            else
            {
                metod = "Градиентный метод";
                numberSheet = 2;
            }
             
            for (int i = statistics.CountStatPeriodsBegin; i <=statistics.CountStatPeriodsEnd; i++)
            {
                countStartPeriods = i;
                PeriodPPG[] periods = ReadStartCharacteristics(countStartPeriods, namefile);
                //PeriodPPG[] periodsSpecimen = FindCharacteristicsSpecimen(countStartPeriods);
                contextMenuStrip1.Enabled = false;
                int averageLengthPeriod = 0;
                int A_RelativePosition = 0;
                int B_RelativePosition = 0;
                int C_RelativePosition = 0;
                int D_RelativePosition = 0;
                int E_RelativePosition = 0;
                int previousIdxP = countStartPeriods - 1;
                int currentIdxP = countStartPeriods;
                List<PeriodPPG> periodPPGs = new List<PeriodPPG>();

                AddMarkedPeriods(ref periodPPGs, countStartPeriods, periods);
                int updateData = countStartPeriods;
                int updateFrequency = 10;
                int previousLengthPeriod = periodPPGs.Last().Length();
                UpdateRelativePosition(periodPPGs, countStartPeriods, ref averageLengthPeriod, ref A_RelativePosition, ref B_RelativePosition,
                                     ref C_RelativePosition, ref D_RelativePosition, ref E_RelativePosition);

                while (periodPPGs.Last().End + averageLengthPeriod < ppg.points.Length)
                {
                    // добавить проверку на критические точки и сглаживание, обновление через 10 периодов для с,d,e
                    PeriodPPG currentPeriod = new PeriodPPG();
                    updateData = currentIdxP % updateFrequency;
                    if (updateData == 0)
                    {
                        UpdateRelativePosition(periodPPGs, countStartPeriods, ref averageLengthPeriod, ref A_RelativePosition, ref B_RelativePosition,
                                      ref C_RelativePosition, ref D_RelativePosition, ref E_RelativePosition);
                    }

                    /* if(CalcCountExtr(Periods[currentIdxP].Begin, Periods[currentIdxP].End)>5)
                     {

                     }*/

                    int end_percent, end_percentD, c_percent, c_percentD, c_percentC, d_percent, d_percentD;
                    Initialization(out end_percent, out end_percentD, out c_percent, out c_percentD, out c_percentC, out d_percent, out d_percentD);
                    if (selectedStateFindCharacteristic == null || selectedStateFindCharacteristic == "Полный перебор значений")
                    {
                        currentPeriod = CalcCurrentPeriod(averageLengthPeriod, D_RelativePosition, C_RelativePosition, end_percent,
                     end_percentD, c_percent, c_percentD, c_percentC, d_percent, d_percentD, periodPPGs);
                    }
                    else if (selectedStateFindCharacteristic == "Градиентный метод")
                    {
                        currentPeriod = CalcCurrentPeriodGrad(averageLengthPeriod, B_RelativePosition, D_RelativePosition, C_RelativePosition, end_percent,
                      end_percentD, c_percent, c_percentD, c_percentC, d_percent, d_percentD, periodPPGs);
                    }

                    WriteLabelForChart(currentPeriod);
                    periodPPGs.Add(currentPeriod);
                    previousLengthPeriod = periodPPGs.Last().Length();

                    previousIdxP = currentIdxP;
                    currentIdxP++;
                }

                WriteIteration(periodPPGs,i, i - 2, statistics);
                int countColumn = (int)Math.Round(periodPPGs.ToArray().Length / 10.0);
                WriteStatisticsToExcel(statistics, i, i - 2, countColumn, startWrite, metod, numberSheet);
                startWrite = startWrite + countColumn + 2;
                MessageBox.Show(
                               "Характеристики найдены. " + "Количество начальных периодов = " + countStartPeriods +
                               " После просмотра периодов нажмите на кнопку \"Просмотр периодов закончен\"",
                               "Сообщение",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                
                Wait();
                IsEndWatch = true;
                if (i != statistics.CountStatPeriodsEnd) Draw(ppg);
            }
           
        }

        private static void Initialization(out int end_percent, out int end_percentD, out int c_percent, out int c_percentD, out int c_percentC, out int d_percent, out int d_percentD)
        {
            end_percent = 50;
            end_percentD = 5;
            c_percent = 10;
            c_percentD = 5;
            c_percentC = 60;
            d_percent = 10;
            d_percentD = 5;
        }

        private void Wait()
        {
            while (IsEndWatch)
            {
                Application.DoEvents();
            }
        }

        //обновить перед заливкой
        private void FindCharacteristics()
        {
            contextMenuStrip1.Enabled = false;
            int averageLengthPeriod = 0;
            int A_RelativePosition = 0;
            int B_RelativePosition = 0;
            int C_RelativePosition = 0;
            int D_RelativePosition = 0;
            int E_RelativePosition = 0;
            int previousIdxP = CountPeriods - 1;
            int currentIdxP = CountPeriods;
            List<PeriodPPG> periodPPGs = new List<PeriodPPG>();

            AddMarkedPeriods(ref periodPPGs,CountPeriods,Periods);
            int updateData = CountPeriods;

            while (periodPPGs.Last().End + averageLengthPeriod < ppg.points.Length)
            {
                // добавить проверку на критические точки и сглаживание, обновление через 10 периодов для с,d,e
                PeriodPPG currentPeriod = new PeriodPPG();
                updateData = currentIdxP % CountPeriods;
                if (updateData == 0)
                {
                    UpdateRelativePosition(periodPPGs, CountPeriods,ref averageLengthPeriod, ref A_RelativePosition, ref B_RelativePosition,
                                  ref C_RelativePosition, ref D_RelativePosition, ref E_RelativePosition);
                }

                /* if(CalcCountExtr(Periods[currentIdxP].Begin, Periods[currentIdxP].End)>5)
                 {

                 }*/

                int end_percent = 50;
                int end_percentD = 5;
                int c_percent = 10;
                int c_percentD = 5;
                int c_percentC = 60;
                int d_percent = 10;
                int d_percentD = 5;
                currentPeriod = CalcCurrentPeriod(averageLengthPeriod, D_RelativePosition, C_RelativePosition, end_percent,
                  end_percentD, c_percent, c_percentD, c_percentC, d_percent, d_percentD, periodPPGs);
                WriteLabelForChart(currentPeriod);
                periodPPGs.Add(currentPeriod);

                previousIdxP = currentIdxP;
                currentIdxP++;
            }
        }

        private PeriodPPG[] FindPeriods(int countPeriods, string nameFile, int[] duplicatePoints, double[] derivativePoints)
        {
            PeriodPPG[] periods = ReadPeriods(countPeriods, nameFile);
            contextMenuStrip1.Enabled = false;
            int averageLengthPeriod = 0;
            int previousIdxP = countPeriods - 1;
            int currentIdxP = countPeriods;
            List<PeriodPPG> periodPPGs = new List<PeriodPPG>();

            AddMarkedPeriods(ref periodPPGs, countPeriods, periods);
            int updateData = countPeriods;
            int updateFrequency = 10;
            averageLengthPeriod = CalcAverageLengthPeriod(countPeriods, periods);

            while (periodPPGs.Last().End + averageLengthPeriod < duplicatePoints.Length)
            {
                PeriodPPG currentPeriod = new PeriodPPG();
                updateData = currentIdxP % updateFrequency;
                if (updateData == 0)
                {
                    averageLengthPeriod = CalcAverageLengthPeriod(countPeriods, periods);
                }

                int end_percent = 50;
                int end_percentD = 5;
                currentPeriod.Begin = periodPPGs.Last().End;
                currentPeriod.End = CalcEndPeriod(currentPeriod, currentPeriod.Begin, averageLengthPeriod, end_percent, end_percentD, duplicatePoints, derivativePoints);

                periodPPGs.Add(currentPeriod);

                previousIdxP = currentIdxP;
                currentIdxP++;
            }

            //WriteLabelForChart(periodPPGs.ToArray());
            return periodPPGs.ToArray();
        }

        private void WriteStatisticsToExcel(Statistics statistics, int countPeriod, int idxP, int countStat, int startWrite, string metod, int numberSheet)
        {
            int countColumns = countStat+1;
            const int countRow = 41;
            string[] nameColumns = new string[countColumns];
            string[,] valueColumns = new string[countColumns, countRow];
            string[] NameStat = new String[countRow];
            valueColumns = FillNameColumns(countRow, ref valueColumns,countPeriod);
            nameColumns[0] = "кол-во итераций";
            for (int i = 1; i <= countColumns; i++)
            {
                if (i == countColumns)
                {
                    break;
                }
                nameColumns[i] = Convert.ToString(i * 10);
               
                for (int j = 0; j < countRow; j++)
                {
                    if ((j + 1) % 6 == 0)
                    {
                        valueColumns[i, j] = "";
                     }
                    else if (j % 6 == 0) switch (j / 6)
                        {
                            case 0:
                                    valueColumns[i, j+35] = " ";
                                    FillColumns(ref valueColumns, i, j+35, statistics.End[idxP].Average[i - 1], statistics.End[idxP].Min[i - 1], statistics.End[idxP].Max[i - 1], statistics.End[idxP].Dispersion[i - 1]);
                                    valueColumns[i, j] = " ";
                                    FillColumns(ref valueColumns, i, j, statistics.LenghtPeriod[idxP].Average[i - 1], statistics.LenghtPeriod[idxP].Min[i - 1], statistics.LenghtPeriod[idxP].Max[i - 1], statistics.LenghtPeriod[idxP].Dispersion[i - 1]);
                                break;
                            case 1:
                                valueColumns[i, j] = " ";
                                FillColumns(ref valueColumns, i,j, statistics.A[idxP].Average[i-1], statistics.A[idxP].Min[i-1], statistics.A[idxP].Max[i-1], statistics.A[idxP].Dispersion[i-1]);
                                break;
                            case 2:
                                valueColumns[i, j] = " ";
                                FillColumns(ref valueColumns,i, j, statistics.B[idxP].Average[i-1], statistics.B[idxP].Min[i-1], statistics.B[idxP].Max[i-1], statistics.B[idxP].Dispersion[i-1]);
                                break;
                            case 3:
                                valueColumns[i, j] = " ";
                                FillColumns(ref valueColumns,i, j, statistics.C[idxP].Average[i-1], statistics.C[idxP].Min[i-1], statistics.C[idxP].Max[i-1], statistics.C[idxP].Dispersion[i-1]);
                                break;
                            case 4:
                                valueColumns[i, j] = " ";
                                FillColumns(ref valueColumns, i,j, statistics.D[idxP].Average[i-1], statistics.D[idxP].Min[i-1], statistics.D[idxP].Max[i], statistics.D[idxP].Dispersion[i-1]);
                                break;
                            case 5:
                                valueColumns[i, j] = "E";
                                FillColumns(ref valueColumns,i, j, statistics.E[idxP].Average[i-1], statistics.E[idxP].Min[i-1], statistics.E[idxP].Max[i-1], statistics.E[idxP].Dispersion[i-1]);
                                break;
                        }
                }
            }
            string nameFile = @"D:\ВУЗ\4 курс\Диплом\DATA\Statistics.xlsx";
            string nameSheet = metod+" Statistics";
            WriteToExcel(nameColumns, valueColumns, countColumns, countRow,
                      nameFile, nameSheet, startWrite, numberSheet);
        }

        private static string[,] FillNameColumns(int countRow, ref string[,] valueColumns, int countPeriod)
        {
            for (int j = 0; j < countRow; j++)
            {
                if ((j + 1) % 6 == 0)
                {
                    valueColumns[0, j] = "";
                }
                else if (j % 6 == 0) switch (j / 6)
                    {
                        case 0:
                                valueColumns[0, j] = "Длина";
                                MarkColumns(ref valueColumns, j);
                                valueColumns[0, j+35] = "Конец периода";
                                MarkColumns(ref valueColumns, j+35);
                            break;
                        case 1:
                            valueColumns[0, j] = "A";
                            MarkColumns(ref valueColumns, j);
                            break;
                        case 2:
                            valueColumns[0, j] = "B";
                            MarkColumns(ref valueColumns, j);
                            break;
                        case 3:
                            valueColumns[0, j] = "C";
                            MarkColumns(ref valueColumns, j);
                            break;
                        case 4:
                            valueColumns[0, j] = "D";
                            MarkColumns(ref valueColumns, j);
                            break;
                        case 5:
                            valueColumns[0, j] = "E";
                            MarkColumns(ref valueColumns, j);
                            break;
                    }
            }

            valueColumns[0, 0] = Convert.ToString(countPeriod)+" (длина нач. данных)";

            return valueColumns;
        }

        private static void FillColumns(ref string[,] valueColumns,int i,  int j, double average, double min, double max, double dispersion)
        {
            valueColumns[i, j + 1] = Convert.ToString(average);
            valueColumns[i, j + 2] = Convert.ToString(min);
            valueColumns[i, j + 3] = Convert.ToString(max);
            valueColumns[i, j + 4] = Convert.ToString(dispersion);
        }

        private static void MarkColumns(ref string[,] valueColumns, int j)
        {
            valueColumns[0, j + 1] = "Среднее";
            valueColumns[0, j + 2] = "Min";
            valueColumns[0, j + 3] = "Max";
            valueColumns[0, j + 4] = "Средн. кв. отклонение";
        }

        private void WriteIteration(List<PeriodPPG>  periodPPGs, int countStartPeriods,int idxStatPeriod, Statistics statistics)
        {
           
            PeriodPPG[] periods = periodPPGs.ToArray();
            double sumLength = 0;
            double sumEnd = 0;
            double sumA = 0;
            double sumB = 0;
            double sumC = 0;
            double sumD = 0;
            double  sumE = 0;
            int lengthP = periods.Length;
            int countColumn = (int)Math.Round(lengthP / 10.0);
            int intPart = lengthP / 10;
            int floatPart = (lengthP % 10);
           
            statistics.End[idxStatPeriod].SetStatistics(countColumn);
            statistics.CountPeriods = lengthP;
            for (int k = 1; k <= countColumn; k++)
            {
                sumLength = 0;
                sumEnd = 0;
                sumA = 0;
                sumB = 0;
                sumC = 0;
                sumD = 0;
                sumE = 0;
                int endCalc;
                if (k <= intPart)
                {
                    endCalc = k * 10;
                }
                else
                {
                    endCalc = floatPart;
                }
                int startCalc = countStartPeriods;
                int n = endCalc - startCalc;

                for (int i = startCalc; i < endCalc; i++)
                {
                    sumLength = sumLength+ periods[i].Length();
                    sumEnd = sumEnd + periods[i].IterationEnd;
                    sumA = sumA + periods[i].IterationA;
                    sumB = sumB + periods[i].IterationB;
                    sumC = sumC + periods[i].IterationC;
                    sumD = sumD + periods[i].IterationD;
                    sumE = sumE + periods[i].IterationE;
                }

                int countC = CalcCountNotZero(periods, periods.Select(el => el.C).ToArray(), startCalc, endCalc);
                int countD = CalcCountNotZero(periods, periods.Select(el => el.D).ToArray(), startCalc, endCalc);
                int countE = CalcCountNotZero(periods, periods.Select(el => el.E).ToArray(), startCalc, endCalc);

                statistics.LenghtPeriod[idxStatPeriod].Average[k-1] = sumLength / n;
                statistics.End[idxStatPeriod].Average[k-1] = sumEnd/ n;
                statistics.A[idxStatPeriod].Average[k-1] = sumA / n;
                statistics.B[idxStatPeriod].Average[k-1] = sumB / n;
                statistics.C[idxStatPeriod].Average[k-1] = sumC / countC;
                statistics.D[idxStatPeriod].Average[k-1] = sumD / countD;
                statistics.E[idxStatPeriod].Average[k-1] = sumE / countE;

                statistics.LenghtPeriod[idxStatPeriod].Min[k-1] = CalcMin(periods.Select(l => l.Length()).ToArray(), startCalc, endCalc);
                statistics.End[idxStatPeriod].Min[k-1] = CalcMin(periods.Select(l => l.IterationEnd).ToArray(), startCalc, endCalc);
                statistics.A[idxStatPeriod].Min[k-1] = CalcMin(periods.Select(l => l.IterationA).ToArray(), startCalc, endCalc);
                statistics.B[idxStatPeriod].Min[k-1] = CalcMin(periods.Select(l => l.IterationB).ToArray(), startCalc, endCalc);
                statistics.C[idxStatPeriod].Min[k-1] = CalcMinCDE(periods.Select(l => l.IterationC).ToArray(), startCalc, endCalc);
                statistics.D[idxStatPeriod].Min[k-1] = CalcMinCDE(periods.Select(l => l.IterationD).ToArray(), startCalc, endCalc);
                statistics.E[idxStatPeriod].Min[k-1] = CalcMinCDE(periods.Select(l => l.IterationE).ToArray(), startCalc, endCalc);

                statistics.LenghtPeriod[idxStatPeriod].Max[k-1] = CalcMax(periods.Select(l => l.Length()).ToArray(), startCalc, endCalc);
                statistics.End[idxStatPeriod].Max[k-1] = CalcMax(periods.Select(l => l.IterationEnd).ToArray(), startCalc, endCalc);
                statistics.A[idxStatPeriod].Max[k-1] = CalcMax(periods.Select(l => l.IterationA).ToArray(), startCalc, endCalc);
                statistics.B[idxStatPeriod].Max[k-1] = CalcMax(periods.Select(l => l.IterationB).ToArray(), startCalc, endCalc);
                statistics.C[idxStatPeriod].Max[k-1] = CalcMaxCDE(periods.Select(l => l.IterationC).ToArray(), startCalc, endCalc);
                statistics.D[idxStatPeriod].Max[k-1] = CalcMaxCDE(periods.Select(l => l.IterationD).ToArray(), startCalc, endCalc);
                statistics.E[idxStatPeriod].Max[k-1] = CalcMaxCDE(periods.Select(l => l.IterationE).ToArray(), startCalc, endCalc);

                statistics.LenghtPeriod[idxStatPeriod].Dispersion[k-1] = Math.Sqrt(CalcDispersion(periods.Select(l => l.Length()).ToArray(), startCalc, endCalc, statistics.LenghtPeriod[idxStatPeriod].Average[k-1]));
                statistics.End[idxStatPeriod].Dispersion[k-1] = Math.Sqrt(CalcDispersion(periods.Select(l => l.IterationEnd).ToArray(), startCalc, endCalc, statistics.End[idxStatPeriod].Average[k-1]));
                statistics.A[idxStatPeriod].Dispersion[k-1] = Math.Sqrt(CalcDispersion(periods.Select(l => l.IterationA).ToArray(), startCalc, endCalc, statistics.A[idxStatPeriod].Average[k-1]));
                statistics.B[idxStatPeriod].Dispersion[k-1] = Math.Sqrt(CalcDispersion(periods.Select(l => l.IterationB).ToArray(), startCalc, endCalc,statistics.B[idxStatPeriod].Average[k-1]));
                statistics.C[idxStatPeriod].Dispersion[k-1] = Math.Sqrt(CalcDispersionCDE(periods.Select(l => l.IterationC).ToArray(), startCalc, endCalc, statistics.C[idxStatPeriod].Average[k-1], countC));
                statistics.D[idxStatPeriod].Dispersion[k-1] = Math.Sqrt(CalcDispersionCDE(periods.Select(l => l.IterationD).ToArray(), startCalc, endCalc, statistics.D[idxStatPeriod].Average[k-1], countD));
                statistics.E[idxStatPeriod].Dispersion[k-1] = Math.Sqrt(CalcDispersionCDE(periods.Select(l => l.IterationE).ToArray(), startCalc, endCalc, statistics.E[idxStatPeriod].Average[k-1], countE));
            }

        }
        int CalcMinCDE(int[] point, int startIdx, int endIdx)
        {
            int min = int.MaxValue;
            for (int i = startIdx; i < endIdx; i++)
            {
                if ((point[i] < min)&&(point[i] != 0))
                {
                    min = point[i];
                }
            }

            return min;
        }

        int CalcMin(int[] point, int startIdx, int endIdx)
        {
            int min = int.MaxValue;
            for (int i = startIdx; i < endIdx; i++)
            {
                if (point[i] < min)
                {
                    min = point[i];
                }
            }

            return min;
        }


        double CalcDispersion(int[] point, int startIdx,int endIdx, double average)
        {
            double sum = 0;
            for(int i = startIdx; i< endIdx; i++)
            {
                sum += Math.Pow((point[i] - average), 2);
            }

            return sum/((endIdx - startIdx+1) - 1);
        }

        double CalcDispersionCDE(int[] point, int startIdx, int endIdx, double average, int countNotZero)
        {
            double sum = 0;
            for (int i = startIdx; i < endIdx; i++)
            {
                if (point[i] != 0)
                { sum += Math.Pow((point[i] - average), 2); }
            }

            return sum / (countNotZero - 1);
        }

        int CalcMax(int[] point, int startIdx,int endIdx)
        {
            int max = int.MinValue;
            for (int i = startIdx; i < endIdx; i++)
            {
                if (point[i] > max)
                {
                    max = point[i];
                }
            }

            return max;
        }

        int CalcMaxCDE(int[] point, int startIdx, int endIdx)
        {
            int max = int.MinValue;
            for (int i = startIdx; i < endIdx; i++)
            {
                if ((point[i] > max)&&(point[i]!=0))
                {
                    max = point[i];
                }
            }

            return max;
        }

        int CalcCountNotZero(PeriodPPG[] periods, int[] statPoints,int startIdx, int endIdx)
        {
            int countNotZero = 0;
            for(int i = startIdx; i< endIdx; i++ )
            {
                if(statPoints[i]!=0)
                {
                    countNotZero++;
                }
            }
            return countNotZero;
        }

        private PeriodPPG CalcCurrentPeriod(int averageLengthPeriod, int D_RelativePosition, int С_RelativePosition,
                                            int end_percent, int end_percentD, int c_percent, int c_percentD, int c_percentC,int d_percent, int d_percentD, List<PeriodPPG> periodPPGs)
        {
            //убрать лишние переменные периода
            PeriodPPG currentPeriod = new PeriodPPG();
            currentPeriod.Begin = periodPPGs.Last().End;
            currentPeriod.End = CalcEndPeriod(ref currentPeriod,currentPeriod.Begin, averageLengthPeriod, end_percent, end_percentD);
            currentPeriod.B = CalcB(ref currentPeriod,currentPeriod.Begin, currentPeriod.End);
            currentPeriod.A = CalcA(ref currentPeriod,currentPeriod.Begin+5, currentPeriod.B);
           /* currentPeriod.C = CalcС(С_RelativePosition, currentPeriod.Length(), c_percent, c_percentD, c_percentC, currentPeriod.Begin, currentPeriod.B, currentPeriod.End);
            if (currentPeriod.C == -1)
            {
                currentPeriod.C = 0;
            }
            else
            {
                currentPeriod.D = CalcD(D_RelativePosition, currentPeriod.Length(), d_percent, d_percentD, currentPeriod.Begin, currentPeriod.C, currentPeriod.End);
            }*/
            return currentPeriod;
        }

        int FindNextMax(int begin, int countIteration)
        {
            int idxNextMax = begin;
            while (!IsMax(dppg.points[idxNextMax].y,dppg.points[idxNextMax - 1].y,dppg.points[idxNextMax + 1].y))
            {
                idxNextMax++;
                countIteration++;
            }

            return idxNextMax;
        }

        int FindNextMin(int begin, int countIteration)
        {
            int idxNextMin = begin;
            while (!IsMin(dppg.points[idxNextMin].y, dppg.points[idxNextMin - 1].y, dppg.points[idxNextMin + 1].y))
            {
                idxNextMin++;
                countIteration++;
            }

            return idxNextMin;
        }


        private PeriodPPG CalcCurrentPeriodGrad(int averageLengthPeriod, int B_RelativePosition, int D_RelativePosition, int С_RelativePosition,
                                            int end_percent, int end_percentD, int c_percent, int c_percentD, int c_percentC, int d_percent, int d_percentD, List<PeriodPPG> periodPPGs)
        {
            //убрать лишние переменные периода
            PeriodPPG currentPeriod = new PeriodPPG();
            currentPeriod.Begin = periodPPGs.Last().End;
            currentPeriod.End = CalcEndPeriodGrad(ref currentPeriod, currentPeriod.Begin, averageLengthPeriod, end_percent, end_percentD);
            //currentPeriod.B = CalcBGrad(B_RelativePosition, currentPeriod, averageLengthPeriod, currentPeriod.Begin, currentPeriod.End);
           //currentPeriod.A = CalcA(currentPeriod, currentPeriod.Begin, currentPeriod.B);
            /*currentPeriod.C = CalcС(С_RelativePosition, currentPeriod.Length(), c_percent, c_percentD, c_percentC, currentPeriod.Begin, currentPeriod.B, currentPeriod.End);
            if (currentPeriod.C == -1)
            {
                currentPeriod.C = 0;
            }
            else
            {
                currentPeriod.D = CalcD(D_RelativePosition, currentPeriod.Length(), d_percent, d_percentD, currentPeriod.Begin, currentPeriod.C, currentPeriod.End);
            }*/
            return currentPeriod;
        }


        private void AddMarkedPeriods(ref List<PeriodPPG> periodPPGs, int countPeriods, PeriodPPG [] periods)
        {
            for(int i = 0; i < countPeriods; i++)
            {
                periodPPGs.Add(periods[i]);
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
            }
            else if (IsFoundDerivative == false)
            {
                MessageBox.Show(
                               "Нажмите на кнопку \"Найти производную\"",
                               "Сообщение",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
            }
            else if (PeriodClick != CountPeriods + 1)
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

        private void UpdateRelativePosition(List<PeriodPPG> periodPPGs, int countPeriods, ref int averageLengthPeriod, ref int A_RelativePosition, ref int B_RelativePosition, 
                                            ref int C_RelativePosition, ref int D_RelativePosition, ref int E_RelativePosition)
        {
            PeriodPPG[] periods = PeriodsToArray(periodPPGs, countPeriods);
            averageLengthPeriod = CalcAverageLengthPeriod(countPeriods, periods);
            A_RelativePosition = CalcRelativePosition(periods,periods.Select(p => p.A).ToArray(), countPeriods);
            B_RelativePosition = CalcRelativePosition(periods,periods.Select(p => p.B).ToArray(), countPeriods);
            /*C_RelativePosition = CalcRelativePosition(periods,periods.Select(p => p.C).ToArray(), countPeriods);
            D_RelativePosition = CalcRelativePosition(periods,periods.Select(p => p.D).ToArray(), countPeriods);
            E_RelativePosition = CalcRelativePosition(periods,periods.Select(p => p.E).ToArray(), countPeriods);*/
        }

        private PeriodPPG[] PeriodsToArray(List<PeriodPPG> periodPPGs, int countPeriods)
        {
            PeriodPPG[] periods = new PeriodPPG[countPeriods];
            int end = periodPPGs.Count - 1;
            int begin = periodPPGs.Count - countPeriods;

            for (int i= countPeriods - 1; i>=0;i--)
            {
                periods[i] = periodPPGs.Last();
            }

            return periods;
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

        private int CalcB(ref PeriodPPG period, int begin, int  end)
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
                period.IterationB++;
            }

            return idxB;
        }

        //не правильно работает
        private int CalcD(int D_RelativePosition, int periodLength, int percent, int percentD, int beginPeriod, int begin, int end)
        {
            int predictionD = (int)Math.Round((Convert.ToDouble(D_RelativePosition) / 100) * periodLength);
            int idxD = -1;
            int searchInterval = percent * periodLength / 100;
            int interval = percentD * periodLength / 100;
            int beginSearch = beginPeriod + predictionD - searchInterval;
            int endSearch = beginPeriod + predictionD + searchInterval;
            if ( beginSearch < begin)
            {
                beginSearch = begin;
                endSearch = begin + 2*searchInterval;
            }
            
            for (int i = beginSearch; i < endSearch; i++)
            {
                if (IsMax(dppg.points[i].y, dppg.points[i - 1].y, dppg.points[i + 1].y))
                {
                    return i;
                }
            }

            while((idxD == -1)||(endSearch<end))
            {
                beginSearch = endSearch;
                endSearch = endSearch + searchInterval;
                for (int i = beginSearch; i < endSearch; i++)
                {
                    if (IsMax(dppg.points[i].y, dppg.points[i - 1].y, dppg.points[i + 1].y))
                    {
                        idxD = i;
                    }
                }
            }

            return idxD;
        }

        private int CalcС(int С_RelativePosition, int periodLength, int percent, int percentD, int percentC, int beginPeriod, int begin, int end)
        {
            int predictionС = (int)Math.Round((Convert.ToDouble(С_RelativePosition) / 100) * periodLength);
            int idxC =-1;
            int searchInterval = percent * periodLength / 100;
            for (int i = beginPeriod + predictionС- searchInterval; i < beginPeriod + predictionС + searchInterval; i++)
            {
                if (!IsMin(dppg.points[i].y, dppg.points[i - 1].y, dppg.points[i + 1].y))
                {
                    return i;
                }
            }
            if (idxC == -1)
            {
                searchInterval = percentC * periodLength / 100;
                for (int i = begin; i < beginPeriod + searchInterval; i++)
                {
                    if (!IsMin(dppg.points[i].y, dppg.points[i - 1].y, dppg.points[i + 1].y))
                    {
                        return i;
                    }
                }
            }
            
             return idxC;
        }


        private int CalcA(ref PeriodPPG period,int begin, int end)
        {
            int idxA = -1;
            double maxDPPG = int.MinValue;
            for (int i = begin; i < end; i++)
            {
                if(dppg.points[i].y >= maxDPPG)
                {
                    maxDPPG = dppg.points[i].y;
                    idxA = i;
                }
                period.IterationA++;
            }
            List<int> maxA = new List<int>();

            for (int i = begin; i < end; i++)
            {
                if (dppg.points[i].y == maxDPPG)
                {
                    maxA.Add(i);
                }
                period.IterationA++;
            }

            idxA = (int)maxA.Average();
            return idxA;
        }

        private int CalcEndPeriod(ref PeriodPPG periodPPG, int begin, int averageLength, int percent, int percentD)
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
                periodPPG.IterationEnd++;
            }
            
            idxEnd = CheckDerivativeMin(periodPPG, averageLength, percentD,  idxEnd);

            return idxEnd;

        }

        private int CalcEndPeriod(PeriodPPG periodPPG, int begin, int averageLength, int percent, int percentD, int[] duplicatePoints, double[] derivativePoints)
        {
            int searchInterval = percent * averageLength / 100;
            int min = int.MaxValue;
            int idxEnd = -1;
            int idxSearchEnd = begin + averageLength + searchInterval;
            if (idxSearchEnd > duplicatePoints.Length)
            {
                idxSearchEnd = duplicatePoints.Length;
            }
            for (int i = begin + searchInterval; i < idxSearchEnd; i++)
            {
                if (duplicatePoints[i] < min)
                {
                    min = duplicatePoints[i];
                    idxEnd = i;
                }
                periodPPG.IterationEnd++;
            }

            idxEnd = CheckDerivativeMax(periodPPG, averageLength, percentD, idxEnd, derivativePoints);

            return idxEnd;

        }

        private int CalcEndPeriodGrad(ref PeriodPPG periodPPG, int begin, int averageLengthPeriod, int percent, int percentD)
        {
            int persent60 = (int)Math.Round(averageLengthPeriod * 0.6);
            int persent20 = (int)Math.Round(averageLengthPeriod * 0.2);
            int persent5 = (int)Math.Round(averageLengthPeriod * 0.05);
            int predictionEnd = begin + averageLengthPeriod - persent20;
            int idxEnd = predictionEnd;
            int beginCheck = idxEnd;
            int endCheck = idxEnd + persent5;
            if(begin + averageLengthPeriod > dppg.points.Length)
            {
                return begin + persent5;
            }
            while (!DerivativeIsIncreasing(beginCheck, endCheck))
            {
                idxEnd = DerivativeIncreasing(beginCheck, endCheck, idxEnd);
                while (!IsMin(dppg.points[idxEnd].y, dppg.points[idxEnd - 1].y, dppg.points[idxEnd + 1].y))
                {
                    if ((dppg.points[idxEnd].y <=0)&&(dppg.points[idxEnd+1].y > 0))
                    {
                        idxEnd++;
                    }
                    else
                    {
                        idxEnd--;
                    }
                    periodPPG.IterationEnd++;
                }
                idxEnd = idxEnd - 1;
                beginCheck = idxEnd+2;
                endCheck = idxEnd + persent5;
                periodPPG.IterationEnd++;
            }
             
            return idxEnd;
        }

        int DerivativeIncreasing(int begin, int end,int idx)
        {
            for (int i = begin; i < end; i++)
            {
                if (dppg.points[i].y < 0)
                {
                    return i;
                }
            }

            return idx;
        }

        bool DerivativeIsIncreasing(int begin,int end)
        { 
            for(int i = begin;i<end;i++)
            {
                if (dppg.points[i].y<0)
                {
                    return false;
                }
            }

            return true;
        }


        private int CalcBGrad(int B_RelativePosition, PeriodPPG period, int averageLengthPeriod, int begin, int end)
        {
            int predictionB = (int)Math.Round((Convert.ToDouble(B_RelativePosition) / 100) * averageLengthPeriod);
            int idxB = begin + predictionB;
            while (!IsMax(dppg.points[idxB].y, dppg.points[idxB - 1].y, dppg.points[idxB + 1].y))
            {
                if (dppg.points[idxB].y > 0)
                {
                    idxB++;
                }
                else
                {
                    idxB--;
                }
                period.IterationB++;
            }

            return idxB;
        }

        bool IsMin(double Derivative, double previousDerivative, double nextDerivative)
        {
            /*bool isMin = false;
             * DPPG dPPG,ref int idx
            if((dPPG.points[idx - 1].y < 0) && (dPPG.points[idx + 1].y > 0))
            {
                isMin = true;
                return true;
            }
            else if((dPPG.points[idx - 1].y == 0)&&(dPPG.points[idx].y>0))
            {
                if (dPPG.points[idx - 2].y < 0)
                {
                    idx = idx - 1;
                    isMin = true;
                }
                else return false;
            }
            else if ((dPPG.points[idx + 1].y == 0) && (dPPG.points[idx - 1].y < 0))
            {
                if (dPPG.points[idx + 2].y > 0)
                {
                    idx = idx - 1;
                    isMin = true;
                }
                else return false;
            }

            return isMin;*/

            return ((previousDerivative < 0) && (nextDerivative > 0));
        }

       

        bool IsMax(double Derivative, double previousDerivative, double nextDerivative)
        {
            return ((Derivative == 0) && (previousDerivative > 0) && (nextDerivative < 0));
        }

        private int CheckDerivativeMin(PeriodPPG periodPPG, int lengthPeriod, int percentD, int idx)
        {
            if (IsMin(dppg.points[idx].y,dppg.points[idx-1].y ,dppg.points[idx + 1].y))
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
                periodPPG.IterationEnd++;
            }

            return idx;
        }

        private int CheckDerivativeMax(PeriodPPG periodPPG, int lengthPeriod, int percentD, int idx, double[] derivativePoints)
        {
            if (IsMax(derivativePoints[idx], derivativePoints[idx - 1], derivativePoints[idx + 1]))
            {
                return idx;
            }

            int searchInterval = percentD * lengthPeriod / 100;
            int idxSearchEnd = idx + searchInterval;

            if (idxSearchEnd > derivativePoints.Length)
            {
                idxSearchEnd = derivativePoints.Length;
            }

            for (int i = idx - searchInterval; i < idxSearchEnd; i++)
            {
                if (IsMax(derivativePoints[idx], derivativePoints[idx - 1], derivativePoints[idx + 1]))
                {
                    idx = i;
                }
                periodPPG.IterationEnd++;
            }

            return idx;
        }

        private int CheckDerivativeMin(int lengthPeriod, int percentD, int idx)
        {
            if (IsMin(dppg.points[idx].y,dppg.points[idx-1].y,dppg.points[idx+1].y))
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


        private int CalcAverageLengthPeriod(int countPeriods, PeriodPPG[] periods)
        {
            int sum = 0;
            for (int i = 0; i < countPeriods; i++)
            {
                sum += periods[i].Length();
            }
             
            return sum / countPeriods;
        }

        private bool IsZeroPoint(int[] point)
        {
            bool isZero = true;
            for(int i =0; i<point.Length;i++)
            {
                if(point[i]!=0)
                {
                    isZero = false;
                    return false;
                }
            }

            return isZero;
        }

        private int CalcRelativePosition(PeriodPPG[] periods, int[] point, int lengthForCalc)
        {
            
            if (IsZeroPoint(point))
            {
                return 0;
            }
            else
            {
                int[] RelativePosition = new int[lengthForCalc];
                int idxPeriod = 0;
                for (int i = 0; i < lengthForCalc; i++)
                {
                    idxPeriod = CalcPeriod(periods, point[i], lengthForCalc);
                    RelativePosition[i] = (int)Math.Round((Convert.ToDouble(point[i] - periods[idxPeriod].Begin) /
                                            periods[idxPeriod].Length()) * 100);
                }

                int sum = 0;
                for (int i = 0; i < lengthForCalc; i++)
                {
                    sum += RelativePosition[i];
                }

                return sum / lengthForCalc;
            }
        }

        private int CalcPeriod(PeriodPPG[] periods, int point, int countPeriods)
        {
            for(int i = 0; i < countPeriods; i++)
            {
                if ((point > periods[i].Begin)&&(point < periods[i].End))
                {
                    return i;
                }
            }
            return -1;
        }

        public double[] differentiation1orderAccuracy(int[] points, int h)
        {
            int n = points.Length;
            double[] derivativePoints = new double[n];

            derivativePoints[0] = (points[1] - points[0]) / h;
            derivativePoints[n - 1] = (points[n - 1] - points[n - 2]) / h;
            for (int i = 1; i < n - 1; i++)
            {
                derivativePoints[i] = Convert.ToDouble(points[i + 1] - points[i]) / Convert.ToDouble(2 * h);
            }

            return derivativePoints;
        }

        public double[] differentiation2orderAccuracy(int[] points, int h)
        {
            int n = points.Length;
            double[] derivativePoints = new double[n];

            for (int i = n - 2; i > 0; i--)
            {
                derivativePoints[i + 1] = Convert.ToDouble(points[i - 1] - 4 * points[i] + 3 * points[i + 1]) / Convert.ToDouble(2 * h);
                //if (derivativePoints[i + 1] == 0)
                //{
                //    richTextBox1.Text += Convert.ToString(i + 1) + '\n';
                //}
            }

            return derivativePoints;
        }

        public double[] differentiation4points(int[] points, int h)
        {
            int n = points.Length;
            double[] derivativePoints = new double[n];

            for (int i = 2; i < n - 2; i++)
            {
                derivativePoints[i] = Convert.ToDouble(points[i - 2] - 8 * points[i - 1] + 8 * points[i + 1] - points[i + 2]) / Convert.ToDouble(12 * h);
                //if (derivativePoints[i + 1] == 0)
                //{
                //    richTextBox1.Text += Convert.ToString(i + 1) + '\n';
                //}
            }

            return derivativePoints;
        }

        private double[] SmoothingDerivative(double[] derivativePoints)
        {
            int n = derivativePoints.Length;
            double k1, k2;
            int buf;
            double del1, del2, len;
            for (int i = 0; i < n - 1; i++) 
            {
                if (derivativePoints[i] < 0 && derivativePoints[i + 1] == 0)
                {
                    buf = i;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (derivativePoints[j] > 0)
                        {
                            len = j - buf - 1;
                            k1 = Math.Round(len / 2.0);
                            k2 = len - k1 - 1;
                            del1 = (-derivativePoints[buf]) / (k1 + 1);
                            del2 = derivativePoints[j] / (k2 + 1);
                            for (int l = buf + 1; l < buf + k1 + 1; l++)
                            {
                                //if (l == buf + 1)
                                //derivativePoints[l] = derivativePoints[l - 1];
                                //else 
                                derivativePoints[l] = derivativePoints[l - 1] + del1;
                            }
                            for (int l = j - 1; l > buf + k1 + 1; l--)
                            {
                                //if (l == j - 1)
                                // derivativePoints[l] = derivativePoints[l + 1];
                                //else 
                                derivativePoints[l] = derivativePoints[l + 1] - del2;
                            }
                            i = j - 1;
                            break;
                        }

                        else if (derivativePoints[j] != 0)
                        {
                            i = j - 1;
                            break;
                        }

                    }
                }
            }

            for (int i = 0; i < n - 1; i++) 
            {
                if (derivativePoints[i] > 0 && derivativePoints[i + 1] == 0)
                {
                    buf = i;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (derivativePoints[j] < 0)
                        {
                            len = j - buf - 1;
                            k1 = Math.Round(len / 2.0);
                            k2 = len - k1 - 1;
                            del1 = derivativePoints[buf] / (k1 + 1);
                            del2 = (-derivativePoints[j]) / (k2 + 1);
                            for (int l = buf + 1; l < buf + k1 + 1; l++)
                            {
                                //if (l == buf + 1)
                                //derivativePoints[l] = derivativePoints[l - 1];
                                //else 
                                derivativePoints[l] = derivativePoints[l - 1] - del1;
                            }
                            for (int l = j - 1; l > buf + k1 + 1; l--)
                            {
                                //if (l == j - 1)
                                // derivativePoints[l] = derivativePoints[l + 1];
                                //else 
                                derivativePoints[l] = derivativePoints[l + 1] + del2;
                            }
                            i = j - 1;
                            break;
                        }

                        else if (derivativePoints[j] != 0)
                        {
                            i = j - 1;
                            break;
                        }

                    }
                }
            }


            for (int i = 0; i < n - 2; i++)
            {
                if (derivativePoints[i] != 0)
                {
                    if (derivativePoints[i] == derivativePoints[i + 2]) derivativePoints[i + 1] = derivativePoints[i];
                    for (int number = -3; number < 4; number++)
                    {
                        if (number == 0) break;
                        if (derivativePoints[i] == number && number < 0)
                        {
                            for (int j = i + 1; j < n; j++)
                            {
                                if (derivativePoints[j] > (number + 1) || derivativePoints[j] < number) break;
                                if (derivativePoints[j] == number)
                                    for (int k = i + 1; k < j + 1; k++)
                                        derivativePoints[k] = derivativePoints[i];
                            }
                        }
                        if (derivativePoints[i] == number && number > 0)
                        {
                            for (int j = i + 1; j < n; j++)
                            {
                                if (derivativePoints[j] < (number - 1) || derivativePoints[j] > number) break;
                                if (derivativePoints[j] == number)
                                    for (int k = i + 1; k < j + 1; k++)
                                        derivativePoints[k] = derivativePoints[i];
                            }
                        }
                    }
                }
            }

            return derivativePoints;
        }

        double[] derivativePoints;
        private void buttonFindDerivate_Click(object sender, EventArgs e)
        {
            if (drawChartClick == false)
            {
                MessageBox.Show("Для нахождения производной сначала нужно нарисовать график ФПГ. Нажмите \" Нарисовать график\"",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            else
            {
                int rate = int.Parse(textBoxSinglingRate.Text); //частота прореживания
                richTextBox1.Text = "";
                int n = duplicatePoints.Length;
                PointDPPG[] pointDPPG = new PointDPPG[n];
                

                if (selectedState == null || selectedState == "Дифференцирование первого порядка точности")
                derivativePoints = differentiation1orderAccuracy(duplicatePoints, rate);


                if (selectedState == "Дифференцирование второго порядка точности") 
                {
                    derivativePoints = differentiation2orderAccuracy(duplicatePoints, rate);
                }

                if (selectedState == "Дифференцирование по 4 узловым точкам")
                {
                    derivativePoints = differentiation4points(duplicatePoints, rate);
                }

                derivativePoints = SmoothingDerivative(derivativePoints);

                for (int i = 0; i < n; i++)
                {
                    pointDPPG[i] = new PointDPPG(i, derivativePoints[i]);
                }
                dppg = new DPPG(pointDPPG, n);
                IsFoundDerivative = true;
                Draw(dppg);
                chartDPPG.ChartAreas[0].AxisX.ScaleView.Size = chartPPG.ChartAreas[0].AxisX.ScaleView.Size;
            }
        }

        private double[] SecondDerivation(int[] points, int h)
        {
            int n = points.Length;
            double[] secondDerivativePoints = new double[n];

            for (int i = 1; i < n - 2; i++) 
            {
                secondDerivativePoints[i - 1] = (2 * points[i - 1] - 5 * points[i] + 4 * points[i + 1] - points[i + 2]) / (h * h);
            }

            return secondDerivativePoints;
        }
        
        private void buttonFindSecondDerivate_Click(object sender, EventArgs e)
        {
            if (IsFoundDerivative == false)
            {
                MessageBox.Show("Для нахождения второй производной, сначала нужно найти первую производную",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            else
            {
                int n = derivativePoints.Length;
                double[] secondDerivativePoints = new double[n];
                int rate = int.Parse(textBoxSinglingRate.Text);

                secondDerivativePoints = SecondDerivation(duplicatePoints, rate);

                secondDerivativePoints = SmoothingDerivative(secondDerivativePoints);

                PointDDPPG[] pointDDPPG = new PointDDPPG[n];
                for (int i = 0; i < n; i++)
                {
                    pointDDPPG[i] = new PointDDPPG(i, secondDerivativePoints[i]);
                }
                ddppg = new DDPPG(pointDDPPG, n);
                Draw(ddppg);
                chartDDPPG.ChartAreas[0].AxisX.ScaleView.Size = chartPPG.ChartAreas[0].AxisX.ScaleView.Size;
            }
        }

        private void WriteToExcel(string [] nameСolumns, string [,] valueColumns, int countColumns, int countValues, 
                                string nameFile, string nameSheet, int startWrite=1,int numberSheet=1)
        {
            FileInfo file = new FileInfo(nameFile);
            Random r = new Random();
            if ((!file.Exists|| startWrite==1)&&(numberSheet==1))
            {
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PPGReader";
                    excelPackage.Workbook.Properties.Title = "Characteristics";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;
                    ExcelWorksheet  worksheet = excelPackage.Workbook.Worksheets.Add(nameSheet);
                  
                    for (int j = startWrite; j < startWrite + countColumns; j++)
                    {
                        worksheet.Cells[1, j].Value = nameСolumns[j - startWrite];
                    }
                    for (int j = startWrite; j < startWrite + countColumns; j++)
                    {
                        for (int i = 2; i <= countValues + 1; i++)
                        {
                            worksheet.Cells[i, j].Value = valueColumns[j - startWrite, i - 2];
                        }
                    }
                    excelPackage.SaveAs(file);
                }
            } else
            {
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PPGReader";
                    excelPackage.Workbook.Properties.Title = "Characteristics";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;
                    while(excelPackage.Workbook.Worksheets.ToArray().Length <= numberSheet)
                    {
                        excelPackage.Workbook.Worksheets.Add(Convert.ToString(r.Next()));
                        if (excelPackage.Workbook.Worksheets.ToArray().Length == numberSheet)
                        { excelPackage.Workbook.Worksheets[numberSheet].Name=nameSheet; }
                    }
                  
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[numberSheet];

                    for (int j = startWrite; j < startWrite + countColumns; j++)
                    {
                        worksheet.Cells[1, j].Value = nameСolumns[j - startWrite];
                    }
                    for (int j = startWrite; j < startWrite + countColumns; j++)
                    {
                        for (int i = 2; i <= countValues + 1; i++)
                        {
                            worksheet.Cells[i, j].Value = valueColumns[j - startWrite, i - 2];
                        }
                    }
                    
                    excelPackage.SaveAs(file);
                }
            }

        }

        private void WriteExcel(string[] nameСolumns, string[,] valueColumns, int countColumns, int countValues,
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

                for (int j = 0; j < countColumns; j++)
                {
                    worksheet.Cells[1, j + 1].Value = nameСolumns[j];
                }

                for (int i = 0; i < countValues; i++)
                    for (int j = 0; j < countColumns; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = valueColumns[i, j];
                    }

                //for (int j = 1; j <= countColumns; j++)
                //{
                //    worksheet.Cells[1, j].Value = nameСolumns[j - 1];
                //}

                //for (int j = 2; j <= countColumns + 1; j++)
                //    for (int i = 1; i <= countValues; i++)
                //    {
                //        worksheet.Cells[j, i].Value = valueColumns[j - 2, i - 1];
                //    }

                //for (int j = 1; j <= countColumns; j++) 
                //{ 
                // for (int i = 2; i <= countValues + 1; i++) 
                // { 
                // worksheet.Cells[i, j].Value = valueColumns[j - 1, i - 2]; 
                // } 
                //} 

                //Save your file 
                FileInfo file = new FileInfo(nameFile); 
                excelPackage.SaveAs(file);
            }

        }


        private void WriteExcel(string[] nameСolumns, double[,] valueColumns, int countColumns, int countValues,
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

                for (int j = 1; j <= countColumns; j++)
                {
                    worksheet.Cells[1, j].Value = nameСolumns[j - 1];
                }
                for (int j = 1; j <= countColumns; j++)
                {
                    for (int i = 2; i <= countValues + 1; i++)
                    {
                        worksheet.Cells[i, j].Value = valueColumns[j - 1, i - 2];
                    }
                }

                //Save your file
                FileInfo file = new FileInfo(nameFile);
                excelPackage.SaveAs(file);
            }

        }
      
        private PeriodPPG[] ReadStartCharacteristics(int countPeriods,string namefile)
        {
            
            string[,] excelData = ReadExcelSheet(namefile);
            int rows = excelData.GetUpperBound(0) + 1;
            int columns = excelData.Length / rows;
            PeriodPPG[] periods = new PeriodPPG[countPeriods];
            for(int i =0; i< countPeriods;i++)
            {
                periods[i] = new PeriodPPG();
            }
            for (int j = 1; j < countPeriods+1; j++)
            {
                for(int i = 1; i < rows; i++)
                {
                    switch (i)
                    {
                        case 1:
                            periods[j-1].Begin = Convert.ToInt32(excelData[i,j]);
                            break;
                        case 2:
                            periods[j-1].End = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 3:
                            periods[j-1].A = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 4:
                            periods[j-1].B = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 5:
                            periods[j-1].C = Convert.ToInt32(excelData[i, j]);
                            break;
                        case 6:
                            periods[j-1].D = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 7:
                            periods[j-1].E = Convert.ToInt32(excelData[i, j]); ;
                            break;
                    }
                }
            }

            WriteLabelForChart(periods);
            return periods;
        }

        private PeriodPPG[] ReadPeriods(int countPeriods, string nameFile)
        {
            //string namefile = @"J:\Documents\8 семестр\Диплом\Characteristics.xlsx";
            string[,] excelData = ReadExcelSheet(nameFile);
            int rows = excelData.GetUpperBound(0) + 1;
            int columns = excelData.Length / rows;
            PeriodPPG[] periods = new PeriodPPG[countPeriods];
            for (int i = 0; i < countPeriods; i++)
            {
                periods[i] = new PeriodPPG();
            }
            for (int j = 1; j < countPeriods + 1; j++)
            {
                for (int i = 1; i < rows; i++)
                {
                    switch (i)
                    {
                        case 1:
                            periods[j - 1].Begin = Convert.ToInt32(excelData[i, j]);
                            break;
                        case 2:
                            periods[j - 1].End = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 3:
                            periods[j - 1].A = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 4:
                            periods[j - 1].B = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 5:
                            periods[j - 1].C = Convert.ToInt32(excelData[i, j]);
                            break;
                        case 6:
                            periods[j - 1].D = Convert.ToInt32(excelData[i, j]); ;
                            break;
                        case 7:
                            periods[j - 1].E = Convert.ToInt32(excelData[i, j]); ;
                            break;
                    }
                }
            }

            return periods;
        }

        private string[,] ReadExcelSheet(string namefile)
        {
            //read the Excel file as byte array
            byte[] bin = File.ReadAllBytes(namefile);
            string[,] excelData = null;

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
                                excelData[j - 1, i - 1] = worksheet.Cells[i, j].Value.ToString();
                            }
                        }
                    }
                }

            }
            return excelData;
        }

        private void comboBoxDifferentiationMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedState = comboBoxDifferentiationMethod.SelectedItem.ToString();
        }

        private int[] CountZero(PeriodPPG[] periodPPG, double[] derivativePoints)
        {
            int n = periodPPG.Length;
            int[] countZero = new int[n];

            for (int i = 0; i < n; i++)
            {
                for (int j = periodPPG[i].Begin + 1; j < periodPPG[i].End - 1; j++) 
                {
                    if ((derivativePoints[j] == 0 && derivativePoints[j - 1] > 0 && derivativePoints[j + 1] < 0) || (derivativePoints[j] == 0 && derivativePoints[j - 1] < 0 && derivativePoints[j + 1] > 0)) countZero[i]++;
                }
            }
            

            return countZero;
        }

        //private string[,] fillingExcel(int[] countZeroInInterval, int countColumns, int countLines, int column)
        //{
        //    string[,] valueColumns = new string[countColumns, countLines];
        //    for (int i = 0; i < countLines; i++)
        //    {
        //        valueColumns[i, column] = Convert.ToString(countZeroInInterval[i]);
                    
        //    }
        //    return valueColumns;
        //}

        private int[] CountZeroInInterval(int[] countZero)
        {
            int countInterval = 6;
            int[] countZeroInInterval = new int[countInterval];

            for (int i = 0; i < countZero.Length; i++)
            {
                if (countZero[i] == 0) countZeroInInterval[0]++;
                else
                {
                    if (countZero[i] < 5) countZeroInInterval[1]++;
                    else
                    {
                        if (countZero[i] < 11 && countZero[i] > 4) countZeroInInterval[2]++;
                        else
                        {
                            if (countZero[i] < 16 && countZero[i] > 10) countZeroInInterval[3]++;
                            else
                            {
                                if (countZero[i] < 21 && countZero[i] > 15) countZeroInInterval[4]++;
                                else
                                {
                                    if (countZero[i] > 20) countZeroInInterval[5]++;
                                }
                            }
                        }
                    }
                }
            }

            return countZeroInInterval;
        }

        private void buttonFindCountOfZeros_Click(object sender, EventArgs e)
        {


            const int countColumns = 13;
            const int countLines = 6;
            string[] nameColumns = new string[countColumns] { "Интервалы | Параметры", "Коэф. прореж. 1", "Коэф. прореж. 2", "Коэф. прореж. 3", "Коэф. прореж. 4",
            "Коэф. прореж. 1, сглаж. скользящ.средн.", "Коэф. прореж. 1, сглаж. полиномами",
            "Коэф. прореж. 2, сглаж. скользящ.средн.", "Коэф. прореж. 2, сглаж. полиномами",
            "Коэф. прореж. 3, сглаж. скользящ.средн.", "Коэф. прореж. 3, сглаж. полиномами",
            "Коэф. прореж. 4, сглаж. скользящ.средн.", "Коэф. прореж. 4, сглаж. полиномами"};
            string[,] valueColumns = new string[countLines, countColumns];

            string nameFile;
            string namefile;

            int j = 0;

            valueColumns[0, 0] = "0";
            valueColumns[1, 0] = "1-4";
            valueColumns[2, 0] = "5-10";
            valueColumns[3, 0] = "11-15";
            valueColumns[4, 0] = "16-20";
            valueColumns[5, 0] = "21и больше";

            int[] duplicatePoints11;
            int[] duplicatePoints12;
            int[] duplicatePoints21;
            int[] duplicatePoints22;
            int[] duplicatePoints31;
            int[] duplicatePoints32;
            int[] duplicatePoints41;
            int[] duplicatePoints42;

            double[] derivativePointsCoefThin1;
            double[] derivativePointsCoefThin11;
            double[] derivativePointsCoefThin12;
            double[] derivativePointsCoefThin2;
            double[] derivativePointsCoefThin21;
            double[] derivativePointsCoefThin22;
            double[] derivativePointsCoefThin3;
            double[] derivativePointsCoefThin31;
            double[] derivativePointsCoefThin32;
            double[] derivativePointsCoefThin4;
            double[] derivativePointsCoefThin41;
            double[] derivativePointsCoefThin42;

            int[] countZero1;
            int[] countZero2;
            int[] countZero3;
            int[] countZero4;
            int[] countZero1InInterval;
            int[] countZero2InInterval;
            int[] countZero3InInterval;
            int[] countZero4InInterval;

            //производная 1 пор-ка точности
            nameFile = @"J:\Documents\8 семестр\Диплом\CountZero1.xlsx";
            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics1.xlsx";
            derivativePointsCoefThin1 = differentiation1orderAccuracy(duplicatePoints1, 1);
            derivativePointsCoefThin1 = SmoothingDerivative(derivativePointsCoefThin1);
            PeriodPPG[] periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints1, derivativePointsCoefThin1);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin1);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 1;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            duplicatePoints11 = MovingAverageMethod(0, duplicatePoints1.Length, 3, duplicatePoints1);
            derivativePointsCoefThin11 = differentiation1orderAccuracy(duplicatePoints11, 1);
            derivativePointsCoefThin11 = SmoothingDerivative(derivativePointsCoefThin11);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints11, derivativePointsCoefThin11);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin11);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 5;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            duplicatePoints12 = NonlinearSmoothing(0, duplicatePoints1.Length, duplicatePoints1);
            derivativePointsCoefThin12 = differentiation1orderAccuracy(duplicatePoints12, 1);
            derivativePointsCoefThin12 = SmoothingDerivative(derivativePointsCoefThin12);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints11, derivativePointsCoefThin12);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin12);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 6;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics2.xlsx";
            derivativePointsCoefThin2 = differentiation1orderAccuracy(duplicatePoints2, 2);
            derivativePointsCoefThin2 = SmoothingDerivative(derivativePointsCoefThin2);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints2, derivativePointsCoefThin2);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin2);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 2;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            duplicatePoints21 = MovingAverageMethod(0, duplicatePoints2.Length, 3, duplicatePoints2);
            derivativePointsCoefThin21 = differentiation1orderAccuracy(duplicatePoints21, 2);
            derivativePointsCoefThin21 = SmoothingDerivative(derivativePointsCoefThin21);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints21, derivativePointsCoefThin21);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin21);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 7;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            duplicatePoints22 = NonlinearSmoothing(0, duplicatePoints2.Length, duplicatePoints2);
            derivativePointsCoefThin22 = differentiation1orderAccuracy(duplicatePoints22, 2);
            derivativePointsCoefThin22 = SmoothingDerivative(derivativePointsCoefThin22);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints22, derivativePointsCoefThin22);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin22);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 8;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics3.xlsx";
            derivativePointsCoefThin3 = differentiation1orderAccuracy(duplicatePoints3, 3);
            derivativePointsCoefThin3 = SmoothingDerivative(derivativePointsCoefThin3);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints3, derivativePointsCoefThin3);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin3);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 3;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            duplicatePoints31 = MovingAverageMethod(0, duplicatePoints3.Length, 3, duplicatePoints3);
            derivativePointsCoefThin31 = differentiation1orderAccuracy(duplicatePoints31, 3);
            derivativePointsCoefThin31 = SmoothingDerivative(derivativePointsCoefThin31);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints31, derivativePointsCoefThin31);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin31);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 9;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            duplicatePoints32 = NonlinearSmoothing(0, duplicatePoints3.Length, duplicatePoints3);
            derivativePointsCoefThin32 = differentiation1orderAccuracy(duplicatePoints32, 3);
            derivativePointsCoefThin32 = SmoothingDerivative(derivativePointsCoefThin32);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints32, derivativePointsCoefThin32);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin32);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 10;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics4.xlsx";
            derivativePointsCoefThin4 = differentiation1orderAccuracy(duplicatePoints4, 4);
            derivativePointsCoefThin4 = SmoothingDerivative(derivativePointsCoefThin4);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints4, derivativePointsCoefThin4);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin4);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 4;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            duplicatePoints41 = MovingAverageMethod(0, duplicatePoints4.Length, 3, duplicatePoints4);
            derivativePointsCoefThin41 = differentiation1orderAccuracy(duplicatePoints41, 4);
            derivativePointsCoefThin41 = SmoothingDerivative(derivativePointsCoefThin41);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints41, derivativePointsCoefThin41);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin41);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 11;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            duplicatePoints42 = NonlinearSmoothing(0, duplicatePoints4.Length, duplicatePoints4);
            derivativePointsCoefThin42 = differentiation1orderAccuracy(duplicatePoints42, 4);
            derivativePointsCoefThin42 = SmoothingDerivative(derivativePointsCoefThin42);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints42, derivativePointsCoefThin42);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin42);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 12;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            string nameSheet = "Дифференцирование первого порядка точности";
            WriteExcel(nameColumns, valueColumns, countColumns, countLines, nameFile, nameSheet);


            //производная 2ого пор-ка точности
            nameFile = @"J:\Documents\8 семестр\Диплом\CountZero2.xlsx";
            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics1.xlsx";
            derivativePointsCoefThin1 = differentiation2orderAccuracy(duplicatePoints1, 1);
            derivativePointsCoefThin1 = SmoothingDerivative(derivativePointsCoefThin1);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints1, derivativePointsCoefThin1);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin1);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 1;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            duplicatePoints11 = MovingAverageMethod(0, duplicatePoints1.Length, 3, duplicatePoints1);
            derivativePointsCoefThin11 = differentiation2orderAccuracy(duplicatePoints11, 1);
            derivativePointsCoefThin11 = SmoothingDerivative(derivativePointsCoefThin11);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints11, derivativePointsCoefThin11);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin11);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 5;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            duplicatePoints12 = NonlinearSmoothing(0, duplicatePoints1.Length, duplicatePoints1);
            derivativePointsCoefThin12 = differentiation2orderAccuracy(duplicatePoints12, 1);
            derivativePointsCoefThin12 = SmoothingDerivative(derivativePointsCoefThin12);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints11, derivativePointsCoefThin12);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin12);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 6;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics2.xlsx";
            derivativePointsCoefThin2 = differentiation2orderAccuracy(duplicatePoints2, 2);
            derivativePointsCoefThin2 = SmoothingDerivative(derivativePointsCoefThin2);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints2, derivativePointsCoefThin2);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin2);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 2;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            duplicatePoints21 = MovingAverageMethod(0, duplicatePoints2.Length, 3, duplicatePoints2);
            derivativePointsCoefThin21 = differentiation2orderAccuracy(duplicatePoints21, 2);
            derivativePointsCoefThin21 = SmoothingDerivative(derivativePointsCoefThin21);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints21, derivativePointsCoefThin21);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin21);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 7;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            duplicatePoints22 = NonlinearSmoothing(0, duplicatePoints2.Length, duplicatePoints2);
            derivativePointsCoefThin22 = differentiation2orderAccuracy(duplicatePoints22, 2);
            derivativePointsCoefThin22 = SmoothingDerivative(derivativePointsCoefThin22);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints22, derivativePointsCoefThin22);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin22);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 8;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics3.xlsx";
            derivativePointsCoefThin3 = differentiation2orderAccuracy(duplicatePoints3, 3);
            derivativePointsCoefThin3 = SmoothingDerivative(derivativePointsCoefThin3);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints3, derivativePointsCoefThin3);            
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin3);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 3;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            duplicatePoints31 = MovingAverageMethod(0, duplicatePoints3.Length, 3, duplicatePoints3);
            derivativePointsCoefThin31 = differentiation2orderAccuracy(duplicatePoints31, 3);
            derivativePointsCoefThin31 = SmoothingDerivative(derivativePointsCoefThin31);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints31, derivativePointsCoefThin31);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin31);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 9;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            duplicatePoints32 = NonlinearSmoothing(0, duplicatePoints3.Length, duplicatePoints3);
            derivativePointsCoefThin32 = differentiation2orderAccuracy(duplicatePoints32, 3);
            derivativePointsCoefThin32 = SmoothingDerivative(derivativePointsCoefThin32);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints32, derivativePointsCoefThin32);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin32);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 10;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics4.xlsx";
            derivativePointsCoefThin4 = differentiation2orderAccuracy(duplicatePoints4, 4);
            derivativePointsCoefThin4 = SmoothingDerivative(derivativePointsCoefThin4);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints4, derivativePointsCoefThin4);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin4);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 4;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            duplicatePoints41 = MovingAverageMethod(0, duplicatePoints4.Length, 3, duplicatePoints4);
            derivativePointsCoefThin41 = differentiation2orderAccuracy(duplicatePoints41, 4);
            derivativePointsCoefThin41 = SmoothingDerivative(derivativePointsCoefThin41);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints41, derivativePointsCoefThin41);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin41);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 11;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            duplicatePoints42 = NonlinearSmoothing(0, duplicatePoints4.Length, duplicatePoints4);
            derivativePointsCoefThin42 = differentiation2orderAccuracy(duplicatePoints42, 4);
            derivativePointsCoefThin42 = SmoothingDerivative(derivativePointsCoefThin42);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints42, derivativePointsCoefThin42);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin42);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 12;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            nameSheet = "Дифференцирование второго порядка точности";
            WriteExcel(nameColumns, valueColumns, countColumns, countLines, nameFile, nameSheet);


            //производная по 4 узл точкам
            nameFile = @"J:\Documents\8 семестр\Диплом\CountZero3.xlsx";
            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics1.xlsx";
            derivativePointsCoefThin1 = differentiation4points(duplicatePoints1, 1);
            derivativePointsCoefThin1 = SmoothingDerivative(derivativePointsCoefThin1);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints1, derivativePointsCoefThin1);            
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin1);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 1;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            duplicatePoints11 = MovingAverageMethod(0, duplicatePoints1.Length, 3, duplicatePoints1);
            derivativePointsCoefThin11 = differentiation4points(duplicatePoints11, 1);
            derivativePointsCoefThin11 = SmoothingDerivative(derivativePointsCoefThin11);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints11, derivativePointsCoefThin11);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin11);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 5;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            duplicatePoints12 = NonlinearSmoothing(0, duplicatePoints1.Length, duplicatePoints1);
            derivativePointsCoefThin12 = differentiation4points(duplicatePoints12, 1);
            derivativePointsCoefThin12 = SmoothingDerivative(derivativePointsCoefThin12);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints11, derivativePointsCoefThin12);
            countZero1 = CountZero(periodPPG, derivativePointsCoefThin12);
            countZero1InInterval = CountZeroInInterval(countZero1);
            j = 6;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero1InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics2.xlsx";
            derivativePointsCoefThin2 = differentiation4points(duplicatePoints2, 2);
            derivativePointsCoefThin2 = SmoothingDerivative(derivativePointsCoefThin2);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints2, derivativePointsCoefThin2);            
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin2);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 2;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            duplicatePoints21 = MovingAverageMethod(0, duplicatePoints2.Length, 3, duplicatePoints2);
            derivativePointsCoefThin21 = differentiation4points(duplicatePoints21, 2);
            derivativePointsCoefThin21 = SmoothingDerivative(derivativePointsCoefThin21);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints21, derivativePointsCoefThin21);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin21);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 7;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            duplicatePoints22 = NonlinearSmoothing(0, duplicatePoints2.Length, duplicatePoints2);
            derivativePointsCoefThin22 = differentiation4points(duplicatePoints22, 2);
            derivativePointsCoefThin22 = SmoothingDerivative(derivativePointsCoefThin22);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints22, derivativePointsCoefThin22);
            countZero2 = CountZero(periodPPG, derivativePointsCoefThin22);
            countZero2InInterval = CountZeroInInterval(countZero2);
            j = 8;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero2InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics3.xlsx";
            derivativePointsCoefThin3 = differentiation4points(duplicatePoints3, 3);
            derivativePointsCoefThin3 = SmoothingDerivative(derivativePointsCoefThin3);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints3, derivativePointsCoefThin3);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin3);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 3;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            duplicatePoints31 = MovingAverageMethod(0, duplicatePoints3.Length, 3, duplicatePoints3);
            derivativePointsCoefThin31 = differentiation4points(duplicatePoints31, 3);
            derivativePointsCoefThin31 = SmoothingDerivative(derivativePointsCoefThin31);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints31, derivativePointsCoefThin31);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin31);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 9;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            duplicatePoints32 = NonlinearSmoothing(0, duplicatePoints3.Length, duplicatePoints3);
            derivativePointsCoefThin32 = differentiation4points(duplicatePoints32, 3);
            derivativePointsCoefThin32 = SmoothingDerivative(derivativePointsCoefThin32);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints32, derivativePointsCoefThin32);
            countZero3 = CountZero(periodPPG, derivativePointsCoefThin32);
            countZero3InInterval = CountZeroInInterval(countZero3);
            j = 10;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero3InInterval[i]);
            }

            namefile = @"J:\Documents\8 семестр\Диплом\Characteristics4.xlsx";
            derivativePointsCoefThin4 = differentiation4points(duplicatePoints4, 4);
            derivativePointsCoefThin4 = SmoothingDerivative(derivativePointsCoefThin4);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints4, derivativePointsCoefThin4);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin4);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 4;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            duplicatePoints41 = MovingAverageMethod(0, duplicatePoints4.Length, 3, duplicatePoints4);
            derivativePointsCoefThin41 = differentiation4points(duplicatePoints41, 4);
            derivativePointsCoefThin41 = SmoothingDerivative(derivativePointsCoefThin41);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints41, derivativePointsCoefThin41);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin41);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 11;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            duplicatePoints42 = NonlinearSmoothing(0, duplicatePoints4.Length, duplicatePoints4);
            derivativePointsCoefThin42 = differentiation4points(duplicatePoints42, 4);
            derivativePointsCoefThin42 = SmoothingDerivative(derivativePointsCoefThin42);
            periodPPG = FindPeriods(CountPeriods, namefile, duplicatePoints42, derivativePointsCoefThin42);
            countZero4 = CountZero(periodPPG, derivativePointsCoefThin42);
            countZero4InInterval = CountZeroInInterval(countZero4);
            j = 12;
            for (int i = 0; i < countLines; i++)
            {
                valueColumns[i, j] = Convert.ToString(countZero4InInterval[i]);
            }

            nameSheet = "Дифференцирование по четырем узловым точкам";
            WriteExcel(nameColumns, valueColumns, countColumns, countLines, nameFile, nameSheet);







           
        }

       

        private void WriteCharacteristics_Click_1(object sender, EventArgs e)
        {

            if (PeriodClick != CountPeriods + 1)
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
                string[,] valueColumns = new string[countColumns, CountPeriods];
                for (int i = 0; i < countColumns; i++)
                {
                    for (int j = 0; j < CountPeriods; j++)
                    {
                        switch (i)
                        {
                            case 0:
                                valueColumns[i, j] = Convert.ToString(j + 1);
                                break;
                            case 1:
                                valueColumns[i, j] = Convert.ToString(Periods[j].Begin);
                                break;
                            case 2:
                                valueColumns[i, j] = Convert.ToString(Periods[j].End);
                                break;
                            case 3:
                                valueColumns[i, j] = Convert.ToString(Periods[j].A);
                                break;
                            case 4:
                                valueColumns[i, j] = Convert.ToString(Periods[j].B);
                                break;
                            case 5:
                                valueColumns[i, j] = Convert.ToString(Periods[j].C);
                                break;
                            case 6:
                                valueColumns[i, j] = Convert.ToString(Periods[j].D);
                                break;
                            case 7:
                                valueColumns[i, j] = Convert.ToString(Periods[j].E);
                                break;
                        }
                    }
                }
                string nameFile = @"J:\Documents\8 семестр\Диплом\Characteristics.xlsx";//@"D:\ВУЗ\4 курс\Диплом\DATA\Characteristics.xlsx";
                string nameSheet = "Characteristics";
                WriteToExcel(nameСolumns, valueColumns, countColumns, CountPeriods, nameFile, nameSheet);
            }
        }

        private void comboBoxFindCharacteristic_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedStateFindCharacteristic = comboBoxFindCharacteristic.SelectedItem.ToString();
        }

        private void EndWatch_Click(object sender, EventArgs e)
        {
            IsEndWatch = false;
        }

        private void comboBoxSmoothingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSmoothingMethod = comboBoxSmoothingMethod.SelectedItem.ToString();
            if (selectedSmoothingMethod== "Сглаживание методом скользящего среднего")
            {
                labelValueSmoothingWindow.Visible = true;
                buttonIncreaseWindow.Visible = true;
                buttonDecreaseWindow.Visible = true;
                labelSmoothingWindow.Visible = true;
            }
            if (selectedSmoothingMethod == "Сглаживание полиномами 2 порядка по 7 точкам")
            {
                labelValueSmoothingWindow.Visible = false;
                buttonIncreaseWindow.Visible = false;
                buttonDecreaseWindow.Visible = false;
                labelSmoothingWindow.Visible = false;
            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            int widthIndentation = 10;
            int heightIndentation = 5;
            int widthChartPPG = this.Width - (chartPPG.Left + chartPPG.Width) + widthIndentation;
            int heightPPG = (int)Math.Round(this.Height * 0.4);
            int heightDPPG = (int)Math.Round(this.Height * 0.3);
            int heightDDPPG = (int)Math.Round(this.Height * 0.2);
            int widthCharts = (int)Math.Round(this.Width * 0.6);
            int widthGropBox = (int)Math.Round(this.Width * 0.36);
            int heightGropBox = (int)Math.Round(this.Width * 0.70);
            WidthChartChange(widthIndentation, widthChartPPG, widthCharts, widthGropBox);
            HeightChartChange(heightIndentation, heightPPG, heightDPPG, heightDDPPG, heightGropBox);

        }

        private void WidthChartChange(int widthIndentation, int widthChartPPG, int widthCharts, int widthGropBox)
        {
            if (chartPPG.Location.X != widthIndentation)
            {
                chartPPG.Left = widthIndentation;
                chartPPG.Width = widthCharts;
            }
            int widthChartDPPG = this.Width - (chartDPPG.Left + chartDPPG.Width) + widthIndentation;
            if (chartDPPG.Location.X != widthIndentation)
            {
                chartDPPG.Left = widthIndentation;
                chartDPPG.Width = widthCharts;
            }
            int widthChartDDPPG = this.Width - (chartDDPPG.Left + chartDDPPG.Width) + widthIndentation;
            if (chartDDPPG.Location.X != widthIndentation)
            {
                chartDDPPG.Left = widthIndentation;
                chartDDPPG.Width = widthCharts;
            }
            if (groupBox1.Location.X != chartPPG.Width+ 2*widthIndentation)
            {
                groupBox1.Left = chartPPG.Width + 2 * widthIndentation;
                groupBox1.Width = widthGropBox;
            }
            label1.Width = (int)Math.Round(groupBox1.Width * 0.2);
            textBoxPath.Width = (int)Math.Round(groupBox1.Width * 0.6);
            textBoxPath.Left = label1.Left + label1.Width + widthIndentation;

            buttonDrawPPG.Width = label5.Left - widthIndentation;
            buttonTagSmoothingPeriod.Width = (int)Math.Round(groupBox1.Width * 0.4);
            textBoxSmoothingPeriod.Width = (int)Math.Round(groupBox1.Width * 0.55);
            textBoxSmoothingPeriod.Left = buttonTagSmoothingPeriod.Left + buttonTagSmoothingPeriod.Width + widthIndentation;

            comboBoxSmoothingMethod.Width = (int)Math.Round(groupBox1.Width * 0.5);
            labelSmoothingWindow.Width = (int)Math.Round(groupBox1.Width * 0.2);
            labelSmoothingWindow.Left = comboBoxSmoothingMethod.Left + comboBoxSmoothingMethod.Width + widthIndentation;
            labelValueSmoothingWindow.Width = (int)Math.Round(groupBox1.Width * 0.1);
            labelValueSmoothingWindow.Left = labelSmoothingWindow.Left + labelSmoothingWindow.Width + widthIndentation;
            buttonIncreaseWindow.Width = (int)Math.Round(groupBox1.Width * 0.1);
            buttonDecreaseWindow.Width = (int)Math.Round(groupBox1.Width * 0.1);
            buttonIncreaseWindow.Left = labelValueSmoothingWindow.Left + labelValueSmoothingWindow.Width + widthIndentation;
            buttonDecreaseWindow.Left = labelValueSmoothingWindow.Left + labelValueSmoothingWindow.Width + widthIndentation;

            buttonSmoothingPeriod.Width = (int)Math.Round(groupBox1.Width * 0.3);
            buttonCancelSmoothingPeriod.Width = (int)Math.Round(groupBox1.Width * 0.3);
            buttonCancelSmoothingPeriod.Left = buttonSmoothingPeriod.Left + buttonSmoothingPeriod.Width + widthIndentation;
            buttonApplySmoothing.Width = (int)Math.Round(groupBox1.Width * 0.3);
            buttonApplySmoothing.Left = buttonCancelSmoothingPeriod.Left + buttonCancelSmoothingPeriod.Width + widthIndentation;
            buttonFindDerivate.Width = (int)Math.Round(groupBox1.Width * 0.4);
            comboBoxDifferentiationMethod.Width = (int)Math.Round(groupBox1.Width * 0.55);
            comboBoxDifferentiationMethod.Left = buttonFindDerivate.Left + buttonFindDerivate.Width + widthIndentation;
            buttonFindSecondDerivate.Width = (int)Math.Round(groupBox1.Width * 0.4);
            FullSearchCharacteristics.Width = (int)Math.Round(groupBox1.Width * 0.4);
            comboBoxFindCharacteristic.Width = (int)Math.Round(groupBox1.Width * 0.55);
            comboBoxFindCharacteristic.Left = buttonFindDerivate.Left + buttonFindDerivate.Width + widthIndentation;
            WriteCharacteristics.Width = (int)Math.Round(groupBox1.Width * 0.4);
            EndWatch.Width = (int)Math.Round(groupBox1.Width * 0.4);
            richTextBox1.Width = (int)Math.Round(groupBox1.Width * 0.9);

        }

        private void HeightChartChange( int heightIndentation, int heightPPG, int heightDPPG, int heightDDPPG, int heightGropBox)
        {
            if (chartDDPPG.Location.Y != chartDPPG.Top + heightDPPG + heightIndentation)
            {
                chartPPG.Top = heightIndentation;
                chartPPG.Height = heightPPG;
                chartDPPG.Top = chartPPG.Top + heightPPG + heightIndentation;
                chartDPPG.Height = heightDPPG;
                chartDDPPG.Top = chartDPPG.Top + heightDPPG + heightIndentation;
                chartDDPPG.Height = heightDDPPG;
            }
            groupBox1.Height = heightGropBox;
        }

        private void chartPPG_AxisViewChanged(object sender, ViewEventArgs e)
        {
          chartDPPG.ChartAreas[0].AxisX.ScaleView.Scroll(e.Axis.ScaleView.Position);
        }

        private void chartDPPG_AxisViewChanged(object sender, ViewEventArgs e)
        {
            chartPPG.ChartAreas[0].AxisX.ScaleView.Scroll(e.Axis.ScaleView.Position);
        }
    }
}
