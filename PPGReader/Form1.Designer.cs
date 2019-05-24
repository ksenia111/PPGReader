namespace PPGReader
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series13 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series14 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series15 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series16 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.buttonDrawPPG = new System.Windows.Forms.Button();
            this.chartPPG = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.textBoxW = new System.Windows.Forms.TextBox();
            this.buttonIncreaseScale = new System.Windows.Forms.Button();
            this.buttonDecreaseScale = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSinglingRate = new System.Windows.Forms.TextBox();
            this.labelSmoothingWindow = new System.Windows.Forms.Label();
            this.buttonIncreaseWindow = new System.Windows.Forms.Button();
            this.buttonDecreaseWindow = new System.Windows.Forms.Button();
            this.buttonSmoothingPeriod = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.FullSearchCharacteristics = new System.Windows.Forms.Button();
            this.buttonTagSmoothingPeriod = new System.Windows.Forms.Button();
            this.textBoxSmoothingPeriod = new System.Windows.Forms.TextBox();
            this.labelValueSmoothingWindow = new System.Windows.Forms.Label();
            this.buttonFindDerivate = new System.Windows.Forms.Button();
            this.buttonCancelSmoothingPeriod = new System.Windows.Forms.Button();
            this.buttonApplySmoothing = new System.Windows.Forms.Button();
            this.chartDPPG = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBoxDifferentiationMethod = new System.Windows.Forms.ComboBox();
            this.buttonFindCountOfZeros = new System.Windows.Forms.Button();
            this.EndWatch = new System.Windows.Forms.Button();
            this.WriteCharacteristics = new System.Windows.Forms.Button();
            this.buttonFindSecondDerivate = new System.Windows.Forms.Button();
            this.chartDDPPG = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBoxSmoothingMethod = new System.Windows.Forms.ComboBox();
            this.comboBoxFindCharacteristic = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartPPG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartDPPG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartDDPPG)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonDrawPPG
            // 
            this.buttonDrawPPG.Location = new System.Drawing.Point(1001, 103);
            this.buttonDrawPPG.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.buttonDrawPPG.Name = "buttonDrawPPG";
            this.buttonDrawPPG.Size = new System.Drawing.Size(200, 28);
            this.buttonDrawPPG.TabIndex = 0;
            this.buttonDrawPPG.Text = "Нарисовать график";
            this.buttonDrawPPG.UseVisualStyleBackColor = true;
            this.buttonDrawPPG.Click += new System.EventHandler(this.buttonDrawPPG_Click);
            // 
            // chartPPG
            // 
            chartArea2.Name = "ChartArea1";
            this.chartPPG.ChartAreas.Add(chartArea2);
            this.chartPPG.ContextMenuStrip = this.contextMenuStrip1;
            legend2.Name = "Legend1";
            this.chartPPG.Legends.Add(legend2);
            this.chartPPG.Location = new System.Drawing.Point(13, 6);
            this.chartPPG.Margin = new System.Windows.Forms.Padding(4);
            this.chartPPG.Name = "chartPPG";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "Series2";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Legend = "Legend1";
            series7.Name = "Series3";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Legend = "Legend1";
            series8.Name = "Series4";
            this.chartPPG.Series.Add(series5);
            this.chartPPG.Series.Add(series6);
            this.chartPPG.Series.Add(series7);
            this.chartPPG.Series.Add(series8);
            this.chartPPG.Size = new System.Drawing.Size(963, 391);
            this.chartPPG.TabIndex = 1;
            this.chartPPG.Text = "chartPPG";
            this.chartPPG.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chartPPG_MouseClick);
            this.chartPPG.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chartPPG_MouseDown);
            this.chartPPG.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chartPPG_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Enabled = false;
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(1001, 532);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(616, 150);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1003, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Файл";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1468, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "w";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(1063, 30);
            this.textBoxPath.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(371, 22);
            this.textBoxPath.TabIndex = 5;
            // 
            // textBoxW
            // 
            this.textBoxW.Location = new System.Drawing.Point(1501, 34);
            this.textBoxW.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBoxW.Name = "textBoxW";
            this.textBoxW.Size = new System.Drawing.Size(25, 22);
            this.textBoxW.TabIndex = 6;
            this.textBoxW.Text = "5";
            // 
            // buttonIncreaseScale
            // 
            this.buttonIncreaseScale.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonIncreaseScale.Image = ((System.Drawing.Image)(resources.GetObject("buttonIncreaseScale.Image")));
            this.buttonIncreaseScale.Location = new System.Drawing.Point(1311, 90);
            this.buttonIncreaseScale.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.buttonIncreaseScale.Name = "buttonIncreaseScale";
            this.buttonIncreaseScale.Size = new System.Drawing.Size(57, 22);
            this.buttonIncreaseScale.TabIndex = 7;
            this.buttonIncreaseScale.UseVisualStyleBackColor = false;
            this.buttonIncreaseScale.Click += new System.EventHandler(this.buttonIncreaseScale_Click);
            // 
            // buttonDecreaseScale
            // 
            this.buttonDecreaseScale.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonDecreaseScale.Image = ((System.Drawing.Image)(resources.GetObject("buttonDecreaseScale.Image")));
            this.buttonDecreaseScale.Location = new System.Drawing.Point(1311, 117);
            this.buttonDecreaseScale.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.buttonDecreaseScale.Name = "buttonDecreaseScale";
            this.buttonDecreaseScale.Size = new System.Drawing.Size(57, 22);
            this.buttonDecreaseScale.TabIndex = 8;
            this.buttonDecreaseScale.UseVisualStyleBackColor = false;
            this.buttonDecreaseScale.Click += new System.EventHandler(this.buttonDecreaseScale_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1003, 70);
            this.label3.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Частота прореживания";
            // 
            // textBoxSinglingRate
            // 
            this.textBoxSinglingRate.Location = new System.Drawing.Point(1184, 66);
            this.textBoxSinglingRate.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.textBoxSinglingRate.Name = "textBoxSinglingRate";
            this.textBoxSinglingRate.Size = new System.Drawing.Size(23, 22);
            this.textBoxSinglingRate.TabIndex = 10;
            this.textBoxSinglingRate.Text = "5";
            // 
            // labelSmoothingWindow
            // 
            this.labelSmoothingWindow.AutoSize = true;
            this.labelSmoothingWindow.Location = new System.Drawing.Point(1391, 196);
            this.labelSmoothingWindow.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelSmoothingWindow.Name = "labelSmoothingWindow";
            this.labelSmoothingWindow.Size = new System.Drawing.Size(130, 17);
            this.labelSmoothingWindow.TabIndex = 11;
            this.labelSmoothingWindow.Text = "Окно сглаживания";
            // 
            // buttonIncreaseWindow
            // 
            this.buttonIncreaseWindow.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonIncreaseWindow.Image = ((System.Drawing.Image)(resources.GetObject("buttonIncreaseWindow.Image")));
            this.buttonIncreaseWindow.Location = new System.Drawing.Point(1579, 180);
            this.buttonIncreaseWindow.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.buttonIncreaseWindow.Name = "buttonIncreaseWindow";
            this.buttonIncreaseWindow.Size = new System.Drawing.Size(57, 22);
            this.buttonIncreaseWindow.TabIndex = 13;
            this.buttonIncreaseWindow.UseVisualStyleBackColor = false;
            this.buttonIncreaseWindow.Click += new System.EventHandler(this.buttonIncreaseWindow_Click_1);
            // 
            // buttonDecreaseWindow
            // 
            this.buttonDecreaseWindow.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonDecreaseWindow.Image = ((System.Drawing.Image)(resources.GetObject("buttonDecreaseWindow.Image")));
            this.buttonDecreaseWindow.Location = new System.Drawing.Point(1579, 206);
            this.buttonDecreaseWindow.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.buttonDecreaseWindow.Name = "buttonDecreaseWindow";
            this.buttonDecreaseWindow.Size = new System.Drawing.Size(57, 22);
            this.buttonDecreaseWindow.TabIndex = 14;
            this.buttonDecreaseWindow.UseVisualStyleBackColor = false;
            this.buttonDecreaseWindow.Click += new System.EventHandler(this.buttonDecreaseWindow_Click_1);
            // 
            // buttonSmoothingPeriod
            // 
            this.buttonSmoothingPeriod.Location = new System.Drawing.Point(1001, 223);
            this.buttonSmoothingPeriod.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.buttonSmoothingPeriod.Name = "buttonSmoothingPeriod";
            this.buttonSmoothingPeriod.Size = new System.Drawing.Size(169, 59);
            this.buttonSmoothingPeriod.TabIndex = 15;
            this.buttonSmoothingPeriod.Text = "Сгладить";
            this.buttonSmoothingPeriod.UseVisualStyleBackColor = true;
            this.buttonSmoothingPeriod.Click += new System.EventHandler(this.buttonSmoothingPeriod_Click_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1217, 110);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 16;
            this.label5.Text = "Масштаб";
            // 
            // FullSearchCharacteristics
            // 
            this.FullSearchCharacteristics.Location = new System.Drawing.Point(999, 405);
            this.FullSearchCharacteristics.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FullSearchCharacteristics.Name = "FullSearchCharacteristics";
            this.FullSearchCharacteristics.Size = new System.Drawing.Size(267, 23);
            this.FullSearchCharacteristics.TabIndex = 17;
            this.FullSearchCharacteristics.Text = "Найти характеристики";
            this.FullSearchCharacteristics.UseVisualStyleBackColor = true;
            this.FullSearchCharacteristics.Click += new System.EventHandler(this.FullSearchCharacteristics_Click);
            // 
            // buttonTagSmoothingPeriod
            // 
            this.buttonTagSmoothingPeriod.Location = new System.Drawing.Point(1001, 142);
            this.buttonTagSmoothingPeriod.Margin = new System.Windows.Forms.Padding(5);
            this.buttonTagSmoothingPeriod.Name = "buttonTagSmoothingPeriod";
            this.buttonTagSmoothingPeriod.Size = new System.Drawing.Size(287, 30);
            this.buttonTagSmoothingPeriod.TabIndex = 18;
            this.buttonTagSmoothingPeriod.Text = "Отметить период сглаживания";
            this.buttonTagSmoothingPeriod.UseVisualStyleBackColor = true;
            this.buttonTagSmoothingPeriod.Click += new System.EventHandler(this.buttonTagSmoothingPeriod_Click);
            // 
            // textBoxSmoothingPeriod
            // 
            this.textBoxSmoothingPeriod.Location = new System.Drawing.Point(1311, 146);
            this.textBoxSmoothingPeriod.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxSmoothingPeriod.Name = "textBoxSmoothingPeriod";
            this.textBoxSmoothingPeriod.Size = new System.Drawing.Size(256, 22);
            this.textBoxSmoothingPeriod.TabIndex = 19;
            // 
            // labelValueSmoothingWindow
            // 
            this.labelValueSmoothingWindow.AutoSize = true;
            this.labelValueSmoothingWindow.Location = new System.Drawing.Point(1551, 196);
            this.labelValueSmoothingWindow.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelValueSmoothingWindow.Name = "labelValueSmoothingWindow";
            this.labelValueSmoothingWindow.Size = new System.Drawing.Size(16, 17);
            this.labelValueSmoothingWindow.TabIndex = 20;
            this.labelValueSmoothingWindow.Text = "3";
            // 
            // buttonFindDerivate
            // 
            this.buttonFindDerivate.Location = new System.Drawing.Point(999, 295);
            this.buttonFindDerivate.Margin = new System.Windows.Forms.Padding(5);
            this.buttonFindDerivate.Name = "buttonFindDerivate";
            this.buttonFindDerivate.Size = new System.Drawing.Size(267, 31);
            this.buttonFindDerivate.TabIndex = 21;
            this.buttonFindDerivate.Text = "Найти производную";
            this.buttonFindDerivate.UseVisualStyleBackColor = true;
            this.buttonFindDerivate.Click += new System.EventHandler(this.buttonFindDerivate_Click);
            // 
            // buttonCancelSmoothingPeriod
            // 
            this.buttonCancelSmoothingPeriod.Location = new System.Drawing.Point(1180, 223);
            this.buttonCancelSmoothingPeriod.Margin = new System.Windows.Forms.Padding(5);
            this.buttonCancelSmoothingPeriod.Name = "buttonCancelSmoothingPeriod";
            this.buttonCancelSmoothingPeriod.Size = new System.Drawing.Size(162, 59);
            this.buttonCancelSmoothingPeriod.TabIndex = 22;
            this.buttonCancelSmoothingPeriod.Text = "Отменить сглаживание";
            this.buttonCancelSmoothingPeriod.UseVisualStyleBackColor = true;
            this.buttonCancelSmoothingPeriod.Click += new System.EventHandler(this.buttonCancelSmoothingPeriod_Click);
            // 
            // buttonApplySmoothing
            // 
            this.buttonApplySmoothing.Location = new System.Drawing.Point(1352, 223);
            this.buttonApplySmoothing.Margin = new System.Windows.Forms.Padding(5);
            this.buttonApplySmoothing.Name = "buttonApplySmoothing";
            this.buttonApplySmoothing.Size = new System.Drawing.Size(164, 59);
            this.buttonApplySmoothing.TabIndex = 23;
            this.buttonApplySmoothing.Text = "Применить сглаживание";
            this.buttonApplySmoothing.UseVisualStyleBackColor = true;
            this.buttonApplySmoothing.Click += new System.EventHandler(this.buttonApplySmoothing_Click);
            // 
            // chartDPPG
            // 
            chartArea3.Name = "ChartArea1";
            this.chartDPPG.ChartAreas.Add(chartArea3);
            this.chartDPPG.ContextMenuStrip = this.contextMenuStrip1;
            legend3.Name = "Legend1";
            this.chartDPPG.Legends.Add(legend3);
            this.chartDPPG.Location = new System.Drawing.Point(13, 405);
            this.chartDPPG.Margin = new System.Windows.Forms.Padding(4);
            this.chartDPPG.Name = "chartDPPG";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series9.Legend = "Legend1";
            series9.Name = "Series1";
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series10.Legend = "Legend1";
            series10.Name = "Series2";
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series11.Legend = "Legend1";
            series11.Name = "Series3";
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series12.Legend = "Legend1";
            series12.Name = "Series4";
            this.chartDPPG.Series.Add(series9);
            this.chartDPPG.Series.Add(series10);
            this.chartDPPG.Series.Add(series11);
            this.chartDPPG.Series.Add(series12);
            this.chartDPPG.Size = new System.Drawing.Size(963, 278);
            this.chartDPPG.TabIndex = 24;
            this.chartDPPG.Text = "chart2";
            // 
            // comboBoxDifferentiationMethod
            // 
            this.comboBoxDifferentiationMethod.BackColor = System.Drawing.SystemColors.HighlightText;
            this.comboBoxDifferentiationMethod.FormattingEnabled = true;
            this.comboBoxDifferentiationMethod.Items.AddRange(new object[] {
            "Дифференцирование первого порядка точности",
            "Дифференцирование второго порядка точности",
            "Дифференцирование по 4 узловым точкам"});
            this.comboBoxDifferentiationMethod.Location = new System.Drawing.Point(1275, 302);
            this.comboBoxDifferentiationMethod.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.comboBoxDifferentiationMethod.Name = "comboBoxDifferentiationMethod";
            this.comboBoxDifferentiationMethod.Size = new System.Drawing.Size(360, 24);
            this.comboBoxDifferentiationMethod.TabIndex = 25;
            this.comboBoxDifferentiationMethod.Text = "Дифференцирование первого порядка точности";
            this.comboBoxDifferentiationMethod.SelectedIndexChanged += new System.EventHandler(this.comboBoxDifferentiationMethod_SelectedIndexChanged);
            // 
            // buttonFindCountOfZeros
            // 
            this.buttonFindCountOfZeros.Location = new System.Drawing.Point(1278, 330);
            this.buttonFindCountOfZeros.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonFindCountOfZeros.Name = "buttonFindCountOfZeros";
            this.buttonFindCountOfZeros.Size = new System.Drawing.Size(175, 63);
            this.buttonFindCountOfZeros.TabIndex = 26;
            this.buttonFindCountOfZeros.Text = "Посчитать количество нулей на периодах";
            this.buttonFindCountOfZeros.UseVisualStyleBackColor = true;
            this.buttonFindCountOfZeros.Click += new System.EventHandler(this.buttonFindCountOfZeros_Click);
            // 
            // EndWatch
            // 
            this.EndWatch.Location = new System.Drawing.Point(1001, 471);
            this.EndWatch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EndWatch.Name = "EndWatch";
            this.EndWatch.Size = new System.Drawing.Size(267, 30);
            this.EndWatch.TabIndex = 29;
            this.EndWatch.Text = "Просмотр периодов закончен";
            this.EndWatch.UseVisualStyleBackColor = true;
            this.EndWatch.Click += new System.EventHandler(this.EndWatch_Click);
            // 
            // WriteCharacteristics
            // 
            this.WriteCharacteristics.Location = new System.Drawing.Point(999, 438);
            this.WriteCharacteristics.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.WriteCharacteristics.Name = "WriteCharacteristics";
            this.WriteCharacteristics.Size = new System.Drawing.Size(267, 28);
            this.WriteCharacteristics.TabIndex = 27;
            this.WriteCharacteristics.Text = "Записать характеристики";
            this.WriteCharacteristics.UseVisualStyleBackColor = true;
            this.WriteCharacteristics.Click += new System.EventHandler(this.WriteCharacteristics_Click_1);
            // 
            // buttonFindSecondDerivate
            // 
            this.buttonFindSecondDerivate.Location = new System.Drawing.Point(1001, 342);
            this.buttonFindSecondDerivate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonFindSecondDerivate.Name = "buttonFindSecondDerivate";
            this.buttonFindSecondDerivate.Size = new System.Drawing.Size(267, 31);
            this.buttonFindSecondDerivate.TabIndex = 28;
            this.buttonFindSecondDerivate.Text = "Найти вторую производную";
            this.buttonFindSecondDerivate.UseVisualStyleBackColor = true;
            this.buttonFindSecondDerivate.Click += new System.EventHandler(this.buttonFindSecondDerivate_Click);
            // 
            // chartDDPPG
            // 
            chartArea4.Name = "ChartArea1";
            this.chartDDPPG.ChartAreas.Add(chartArea4);
            this.chartDDPPG.ContextMenuStrip = this.contextMenuStrip1;
            legend4.Name = "Legend1";
            this.chartDDPPG.Legends.Add(legend4);
            this.chartDDPPG.Location = new System.Drawing.Point(13, 690);
            this.chartDDPPG.Margin = new System.Windows.Forms.Padding(4);
            this.chartDDPPG.Name = "chartDDPPG";
            series13.ChartArea = "ChartArea1";
            series13.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series13.Legend = "Legend1";
            series13.Name = "Series1";
            series14.ChartArea = "ChartArea1";
            series14.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series14.Legend = "Legend1";
            series14.Name = "Series2";
            series15.ChartArea = "ChartArea1";
            series15.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series15.Legend = "Legend1";
            series15.Name = "Series3";
            series16.ChartArea = "ChartArea1";
            series16.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series16.Legend = "Legend1";
            series16.Name = "Series4";
            this.chartDDPPG.Series.Add(series13);
            this.chartDDPPG.Series.Add(series14);
            this.chartDDPPG.Series.Add(series15);
            this.chartDDPPG.Series.Add(series16);
            this.chartDDPPG.Size = new System.Drawing.Size(963, 226);
            this.chartDDPPG.TabIndex = 29;
            this.chartDDPPG.Text = "chart3";
            // 
            // comboBoxSmoothingMethod
            // 
            this.comboBoxSmoothingMethod.FormattingEnabled = true;
            this.comboBoxSmoothingMethod.Items.AddRange(new object[] {
            "Сглаживание методом скользящего среднего",
            "Сглаживание полиномами 2 порядка по 7 точкам"});
            this.comboBoxSmoothingMethod.Location = new System.Drawing.Point(1001, 192);
            this.comboBoxSmoothingMethod.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxSmoothingMethod.Name = "comboBoxSmoothingMethod";
            this.comboBoxSmoothingMethod.Size = new System.Drawing.Size(356, 24);
            this.comboBoxSmoothingMethod.TabIndex = 30;
            this.comboBoxSmoothingMethod.Text = "Сглаживание методом скользящего среднего";
            this.comboBoxSmoothingMethod.SelectedIndexChanged += new System.EventHandler(this.comboBoxSmoothingMethod_SelectedIndexChanged);
            // 
            // comboBoxFindCharacteristic
            // 
            this.comboBoxFindCharacteristic.BackColor = System.Drawing.SystemColors.HighlightText;
            this.comboBoxFindCharacteristic.FormattingEnabled = true;
            this.comboBoxFindCharacteristic.Items.AddRange(new object[] {
            "Полный перебор значений",
            "Градиентный метод",
            "Метод Ньютона"});
            this.comboBoxFindCharacteristic.Location = new System.Drawing.Point(1278, 405);
            this.comboBoxFindCharacteristic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxFindCharacteristic.Name = "comboBoxFindCharacteristic";
            this.comboBoxFindCharacteristic.Size = new System.Drawing.Size(289, 24);
            this.comboBoxFindCharacteristic.TabIndex = 28;
            this.comboBoxFindCharacteristic.Text = "Полный перебор значений";
            this.comboBoxFindCharacteristic.SelectedIndexChanged += new System.EventHandler(this.comboBoxFindCharacteristic_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1685, 750);
            this.Controls.Add(this.EndWatch);
            this.Controls.Add(this.comboBoxFindCharacteristic);
            this.Controls.Add(this.comboBoxSmoothingMethod);
            this.Controls.Add(this.chartDDPPG);
            this.Controls.Add(this.buttonFindSecondDerivate);
            this.Controls.Add(this.WriteCharacteristics);
            this.Controls.Add(this.buttonFindCountOfZeros);
            this.Controls.Add(this.comboBoxDifferentiationMethod);
            this.Controls.Add(this.chartDPPG);
            this.Controls.Add(this.buttonApplySmoothing);
            this.Controls.Add(this.buttonCancelSmoothingPeriod);
            this.Controls.Add(this.buttonFindDerivate);
            this.Controls.Add(this.labelValueSmoothingWindow);
            this.Controls.Add(this.textBoxSmoothingPeriod);
            this.Controls.Add(this.buttonTagSmoothingPeriod);
            this.Controls.Add(this.FullSearchCharacteristics);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonSmoothingPeriod);
            this.Controls.Add(this.buttonDecreaseWindow);
            this.Controls.Add(this.buttonIncreaseWindow);
            this.Controls.Add(this.labelSmoothingWindow);
            this.Controls.Add(this.textBoxSinglingRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonDecreaseScale);
            this.Controls.Add(this.buttonIncreaseScale);
            this.Controls.Add(this.textBoxW);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.chartPPG);
            this.Controls.Add(this.buttonDrawPPG);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Form1";
            this.Text = "PPGReader";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.chartPPG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartDPPG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartDDPPG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDrawPPG;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPPG;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.TextBox textBoxW;
        private System.Windows.Forms.Button buttonIncreaseScale;
        private System.Windows.Forms.Button buttonDecreaseScale;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSinglingRate;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label labelSmoothingWindow;
        private System.Windows.Forms.Button buttonIncreaseWindow;
        private System.Windows.Forms.Button buttonDecreaseWindow;
        private System.Windows.Forms.Button buttonSmoothingPeriod;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button FullSearchCharacteristics;
        private System.Windows.Forms.Button buttonTagSmoothingPeriod;
        private System.Windows.Forms.TextBox textBoxSmoothingPeriod;
        private System.Windows.Forms.Label labelValueSmoothingWindow;
        private System.Windows.Forms.Button buttonFindDerivate;
        private System.Windows.Forms.Button buttonCancelSmoothingPeriod;
        private System.Windows.Forms.Button buttonApplySmoothing;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartDPPG;
        private System.Windows.Forms.ComboBox comboBoxDifferentiationMethod;
        private System.Windows.Forms.Button buttonFindCountOfZeros;
        private System.Windows.Forms.Button EndWatch;
        private System.Windows.Forms.Button WriteCharacteristics;
        private System.Windows.Forms.ComboBox comboBoxSmoothingMethod;
        private System.Windows.Forms.Button buttonFindSecondDerivate;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartDDPPG;
        private System.Windows.Forms.ComboBox comboBoxFindCharacteristic;
    }
}

