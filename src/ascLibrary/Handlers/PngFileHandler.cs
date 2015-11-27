using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AllSkyCamera.Library.Delegates;

namespace AllSkyCamera.Library.Handlers
{
    public class PngFileHandler : IImageHandler
    {
        private string m_OutputFolder;
        private List<IImageHandler> m_PostImageHandlers;
        public FileSaved FileSaved;

        public PngFileHandler(
            string _outputFolder,
            List<IImageHandler> _postHandlers = null)
        {
            m_OutputFolder = _outputFolder;
            m_PostImageHandlers = _postHandlers;
        }

        public void HandleImage(Bitmap _bitmap)
        {
            string filename;

            if (_bitmap.Tag != null)
            {
                filename = string.Format("{0}.png", _bitmap.Tag);
            }
            else
            {
                filename = string.Format("{0}.png", DateTime.UtcNow.ToFileTime());
            }

            string fullName = Path.Combine(m_OutputFolder, filename);
            _bitmap.Save(fullName, ImageFormat.Png);

            if (FileSaved != null)
            {
                FileSaved(fullName);
            }

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
