using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace CaptureIDCard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string IDNumber { get; set; }
        public string OutputDir { get; set; }
        public int CropWidth { get; set; }
        public int CropHeight { get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IDNumber = "idcard";
            OutputDir = "";
            CropWidth = 400;
            CropHeight = 256;

            if (e.Args.Length >= 1)
            {
                IDNumber = e.Args[0];
            }
            if (e.Args.Length >= 2)
            { 
                OutputDir = e.Args[1];
            }
            if (e.Args.Length >= 3)
            {
                CropWidth = int.Parse(e.Args[2]);
            }
            if (e.Args.Length >= 4)
            {
                CropHeight = int.Parse(e.Args[3]);
            }
        }
    }
}
