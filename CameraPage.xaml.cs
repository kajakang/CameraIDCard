using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections;
using System.Drawing;

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using System.Windows.Media.Imaging;

namespace CaptureIDCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CameraPage : Page
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] snapshotCapabilities;
        private ArrayList listCamera = new ArrayList();

        private MainWindow mainWindow;

        private System.Drawing.Size cropSize;
        private Rectangle cropRect;
        private System.Drawing.Image currentImage;

        public CameraPage()
        {
            InitializeComponent();
            int cropWidth = ((App)Application.Current).CropWidth;
            int cropHeight = ((App)Application.Current).CropHeight;

            cropSize = new System.Drawing.Size(cropWidth, cropHeight);
        }

        public void SetParent(MainWindow parent)
        {
            mainWindow = parent;
        }

        void CameraPage_Loaded(object sender, RoutedEventArgs e)
        {
            OpenCamera();
        }

        private void OpenCamera()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count != 0)
                {
                    // add all devices to combo
                    foreach (FilterInfo device in videoDevices)
                    {
                        listCamera.Add(device.Name);
                    }
                }
                else
                {
                    MessageBox.Show("Camera devices not found");
                }
                videoDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);
                snapshotCapabilities = videoDevice.SnapshotCapabilities;
                if (snapshotCapabilities.Length == 0)
                {
                    //MessageBox.Show("Camera Capture Not supported");
                }
                // open device
                OpenVideoSource(videoDevice);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        public void OpenVideoSource(IVideoSource source)
        {
            try
            {
                // set busy cursor
                this.Cursor = Cursors.Wait;
                // stop current video source
                CloseCurrentVideoSource();
                // start new video source
                videoSourcePlayer.VideoSource = source;
                videoSourcePlayer.Start();
                // reset stop watch
                this.Cursor = Cursors.Arrow;
            }
            catch { }
        }

        public void CloseCurrentVideoSource()
        {
            try
            {
                if (videoSourcePlayer.VideoSource != null)
                {
                    videoSourcePlayer.SignalToStop();
                    // wait ~ 3 seconds
                    for (int i = 0; i < 30; i++)
                    {
                        if (!videoSourcePlayer.IsRunning)
                            break;
                        System.Threading.Thread.Sleep(100);
                    }
                    if (videoSourcePlayer.IsRunning)
                    {
                        videoSourcePlayer.Stop();
                    }
                    videoSourcePlayer.VideoSource = null;
                }
            }
            catch { }
        }
        private void VideoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
            try
            {
                // get crop rect
                int frameWidth = image.Width;
                int frameHeight = image.Height;
                cropRect = new Rectangle(new System.Drawing.Point((frameWidth - cropSize.Width) / 2, (frameHeight - cropSize.Height) / 2), cropSize);

                image.RotateFlip(RotateFlipType.RotateNoneFlipX);

                Graphics g = Graphics.FromImage(image);
                // paint current time
                SolidBrush brush = new SolidBrush(Color.Red);
                Pen pen = new Pen(brush, 5);
                int cornerLength = 50;
                int padding = 3;
                Rectangle cropRect_padding = new Rectangle(cropRect.Left - padding, cropRect.Top - padding, cropRect.Width + padding * 2, cropRect.Height + padding * 2);

                // g.DrawRectangle(pen, cropRect);
                g.DrawLine(pen, new PointF(cropRect_padding.Left, cropRect_padding.Top), new PointF(cropRect_padding.Left + cornerLength, cropRect_padding.Top));
                g.DrawLine(pen, new PointF(cropRect_padding.Left, cropRect_padding.Top), new PointF(cropRect_padding.Left, cropRect_padding.Top + cornerLength));

                g.DrawLine(pen, new PointF(cropRect_padding.Right, cropRect_padding.Top), new PointF(cropRect_padding.Right - cornerLength, cropRect_padding.Top));
                g.DrawLine(pen, new PointF(cropRect_padding.Right, cropRect_padding.Top), new PointF(cropRect_padding.Right, cropRect_padding.Top + cornerLength));

                g.DrawLine(pen, new PointF(cropRect_padding.Left, cropRect_padding.Bottom), new PointF(cropRect_padding.Left + cornerLength, cropRect_padding.Bottom));
                g.DrawLine(pen, new PointF(cropRect_padding.Left, cropRect_padding.Bottom), new PointF(cropRect_padding.Left, cropRect_padding.Bottom - cornerLength));

                g.DrawLine(pen, new PointF(cropRect_padding.Right, cropRect_padding.Bottom), new PointF(cropRect_padding.Right - cornerLength, cropRect_padding.Bottom));
                g.DrawLine(pen, new PointF(cropRect_padding.Right, cropRect_padding.Bottom), new PointF(cropRect_padding.Right, cropRect_padding.Bottom - cornerLength));

                pen.Dispose();
                brush.Dispose();

                g.Dispose();

                currentImage = image;
            }
            catch
            { }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseCurrentVideoSource();
            Application.Current.Shutdown(1);
        }

        private void btnShoot_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow != null)
            {
                mainWindow.CapturedImage = new Bitmap(cropRect.Width, cropRect.Height);
                using (Graphics g = Graphics.FromImage(mainWindow.CapturedImage))
                {
                    g.DrawImage(currentImage, new Rectangle(0, 0, cropRect.Width, cropRect.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }
                mainWindow.CapturedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                mainWindow.MoveToPage(MainWindow.UIPage.CONFIRM_PAGE);
            }
        }
    }
}
