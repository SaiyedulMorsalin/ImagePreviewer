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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Image_Preview
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

        }
        public static string Filepath = @"C:\Users\vn\Desktop";
        public static string extentions = @".jpg|.png";

        public void DirectoryLoad(string path)
        {
            if (Directory.Exists(path))
            {
                if (!Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(Filepath);
                }

                DirectoryInfo dr = new DirectoryInfo(path);
                FileInfo[] fls = dr.GetFiles();

                string[] exts = extentions.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < fls.Length; i++)
                {
                    for (int j = 0; j < exts.Length; j++)
                    {
                        if (fls[i].Extension.ToLower().Contains(exts[j].ToLower()))
                        {
                         
                            

                          
                            string destinationFile = Path.Combine(Filepath, fls[i].Name);

                            if (!File.Exists(destinationFile))
                            {
                                try
                                {
                                   
                                    File.Copy(fls[i].FullName, destinationFile);
                                    Controls.mybtn btn = new Controls.mybtn();
                                    btn.btn_text = fls[i].Name;
                                    btn.filepath = fls[i];
                                    btn.refreshme();
                                    flowLayoutPanel1.Controls.Add(btn);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Error copying file {fls[i].Name}: {ex.Message}");
                                }
                            }
                            else
                            {
                                
                                Console.WriteLine($"File {fls[i].Name} already exists in the destination directory.");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Source directory does not exist.");
            }
        }

        public void Reload()
        {
            flowLayoutPanel1.Width = this.Width - 20;
            flowLayoutPanel1.Controls.Clear();
            if (Directory.Exists(Filepath))
            {
                DirectoryInfo dr = new DirectoryInfo(Filepath);
                FileInfo[] fls = dr.GetFiles();

                string[] exts = extentions.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < fls.Length; i++)
                {
                    for (int j = 0; j < exts.Length; j++)
                    {
                        if (fls[i].Extension.ToLower().Contains(exts[j].ToLower()))
                        {
                            Controls.mybtn btn = new Controls.mybtn();
                            btn.btn_text = fls[i].Name;
                            btn.filepath = fls[i];
                            btn.refreshme();
                            flowLayoutPanel1.Controls.Add(btn);
                        }
                    }
                }
            }
            else
            {
                string message = "Folder Directory is not Found!";
                MessageBox.Show(message, "Image Details", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public static void ShowImageDetails(string imagePath)
        {
            FileInfo imageFile = new FileInfo(imagePath);


            long fileSize = imageFile.Length;
            string fileSizeInKB = (fileSize / 1024).ToString() + " KB";

            // Get creation date
            string creationDate = imageFile.CreationTime.ToString("g");

            // Get image resolution if applicable
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

        private void button1_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
