using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using AForge.Vision.Motion;
using TreeGecko.Library.Common.Helpers;
using TreeGecko.Library.Common.Interfaces;

namespace AllSkyCamera.Library.Managers
{
    /// <summary>
    /// This class compares a pipeline of images to determine 
    /// which images should be kept and which should be discarded.
    /// 
    /// Rules for image retention
    /// - Images that show sufficent motion are kept. 
    /// - At least one image per x seconds is kept.
    /// </summary>
    public class ImageProcessingManager : IRunnable
    {
        private readonly string m_CapturedImageFolder;
        private readonly string m_ProcessedImageFolder;
        private bool m_StopRequested;
        private MotionDetector m_MotionDetector;
        private DateTime m_LastSavedImageTime;
        private readonly long m_JpegQuality;
        private readonly double m_MotionThreshold;
        private double m_MaximumSecondsPerImage;

        public ImageProcessingManager(
            string _capturedImageFolder,
            string _processedImageFolder,
            long _jpegQuality = 85L,
            double _motionThreshold = 1,
            double _maximumSecondsPerImage = 5)
        {
            m_CapturedImageFolder = _capturedImageFolder;
            m_ProcessedImageFolder = _processedImageFolder;
            m_JpegQuality = _jpegQuality;
            m_MotionThreshold = _motionThreshold;
            m_MaximumSecondsPerImage = _maximumSecondsPerImage;
        }

        public void Start()
        {
            m_StopRequested = false;
            m_MotionDetector = new MotionDetector(new SimpleBackgroundModelingDetector(false));

            //So we always save the first image
            m_LastSavedImageTime = DateTime.MinValue;

            //Prepare image processing
            var imageCodec = GetEncoderInfo(ImageFormat.Jpeg);
            var encoderQuality = Encoder.Quality;
            var myEncoderParameter = new EncoderParameter(encoderQuality, m_JpegQuality);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = myEncoderParameter;
            
            do
            {
                IEnumerable<FileInfo> files = new DirectoryInfo(m_CapturedImageFolder).GetFiles().OrderBy(f => f.Name);

                foreach (var file in files)
                {
                    try
                    {
                        bool saveImage = false;

                        //Get the new frame
                        using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                        {
                            using (var frame = (Bitmap) Bitmap.FromStream(stream))
                            {
                                //Has image changed enough to warrent saving
                                saveImage = m_MotionDetector.ProcessFrame(frame) > m_MotionThreshold;

                                //What was the timestamp of the image
                                string timestamp = Path.GetFileNameWithoutExtension(file.Name);
                                long ticks;
                                if (long.TryParse(timestamp, out ticks))
                                {
                                    //So the image had a timestamp (rather than some random filename).
                                    DateTime imageTime = DateTime.FromFileTime(ticks);

                                    //Was the image more than a second from last image.
                                    if ((imageTime - m_LastSavedImageTime).TotalSeconds > m_MaximumSecondsPerImage)
                                    {
                                        saveImage = true;
                                    }

                                    if (saveImage)
                                    {
                                        m_LastSavedImageTime = imageTime;

                                        string filename = Path.GetFileNameWithoutExtension(file.Name) + ".jpg";
                                        string fullName = Path.Combine(m_ProcessedImageFolder, filename);
                                        if (!File.Exists(fullName))
                                        {
                                            frame.Save(fullName, imageCodec, encoderParameters);
                                        }
                                    }
                                }
                            }
                        }

                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        TraceFileHelper.Exception(ex);
                    }

                    Thread.Sleep(1);
                }

                Thread.Sleep(100);
            } while (!m_StopRequested);
        }

        public void Stop()
        {
            m_StopRequested = true;
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat _format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == _format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
