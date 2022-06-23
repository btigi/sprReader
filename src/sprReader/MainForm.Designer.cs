namespace sprReader
{
    partial class MainForm
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
            this.rtOutput = new System.Windows.Forms.RichTextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnOpen = new System.Windows.Forms.Button();
            this.tbPalette = new System.Windows.Forms.TextBox();
            this.lblPalette = new System.Windows.Forms.Label();
            this.btnBulk = new System.Windows.Forms.Button();
            this.lblFolder = new System.Windows.Forms.Label();
            this.tbFolder = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // rtOutput
            // 
            this.rtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtOutput.Location = new System.Drawing.Point(12, 159);
            this.rtOutput.Name = "rtOutput";
            this.rtOutput.Size = new System.Drawing.Size(645, 182);
            this.rtOutput.TabIndex = 1;
            this.rtOutput.Text = "";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "SPR files|*.spr";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(15, 112);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // tbPalette
            // 
            this.tbPalette.Location = new System.Drawing.Point(75, 6);
            this.tbPalette.Name = "tbPalette";
            this.tbPalette.Size = new System.Drawing.Size(582, 20);
            this.tbPalette.TabIndex = 3;
            this.tbPalette.Text = "D:\\data\\DarkReign\\def_pal.bmp";
            // 
            // lblPalette
            // 
            this.lblPalette.AutoSize = true;
            this.lblPalette.Location = new System.Drawing.Point(12, 9);
            this.lblPalette.Name = "lblPalette";
            this.lblPalette.Size = new System.Drawing.Size(40, 13);
            this.lblPalette.TabIndex = 4;
            this.lblPalette.Text = "Palette";
            // 
            // btnBulk
            // 
            this.btnBulk.Location = new System.Drawing.Point(582, 43);
            this.btnBulk.Name = "btnBulk";
            this.btnBulk.Size = new System.Drawing.Size(75, 23);
            this.btnBulk.TabIndex = 5;
            this.btnBulk.Text = "Bulk";
            this.btnBulk.UseVisualStyleBackColor = true;
            this.btnBulk.Click += new System.EventHandler(this.btnBulk_Click);
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(12, 48);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(36, 13);
            this.lblFolder.TabIndex = 6;
            this.lblFolder.Text = "Folder";
            // 
            // tbFolder
            // 
            this.tbFolder.Location = new System.Drawing.Point(75, 45);
            this.tbFolder.Name = "tbFolder";
            this.tbFolder.Size = new System.Drawing.Size(501, 20);
            this.tbFolder.TabIndex = 7;
            this.tbFolder.Text = "D:\\data\\DarkReign";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(75, 71);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(501, 23);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(682, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(301, 328);
            this.panel1.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 362);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tbFolder);
            this.Controls.Add(this.lblFolder);
            this.Controls.Add(this.btnBulk);
            this.Controls.Add(this.lblPalette);
            this.Controls.Add(this.tbPalette);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.rtOutput);
            this.MinimumSize = new System.Drawing.Size(1011, 0);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SprReader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtOutput;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox tbPalette;
        private System.Windows.Forms.Label lblPalette;
        private System.Windows.Forms.Button btnBulk;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox tbFolder;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel panel1;
    }
}

