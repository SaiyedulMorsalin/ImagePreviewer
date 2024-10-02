using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Image_Preview.Controls
{
    public partial class MyButton : UserControl
    {
        public string BtnText { get; set; } = "";
        public FileInfo FilePath { get; set; }

        public MyButton()
        {
            InitializeComponent();
        }

        public void RefreshMe()
        {
            button1.Text = BtnText;
            LoadImages();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (FilePath != null)
            {
                Clipboard.SetText(FilePath.FullName);
            }
        }

        private void LoadImages()
        {
            Bitmap bigImage = null;
            ToolTip toolTip = new ToolTip();

            try
            {
                if (FilePath != null)
                {
                    if (FilePath.Extension.ToUpper() == ".MAX")
                    {
                        bigImage = GetMaxPreviewBitmapFromFile(FilePath.FullName);
                    }
                    else
                    {
                        bigImage = (Bitmap)Image.FromFile(FilePath.FullName);
                    }

                    button1.BackgroundImage = bigImage;
                    button1.Text = "";
                    toolTip.SetToolTip(button1, $"Filename: {FilePath.Name}\nPath: {FilePath.FullName}");
                }
            }
            catch
            {
                // Log or handle exception if needed
            }

            button1.Click += Button3_Click;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ShowImageDetails(FilePath.FullName);
        }

        private void ShowImageDetails(string imagePath)
        {
            var imageFile = new FileInfo(imagePath);

            if (!imageFile.Exists) return;

            string fileSizeInKB = (imageFile.Length / 1024).ToString() + " KB";
            string creationDate = imageFile.CreationTime.ToString("g");

            string resolution = "";
            if (imageFile.Extension.ToLower() is ".jpg" or ".png")
            {
                using (var image = Image.FromFile(imageFile.FullName))
                {
                    resolution = $"{image.Width}x{image.Height}";
                }
            }

            string message = $"File: {imageFile.Name}\n" +
                             $"Size: {fileSizeInKB}\n" +
                             $"Resolution: {resolution}\n" +
                             $"Created On: {creationDate}";

            MessageBox.Show(message, "Image Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region MAX File Preview
        // Related to .MAX file preview, taken from the reference forum thread
        [Flags]
        private enum STGM
        {
            DIRECT = 0x00000000,
            READWRITE = 0x00000002,
            SHARE_EXCLUSIVE = 0x00000010,
            READ = 0x00000000
        }

        [DllImport("ole32.dll")]
        static extern int StgOpenStorage(string pwcsName, IStorage pstgPriority, int grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstgOpen);

        [DllImport("ole32.dll")]
        static extern int StgCreatePropSetStg(IStorage pStorage, uint reserved, out IPropertySetStorage ppPropSetStg);

        static public Bitmap GetMaxPreviewBitmapFromFile(string path)
        {
            try
            {
                var FMTID_SummaryInformation = new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}");
                Bitmap bitmap = null;
                if (StgOpenStorage(path, null, (int)(STGM.SHARE_EXCLUSIVE | STGM.READWRITE), IntPtr.Zero, 0, out var storage) == 0 && storage != null)
                {
                    if (StgCreatePropSetStg(storage, 0, out var propSetStorage) == 0)
                    {
                        if (propSetStorage.Open(ref FMTID_SummaryInformation, STGM.SHARE_EXCLUSIVE | STGM.READ, out var propStorage) == 0)
                        {
                            var propSpec = new PropertySpec[1];
                            var propVariant = new PropertyVariant[1];

                            propSpec[0].Kind = PropertySpecKind.PropId;
                            propSpec[0].Data.PropertyId = (uint)SumInfoProperty.PIDSI_THUMBNAIL;

                            propStorage.ReadMultiple(1, propSpec, propVariant);

                            if (propVariant[0].UnionMember.PszVal != IntPtr.Zero)
                            {
                                var clipData = Marshal.PtrToStructure<CLIPDATA>(propVariant[0].UnionMember.PszVal);
                                var pb = clipData.pClipData + sizeof(uint);
                                var packedMeta = Marshal.PtrToStructure<PACKEDMETA>(pb);
                                pb += Marshal.SizeOf<PACKEDMETA>();
                                pb += 3 * 29;

                                var pformat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                                int stride = 4 * ((packedMeta.xExt * 3 + 3) / 4);

                                unsafe
                                {
                                    byte* ptr = (byte*)pb;
                                    for (int y = 0; y < packedMeta.yExt; y++)
                                    {
                                        for (int x = 0; x < packedMeta.xExt; x++)
                                        {
                                            var i = (x * 3) + y * stride;
                                            var temp = ptr[i];
                                            ptr[i] = ptr[i + 2];
                                            ptr[i + 2] = temp;
                                        }
                                    }

                                    bitmap = new Bitmap(packedMeta.xExt, packedMeta.yExt, stride, pformat, (IntPtr)ptr);
                                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                                }

                                PropVariantClear(ref propVariant[0]);
                            }

                            Marshal.ReleaseComObject(propStorage);
                        }

                        Marshal.ReleaseComObject(propSetStorage);
                    }

                    Marshal.ReleaseComObject(storage);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        private static void PropVariantClear(ref PropertyVariant propertyVariant) { }
        #endregion
    }

    // Necessary structures and interfaces for handling .MAX file previews
    internal class CLIPDATA
    {
        public int pClipData { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PACKEDMETA
    {
        public ushort mm, xExt, yExt, reserved;
    }

    public interface IStorage { }
    public interface IPropertySetStorage
    {
        int Open(ref Guid rfmtid, STGM grfMode, out IPropertyStorage ppprstg);
    }

    public interface IPropertyStorage
    {
        int ReadMultiple(uint cpspec, PropertySpec[] rgpspec, PropertyVariant[] rgpropvar);
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct PROPVARIANTunion
    {
        [FieldOffset(0)] public IntPtr PszVal;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct PropertyVariant
    {
        [FieldOffset(0)] public VARTYPE Vt;
        [FieldOffset(16)] public PROPVARIANTunion UnionMember;
    }

    public struct PropertySpec
    {
        public PropertySpecKind Kind;
        public PropertySpecData Data;
    }

    public struct PropertySpecData
    {
        public uint PropertyId;
    }

    public enum PropertySpecKind
    {
        PropId
    }

    public enum SumInfoProperty : uint
    {
        PIDSI_THUMBNAIL = 0x00000011
    }

    public enum VARTYPE : short
    {
        VT_BSTR = 8
    }
}
