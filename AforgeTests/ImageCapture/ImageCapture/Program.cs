using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Imaging;
using AForge.Video.DirectShow;

namespace ImageCapture
{
    class Program
    {
        private static int imageCount = 0;
        private static Guid sequenceGuid = Guid.NewGuid();

        private static void Main(string[] args)
        {
            // enumerate video devices
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                // create video source
                var videoSource = new VideoCaptureDevice(
                    videoDevices[0].MonikerString);

                int maxCapabilitieIndex = videoSource.VideoCapabilities.Count() - 1;

                if (maxCapabilitieIndex > 0)
                {
                    VideoCapabilities resolution = videoSource.VideoCapabilities[maxCapabilitieIndex];
                    videoSource.VideoResolution = resolution;

                    videoSource.SetCameraProperty(CameraControlProperty.Exposure, +20, CameraControlFlags.Manual);
                    
                    // set NewFrame event handler
                    videoSource.NewFrame += video_NewFrame;


                    // start the video source
                    videoSource.Start();

                    int count = 0;
                    do
                    {
                        Thread.Sleep(100);
                    } while (imageCount < 20);

                    videoSource.SignalToStop();
                }
                else
                {
                    //no capabilities
                }
            }

            // ...
        }

        private static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            imageCount ++;

            // get new frame
            Bitmap bitmap = eventArgs.Frame;

            string filename = "c:\\temp\\" + sequenceGuid + "_" + imageCount + ".png";

            bitmap.Save(filename, ImageFormat.Png);
            // process the frame
        }
    }
}
