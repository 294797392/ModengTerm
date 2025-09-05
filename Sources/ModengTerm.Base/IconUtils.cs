using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

namespace ModengTerm.Base
{
    public static class IconUtils
    {
        #region WinAPI

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            public char szDisplayName;
            public char szTypeName;
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        // 标志位
        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_LARGEICON = 0x000000000;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;


        #endregion

        private static SHFILEINFO sfi = new SHFILEINFO();
        private static uint cbSizeFileInfo = (uint)Marshal.SizeOf(sfi);
        private static Dictionary<string, BitmapSource> iconCache = new Dictionary<string, BitmapSource>();

        public static void Initialize()
        {
            SHFILEINFO folderInfo = new SHFILEINFO();
            SHGetFileInfo("folder", FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIRECTORY, ref folderInfo, cbSizeFileInfo, SHGFI_ICON | SHGFI_USEFILEATTRIBUTES | SHGFI_SMALLICON);
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(folderInfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmapSource.Freeze();
            DestroyIcon(folderInfo.hIcon);
            iconCache["folder"] = bitmapSource;
        }

        public static BitmapSource GetIcon(string filePath)
        {
            string ext = Path.GetExtension(filePath);

            BitmapSource bitmapSource;
            if (!iconCache.TryGetValue(ext, out bitmapSource))
            {
                lock (iconCache)
                {
                    if (!iconCache.TryGetValue(ext, out bitmapSource))
                    {
                        SHGetFileInfo(ext, FILE_ATTRIBUTE_NORMAL, ref sfi, cbSizeFileInfo, SHGFI_ICON | SHGFI_USEFILEATTRIBUTES | SHGFI_SMALLICON);
                        bitmapSource = Imaging.CreateBitmapSourceFromHIcon(sfi.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        bitmapSource.Freeze();
                        DestroyIcon(sfi.hIcon);
                        iconCache[ext] = bitmapSource;
                    }
                }
            }

            return bitmapSource;
        }

        public static BitmapSource GetFolderIcon()
        {
            return iconCache["folder"];
        }
    }
}
