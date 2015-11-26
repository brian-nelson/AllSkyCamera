using System;
using System.Collections.Generic;
using AllSkyCamera.Library.Handlers;
using AllSkyCamera.Library.Managers;
using TreeGecko.Library.Common.Helpers;

namespace ascConsole
{
    class Program
    {
        private static ImageCollectionManager m_ImageCollectionManager;

        static void Main(string[] _args)
        {
            StartCapture();
            StartTransfer();

            //Wait until done.
            Console.ReadKey();  
          
            StopCapture();
            StopTransfer();
        }

        private static void StartCapture()
        {
            //Load required settings
            string cameraName = Config.GetSettingValue("ImageCaptureDevice");
            string processedImageFolder = Config.GetSettingValue("ProcessedImageFolder");

            JpegFileHandler fileHandler = new JpegFileHandler(processedImageFolder);

            ImageRetentionHandler retentionHandler = new ImageRetentionHandler(
                5, 
                .05,
                new List<IImageHandler> {fileHandler});

            ImageTagHandler tagHandler = new ImageTagHandler(
                new List<IImageHandler> {retentionHandler});

            m_ImageCollectionManager = new ImageCollectionManager(
                cameraName,
                new List<IImageHandler> {tagHandler});

            m_ImageCollectionManager.Start();
        }


        private static void StartTransfer()
        {
            //Load required settings
            string processedImageFolder = Config.GetSettingValue("ProcessedImageFolder");
            string destinationS3Bucket = Config.GetSettingValue("DestinationS3Bucket");
            string destinationS3AccessKey = Config.GetSettingValue("DestinationS3AccessKey");
            string destinationS3SecretKey = Config.GetSettingValue("DestinationS3SecretKey");

        }

        private static void StopCapture()
        {
            if (m_ImageCollectionManager != null)
            {
                m_ImageCollectionManager.Stop();
            }
        }

        private static void StopTransfer()
        {
            
        }

    }
}
