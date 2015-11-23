using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeGecko.Library.Common.Interfaces;

namespace ascLibrary.Managers
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
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
