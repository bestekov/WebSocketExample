﻿using System;
using System.Globalization;
using System.Windows.Forms;

namespace WebSocketExample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Application.Run(new Form1(new CultureInfo("en-UK")));
            //Application.Run(new Form1(new CultureInfo("de-DE")));
            //Application.Run(new Form1(new CultureInfo("es-MX")));
            //Application.Run(new Form1(new CultureInfo("lo-LA")));
            //Application.Run(new Form1(new CultureInfo("th")));
        }
    }
}
