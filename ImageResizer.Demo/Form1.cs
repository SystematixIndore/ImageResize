using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageResizer.Demo
{
    public partial class Form1 : Form
    {
        string[] allowedExtensions = new string[] { ".jpg", ".bmp" };

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                lblPath.Text = fbd.SelectedPath;
                var files = Directory.GetFiles(fbd.SelectedPath, "*.*").ToList();
                lblTotalFiles.Text = files.Count.ToString();
                var images = files.Where(file => file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".gif")).ToList();
                lblTotalImages.Text = images.Count().ToString();
            }
        }
        private void btnResize_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(lblPath.Text, "*.*").ToList();
            var images = files.Where(file => file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".gif")).ToList();
            foreach (var pic in images)
            {
                Image simage = Image.FromFile(pic.ToString());
                var imgStream = ImageResizeAspect.ResizeImage(simage, new Size(200, 200));
                Bitmap omg = new Bitmap(imgStream);

                string dir = Path.GetDirectoryName(pic);
                string fname = Path.GetFileName(pic);
                dir = dir + "\\Resized";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                omg.Save(dir + "\\" + fname, System.Drawing.Imaging.ImageFormat.Gif);
            }
        }

        private void fileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            lblImageAdded.Text += e.ChangeType + ": " + e.FullPath + "\r\n";
        }

        private void fileWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            lblImageAdded.Text += e.ChangeType + ": " + e.FullPath + "\r\n";
            Image simage = Image.FromFile(e.FullPath.ToString());
            var imgStream = ImageResizeAspect.ResizeImage(simage, new Size(200, 200));
            Bitmap omg = new Bitmap(imgStream);

            string dir = Path.GetDirectoryName(e.FullPath);
            string fname = Path.GetFileName(e.FullPath);
            dir = dir + "\\Resized";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            omg.Save(dir + "\\" + fname, System.Drawing.Imaging.ImageFormat.Gif);
        }

        private void fileWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            lblImageAdded.Text += e.ChangeType + ": " + e.FullPath + "\r\n";
        }

        private void fileWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            lblImageAdded.Text += e.ChangeType + ": " + e.OldFullPath + " renamed to " + e.FullPath;
        }

        
        private void btnFileTracking_Click(object sender, EventArgs e)
        {
            fileSystemWatcher1.Path = lblPath.Text;
            fileSystemWatcher1.Filter = "*.png";
            fileSystemWatcher1.IncludeSubdirectories = checkBox1.Checked;
            // Add event handlers.
            fileSystemWatcher1.Changed += new FileSystemEventHandler(fileWatcher_Changed);
            fileSystemWatcher1.Created += new FileSystemEventHandler(fileWatcher_Created);
            fileSystemWatcher1.Deleted += new FileSystemEventHandler(fileWatcher_Deleted);
            fileSystemWatcher1.Renamed += new RenamedEventHandler(fileWatcher_Renamed);

            fileSystemWatcher1.EnableRaisingEvents = true;
        }
    }
}
