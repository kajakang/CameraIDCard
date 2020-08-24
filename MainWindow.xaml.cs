using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
using AForge.Video;
using AForge.Video.DirectShow;

namespace CaptureIDCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum UIPage { CAMERA_PAGE, CONFIRM_PAGE };
        private CameraPage cameraPage;
        private ConfirmPage confirmPage;
        internal System.Drawing.Image CapturedImage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            cameraPage = new CameraPage();
            confirmPage = new ConfirmPage();
            cameraPage.SetParent(this);
            confirmPage.SetParent(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MoveToPage(UIPage.CAMERA_PAGE);
        }

        public void MoveToPage(UIPage page)
        {
            switch (page)
            {
                case UIPage.CAMERA_PAGE:
                    cameraPage.CloseCurrentVideoSource();
                    layoutRoot.NavigationService.Navigate(cameraPage);
                    break;
                case UIPage.CONFIRM_PAGE:
                    cameraPage.CloseCurrentVideoSource();
                    layoutRoot.NavigationService.Navigate(confirmPage);
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cameraPage.CloseCurrentVideoSource();
            Application.Current.Shutdown(-1);
        }
    }
}
