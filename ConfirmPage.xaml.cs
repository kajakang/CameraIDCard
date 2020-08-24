using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaptureIDCard
{
    /// <summary>
    /// Interaction logic for ConfirmPage.xaml
    /// </summary>
    public partial class ConfirmPage : Page
    {
        private MainWindow mainWindow;

        public ConfirmPage()
        {
            InitializeComponent();
        }

        public void SetParent(MainWindow parent)
        {
            mainWindow = parent;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(1);
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            // Save image
            string id = ((App)Application.Current).IDNumber;
            string outputdir = ((App)Application.Current).OutputDir;
            string outputfile = System.IO.Path.Combine(outputdir, id + ".jpg");
            try
            {
                mainWindow.CapturedImage.Save(outputfile, ImageFormat.Jpeg);
                Application.Current.Shutdown(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Application.Current.Shutdown(2);
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow != null)
            {
                mainWindow.MoveToPage(MainWindow.UIPage.CAMERA_PAGE);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                mainWindow.CapturedImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                croppedImage.Source = bitmapimage;
            }
        }
    }
}
