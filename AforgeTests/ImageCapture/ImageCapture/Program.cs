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
        static void Main(string[] args)
        {
            // enumerate video devices
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice );

            // create video source
            VideoCaptureDevice videoSource = new VideoCaptureDevice(
                    videoDevices[1].MonikerString );
            
            // set NewFrame event handler
            videoSource.NewFrame += video_NewFrame;
            
            // start the video source
            videoSource.Start();

            do
            {
                Thread.Sleep(100);
            } while (true);

            videoSource.SignalToStop( );
            // ...
        }

        private static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;

            bitmap.Save("c:\\temp\\" + DateTime.Now + ".png", ImageFormat.Png);
            // process the frame
        }
    }
}
