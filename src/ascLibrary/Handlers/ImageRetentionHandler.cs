using System;
using System.Collections.Generic;
using System.Drawing;
using AForge.Vision.Motion;

namespace AllSkyCamera.Library.Handlers
{
    public class ImageRetentionHandler : IImageHandler
    {
        private readonly double m_MotionThreshold;
        private readonly double m_MaximumSecondsBetweenImage;
        private readonly List<IImageHandler> m_PostImageHandlers;
        private readonly MotionDetector m_MotionDetector;
        private DateTime m_LastSavedImageTime;
        
        public ImageRetentionHandler(
            double _maxSecondsBetweenImage,
            double _motionThreshold,
            List<IImageHandler> _postHandlers = null)
        {
            m_MotionDetector = new MotionDetector(new TwoFramesDifferenceDetector(false));
            m_MaximumSecondsBetweenImage = _maxSecondsBetweenImage;
            m_MotionThreshold = _motionThreshold;
            m_PostImageHandlers = _postHandlers;
        }

        public void HandleImage(Bitmap _bitmap)
        {
            //Has image changed enough to warrent saving
            bool saveImage = m_MotionDetector.ProcessFrame(_bitmap) > m_MotionThreshold;

            DateTime imageTime;
            if (_bitmap.Tag != null)
            {
                imageTime = (DateTime) _bitmap.Tag;

                if ((imageTime - m_LastSavedImageTime).TotalSeconds > m_MaximumSecondsBetweenImage)
                {
                    saveImage = true;
                }
            }
            else
            {
                imageTime = DateTime.UtcNow;
            }

            if (saveImage)
            {
                m_LastSavedImageTime = imageTime;

                if (m_PostImageHandlers != null)
                {
                    foreach (var postImageHandler in m_PostImageHandlers)
                    {
                        postImageHandler.HandleImage(_bitmap);
                    }
                }
            }
        }
    }
}
