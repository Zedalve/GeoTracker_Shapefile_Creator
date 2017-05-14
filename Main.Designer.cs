namespace GeotrackerShapefileMaker
{
    partial class Main
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
            this.btn_Run = new System.Windows.Forms.Button();
            this.cBox_Basins = new System.Windows.Forms.ComboBox();
            this.cBox_Contams = new System.Windows.Forms.ComboBox();
            this.cBox_TimeFrame = new System.Windows.Forms.ComboBox();
            this.cBox_Selection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Bground_Main = new System.ComponentModel.BackgroundWorker();
            this.Bground_End = new System.ComponentModel.BackgroundWorker();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.lbl_selection = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Run
            // 
            this.btn_Run.Location = new System.Drawing.Point(485, 226);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(102, 23);
            this.btn_Run.TabIndex = 0;
            this.btn_Run.Text = "Get Shapefile";
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // cBox_Basins
            // 
            this.cBox_Basins.FormattingEnabled = true;
            this.cBox_Basins.Location = new System.Drawing.Point(12, 46);
            this.cBox_Basins.Name = "cBox_Basins";
            this.cBox_Basins.Size = new System.Drawing.Size(318, 21);
            this.cBox_Basins.TabIndex = 1;
            // 
            // cBox_Contams
            // 
            this.cBox_Contams.FormattingEnabled = true;
            this.cBox_Contams.Location = new System.Drawing.Point(12, 87);
            this.cBox_Contams.Name = "cBox_Contams";
            this.cBox_Contams.Size = new System.Drawing.Size(318, 21);
            this.cBox_Contams.TabIndex = 2;
            this.cBox_Contams.SelectedIndexChanged += new System.EventHandler(this.cBox_Contams_SelectedIndexChanged);
            // 
            // cBox_TimeFrame
            // 
            this.cBox_TimeFrame.FormattingEnabled = true;
            this.cBox_TimeFrame.Location = new System.Drawing.Point(12, 126);
            this.cBox_TimeFrame.Name = "cBox_TimeFrame";
            this.cBox_TimeFrame.Size = new System.Drawing.Size(90, 21);
            this.cBox_TimeFrame.TabIndex = 3;
            // 
            // cBox_Selection
            // 
            this.cBox_Selection.FormattingEnabled = true;
            this.cBox_Selection.Location = new System.Drawing.Point(12, 164);
            this.cBox_Selection.Name = "cBox_Selection";
            this.cBox_Selection.Size = new System.Drawing.Size(90, 21);
            this.cBox_Selection.TabIndex = 4;
            this.cBox_Selection.SelectedIndexChanged += new System.EventHandler(this.cBox_Selection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Basin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Contaminant";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Time Frame";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Selection";
            // 
            // Bground_Main
            // 
            this.Bground_Main.WorkerReportsProgress = true;
            this.Bground_Main.WorkerSupportsCancellation = true;
            this.Bground_Main.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Bground_Main_DoWork);
            this.Bground_Main.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Bground_Main_RunWorkerCompleted);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(105, 210);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(304, 39);
            this.lbl_Status.TabIndex = 9;
            this.lbl_Status.Text = "Select Parameters";
            // 
            // lbl_selection
            // 
            this.lbl_selection.AutoSize = true;
            this.lbl_selection.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_selection.Location = new System.Drawing.Point(108, 165);
            this.lbl_selection.Name = "lbl_selection";
            this.lbl_selection.Size = new System.Drawing.Size(31, 20);
            this.lbl_selection.TabIndex = 10;
            this.lbl_selection.Text = "≥ 0";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 261);
            this.Controls.Add(this.lbl_selection);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cBox_Selection);
            this.Controls.Add(this.cBox_TimeFrame);
            this.Controls.Add(this.cBox_Contams);
            this.Controls.Add(this.cBox_Basins);
            this.Controls.Add(this.btn_Run);
            this.Name = "Main";
            this.Text = "GeoTracker Shapefile Maker";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.ComboBox cBox_Basins;
        private System.Windows.Forms.ComboBox cBox_Contams;
        private System.Windows.Forms.ComboBox cBox_TimeFrame;
        private System.Windows.Forms.ComboBox cBox_Selection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker Bground_Main;
        private System.ComponentModel.BackgroundWorker Bground_End;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Label lbl_selection;
    }
}

