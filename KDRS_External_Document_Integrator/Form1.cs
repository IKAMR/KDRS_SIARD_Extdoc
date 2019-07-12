using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KDRS_External_Document_Integrator
{
    public partial class Form1 : Form
    {

        public static string inFile = string.Empty;
        public static string docFolder = string.Empty;
        public static string outFile = string.Empty;
        public static string outPath = string.Empty;

        public int totalFileCount;
        public int fileCounter;

        public bool isZipped;

        public string zipJarPath = string.Empty;

        XmlWriter xmlWriter;

        SiardZipper zipper = new SiardZipper();
        //--------------------------------------------------------------------------------

        public Form1()
        {
            InitializeComponent();
        }
        //--------------------------------------------------------------------------------

        private void btnChooseInPath_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                txtInFile.Text = openFileDialog1.FileName;
        }
        //--------------------------------------------------------------------------------

        private void btnChooseExtDoc_Click(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                txtExtDocFolder.Text = folderBrowserDialog1.SelectedPath;
        }
        //--------------------------------------------------------------------------------

        private void btnChooseOutFile_Click(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                outPath = folderBrowserDialog1.SelectedPath;
            
        }
        //--------------------------------------------------------------------------------
        // Start program
        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            inFile = txtInFile.Text;
            outPath = txtOutFile.Text;

            zipJarPath = txtInFile.Text;

            Console.WriteLine("outPath: " + outPath);

            if (outPath != string.Empty)
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
                backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
                backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }
        //--------------------------------------------------------------------------------

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                textBox1.Text = "Files copied: " + e.UserState.ToString() + " of total: " + totalFileCount;
            }
            else if (e.ProgressPercentage == 2)
            {
                textBox1.AppendText("\r\n");
            }
            else if (e.ProgressPercentage == 3)
            {
                textBox1.Text = "Writing xml file\r\n" +
                    "File info written: " + e.UserState.ToString() + " of total: " + totalFileCount;
            }
        }
        //--------------------------------------------------------------------------------

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                textBox1.Text = "Error: " + e.Error.Message;

            }
            else
            {
                textBox1.AppendText("\r\nJob complete");
            }
        }

        //--------------------------------------------------------------------------------

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DoTransform();

            }
            catch (IOException)
            {
                throw new Exception("Files already exits in target folder");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //--------------------------------------------------------------------------------

        private void DoTransform()
        {
            docFolder = txtExtDocFolder.Text;
            Console.WriteLine("Document folder: " + docFolder);

            string lobFolder = @"schema0\table0\lob0";
            string tablefolder = @"schema0\table0";

            string lobFolderPath = Path.Combine(outPath, @"siard\content\" + lobFolder);
            string tableFolderPath = Path.Combine(outPath, @"siard\content\" + tablefolder);

            int counter = 0;

            DirectoryInfo sourceFolder = new DirectoryInfo(docFolder);

            int fileCount = 0;
            FileInfo[] clobFiles = sourceFolder.GetFiles("*", SearchOption.AllDirectories);

            totalFileCount = clobFiles.Count();

            fileCounter = 0;
            DirectoryCopy(docFolder, lobFolderPath);

            DirectoryInfo targetDir = new DirectoryInfo(lobFolderPath);
            FileInfo[] copiedClobFiles = targetDir.GetFiles("*", SearchOption.AllDirectories);

            Console.WriteLine("Targetdir: " + targetDir.Name);
            if (!targetDir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Target folder does not exist or could not be found: "
                    + targetDir);
            }

            Console.WriteLine("Creating metadata.xml");

            CreateMetadataXML(lobFolder);

            Console.WriteLine("Creating table.xml");

            CreateTableXML(tableFolderPath);

            backgroundWorker1.ReportProgress(2);
            foreach (FileInfo file in copiedClobFiles)
            {
                AddTableXMLFileInfo(fileCount, file);
                counter++;

                fileCount++;
                //Console.WriteLine("Filecount: " + fileCount);
                backgroundWorker1.ReportProgress(3, fileCount);
            }

            xmlWriter.WriteComment("Row count: " + counter);
            xmlWriter.WriteComment("Finshed at: " + GetTimeStamp(DateTime.Now));

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            isZipped = zipper.SiardZip(outPath + @"\siard", Path.Combine(outPath, "Ext_Doc_Int"));
            Console.WriteLine("isZipped: " + isZipped);

            Console.WriteLine("Job complete");

        }

        //--------------------------------------------------------------------------------
        // Creates table.xml file containing information about all the table files.
        private void CreateMetadataXML(string lobFolder)
        {
            string headerPath = Path.Combine(outPath, @"siard\header");
            Directory.CreateDirectory(headerPath);

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            xmlWriter = XmlWriter.Create(headerPath + "/metadata.xml", xmlWriterSettings);

            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("siardArchive", "http://www.bar.admin.ch/xmlns/siard/2/metadata.xsd");
            xmlWriter.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.bar.admin.ch/xmlns/siard/2/metadata.xsd metadata.xsd");
            xmlWriter.WriteAttributeString("version", "2.1");

            xmlWriter.WriteStartElement("schemas");
            xmlWriter.WriteStartElement("schema");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("schemaName");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("folder");
            xmlWriter.WriteString("schema0");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("tables");
            xmlWriter.WriteStartElement("table");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("object");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("folder");
            xmlWriter.WriteString("table0");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("columns");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("fileObject");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("lobFolder");
            xmlWriter.WriteString(lobFolder);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }
        //--------------------------------------------------------------------------------
        // Creates table.xml file containing information about all the table files.
        private void CreateTableXML(string lobFolderPath)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            xmlWriter = XmlWriter.Create(Path.Combine(lobFolderPath, "table0.xml"), xmlWriterSettings);

            xmlWriter.WriteStartDocument();

            string timeStampDate = DateTime.Now.ToShortDateString();
            string timeStampTime = DateTime.Now.ToLongTimeString();

            xmlWriter.WriteComment("Create time: " + timeStampDate + " " + timeStampTime);
            //xmlWriter.WriteComment("Table " + tableSchema + "/" + tableName + " corresponds to actual table " + actualTable);

            xmlWriter.WriteStartElement("table", "http://www.bar.admin.ch/xmlns/siard/2/table.xsd");
            xmlWriter.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.bar.admin.ch/xmlns/siard/2/table.xsd table.xsd");
            xmlWriter.WriteAttributeString("version", "2.1");

        }
        //--------------------------------------------------------------------------------
        // Adds info for all table files to the table.xml file.
        private void AddTableXMLFileInfo(int fileCount, FileInfo fileInfo)
        {
            //Console.WriteLine("Adding table info for: " + fileInfo.Name);
            //FileInfo fi = new FileInfo(fileName);
            long fileLength = fileInfo.Length;

            xmlWriter.WriteStartElement("row");

            xmlWriter.WriteStartElement("c1");
            xmlWriter.WriteString(fileCount.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("c2");
            xmlWriter.WriteString(fileInfo.Name);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("c3");
            xmlWriter.WriteString(fileInfo.Extension.Replace(".", ""));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("c4");
            xmlWriter.WriteString(fileInfo.FullName);
            xmlWriter.WriteEndElement();

            string digest = CalculateMD5(fileInfo.FullName);

            // string parentFolder = Path.Combine(Path.GetFileName(Directory.GetParent(fileInfo.FullName).ToString()), fileInfo.Name);

            string parentFolder = Path.Combine(GetParents(fileInfo.Directory),fileInfo.Name);

            xmlWriter.WriteStartElement("c15");
            xmlWriter.WriteAttributeString("file", parentFolder);
            xmlWriter.WriteAttributeString("length", fileLength.ToString());
            xmlWriter.WriteAttributeString("digestType", "MD5");
            xmlWriter.WriteAttributeString("digest", digest);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }
        //--------------------------------------------------------------------------------
        // Calculated MD5 digest of input file.
        static string CalculateMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", String.Empty);
                }
            }
        }

        //-------------------------------------------------------------------------------
        private void DirectoryCopy(string sourceFolder, string targetPath)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceFolder);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source folder does not exist or could not be found: " + sourceFolder);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            DirectoryInfo targetDir = new DirectoryInfo(targetPath);
            if (dir.Exists)
            {
               // throw new Exception("Target folder already exist: " + targetPath);
            }
            Directory.CreateDirectory(targetPath);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(targetPath, file.Name);
                file.CopyTo(tempPath, false);
                fileCounter++;
                backgroundWorker1.ReportProgress(1, fileCounter);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(targetPath, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }

        //-------------------------------------------------------------------------------
        private static String GetTimeStamp(DateTime value)
        {
            return value.ToString("dd.mm.yyyy HH.mm.ss");
        }
        //-------------------------------------------------------------------------------
        private string GetParents(DirectoryInfo fileDirectory)
        {
            string fileName = fileDirectory.Name;
            if (fileDirectory.Parent.Name != "lob0")
                fileName = GetParents(fileDirectory.Parent) + @"\" + fileName;

            return fileName;
        }

        //-------------------------------------------------------------------------------
    }

    //=======================================================================================

    class SiardZipper
    {
        public bool SiardZip(string folder, string targetName)
        {
            string javaPath = @"D:\prog\zip64_v2.1.58\zip64\lib\zip64.jar";
            string source = "-d=" + folder;
            string target = targetName + ".siard";

            string javaCommand = " -jar " + javaPath + " n " + source + " " + target;

            Process proc = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Arguments = javaCommand;
            startInfo.FileName = "java";

            proc.StartInfo = startInfo;

            Console.WriteLine(javaCommand);
            if (!File.Exists(javaPath))
                throw new Exception("Cannot find zip64.jar");
            try
            {
                Console.WriteLine("Creating .siard at: " + target);
                proc.Start();
                proc.WaitForExit();

                return proc.HasExited;
                Console.WriteLine(".siard Created");
            }
            catch
            {
                Console.WriteLine("zip error");
                return proc.HasExited;
            }
        }

        public void SiardUnZip(string targetFolder, string sourceFile)
        {
            string javaPath = @"C:\prog\zip64_v2.1.58\zip64-2.1.58\lib\zip64.jar";
            string target = "-d=" + targetFolder;
            string source = sourceFile + ".siard";

            string javaCommand = " -jar " + javaPath + " x " + target + " " + source;

            Process proc = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Arguments = javaCommand;
            startInfo.FileName = "java";

            proc.StartInfo = startInfo;

            Console.WriteLine(javaCommand);

            System.IO.Directory.CreateDirectory(targetFolder);

            try
            {
                Console.WriteLine("Creating .siard");
                proc.Start();
                Console.WriteLine(".siard Created");
            }
            catch
            {
                Console.WriteLine("unzip error");
            }
        }
    }
}
