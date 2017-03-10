namespace NS_SchaltungWK
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
      this.TimerConnection = new System.Windows.Forms.Timer(this.components);
      this.label1 = new System.Windows.Forms.Label();
      this.TimerDimmen = new System.Windows.Forms.Timer(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // timer1
      // 
      this.timer1.Enabled = true;
      this.timer1.Interval = 500;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::SchaltungWK.Properties.Resources.ElektroLight;
      this.pictureBox1.Location = new System.Drawing.Point(109, 367);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(90, 38);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // progressBar
      // 
      this.progressBar.Location = new System.Drawing.Point(109, 402);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(90, 23);
      this.progressBar.Step = 1;
      this.progressBar.TabIndex = 1;
      this.progressBar.Visible = false;
      // 
      // backgroundWorker
      // 
      this.backgroundWorker.WorkerReportsProgress = true;
      this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
      this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
      this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
      // 
      // TimerConnection
      // 
      this.TimerConnection.Interval = 1000;
      this.TimerConnection.Tick += new System.EventHandler(this.TimerConnection_Tick);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(68, 402);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "label1";
      this.label1.Visible = false;
      // 
      // TimerDimmen
      // 
      this.TimerDimmen.Interval = 15000;
      this.TimerDimmen.Tick += new System.EventHandler(this.TimerDimmen_Tick);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(211, 425);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.pictureBox1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "Form1";
      this.Text = "Schaltung WK";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.ComponentModel.BackgroundWorker backgroundWorker;
    private System.Windows.Forms.Timer TimerConnection;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Timer TimerDimmen;
  }
}

