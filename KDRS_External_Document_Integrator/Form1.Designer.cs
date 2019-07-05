﻿namespace KDRS_External_Document_Integrator
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtInFile = new System.Windows.Forms.TextBox();
            this.btnChooseInFile = new System.Windows.Forms.Button();
            this.btnChooseOutFile = new System.Windows.Forms.Button();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.btnChooseExtDoc = new System.Windows.Forms.Button();
            this.txtExtDocFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtInFile
            // 
            this.txtInFile.Location = new System.Drawing.Point(26, 51);
            this.txtInFile.Name = "txtInFile";
            this.txtInFile.Size = new System.Drawing.Size(374, 20);
            this.txtInFile.TabIndex = 0;
            // 
            // btnChooseInFile
            // 
            this.btnChooseInFile.Location = new System.Drawing.Point(406, 49);
            this.btnChooseInFile.Name = "btnChooseInFile";
            this.btnChooseInFile.Size = new System.Drawing.Size(132, 23);
            this.btnChooseInFile.TabIndex = 1;
            this.btnChooseInFile.Text = "Choose .siard file";
            this.btnChooseInFile.UseVisualStyleBackColor = true;
            this.btnChooseInFile.Click += new System.EventHandler(this.btnChooseInPath_Click);
            // 
            // btnChooseOutFile
            // 
            this.btnChooseOutFile.Location = new System.Drawing.Point(406, 108);
            this.btnChooseOutFile.Name = "btnChooseOutFile";
            this.btnChooseOutFile.Size = new System.Drawing.Size(132, 23);
            this.btnChooseOutFile.TabIndex = 3;
            this.btnChooseOutFile.Text = "Choose target folder";
            this.btnChooseOutFile.UseVisualStyleBackColor = true;
            this.btnChooseOutFile.Click += new System.EventHandler(this.btnChooseOutFile_Click);
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(26, 110);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(374, 20);
            this.txtOutFile.TabIndex = 2;
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.Location = new System.Drawing.Point(26, 136);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(119, 40);
            this.btnStartProcess.TabIndex = 4;
            this.btnStartProcess.Text = "Start process";
            this.btnStartProcess.UseVisualStyleBackColor = true;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartProcess_Click);
            // 
            // btnChooseExtDoc
            // 
            this.btnChooseExtDoc.Location = new System.Drawing.Point(406, 75);
            this.btnChooseExtDoc.Name = "btnChooseExtDoc";
            this.btnChooseExtDoc.Size = new System.Drawing.Size(132, 23);
            this.btnChooseExtDoc.TabIndex = 6;
            this.btnChooseExtDoc.Text = "Choose document folder";
            this.btnChooseExtDoc.UseVisualStyleBackColor = true;
            this.btnChooseExtDoc.Click += new System.EventHandler(this.btnChooseExtDoc_Click);
            // 
            // txtExtDocFolder
            // 
            this.txtExtDocFolder.Location = new System.Drawing.Point(26, 77);
            this.txtExtDocFolder.Name = "txtExtDocFolder";
            this.txtExtDocFolder.Size = new System.Drawing.Size(374, 20);
            this.txtExtDocFolder.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(26, 183);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(598, 78);
            this.textBox1.TabIndex = 7;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 480);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnChooseExtDoc);
            this.Controls.Add(this.txtExtDocFolder);
            this.Controls.Add(this.btnStartProcess);
            this.Controls.Add(this.btnChooseOutFile);
            this.Controls.Add(this.txtOutFile);
            this.Controls.Add(this.btnChooseInFile);
            this.Controls.Add(this.txtInFile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtInFile;
        private System.Windows.Forms.Button btnChooseInFile;
        private System.Windows.Forms.Button btnChooseOutFile;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.Button btnStartProcess;
        private System.Windows.Forms.Button btnChooseExtDoc;
        private System.Windows.Forms.TextBox txtExtDocFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

