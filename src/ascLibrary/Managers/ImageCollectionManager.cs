using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using AllSkyCamera.Library.Handlers;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Common.Interfaces;

namespace AllSkyCamera.Library.Managers
{
    /// <summary>
    /// This manager collects images as fast as apossible 
    /// Still frames rather than a video sequence.  These are
    /// stored on disk.
    /// </summary>
    public class ImageCollectionManager : IRunnable
    {
        private readonly string m_VideoDeviceName;
        private readonly List<IImageHandler> m_Handlers;
        
        private bool m_StopRequested;
        private Thread m_ProcessingThread;

        public ImageCollectionManager(
            string _videoDeviceName,
            List<IImageHandler> _handlers)
        {
            m_VideoDeviceName = _videoDeviceName;
            m_Handlers = _handlers;
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

            if (m_Handlers != null)
            {
                foreach (var imageHandler in m_Handlers)
                {
                    imageHandler.HandleImage(bitmap);
                }
            }
        }

        public void Stop()
        {
            m_StopRequested = true;
        }
    }
}
