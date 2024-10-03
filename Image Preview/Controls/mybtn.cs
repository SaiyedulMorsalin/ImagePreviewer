using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Preview.Controls
{
    public partial class mybtn : UserControl
    {
        private ContextMenuStrip contextMenuStrip;
        private static Dictionary<string, Image> imageCache = new Dictionary<string, Image>(); // Image cache

        public string btn_text = "";
        public FileInfo filepath = null;

        // Constructor
        public mybtn()
        {
            InitializeComponent();
            InitializeContextMenu();
        }

        // Initialize the context menu
        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem item1 = new ToolStripMenuItem("Option 1", null, Item1_Click);
            ToolStripMenuItem item2 = new ToolStripMenuItem("Option 2", null, Item2_Click);
            ToolStripMenuItem item3 = new ToolStripMenuItem("Delete", null, (sender, e) => Item3_Click(filepath.FullName));

            contextMenuStrip.Items.AddRange(new ToolStripItem[] { item1, item2, item3 });
            this.ContextMenuStrip = contextMenuStrip;
        }

        // Refresh button text and load image
        public void RefreshControl()
        {
            button1.Text = btn_text;
            LoadImages();
        }

        // Handle button click to copy the file path to the clipboard
        private void button1_Click(object sender, EventArgs e)
        {
            if (filepath != null)
            {
                Clipboard.SetText(filepath.FullName);
            }
        }

        // Asynchronously load images and cache them
        private async void LoadImages()
        {
            ToolTip toolTip = new ToolTip();

            try
            {
                // Load image and cache it if not already cached
                if (!imageCache.ContainsKey(filepath.FullName))
                {
                    await Task.Run(() =>
                    {
                        using (var img = Image.FromFile(filepath.FullName))
                        {
                            var thumbnail = img.GetThumbnailImage(175, 175, null, IntPtr.Zero);
                            imageCache[filepath.FullName] = new Bitmap(thumbnail);
                        }
                    });
                }

                // Set the image from the cache and configure the button
                button1.BackgroundImage = imageCache[filepath.FullName];
                button1.Text = ""; // Clear text once the image is loaded
                toolTip.SetToolTip(button1, $"Filename: {filepath.Name}\nPath: {filepath.FullName}");
            }
            catch (Exception ex)
            {
                button1.BackgroundImage = null;
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            button1.Click += button3_Click;
        }

        // Show image details when button is clicked
        private void button3_Click(object sender, EventArgs e)
        {
            if (filepath != null)
            {
                ShowImageDetails(filepath.FullName);
            }
        }

        // Show image details in a MessageBox
        private void ShowImageDetails(string imagePath)
        {
            try
            {
                FileInfo imageFile = new FileInfo(imagePath);
                string fileSizeInKB = (imageFile.Length / 1024).ToString() + " KB";
                string creationDate = imageFile.CreationTime.ToString("g");

                string resolution = GetImageResolution(imageFile);

                string message = $"File: {imageFile.Name}\n" +
                                 $"Size: {fileSizeInKB}\n" +
                                 $"Resolution: {resolution}\n" +
                                 $"Created On: {creationDate}";

                MessageBox.Show(message, "Image Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying image details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Get image resolution
        private string GetImageResolution(FileInfo imageFile)
        {
            if (imageFile.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                imageFile.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
            {
                using (var image = Image.FromFile(imageFile.FullName))
                {
                    return $"{image.Width}x{image.Height}";
                }
            }
            return "Unknown";
        }

        // Option 1 selected event
        private void Item1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Option 1 selected!");
        }

        // Option 2 selected event
        private void Item2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Option 2 selected!");
        }

        // Delete the file and reload the panel
        private void Item3_Click(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    // Remove the image from cache
                    if (imageCache.ContainsKey(filePath))
                    {
                        imageCache.Remove(filePath);
                    }

                    // Reload the control to reflect the deletion
                    ReloadControl();

                    MessageBox.Show($"{filePath} deleted successfully.");
                }
                else
                {
                    MessageBox.Show("File not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Reload the control (you should adjust how Reload works as per your application needs)
        private void ReloadControl()
        {
            // You would implement the actual reload logic here
            UserControl1 instance = new UserControl1();
            instance.Reload();
        }

        // Clear the image cache and free memory
        public static void ClearCache()
        {
            foreach (var image in imageCache.Values)
            {
                image.Dispose(); // Dispose images to release resources
            }

            imageCache.Clear(); // Clear the dictionary

            MessageBox.Show("All image cache has been cleared successfully.", "Cache Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
