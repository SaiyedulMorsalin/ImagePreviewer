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

            if (filepath.Extension.ToUpper() == ".MAX")
            {
                try
                {
                    big_image = GetMaxPreviewBitmapFromFile(filepath.FullName);
                    button1.BackgroundImage = big_image;
                    button1.Text = "";

                    // Set tooltip to show filename and path
                    toolTip.SetToolTip(button1, $"Filename: {filepath.Name}\nPath: {filepath.FullName}");
                }
                catch
                {
                    big_image = null;
                }
            }
            else
            {
                big_image = null;
                try
                {
                    button1.BackgroundImage = System.Drawing.Image.FromFile(filepath.FullName);
                    button1.Text = "";

                    toolTip.SetToolTip(button1, $"Filename: {filepath.Name}\nPath: {filepath.FullName}");
                }
                catch
                {

                }
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







        #region-------------------------------------------MAX FILE PREVIEW    shared in   http://forums.cgsociety.org/showthread.php?t=1121870
        [Flags]
        private enum STGM : int
        {
            DIRECT = 0x00000000,
            TRANSACTED = 0x00010000,
            SIMPLE = 0x08000000,
            READ = 0x00000000,
            WRITE = 0x00000001,
            READWRITE = 0x00000002,
            SHARE_DENY_NONE = 0x00000040,
            SHARE_DENY_READ = 0x00000030,
            SHARE_DENY_WRITE = 0x00000020,
            SHARE_EXCLUSIVE = 0x00000010,
            PRIORITY = 0x00040000,
            DELETEONRELEASE = 0x04000000,
            NOSCRATCH = 0x00100000,
            CREATE = 0x00001000,
            CONVERT = 0x00020000,
            FAILIFTHERE = 0x00000000,
            NOSNAPSHOT = 0x00200000,
            DIRECT_SWMR = 0x00400000,
        }

        enum ulKind : uint
        {
            PRSPEC_LPWSTR = 0,
            PRSPEC_PROPID = 1
        }
        enum SumInfoProperty : uint
        {
            PIDSI_TITLE = 0x00000002,
            PIDSI_SUBJECT = 0x00000003,
            PIDSI_AUTHOR = 0x00000004,
            PIDSI_KEYWORDS = 0x00000005,
            PIDSI_COMMENTS = 0x00000006,
            PIDSI_TEMPLATE = 0x00000007,
            PIDSI_LASTAUTHOR = 0x00000008,
            PIDSI_REVNUMBER = 0x00000009,
            PIDSI_EDITTIME = 0x0000000A,
            PIDSI_LASTPRINTED = 0x0000000B,
            PIDSI_CREATE_DTM = 0x0000000C,
            PIDSI_LASTSAVE_DTM = 0x0000000D,
            PIDSI_PAGECOUNT = 0x0000000E,
            PIDSI_WORDCOUNT = 0x0000000F,
            PIDSI_CHARCOUNT = 0x00000010,
            PIDSI_THUMBNAIL = 0x00000011,
            PIDSI_APPNAME = 0x00000012,
            PIDSI_SECURITY = 0x00000013
        }
        private enum VARTYPE : short
        {
            VT_BSTR = 8,
            VT_FILETIME = 0x40,
            VT_LPSTR = 30,
            VT_CF = 71
        }
        [StructLayout(LayoutKind.Explicit)]
        private struct PROPVARIANTunion
        {
            [FieldOffset(0)]
            public sbyte cVal;
            [FieldOffset(0)]
            public byte bVal;
            [FieldOffset(0)]
            public short iVal;
            [FieldOffset(0)]
            public ushort uiVal;
            [FieldOffset(0)]
            public int lVal;
            [FieldOffset(0)]
            public uint ulVal;
            [FieldOffset(0)]
            public int intVal;
            [FieldOffset(0)]
            public uint uintVal;
            [FieldOffset(0)]
            public long hVal;
            [FieldOffset(0)]
            public ulong uhVal;
            [FieldOffset(0)]
            public float fltVal;
            [FieldOffset(0)]
            public double dblVal;
            [FieldOffset(0)]
            public short boolVal;
            [FieldOffset(0)]
            public int scode;
            [FieldOffset(0)]
            public long cyVal;
            [FieldOffset(0)]
            public double date;
            [FieldOffset(0)]
            public long filetime;
            [FieldOffset(0)]
            public IntPtr bstrVal;
            [FieldOffset(0)]
            public IntPtr pszVal;
            [FieldOffset(0)]
            public IntPtr pwszVal;
            [FieldOffset(0)]
            public IntPtr punkVal;
            [FieldOffset(0)]
            public IntPtr pdispVal;
        }
        struct PACKEDMETA
        {
            public ushort mm, xExt, yExt, reserved;
        }
        [DllImport("ole32.dll")]
        static extern int StgOpenStorage(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcsName, IStorage pstgPriority,
            int grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstgOpen);

        [DllImport("ole32.dll")]
        static extern int StgCreatePropSetStg(IStorage pStorage, uint reserved,
            out IPropertySetStorage ppPropSetStg);

        [DllImport("ole32.dll")]
        private extern static int PropVariantClear(ref PROPVARIANT pvar);


        [ComImport]
        [Guid("00000138-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyStorage
        {
            [PreserveSig]
            int ReadMultiple(uint cpspec,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)][In] PropertySpec[] rgpspec,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)][Out] PropertyVariant[] rgpropvar);

            [PreserveSig]
            void WriteMultiple(uint cpspec,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)][In] PropertySpec[] rgpspec,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)][In] PropertyVariant[] rgpropvar,
                uint propidNameFirst);

            [PreserveSig]
            uint DeleteMultiple(uint cpspec,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)][In] PropertySpec[] rgpspec);
            [PreserveSig]
            uint ReadPropertyNames(uint cpropid,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)][In] uint[] rgpropid,
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0)][Out] string[] rglpwstrName);
            [PreserveSig]
            uint NotDeclared1();
            [PreserveSig]
            uint NotDeclared2();
            [PreserveSig]
            uint Commit(uint grfCommitFlags);
            [PreserveSig]
            uint NotDeclared3();
            [PreserveSig]
            uint Enum(out IEnumSTATPROPSTG ppenum);
        }

        [ComImport]
        [Guid("0000013A-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertySetStorage
        {
            [PreserveSig]
            uint Create(ref Guid rfmtid, ref Guid pclsid, uint grfFlags, STGM grfMode, out IPropertyStorage ppprstg);
            [PreserveSig]
            uint Open(ref Guid rfmtid, STGM grfMode, out IPropertyStorage ppprstg);
            [PreserveSig]
            uint NotDeclared3();
            [PreserveSig]
            uint Enum(out IEnumSTATPROPSETSTG ppenum);
        }

        private enum PropertySpecKind
        {
            Lpwstr,
            PropId
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertySpec
        {
            public PropertySpecKind kind;
            public PropertySpecData data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct PropertySpecData
        {
            [FieldOffset(0)]
            public uint propertyId;
            [FieldOffset(0)]
            public IntPtr name;
        }

        private struct PropertyVariant
        {
            public VARTYPE vt;
            public ushort wReserved1;
            public ushort wReserved2;
            public ushort wReserved3;
            public PROPVARIANTunion unionmember;
        }

        //static public Bitmap GetMaxPreviewBitmapFromFile(string path)
        //{
        //    var FMTID_SummaryInformation = new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}");

        //    Bitmap bitmap = null;
        //    IStorage Is;
        //    if (StgOpenStorage(path, null, (int)(STGM.SHARE_EXCLUSIVE | STGM.READWRITE), IntPtr.Zero, 0, out Is) == 0 && Is != null)
        //    {
        //        IPropertySetStorage pss;
        //        if (StgCreatePropSetStg(Is, 0, out pss) == 0)
        //        {
        //            IPropertyStorage ps;
        //            pss.Open(ref FMTID_SummaryInformation, (STGM.SHARE_EXCLUSIVE | STGM.READ), out ps);
        //            if (ps != null)
        //            {
        //                var propSpec = new PropertySpec[1];
        //                var propVariant = new PropertyVariant[1];

        //                propSpec[0].kind = PropertySpecKind.PropId;
        //                propSpec[0].data.propertyId = (uint)SumInfoProperty.PIDSI_THUMBNAIL;

        //                System.UInt32 n = 1;
        //                ps.ReadMultiple(n, propSpec, propVariant);

        //                var clipData =
        //                    (CLIPDATA)Marshal.PtrToStructure(propVariant[0].unionmember.pszVal, typeof(CLIPDATA));

        //                var pb = clipData.pClipData;
        //                pb += sizeof(uint);

        //                var packedMeta = (PACKEDMETA)Marshal.PtrToStructure(pb, typeof(PACKEDMETA));
        //                pb += Marshal.SizeOf(packedMeta);

        //                var magicNumber = 3 * 29;
        //                pb += magicNumber;

        //                var pformat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
        //                int bitsPerPixel = ((int)pformat & 0xff00) >> 8;
        //                int bytesPerPixel = (bitsPerPixel + 7) / 8;
        //                int stride = 4 * ((packedMeta.xExt * bytesPerPixel + 3) / 4);


        //                unsafe
        //                {
        //                    byte* ptr = (byte*)pb;
        //                    for (int y = 0; y < packedMeta.yExt; y++)
        //                        for (int x = 0; x < packedMeta.xExt; x++)
        //                        {
        //                            var i = (x * 3) + y * stride;

        //                            var r = ptr[i];
        //                            var g = ptr[i + 1];
        //                            var b = ptr[i + 2];

        //                            ptr[i] = b;
        //                            ptr[i + 1] = r;
        //                            ptr[i + 2] = g;

        //                        }

        //                    bitmap = new Bitmap(packedMeta.xExt, packedMeta.yExt, stride, pformat, (IntPtr)ptr);

        //                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
        //                }

        //                //PropVariantClear(ref propVariant[0]);
        //                Marshal.FinalReleaseComObject(ps);
        //                ps = null;
        //            }
        //            else
        //            {
        //                Console.WriteLine("Could not open property storage");
        //            }

        //            Marshal.FinalReleaseComObject(pss);
        //            pss = null;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Could not create property set storage");
        //        }

        //        Marshal.FinalReleaseComObject(Is);
        //        Is = null;
        //    }
        //    else
        //    {
        //        Console.WriteLine("File does not contain a structured storage");
        //    }

        //    GC.Collect();
        //    //

        //    return bitmap;
        //}
        #endregion
        static public Bitmap GetMaxPreviewBitmapFromFile(string path)
        {
            try
            {
                var FMTID_SummaryInformation = new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}");

                Bitmap bitmap = null;
                IStorage Is;
                if (StgOpenStorage(path, null, (int)(STGM.SHARE_EXCLUSIVE | STGM.READWRITE), IntPtr.Zero, 0, out Is) == 0 && Is != null)
                {
                    IPropertySetStorage pss;
                    if (StgCreatePropSetStg(Is, 0, out pss) == 0)
                    {
                        IPropertyStorage ps;
                        pss.Open(ref FMTID_SummaryInformation, (STGM.SHARE_EXCLUSIVE | STGM.READ), out ps);
                        if (ps != null)
                        {
                            var propSpec = new PropertySpec[1];
                            var propVariant = new PropertyVariant[1];

                            propSpec[0].kind = PropertySpecKind.PropId;
                            propSpec[0].data.propertyId = (uint)SumInfoProperty.PIDSI_THUMBNAIL;

                            System.UInt32 n = 1;
                            ps.ReadMultiple(n, propSpec, propVariant);

                            if (propVariant[0].unionmember.pszVal != IntPtr.Zero)
                            {
                                var clipData =
                                    (CLIPDATA)Marshal.PtrToStructure(propVariant[0].unionmember.pszVal, typeof(CLIPDATA));

                                var pb = clipData.pClipData;
                                pb += sizeof(uint);

                                var packedMeta = (PACKEDMETA)Marshal.PtrToStructure((IntPtr)pb, typeof(PACKEDMETA));
                                pb += Marshal.SizeOf(packedMeta);

                                var magicNumber = 3 * 29;
                                pb += magicNumber;

                                var pformat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                                int bitsPerPixel = ((int)pformat & 0xff00) >> 8;
                                int bytesPerPixel = (bitsPerPixel + 7) / 8;
                                int stride = 4 * ((packedMeta.xExt * bytesPerPixel + 3) / 4);

                                if (packedMeta.mm == 3 && packedMeta.xExt > 0 && packedMeta.yExt > 0)
                                {
                                    unsafe
                                    {
                                        byte* ptr = (byte*)pb;
                                        for (int y = 0; y < packedMeta.yExt; y++)
                                            for (int x = 0; x < packedMeta.xExt; x++)
                                            {
                                                var i = (x * 3) + y * stride;

                                                var r = ptr[i];
                                                var g = ptr[i + 1];
                                                var b = ptr[i + 2];

                                                ptr[i] = b;
                                                ptr[i + 1] = r;
                                                ptr[i + 2] = g;

                                            }

                                        bitmap = new Bitmap(packedMeta.xExt, packedMeta.yExt, stride, pformat, (IntPtr)ptr);

                                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                                    }
                                }

                                PropVariantClear(ref propVariant[0]);
                                Marshal.FinalReleaseComObject(ps);
                                ps = null;
                            }
                            else
                            {
                                Console.WriteLine("Thumbnail data is null");
                            }

                            Marshal.FinalReleaseComObject(pss);
                            pss = null;
                        }
                        else
                        {
                            Console.WriteLine("Could not open property storage");
                        }

                        Marshal.FinalReleaseComObject(Is);
                        Is = null;
                    }
                    else
                    {
                        Console.WriteLine("Could not create property set storage");
                    }
                }
                else
                {
                    Console.WriteLine("File does not contain a structured storage");
                }

                GC.Collect();

                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }

        private static void PropVariantClear(ref PropertyVariant propertyVariant)
        {
            throw new NotImplementedException();
        }

        private class PROPVARIANT
        {
        }
    }

    internal class CLIPDATA
    {
        public int pClipData { get; internal set; }
    }

    public interface IEnumSTATPROPSETSTG
    {
    }

    public interface IEnumSTATPROPSTG
    {
    }

    public interface IStorage
    {
    }
}
//using System;
//using System.Drawing;
//using System.IO;
//using System.Windows.Forms;

//namespace Image_Preview.Controls
//{
//    public partial class MyButton : UserControl
//    {
//        // Public properties to set text and file path externally
//        public string ButtonText { get; set; }
//        public FileInfo FilePath { get; set; }

//        public MyButton()
//        {
//            InitializeComponent();
//        }

//        // Method to refresh the button's text and load image
//        public void RefreshMe()
//        {
//            button1.Text = ButtonText;
//            LoadImage();
//        }

//        // Event handler for button click, copies file path to clipboard
//        private void Button1_Click(object sender, EventArgs e)
//        {
//            if (FilePath != null)
//            {
//                Clipboard.SetText(FilePath.FullName);
//            }
//        }

//        // Method to load image previews and set tooltips
//        private void LoadImage()
//        {
//            if (FilePath == null) return;

//            Bitmap image = null;
//            ToolTip toolTip = new ToolTip();

//            // Handle .MAX files or regular images
//            try
//            {
//                if (FilePath.Extension.ToUpper() == ".MAX")
//                {
//                    image = GetMaxPreviewBitmapFromFile(FilePath.FullName);
//                }
//                else
//                {
//                    image = new Bitmap(FilePath.FullName);
//                }

//                // Set image as background and remove text
//                button1.BackgroundImage = image;
//                button1.Text = string.Empty;

//                // Set tooltip to display filename and path
//                toolTip.SetToolTip(button1, $"Filename: {FilePath.Name}\nPath: {FilePath.FullName}");
//            }
//            catch (Exception ex)
//            {
//                // Handle image loading exceptions
//                button1.BackgroundImage = null;
//                button1.Text = ButtonText; // Show text if image loading fails
//                toolTip.SetToolTip(button1, $"Failed to load image: {ex.Message}");
//            }

//            // Assign click event for showing image details
//            button1.Click += Button_ShowImageDetails;
//        }

//        // Show image details in a MessageBox
//        private void Button_ShowImageDetails(object sender, EventArgs e)
//        {
//            if (FilePath != null)
//            {
//                ShowImageDetails(FilePath.FullName);
//            }
//        }

//        // Displays details of the image such as resolution, file size, etc.
//        private void ShowImageDetails(string imagePath)
//        {
//            var imageFile = new FileInfo(imagePath);

//            long fileSize = imageFile.Length;
//            string fileSizeInKB = (fileSize / 1024).ToString() + " KB";
//            string creationDate = imageFile.CreationTime.ToString("g");
//            string resolution = string.Empty;

//            // Fetch resolution for common image types
//            if (imageFile.Extension.ToLower() == ".jpg" || imageFile.Extension.ToLower() == ".png")
//            {
//                using (var image = Image.FromFile(imageFile.FullName))
//                {
//                    resolution = $"{image.Width}x{image.Height}";
//                }
//            }

//            // Display image details
//            string message = $"File: {imageFile.Name}\n" +
//                             $"Size: {fileSizeInKB}\n" +
//                             $"Resolution: {resolution}\n" +
//                             $"Created On: {creationDate}";

//            MessageBox.Show(message, "Image Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        // Mock method to handle .MAX file preview extraction (implementation required)
//        private Bitmap GetMaxPreviewBitmapFromFile(string path)
//        {
//            // Placeholder logic for loading a preview from a .MAX file
//            // Actual implementation may involve handling structured storage and custom formats
//            // Return null if not supported
//            return null;
//        }
//    }
//}
