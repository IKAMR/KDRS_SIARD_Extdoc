namespace KDRS_SIARD_Extdoc
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
            this.btnChooseOutFolder = new System.Windows.Forms.Button();
            this.txtOutFolder = new System.Windows.Forms.TextBox();
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.btnChooseExtDoc = new System.Windows.Forms.Button();
            this.txtExtDocFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.txtArchiver = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.txtArchCont = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.txtDatOwn = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.txtDatOriTime = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.txtMetaDesc = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtInFile
            // 
            this.txtInFile.CausesValidation = false;
            this.txtInFile.Location = new System.Drawing.Point(154, 61);
            this.txtInFile.Name = "txtInFile";
            this.txtInFile.Size = new System.Drawing.Size(401, 20);
            this.txtInFile.TabIndex = 0;
            // 
            // btnChooseInFile
            // 
            this.btnChooseInFile.CausesValidation = false;
            this.btnChooseInFile.Location = new System.Drawing.Point(16, 62);
            this.btnChooseInFile.Name = "btnChooseInFile";
            this.btnChooseInFile.Size = new System.Drawing.Size(132, 23);
            this.btnChooseInFile.TabIndex = 1;
            this.btnChooseInFile.Text = "zip64.jar location";
            this.btnChooseInFile.UseVisualStyleBackColor = true;
            this.btnChooseInFile.Click += new System.EventHandler(this.btnChooseInPath_Click);
            // 
            // btnChooseOutFolder
            // 
            this.btnChooseOutFolder.CausesValidation = false;
            this.btnChooseOutFolder.Location = new System.Drawing.Point(16, 33);
            this.btnChooseOutFolder.Name = "btnChooseOutFolder";
            this.btnChooseOutFolder.Size = new System.Drawing.Size(132, 23);
            this.btnChooseOutFolder.TabIndex = 3;
            this.btnChooseOutFolder.Text = "Choose target folder";
            this.btnChooseOutFolder.UseVisualStyleBackColor = true;
            this.btnChooseOutFolder.Click += new System.EventHandler(this.btnChooseOutFolder_Click);
            // 
            // txtOutFolder
            // 
            this.txtOutFolder.AccessibleName = "Choose target folder";
            this.txtOutFolder.Location = new System.Drawing.Point(154, 35);
            this.txtOutFolder.Name = "txtOutFolder";
            this.txtOutFolder.Size = new System.Drawing.Size(401, 20);
            this.txtOutFolder.TabIndex = 2;
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.Location = new System.Drawing.Point(12, 96);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(119, 40);
            this.btnStartProcess.TabIndex = 4;
            this.btnStartProcess.Text = "Start process";
            this.btnStartProcess.UseVisualStyleBackColor = true;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartProcess_Click);
            // 
            // btnChooseExtDoc
            // 
            this.btnChooseExtDoc.CausesValidation = false;
            this.btnChooseExtDoc.Location = new System.Drawing.Point(16, 7);
            this.btnChooseExtDoc.Name = "btnChooseExtDoc";
            this.btnChooseExtDoc.Size = new System.Drawing.Size(132, 23);
            this.btnChooseExtDoc.TabIndex = 6;
            this.btnChooseExtDoc.Text = "Choose document folder";
            this.btnChooseExtDoc.UseVisualStyleBackColor = true;
            this.btnChooseExtDoc.Click += new System.EventHandler(this.btnChooseExtDoc_Click);
            // 
            // txtExtDocFolder
            // 
            this.txtExtDocFolder.AccessibleName = "Choose document folder";
            this.txtExtDocFolder.Location = new System.Drawing.Point(154, 9);
            this.txtExtDocFolder.Name = "txtExtDocFolder";
            this.txtExtDocFolder.Size = new System.Drawing.Size(401, 20);
            this.txtExtDocFolder.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.CausesValidation = false;
            this.textBox1.Location = new System.Drawing.Point(12, 143);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(543, 86);
            this.textBox1.TabIndex = 7;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // txtArchiver
            // 
            this.txtArchiver.CausesValidation = false;
            this.txtArchiver.Location = new System.Drawing.Point(249, 235);
            this.txtArchiver.Name = "txtArchiver";
            this.txtArchiver.Size = new System.Drawing.Size(306, 20);
            this.txtArchiver.TabIndex = 0;
            // 
            // textBox3
            // 
            this.textBox3.CausesValidation = false;
            this.textBox3.Location = new System.Drawing.Point(12, 235);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(204, 20);
            this.textBox3.TabIndex = 1;
            this.textBox3.Text = "Archiver: (person, OPTIONAL)";
            // 
            // txtArchCont
            // 
            this.txtArchCont.CausesValidation = false;
            this.txtArchCont.Location = new System.Drawing.Point(249, 261);
            this.txtArchCont.Name = "txtArchCont";
            this.txtArchCont.Size = new System.Drawing.Size(306, 20);
            this.txtArchCont.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.CausesValidation = false;
            this.textBox4.Location = new System.Drawing.Point(12, 261);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(204, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.Text = "Archiver contact (OPTIONAL)";
            // 
            // txtDatOwn
            // 
            this.txtDatOwn.AccessibleName = "Data owner";
            this.txtDatOwn.Location = new System.Drawing.Point(249, 287);
            this.txtDatOwn.Name = "txtDatOwn";
            this.txtDatOwn.Size = new System.Drawing.Size(306, 20);
            this.txtDatOwn.TabIndex = 4;
            // 
            // textBox6
            // 
            this.textBox6.CausesValidation = false;
            this.textBox6.Location = new System.Drawing.Point(12, 287);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(204, 20);
            this.textBox6.TabIndex = 5;
            this.textBox6.Text = "Data owner (MANDATORY)";
            // 
            // txtDatOriTime
            // 
            this.txtDatOriTime.AccessibleName = "Data origin timespan";
            this.txtDatOriTime.Location = new System.Drawing.Point(249, 313);
            this.txtDatOriTime.Name = "txtDatOriTime";
            this.txtDatOriTime.Size = new System.Drawing.Size(306, 20);
            this.txtDatOriTime.TabIndex = 6;
            // 
            // textBox8
            // 
            this.textBox8.CausesValidation = false;
            this.textBox8.Location = new System.Drawing.Point(12, 313);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(204, 20);
            this.textBox8.TabIndex = 7;
            this.textBox8.Text = "Data origin timespan (MANDATORY)";
            // 
            // txtMetaDesc
            // 
            this.txtMetaDesc.CausesValidation = false;
            this.txtMetaDesc.Location = new System.Drawing.Point(249, 339);
            this.txtMetaDesc.Multiline = true;
            this.txtMetaDesc.Name = "txtMetaDesc";
            this.txtMetaDesc.Size = new System.Drawing.Size(306, 83);
            this.txtMetaDesc.TabIndex = 8;
            // 
            // textBox10
            // 
            this.textBox10.CausesValidation = false;
            this.textBox10.Location = new System.Drawing.Point(12, 339);
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(204, 20);
            this.textBox10.TabIndex = 9;
            this.textBox10.Text = "Description (OPTIONAL)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(568, 443);
            this.Controls.Add(this.textBox10);
            this.Controls.Add(this.txtMetaDesc);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.btnChooseExtDoc);
            this.Controls.Add(this.txtDatOriTime);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.txtExtDocFolder);
            this.Controls.Add(this.txtDatOwn);
            this.Controls.Add(this.btnStartProcess);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.btnChooseOutFolder);
            this.Controls.Add(this.txtArchCont);
            this.Controls.Add(this.txtOutFolder);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.btnChooseInFile);
            this.Controls.Add(this.txtArchiver);
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
        private System.Windows.Forms.Button btnChooseOutFolder;
        private System.Windows.Forms.TextBox txtOutFolder;
        private System.Windows.Forms.Button btnStartProcess;
        private System.Windows.Forms.Button btnChooseExtDoc;
        private System.Windows.Forms.TextBox txtExtDocFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox txtArchiver;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox txtArchCont;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox txtDatOwn;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox txtDatOriTime;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox txtMetaDesc;
        private System.Windows.Forms.TextBox textBox10;
    }
}

