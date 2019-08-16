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
                //DoTransform();
                MakeSiard();
            }
            catch (IOException ex)
            {
                throw ex; //new Exception("Files already exits in target folder");
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

            string lobFolder = "documents";
            string tablefolder = @"schema0\table0\";

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

            CreateMetadataXML(lobFolder, "content", fileCount);

            Console.WriteLine("Creating table.xml");

            CreateTableXML(tableFolderPath);

            backgroundWorker1.ReportProgress(2);

            Console.WriteLine("Clob files: " + copiedClobFiles.Length);


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
        public void MakeSiard()
        {

            docFolder = txtExtDocFolder.Text;

          
            
            string tableFolderPath = Path.Combine(outPath, @"siard\content\schema0\table0\");

            Uri docUri = new Uri(docFolder);
            Uri outUri = new Uri(Path.Combine(outPath, @"siard\header"));

            Uri relativePath = outUri.MakeRelativeUri(docUri);
            //Uri relPathParent = new Uri(relativePath, "..");
            Console.WriteLine("Relpath: " + relativePath);

            int counter = 0;

            DirectoryInfo sourceFolder = new DirectoryInfo(docFolder);

            int fileCount = 0;
            FileInfo[] clobFiles = sourceFolder.GetFiles("*", SearchOption.AllDirectories);

            totalFileCount = clobFiles.Count();

            Console.WriteLine("Source folder: " + docFolder);
            if (!sourceFolder.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source folder does not exist or could not be found: "
                    + sourceFolder);
            }


            Console.WriteLine("Creating metadata.xml");

            CreateMetadataXML( "", relativePath.ToString(), totalFileCount);

            Console.WriteLine("Creating table.xml");

            CreateTableXML(tableFolderPath);

            backgroundWorker1.ReportProgress(2);


            foreach (FileInfo file in clobFiles)
            {
                AddTableXMLFileInfo(fileCount, file);
                counter++;

                fileCount++;
                //Console.WriteLine("Filecount: " + fileCount);
                backgroundWorker1.ReportProgress(3, fileCount);
            }

            xmlWriter.WriteComment("Row count: " + counter);
            xmlWriter.WriteComment("Finished at: " + GetTimeStamp(DateTime.Now));

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            isZipped = zipper.SiardZip(outPath + @"\siard", Path.Combine(outPath, "Ext_Doc_Int"));
            Console.WriteLine("isZipped: " + isZipped);

            Console.WriteLine("Job complete");
        }

        //--------------------------------------------------------------------------------
        // Creates table.xml file containing information about all the table files.
        private void CreateMetadataXML(string lobFolder, string lobFolderPath, int fileCount)
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

            xmlWriter.WriteStartElement("lobFolder");
            xmlWriter.WriteString(lobFolderPath);
            xmlWriter.WriteEndElement();
            
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

            #region Columns
            //Columns

            // c1
            xmlWriter.WriteComment("c1");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("objectID");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("INT");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c2
            xmlWriter.WriteComment("c2");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("fileName");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c3
            xmlWriter.WriteComment("c3");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("fileExt");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(20)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c4
            xmlWriter.WriteComment("c4");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("filePath");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(255)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c5
            xmlWriter.WriteComment("c5");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("fileMime");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c6
            xmlWriter.WriteComment("c6");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("filePuid");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c7
            xmlWriter.WriteComment("c7");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("fileType");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c8
            xmlWriter.WriteComment("c8");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("fileVersion");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c9
            xmlWriter.WriteComment("c9");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("file2Ext");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(20)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c10
            xmlWriter.WriteComment("c10");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("file2Path");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c11
            xmlWriter.WriteComment("c11");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("file2Mime");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c12
            xmlWriter.WriteComment("c12");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("file2Puid");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c13
            xmlWriter.WriteComment("c13");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("file2Type");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c14
            xmlWriter.WriteComment("c14");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("file2Version");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("VARCHAR(100)");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            // c15
            xmlWriter.WriteComment("c15");
            xmlWriter.WriteStartElement("column");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("fileObject");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("lobFolder");
            xmlWriter.WriteString(lobFolder);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("type");
            xmlWriter.WriteString("BLOB");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("nullable");
            xmlWriter.WriteString("true");
            xmlWriter.WriteEndElement();

            //End column
            xmlWriter.WriteEndElement();

            #endregion

            //End columns
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("primaryKey");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("PRIMARY");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("column");
            xmlWriter.WriteString("objectID");
            xmlWriter.WriteEndElement();

            // End primaryKey
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("rows");
            xmlWriter.WriteString(fileCount.ToString());
            xmlWriter.WriteEndElement();

            //End table
            xmlWriter.WriteEndElement();
            //End tables
            xmlWriter.WriteEndElement();

            //End schema
            xmlWriter.WriteEndElement();
            //End schemas
            xmlWriter.WriteEndElement();

            //End siardArchive
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

            Directory.CreateDirectory(Directory.GetParent(lobFolderPath).ToString());

            Console.WriteLine("Creating table0 at: " + lobFolderPath);
            xmlWriter = XmlWriter.Create(Path.Combine(lobFolderPath, "table0.xml"), xmlWriterSettings);
            Console.WriteLine("table0 created");

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
            Console.WriteLine("Adding table info");
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

            string dirctoryName = @"\" + GetParentName(fileInfo.DirectoryName, docFolder);
            
            xmlWriter.WriteStartElement("c4");
            xmlWriter.WriteString(dirctoryName);
            xmlWriter.WriteEndElement();

            string digest = CalculateMD5(fileInfo.FullName);

            string directory = fileInfo.Directory.Name;
            string fileName  = GetParentName(fileInfo.FullName, docFolder);

            xmlWriter.WriteStartElement("c15");
            xmlWriter.WriteAttributeString("file", fileName);
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
        //--------------------------------------------------------------------------------
        private string GetParentName(string fileInfo, string topParent)
        {
            var dir = new DirectoryInfo(fileInfo);

            string tempParent = dir.Name;
            Console.WriteLine("TempParent: " + tempParent);
            if (dir.FullName == topParent)
                return "";

            return Path.Combine(this.GetParentName(dir.Parent.FullName, topParent), tempParent);
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
            Console.WriteLine("Creating:" + targetPath);

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

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = javaCommand,
                FileName = "java"
            };

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

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = javaCommand,
                FileName = "java"
            };

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
