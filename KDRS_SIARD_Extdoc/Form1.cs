using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;

namespace KDRS_SIARD_Extdoc
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

        public string zip64JarPath = string.Empty;

        public string archiver = string.Empty;
        public string archCont = string.Empty;
        public string dataOwner = string.Empty;
        public string dataOrigin = string.Empty;
        public string metaDataDesc = string.Empty;

        XmlWriter xmlWriter;

        SiardZipper zipper = new SiardZipper();
        //--------------------------------------------------------------------------------

        public Form1()
        {
            InitializeComponent();
            Text = Globals.toolName + " " + Globals.toolVersion;

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
        private void btnChooseOutFolder_Click(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                outPath = folderBrowserDialog1.SelectedPath;
            txtOutFolder.Text = outPath;
        }
        //--------------------------------------------------------------------------------
        // Start program
        private void btnStartProcess_Click(object sender, EventArgs e)
        {

            if (textBox_Validate())
            {

                outPath = txtOutFolder.Text;

                archiver = txtArchiver.Text;
                archCont = txtArchCont.Text;
                dataOwner = txtDatOwn.Text;
                dataOrigin = txtDatOriTime.Text;

                metaDataDesc = txtMetaDesc.Text;

                if (txtInFile.Text != "")
                {
                    Properties.Settings.Default.Zip64JarPath = txtInFile.Text;
                    Properties.Settings.Default.Save();
                }

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
        }
        //--------------------------------------------------------------------------------
        // Validates that all mandatory textboxes are filled.
        private bool textBox_Validate()
        {
            foreach (TextBox tb in this.Controls.OfType<TextBox>().Where(x => x.CausesValidation == true))
            {
                if (tb.Text == "")
                {
                    textBox1.Text = string.Format("Fill inn '{0 }'", tb.AccessibleName);
                    return false;
                }
                else
                {
                    textBox1.Clear();
                }
            }
            return true;
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
                textBox1.AppendText("\r\n\r\nError: " + e.Error.Message);
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
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //--------------------------------------------------------------------------------
        // Function to copy all files in document folder to new folder and create siard package with files included.
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

            CreateMetadataXML(lobFolder, "content", fileCount);

            CreateTableXML(tableFolderPath);

            backgroundWorker1.ReportProgress(2);

            foreach (FileInfo file in copiedClobFiles)
            {
                AddTableXMLFileInfo(fileCount, file);
                counter++;

                fileCount++;
                backgroundWorker1.ReportProgress(3, fileCount);
            }

            xmlWriter.WriteComment("Row count: " + counter);
            xmlWriter.WriteComment("Finshed at: " + GetTimeStamp(DateTime.Now));

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            isZipped = zipper.SiardZip(outPath + @"\siard", Path.Combine(outPath, "Ext_Doc_Int"), Properties.Settings.Default.Zip64JarPath);
        }
        //--------------------------------------------------------------------------------
        // Function to create siard package with path to external documents.
        public void MakeSiard()
        {

            docFolder = txtExtDocFolder.Text;

            string tableFolderPath = Path.Combine(outPath, @"siard\content\schema0\table0\");

            Uri docUri = new Uri(docFolder);
            Uri outUri = new Uri(Path.Combine(outPath, @"siard\header"));

            Uri relativePath = outUri.MakeRelativeUri(docUri);

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

            CreateMetadataXML(sourceFolder.Name, relativePath.ToString(), totalFileCount);

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

            isZipped = zipper.SiardZip(outPath + @"\siard", Path.Combine(outPath, "extdoc_" + GetDate(DateTime.Now)), Properties.Settings.Default.Zip64JarPath);
            Console.WriteLine("isZipped: " + isZipped);

            Console.WriteLine("Job complete");
        }

        //--------------------------------------------------------------------------------
        // Creates table.xml file containing information about all the table files.
        private void CreateMetadataXML(string lobFolder, string lobFolderPath, int fileCount)
        {
            string headerPath = Path.Combine(outPath, @"siard\header");
            Directory.CreateDirectory(headerPath);

            CopyFile("schema/metadata.xsd", headerPath);

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            string fileName = Path.Combine(headerPath, "metadata.xml");

            xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings);

            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("siardArchive", "http://www.bar.admin.ch/xmlns/siard/2/metadata.xsd");
            xmlWriter.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.bar.admin.ch/xmlns/siard/2/metadata.xsd metadata.xsd");
            xmlWriter.WriteAttributeString("version", "2.1");

            AddXMLMetadata(fileName, lobFolder, lobFolderPath);

            xmlWriter.WriteStartElement("schemas");
            xmlWriter.WriteStartElement("schema");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("extdoc");
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

            // users
            xmlWriter.WriteStartElement("users");
            // user
            xmlWriter.WriteStartElement("user");
            // name
            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteString("extdocuser");
            // End name
            xmlWriter.WriteEndElement();
            // End user
            xmlWriter.WriteEndElement();
            // End users
            xmlWriter.WriteEndElement();

            //End siardArchive
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        //--------------------------------------------------------------------------------
        private void AddXMLMetadata(string fileName, string lobFolder, string lobFolderPath)
        {

            string dbName = "extdoc";

            xmlWriter.WriteStartElement("dbname");
            xmlWriter.WriteString(dbName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("description");
            xmlWriter.WriteString(metaDataDesc);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("archiver");
            xmlWriter.WriteString(archiver);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("archiverContact");
            xmlWriter.WriteString(archCont);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("dataOwner");
            xmlWriter.WriteString(dataOwner);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("dataOriginTimespan");
            xmlWriter.WriteString(dataOrigin);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("lobFolder");
            xmlWriter.WriteString(lobFolderPath);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("producerApplication");
            xmlWriter.WriteString("KDRS SIARD Extdoc v" + Globals.toolVersion);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("archivalDate");
            xmlWriter.WriteString(GetDate(DateTime.Now));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("clientMachine");
            xmlWriter.WriteString(Environment.MachineName);
            xmlWriter.WriteEndElement();

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

            string tablePath = Directory.GetParent(lobFolderPath).ToString();

            Directory.CreateDirectory(tablePath);

            CopyFile("schema/table0.xsd", tablePath);


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
            string fileName = GetParentName(fileInfo.FullName, docFolder);

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
        //--------------------------------------------------------------------------------
        private void CopyFile(string filePath, string targetFolder)
        {
            FileInfo fi = new FileInfo(filePath);
            string fileName = fi.Name;

            fi.CopyTo(Path.Combine(targetFolder, fileName), true);
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
            return value.ToString("dd.MM.yyyy HH.mm.ss");
        }      
        //-------------------------------------------------------------------------------
        private static String GetDate(DateTime value)
        {
            return value.ToString("yyyy-MM-dd");
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
        // Not used
        private void ReadSettingsFile(string settingsFileName)
        {
            var dic = File.ReadAllLines(settingsFileName)
                .Select(l => l.Split(new[] { '=' }))
                .ToDictionary(s => s[0].Trim(), s => s[1].Trim());

            zip64JarPath = dic["zip64JarPath"];
        }

        //-------------------------------------------------------------------------------
    }

    //=======================================================================================
    public static class Globals
    {
        public static readonly String toolName = "KDRS SIARD Extdoc";
        public static readonly String toolVersion = "0.3.2";
    }

    //=======================================================================================

    class SiardZipper
    {
        public bool SiardZip(string folder, string targetName, string jarPath)
        {
            Console.WriteLine("zip64: " + jarPath);
            // jarPath =Path.Combine(jarPath, "zip64.jar");
            string source = "-d=" + folder;
            string target = targetName + ".siard";

            string javaCommand = " -jar " + jarPath + " n " + source + " " + target;

            Process proc = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = javaCommand,
                FileName = "java"
            };

            proc.StartInfo = startInfo;

            Console.WriteLine(javaCommand);
            if (!File.Exists(jarPath))
                throw new Exception("Cannot find zip64.jar at " + jarPath);
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

        public void SiardUnZip(string targetFolder, string sourceFile, string jarPath)
        {
            jarPath = Path.Combine(jarPath, "zip64.jar");
            string target = "-d=" + targetFolder;
            string source = sourceFile + ".siard";

            string javaCommand = " -jar " + jarPath + " x " + target + " " + source;

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
