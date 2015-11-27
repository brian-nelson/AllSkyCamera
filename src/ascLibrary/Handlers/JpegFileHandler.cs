using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AllSkyCamera.Library.Delegates;

namespace AllSkyCamera.Library.Handlers
{
    public class JpegFileHandler : IImageHandler
    {
        public FileSaved FileSaved;
        private readonly string m_OutputFolder;
        private readonly List<IImageHandler> m_PostImageHandlers;
        private readonly ImageCodecInfo m_ImageCodec;
        private readonly EncoderParameters m_EncoderParameters;

        public JpegFileHandler(
            string _outputFolder,
            long _jpegQuality = 85L,
            List<IImageHandler> _postHandlers = null )
        {
            m_OutputFolder = _outputFolder;
            m_PostImageHandlers = _postHandlers;

            //Prepare image processing
            m_ImageCodec = GetEncoderInfo(ImageFormat.Jpeg);
            var encoderQuality = Encoder.Quality;
            var myEncoderParameter = new EncoderParameter(encoderQuality, _jpegQuality);
            m_EncoderParameters = new EncoderParameters(1);
            m_EncoderParameters.Param[0] = myEncoderParameter;
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

        public void HandleImage(Bitmap _bitmap)
        {
            string filename;

            if (_bitmap.Tag != null)
            {
                DateTime imageTime = (DateTime) _bitmap.Tag;
                filename = string.Format("{0}.jpg", imageTime.ToFileTime());
            }
            else
            {
                filename = string.Format("{0}.jpg", DateTime.UtcNow.ToFileTime());
            }

            string fullName = Path.Combine(m_OutputFolder, filename);
            _bitmap.Save(fullName, m_ImageCodec, m_EncoderParameters);

            if (FileSaved != null)
            {
                FileSaved(fullName);
            }
            
            //Call any post handlers
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
