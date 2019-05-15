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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonCalc = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.textBoxW = new System.Windows.Forms.TextBox();
            this.buttonIncrease = new System.Windows.Forms.Button();
            this.buttonDecrease = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSinglingRate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.FullSearchCharacteristics = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.WriteCharacteristics = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCalc
            // 
            this.buttonCalc.Location = new System.Drawing.Point(646, 344);
            this.buttonCalc.Name = "buttonCalc";
            this.buttonCalc.Size = new System.Drawing.Size(150, 23);
            this.buttonCalc.TabIndex = 0;
            this.buttonCalc.Text = "Нарисовать график";
            this.buttonCalc.UseVisualStyleBackColor = true;
            this.buttonCalc.Click += new System.EventHandler(this.button1_Click);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.ContextMenuStrip = this.contextMenuStrip1;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 21);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "Series2";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Series3";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "Series4";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            this.chart1.Size = new System.Drawing.Size(702, 318);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            this.chart1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseClick);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseMove);
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
            this.richTextBox1.Location = new System.Drawing.Point(751, 22);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(228, 318);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 369);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Файл";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(338, 368);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "w";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(53, 366);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(279, 20);
            this.textBoxPath.TabIndex = 5;
            // 
            // textBoxW
            // 
            this.textBoxW.Location = new System.Drawing.Point(356, 366);
            this.textBoxW.Name = "textBoxW";
            this.textBoxW.Size = new System.Drawing.Size(20, 20);
            this.textBoxW.TabIndex = 6;
            this.textBoxW.Text = "5";
            // 
            // buttonIncrease
            // 
            this.buttonIncrease.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonIncrease.Image = ((System.Drawing.Image)(resources.GetObject("buttonIncrease.Image")));
            this.buttonIncrease.Location = new System.Drawing.Point(608, 357);
            this.buttonIncrease.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonIncrease.Name = "buttonIncrease";
            this.buttonIncrease.Size = new System.Drawing.Size(32, 15);
            this.buttonIncrease.TabIndex = 7;
            this.buttonIncrease.UseVisualStyleBackColor = false;
            this.buttonIncrease.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonDecrease
            // 
            this.buttonDecrease.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonDecrease.Image = ((System.Drawing.Image)(resources.GetObject("buttonDecrease.Image")));
            this.buttonDecrease.Location = new System.Drawing.Point(608, 375);
            this.buttonDecrease.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonDecrease.Name = "buttonDecrease";
            this.buttonDecrease.Size = new System.Drawing.Size(32, 15);
            this.buttonDecrease.TabIndex = 8;
            this.buttonDecrease.UseVisualStyleBackColor = false;
            this.buttonDecrease.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(381, 369);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Частота прореживания";
            // 
            // textBoxSinglingRate
            // 
            this.textBoxSinglingRate.Location = new System.Drawing.Point(512, 366);
            this.textBoxSinglingRate.Name = "textBoxSinglingRate";
            this.textBoxSinglingRate.Size = new System.Drawing.Size(26, 20);
            this.textBoxSinglingRate.TabIndex = 10;
            this.textBoxSinglingRate.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(490, 405);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Окно сглаживания";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(608, 393);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(32, 15);
            this.button1.TabIndex = 13;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(608, 413);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(32, 15);
            this.button2.TabIndex = 14;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(647, 405);
            this.button3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 20);
            this.button3.TabIndex = 15;
            this.button3.Text = "Сгладить период";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(552, 368);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Масштаб";
            // 
            // FullSearchCharacteristics
            // 
            this.FullSearchCharacteristics.Location = new System.Drawing.Point(646, 373);
            this.FullSearchCharacteristics.Name = "FullSearchCharacteristics";
            this.FullSearchCharacteristics.Size = new System.Drawing.Size(335, 23);
            this.FullSearchCharacteristics.TabIndex = 17;
            this.FullSearchCharacteristics.Text = "Найти характеристики с помощью полного перебора";
            this.FullSearchCharacteristics.UseVisualStyleBackColor = true;
            this.FullSearchCharacteristics.Click += new System.EventHandler(this.FullSearchCharacteristics_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(186, 395);
            this.button4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(146, 38);
            this.button4.TabIndex = 18;
            this.button4.Text = "Отметить период сглаживания";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(340, 405);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(146, 20);
            this.textBox2.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(591, 405);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "3";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(646, 428);
            this.button5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(150, 20);
            this.button5.TabIndex = 21;
            this.button5.Text = "Найти производную";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(801, 405);
            this.button6.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(178, 20);
            this.button6.TabIndex = 22;
            this.button6.Text = "Отменить сглаживание периода";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(983, 405);
            this.button7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(76, 20);
            this.button7.TabIndex = 23;
            this.button7.Text = "Применить";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // WriteCharacteristics
            // 
            this.WriteCharacteristics.Location = new System.Drawing.Point(803, 346);
            this.WriteCharacteristics.Name = "WriteCharacteristics";
            this.WriteCharacteristics.Size = new System.Drawing.Size(176, 23);
            this.WriteCharacteristics.TabIndex = 24;
            this.WriteCharacteristics.Text = "Записать характеристики";
            this.WriteCharacteristics.UseVisualStyleBackColor = true;
            this.WriteCharacteristics.Click += new System.EventHandler(this.WriteCharacteristics_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 459);
            this.Controls.Add(this.WriteCharacteristics);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.FullSearchCharacteristics);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxSinglingRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonDecrease);
            this.Controls.Add(this.buttonIncrease);
            this.Controls.Add(this.textBoxW);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.buttonCalc);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCalc;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.TextBox textBoxW;
        private System.Windows.Forms.Button buttonIncrease;
        private System.Windows.Forms.Button buttonDecrease;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSinglingRate;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button FullSearchCharacteristics;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button WriteCharacteristics;
    }
}

