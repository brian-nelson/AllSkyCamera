using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Common.Interfaces;

namespace ascLibrary.Managers
{
    /// <summary>
    /// This manager collects images as fast as possible 
    /// Still frames rather than a video sequence.  These are
    /// stored on disk.
    /// </summary>
    public class ImageCollectionManager : IRunnable
    {
        private readonly string m_VideoDeviceName;
        private readonly string m_ImagePath;
        
        private bool m_StopRequested;
        private Thread m_ProcessingThread;

        public ImageCollectionManager(
            string _videoDeviceName,
            string _imagePath)
        {
            m_VideoDeviceName = _videoDeviceName;
            m_ImagePath = _imagePath;
        }

        public void Start()
        {
            if (m_ProcessingThread == null)
            {
                m_ProcessingThread = new Thread(InternalStart);
                m_ProcessingThread.Start();
            }
        }

        private void InternalStart()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                TraceFileHelper.Error("No video devices found.");
            }

            string moniker = null;

            foreach (FilterInfo videoDevice in videoDevices)
            {
                if (videoDevice.Name == m_VideoDeviceName)
                {
                    moniker = videoDevice.MonikerString;
                    break;
                }
            }

            if (moniker == null)
            {
                TraceFileHelper.Error("Device not found");
                return;
            }

            // create video source
            var videoSource = new VideoCaptureDevice(moniker);

            int maxCapabilitieIndex = videoSource.VideoCapabilities.Count() - 1;

            if (maxCapabilitieIndex < 0)
            {
                TraceFileHelper.Warning("No camera image capabilities found.");
                return;
            }

            VideoCapabilities resolution = videoSource.VideoCapabilities[maxCapabilitieIndex];
            videoSource.VideoResolution = resolution;

            // set NewFrame event handler
            videoSource.NewFrame += video_NewFrame;

            // start the video source
            videoSource.Start();

            do
            {
                Thread.Sleep(100);
            } while (! m_StopRequested);

            videoSource.SignalToStop();

            m_ProcessingThread = null;
        }

        private void video_NewFrame(object _sender, NewFrameEventArgs _eventArgs)
        {
            // get new frame
            Bitmap bitmap = _eventArgs.Frame;

            string filename = DateTime.UtcNow.ToFileTimeUtc() + ".png";
            string fullname = Path.Combine(m_ImagePath, filename);

            bitmap.Save(fullname, ImageFormat.Png);
        }

        public void Stop()
        {
            m_StopRequested = true;
        }
    }
}
