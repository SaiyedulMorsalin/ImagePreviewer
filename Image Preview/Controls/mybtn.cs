﻿using System;
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
        public mybtn()
        {
            InitializeComponent();
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

            //if (filepath.Extension.ToUpper() == ".MAX")
            //{
            //    try
            //    {
            //        big_image = GetMaxPreviewBitmapFromFile(filepath.FullName);
            //        button1.BackgroundImage = big_image;
            //        button1.Text = "";

            //        // Set tooltip to show filename and path
            //        toolTip.SetToolTip(button1, $"Filename: {filepath.Name}\nPath: {filepath.FullName}");
            //    }
            //    catch
            //    {
            //        big_image = null;
            //    }
            //}
            //else
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


    }
}


