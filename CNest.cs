// Includes
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CNestLib
{
    public static class CNest
    {
        // Imports
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static int GWL_EXSTYLE = -20;
        private static int WS_EX_LAYERED = 0x80000;
        private static int WS_EX_TRANSPARENT = 0x20;

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        private static int WM_NCLBUTTONDOWN = 0xA1;
        private static int HT_CAPTION = 0x2;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        [DllImport("user32.dll")]
        private static extern ushort GetAsyncKeyState(int vKey);

        [DllImport("kernel32.dll")]
        public static extern long GetVolumeInformation(
        string PathName,
        StringBuilder VolumeNameBuffer,
        UInt32 VolumeNameSize,
        ref UInt32 VolumeSerialNumber,
        ref UInt32 MaximumComponentLength,
        ref UInt32 FileSystemFlags,
        StringBuilder FileSystemNameBuffer,
        UInt32 FileSystemNameSize);

        // Code

        // Control
        /** <summary>CNest Set radius of Control</summary> **/
        public static Control Radius(this Control ctrl, int radiusX, int radiusY)
        {
            ctrl.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, ctrl.Width, ctrl.Height, radiusX, radiusY));

            return ctrl;
        }

        /** <summary>CNest Set radius of Control</summary> **/
        public static Control Radius(this Control ctrl, Point radiusCoords)
        {
            return ctrl.Radius(radiusCoords.X, radiusCoords.Y);
        }

        /** <summary>CNest Set radius of Control</summary> **/
        public static Control Radius(this Control ctrl, int radius)
        {
            return ctrl.Radius(radius, radius);
        }

        /** <summary>CNest Set position of Control using "Location"</summary> **/
        public static Control Position(this Control ctrl, int x, int y)
        {
            ctrl.Location = new Point(x, y);

            return ctrl;
        }

        /** <summary>CNest Set position of Control using "Location"</summary> **/
        public static Control Position(this Control ctrl, Point coords)
        {
            return ctrl.Position(coords.X, coords.Y);
        }

        /** <summary>CNest Set position X of Control using "Location"</summary> **/
        public static Control PositionX(this Control ctrl, int x)
        {
            ctrl.Location = new Point(x, ctrl.Location.Y);

            return ctrl;
        }

        /** <summary>CNest Set position Y of Control using "Location"</summary> **/
        public static Control PositionY(this Control ctrl, int y)
        {
            ctrl.Location = new Point(ctrl.Location.X, y);

            return ctrl;
        }

        /** <summary>CNest Get position of Control using "Location"</summary> **/
        public static (int X, int Y, int Left, int Top, int Right, int Bottom) Position(this Control ctrl)
        {
            return (ctrl.Location.X, ctrl.Location.Y, ctrl.Location.X, ctrl.Location.Y, ctrl.Location.X + ctrl.Width, ctrl.Location.Y + ctrl.Height);
        }

        /** <summary>CNest Set width of Control</summary> **/
        public static Control Width(this Control ctrl, int width)
        {
            ctrl.Width = width;

            return ctrl;
        }

        /** <summary>CNest Get width of Control</summary> **/
        public static int Width(this Control ctrl)
        {
            return ctrl.Width;
        }

        /** <summary>CNest Get outer width of Control using "margin"</summary> **/
        public static int OuterWidth(this Control ctrl)
        {
            return ctrl.Width + ctrl.Margin.Left + ctrl.Margin.Right;
        }

        /** <summary>CNest Get inner width of Control using "padding"</summary> **/
        public static int InnerWidth(this Control ctrl)
        {
            return ctrl.Width - ctrl.Padding.Left - ctrl.Padding.Right;
        }

        /** <summary>CNest Set height of Control</summary> **/
        public static Control Height(this Control ctrl, int height)
        {
            ctrl.Height = height;
            return ctrl;
        }

        /** <summary>CNest Get height of Control</summary> **/
        public static int Height(this Control ctrl)
        {
            return ctrl.Height;
        }

        /** <summary>CNest Get outer height of Control using "margin"</summary> **/
        public static int OuterHeight(this Control ctrl)
        {
            return ctrl.Height + ctrl.Margin.Top + ctrl.Margin.Bottom;
        }

        /** <summary>CNest Get inner height of Control using "padding"</summary> **/
        public static int InnerHeight(this Control ctrl)
        {
            return ctrl.Height - ctrl.Padding.Top - ctrl.Padding.Bottom;
        }

        /** <summary>CNest Set background color of Control</summary> **/
        public static Control BGColor(this Control ctrl, int r, int g, int b, int a = 255)
        {
            ctrl.BackColor = Color.FromArgb(a, r, g, b);

            return ctrl;
        }

        /** <summary>CNest Set background color of Control</summary> **/
        public static Control BGColor(this Control ctrl, Color color)
        {
            return ctrl.BGColor(color.A, color.R, color.G, color.B);
        }

        /** <summary>CNest Set background color of Control</summary> **/
        public static Control BGColor(this Control ctrl, int c, int m, int y, int k, int a = 255)
        {
            int r, g, b;

            r = Convert.ToInt32(255 * (1 - c) * (1 - k));
            g = Convert.ToInt32(255 * (1 - m) * (1 - k));
            b = Convert.ToInt32(255 * (1 - y) * (1 - k));

            return ctrl.BGColor(r, g, b, a);
        }

        /** <summary>CNest Set background color of Control</summary> **/
        public static Control BGColor(this Control ctrl, string hex)
        {
            if (hex.IndexOf('#') != -1)
                hex = hex.Replace("#", "");

            int r, g, b = 0;

            r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            return ctrl.BGColor(r, g, b);
        }

        /** <summary>CNest Get background color of Control</summary> **/
        public static Color BGColor(this Control ctrl)
        {
            return ctrl.BackColor;
        }

        /** <summary>CNest Set foreground color of Control</summary> **/
        public static Control FGColor(this Control ctrl, int r, int g, int b, int a = 255)
        {
            ctrl.ForeColor = Color.FromArgb(a, r, g, b);

            return ctrl;
        }

        /** <summary>CNest Set foreground color of Control</summary> **/
        public static Control FGColor(this Control ctrl, Color color)
        {
            return ctrl.FGColor(color.A, color.R, color.G, color.B);
        }

        /** <summary>CNest Set foreground color of Control</summary> **/
        public static Control FGColor(this Control ctrl, int c, int m, int y, int k, int a = 255)
        {
            int r, g, b;

            r = Convert.ToInt32(255 * (1 - c) * (1 - k));
            g = Convert.ToInt32(255 * (1 - m) * (1 - k));
            b = Convert.ToInt32(255 * (1 - y) * (1 - k));

            return ctrl.FGColor(r, g, b, a);
        }

        /** <summary>CNest Set foreground color of Control</summary> **/
        public static Control FGColor(this Control ctrl, string hex)
        {
            if (hex.IndexOf('#') != -1)
                hex = hex.Replace("#", "");

            int r, g, b = 0;

            r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            return ctrl.FGColor(r, g, b);
        }

        /** <summary>CNest Get foreground color of Control</summary> **/
        public static Color FGColor(this Control ctrl)
        {
            return ctrl.ForeColor;
        }

        /** <summary>CNest Set text of Control</summary> **/
        public static Control Text(this Control ctrl, string text)
        {
            ctrl.Text = text;

            return ctrl;
        }

        /** <summary>CNest Get text of Control</summary> **/
        public static string Text(this Control ctrl)
        {
            return ctrl.Text;
        }

        /** <summary>CNest Set visibility of Control</summary> **/
        public static Control Visible(this Control ctrl, bool toggle)
        {
            ctrl.Visible = toggle;

            return ctrl;
        }

        /** <summary>CNest Get visibility of Control</summary> **/
        public static bool Visible(this Control ctrl)
        {
            return ctrl.Visible;
        }

        /** <summary>CNest Set enabled state of Control</summary> **/
        public static Control Enabled(this Control ctrl, bool toggle)
        {
            ctrl.Enabled = toggle;

            return ctrl;
        }

        /** <summary>CNest Get enabled state of Control</summary> **/
        public static bool Enabled(this Control ctrl)
        {
            return ctrl.Enabled;
        }

        /** <summary>CNest Set background image of Control</summary> **/
        public static Control Img(this Control ctrl, Image img)
        {
            ctrl.BackgroundImage = img;

            return ctrl;
        }

        /** <summary>CNest Get background image of Control</summary> **/
        public static Image Img(this Control ctrl)
        {
            return ctrl.BackgroundImage;
        }

        /** <summary>CNest Makes the current window drag & movable</summary> **/
        public static Control MoveWindow(this Control ctrl)
        {
            ReleaseCapture();
            SendMessage(ctrl.FindForm().Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);

            return ctrl;
        }

        // Form
        /** <summary>CNest Convert form to layer</summary> **/
        public static Form ToLayer(this Form frm)
        {
            var style = GetWindowLong(frm.Handle, GWL_EXSTYLE);

            SetWindowLong(frm.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            return frm;
        }

        /** <summary>CNest Set windows state of Form</summary> **/
        public static Form WindowState(this Form frm, FormWindowState state)
        {
            frm.WindowState = state;

            return frm;
        }

        /** <summary>CNest Get windows state of Form</summary> **/
        public static FormWindowState WindowState(this Form frm)
        {
            return frm.WindowState;
        }

        /** <summary>CNest Get windows state of Form</summary> **/
        public static string WindowStateString(this Form frm)
        {
            switch (frm.WindowState)
            {
                case FormWindowState.Minimized:
                    return "Minimized";
                case FormWindowState.Normal:
                    return "Normal";
                case FormWindowState.Maximized:
                    return "Maximized";
                default:
                    return null;
            }
        }

        // Text
        /** <summary>CNest Converts a string to Base64</summary> **/
        public static String EncodeBase64(this String str)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes(str);
            return Convert.ToBase64String(data);
        }

        /** <summary>CNest Converts a string to Base64</summary> **/
        public static String DecodeBase64(this String str)
        {
            byte[] data = Convert.FromBase64String(str);
            return ASCIIEncoding.ASCII.GetString(data);
        }

        /** <summary>CNest Creates a MD5 hash of a string</summary> **/
        public static (String String, byte[] Bytes) ToMD5(this String str)
        {
            MD5 md5 = MD5.Create();

            byte[] data = Encoding.ASCII.GetBytes(str);
            byte[] hash = md5.ComputeHash(data);

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return (stringBuilder.ToString(), hash);
        }

        /** <summary>CNest Creates a SHA1 hash of a string</summary> **/
        public static (String String, byte[] Bytes) ToSHA1(this String str)
        {
            SHA1 sha1 = SHA1.Create();

            byte[] data = Encoding.ASCII.GetBytes(str);
            byte[] hash = sha1.ComputeHash(data);

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return (stringBuilder.ToString(), hash);
        }

        /** <summary>CNest Creates a SHA256 hash of a string</summary> **/
        public static (String String, byte[] Bytes) ToSHA256(this String str)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] data = Encoding.ASCII.GetBytes(str);
            byte[] hash = sha256.ComputeHash(data);

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return (stringBuilder.ToString(), hash);
        }

        /** <summary>CNest Creates a SHA384 hash of a string</summary> **/
        public static (String String, byte[] Bytes) ToSHA384(this String str)
        {
            SHA384 sha384 = SHA384.Create();

            byte[] data = Encoding.ASCII.GetBytes(str);
            byte[] hash = sha384.ComputeHash(data);

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return (stringBuilder.ToString(), hash);
        }

        /** <summary>CNest Creates a SHA512 hash of a string</summary> **/
        public static (String String, byte[] Bytes) ToSHA512(this String str)
        {
            SHA512 sha512 = SHA512.Create();

            byte[] data = Encoding.ASCII.GetBytes(str);
            byte[] hash = sha512.ComputeHash(data);

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return (stringBuilder.ToString(), hash);
        }

        /** <summary>CNest Builds important navigation symbols</summary> **/
        public static (string Close, string CloseBig, string Maximize, string Normalize, string Minimize) GetSymbol()
        {
            return ("\u00D7", "\u2715", "\uD83D\uDDD6", "\uD83D\uDDD7", "\uD83D\uDDD5");
        }

        /** <summary>CNest Use CMD enviroment variables</summary> **/
        public static string PathVar(string shortcut = "this")
        {
            shortcut = shortcut.ToLower().Replace("%", "").Replace(" ", "").Replace("-", "").Replace("_", "");

            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            if (shortcut == "this")
            {
                path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            }
            else if (shortcut == "appdata")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            }
            else if (shortcut == "programdata")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            }
            else if (shortcut == "local")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            }
            else if (shortcut == "personal")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            }
            else if (shortcut == "user")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            }
            else if (shortcut == "documents")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }
            else if (shortcut == "music")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
            }
            else if (shortcut == "pictures")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
            }
            else if (shortcut == "videos")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos);
            }
            else if (shortcut == "programfiles")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
            }
            else if (shortcut == "programfiles86" || shortcut == "programfilesx86" || shortcut == "programfiles32" || shortcut == "programfilesx32")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86);
            }
            else if (shortcut == "admintools")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.AdminTools);
            }
            else if (shortcut == "burn" || shortcut == "cdburning")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CDBurning);
            }
            else if (shortcut == "desktopdirectory")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            }
            else if (shortcut == "programs")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Programs);
            }
            else if (shortcut == "startmenu")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu);
            }
            else if (shortcut == "startup")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);
            }
            else if (shortcut == "sendto")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.SendTo);
            }
            else if (shortcut == "templates")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Templates);
            }
            else if (shortcut == "cookies")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Cookies);
            }
            else if (shortcut == "desktop")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            }
            else if (shortcut == "favorits")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Favorites);
            }
            else if (shortcut == "fonts")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts);
            }
            else if (shortcut == "history")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.History);
            }
            else if (shortcut == "internetcache")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.InternetCache);
            }
            else if (shortcut == "computer")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyComputer);
            }
            else if (shortcut == "networkshortcuts")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.NetworkShortcuts);
            }
            else if (shortcut == "recent")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Recent);
            }
            else if (shortcut == "resources")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Resources);
            }
            else if (shortcut == "system")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System);
            }
            else if (shortcut == "system86" || shortcut == "systemx86" || shortcut == "system32" || shortcut == "systemx32")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.SystemX86);
            }
            else if (shortcut == "windows")
            {
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows);
            }

            return path;
        }

        // Numbers
        /** <summary>CNest Returns a random generated number between min and max</summary> **/
        public static int Rand(int min, int max)
        {
            Random rand = new Random();

            return rand.Next(min, max);
        }

        // Color
        /** <summary>CNest Get color of pixel at coords</summary> **/
        public static Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);

            uint pixel = GetPixel(hdc, x, y);

            ReleaseDC(IntPtr.Zero, hdc);

            return Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel & 0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);
        }

        /** <summary>CNest Get color of pixel at coords</summary> **/
        public static Color GetPixelColor(Point coords)
        {
            return GetPixelColor(coords.X, coords.Y);
        }

        public class RGBColor
        {
            private int r = 255;
            private int rStep = 1;
            private int g = 50;
            private int gStep = 1;
            private int b = 0;
            private int bStep = 1;

            /** <summary>CNest Returns a smooth RGB color change. Recommend: Usage with timer. Every call = a new color</summary> **/
            public Color Get()
            {
                if (r >= 254)
                {
                    rStep = -1;
                }
                else if (r <= 1)
                {
                    rStep = 1;
                }
                if (g >= 254)
                {
                    gStep = -1;
                }
                else if (g <= 1)
                {
                    gStep = 1;
                }
                if (b >= 254)
                {
                    bStep = -1;
                }
                else if (b <= 1)
                {
                    bStep = 1;
                }

                r += rStep;
                g += gStep;
                b += bStep;

                return System.Drawing.Color.FromArgb(r, g, b);
            }
        }
        
        /** <summary>CNest Returns a random generated RGB color</summary> **/
        public static Color RandomColor()
        {
            Random rand = new Random();

            return Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
        }

        /** <summary>CNest Util: Returns the invertation of the given color</summary> **/
        public static Color Invert(this Color col)
        {
            return Color.FromArgb(col.ToArgb() ^ 0xffffff);
        }

        // Array
        /** <summary>CNest Resize and push into the given array (UNTESTED)</summary> **/
        public static void PushArray<T>(ref T[] array, ref T value)
        {
            System.Array.Resize(ref array, array.Length + 1);
            array[array.GetUpperBound(0)] = value;
        }

        // Console
        /** <summary>CNest Attach / Detach a Console to your program</summary> **/
        public static bool Console(bool show)
        {
            if (show)
            {
                return AllocConsole();
            }
            else
            {
                return FreeConsole();
            }
        }

        // Bitmap
        /** <summary>CNest Creates a screenshot of the given screen</summary> **/
        public static Bitmap DoScreenshot(this Screen screen)
        {
            Bitmap bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics screenshotArea = Graphics.FromImage(bitmap);

            screenshotArea.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

            return bitmap;
        }

        // KeyState
        /** <summary>CNest A simple async key state check</summary> **/
        public static bool GetAsyncKeyState(Keys key)
        {
            return 0 != (GetAsyncKeyState((int)key) & 0x8000);
        }

        /** <summary>CNest Converts Keys to int</summary> **/
        public static int ToInt(this Keys key)
        {
            return (int)key;
        }

        // Hardwarebased
        /** <summary>CNest Get hardware informations</summary> **/
        public static (string VolumeSerialNumber, string volumeMaxComponentLength, string VolumeFileSystemName, string ComputerName) GetHardwareInfo()
        {
            string vsn = "";
            string vmcl = "";
            string vfsn = "";
            string cn = "";

            string drive_letter = "C:\\";
            drive_letter = drive_letter.Substring(0, 1) + ":\\";

            uint serial_number = 0;
            uint max_component_length = 0;
            StringBuilder sb_volume_name = new StringBuilder(256);
            UInt32 file_system_flags = new UInt32();
            StringBuilder sb_file_system_name = new StringBuilder(256);

            if (GetVolumeInformation(drive_letter, sb_volume_name,
                (UInt32)sb_volume_name.Capacity, ref serial_number,
                ref max_component_length, ref file_system_flags,
                sb_file_system_name,
                (UInt32)sb_file_system_name.Capacity) != 0)
            {
                    vsn = serial_number.ToString();
                    vmcl = max_component_length.ToString();
                    vfsn = sb_file_system_name.ToString();
            }
            else
            {
                vsn = null;
                vmcl = null;
                vfsn = null;
            }

            cn = Environment.MachineName;

            return (vsn, vmcl, vfsn, cn);
        }

        // Encryption
        public class AES
        {
            AesCryptoServiceProvider aesCrypto;

            /** <summary>CNest AES Class</summary> **/
            public AES(string key = "Jj2BzcCutd5ntXu2Z8HqD5i23VAdir5n", string iv = "lksguldusfgiulsg", CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
            {
                aesCrypto = new AesCryptoServiceProvider();

                aesCrypto.BlockSize = 128;
                aesCrypto.KeySize = 256;
                aesCrypto.Key = ASCIIEncoding.ASCII.GetBytes(key);
                aesCrypto.IV = ASCIIEncoding.ASCII.GetBytes(iv);
                aesCrypto.Mode = cipherMode;
                aesCrypto.Padding = paddingMode;
            }

            /** <summary>CNest AES Class</summary> **/
            public AES(byte[] key, byte[] iv, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
            {
                aesCrypto = new AesCryptoServiceProvider();

                aesCrypto.BlockSize = 128;
                aesCrypto.KeySize = 256;
                aesCrypto.Key = key;
                aesCrypto.IV = iv;
                aesCrypto.Mode = cipherMode;
                aesCrypto.Padding = paddingMode;
            }

            /** <summary>CNest Encrypts a string into AES</summary> **/
            public (byte[] Bytes, string Base64) Encrypt(string text)
            {
                ICryptoTransform transform = aesCrypto.CreateEncryptor();

                byte[] encryptedBytes = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);

                string base64 = Convert.ToBase64String(encryptedBytes);

                return (encryptedBytes, base64);
            }

            /** <summary>CNest Decrypts an AES string into a normal string</summary> **/
            public string Decrypt(string text)
            {
                ICryptoTransform transform = aesCrypto.CreateDecryptor();

                byte[] encBytes = Convert.FromBase64String(text);
                byte[] decryptedBytes = transform.TransformFinalBlock(encBytes, 0, encBytes.Length);

                string str = ASCIIEncoding.ASCII.GetString(decryptedBytes);

                return str;
            }

            /** <summary>CNest Decrypts an AES byte array into a normal string</summary> **/
            public string Decrypt(byte[] text)
            {
                ICryptoTransform transform = aesCrypto.CreateDecryptor();

                byte[] encBytes = text;
                byte[] decryptedBytes = transform.TransformFinalBlock(encBytes, 0, encBytes.Length);

                string str = ASCIIEncoding.ASCII.GetString(decryptedBytes);

                return str;
            }
        }
    }
}
