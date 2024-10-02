using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualStudio.OLE.Interop;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Image_Preview.Controls
{
    public partial class mybtn : UserControl
    {
        private ContextMenuStrip contextMenuStrip;
        public mybtn()
        {
            InitializeComponent();
            contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem item1 = new ToolStripMenuItem("Option 1");
            ToolStripMenuItem item2 = new ToolStripMenuItem("Option 2");
            ToolStripMenuItem item3 = new ToolStripMenuItem("Option 3");
            item1.Click += Item1_Click;
            item2.Click += Item2_Click;
            item3.Click += Item3_Click;
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { item1, item2, item3 });
            this.ContextMenuStrip = contextMenuStrip;

        }

        public string btn_text = "";
        public FileInfo filepath = null;
        public void refreshme()
        {
            button1.Text = btn_text;

            load_images();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.filepath.FullName);
        }



        private void load_images()
        {
            Bitmap big_image = null;
            ToolTip toolTip = new ToolTip();

            try
            {
                button1.BackgroundImage = System.Drawing.Image.FromFile(filepath.FullName);
                button1.Text = "";
                Console.WriteLine(filepath.FullName);
                toolTip.SetToolTip(button1, $"Filename: {filepath.Name}\nPath: {filepath.FullName}");
            }
            catch
            {
                big_image = null;
            }



            button1.Click += new EventHandler(button3_Click);
        }


        private void button3_Click(object sender, EventArgs e)
        {

            ShowImageDetails(filepath.FullName);
        }


        private void ShowImageDetails(string imagePath)
        {
            FileInfo imageFile = new FileInfo(imagePath);

            long fileSize = imageFile.Length;
            string fileSizeInKB = (fileSize / 1024).ToString() + " KB";


            string creationDate = imageFile.CreationTime.ToString("g");

            string resolution = "";
            if (imageFile.Extension.ToLower() == ".jpg" || imageFile.Extension.ToLower() == ".png")
            {
                using (var image = System.Drawing.Image.FromFile(imageFile.FullName))
                {
                    resolution = image.Width + "x" + image.Height;
                }
            }

            // Display image details in a MessageBox
            string message = $"File: {imageFile.Name}\n" +
                                $"Size: {fileSizeInKB}\n" +
                                $"Resolution: {resolution}\n" +
                                $"Created On: {creationDate}";

            MessageBox.Show(message, "Image Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Item1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Option 1 selected!");
        }

        private void Item2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Option 2 selected!");
        }

        private void Item3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Option 3 selected!");
        }


    }
}


