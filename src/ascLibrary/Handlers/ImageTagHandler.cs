using System;
using System.Collections.Generic;
using System.Drawing;

namespace AllSkyCamera.Library.Handlers
{
    public class ImageTagHandler : IImageHandler
    {
        private readonly List<IImageHandler> m_PostImageHandlers;

        public ImageTagHandler(
            List<IImageHandler> _postHandlers = null)
        {
            m_PostImageHandlers = _postHandlers;
        }

        public void HandleImage(Bitmap _bitmap)
        {
            _bitmap.Tag = DateTime.UtcNow;

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
