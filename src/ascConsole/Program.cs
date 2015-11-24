using System;
using ascLibrary.Managers;
using TreeGecko.Library.Common.Helpers;

namespace ascConsole
{
    class Program
    {
        private static ImageCollectionManager m_ImageCollectionManager;
        private static ImageProcessingManager m_ImageProcessingManager;
        private static ImageTransferManager m_ImageTransferManager;

        static void Main(string[] _args)
        {
            StartCapture();
            StartProcessing();
            StartTransfer();

            //Wait until done.
            Console.ReadKey();  
          
            StopCapture();
            StopProcessing();
            StopTransfer();
        }

        private static void StartCapture()
        {
            //Load required settings
            string cameraName = Config.GetSettingValue("ImageCaptureDevice");
            string imageCaptureFolder = Config.GetSettingValue("CapturedImageFolder");

            m_ImageCollectionManager = new ImageCollectionManager(cameraName, imageCaptureFolder);
            m_ImageCollectionManager.Start();
        }

        private static void StartProcessing()
        {
            //Load required settings
            string imageCaptureFolder = Config.GetSettingValue("CapturedImageFolder");
            string processedImageFolder = Config.GetSettingValue("ProcessedImageFolder");

            m_ImageProcessingManager = new ImageProcessingManager(imageCaptureFolder, processedImageFolder);
            m_ImageProcessingManager.Start();
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

        private static void StopProcessing()
        {
            if (m_ImageProcessingManager != null)
            {
                m_ImageProcessingManager.Stop();
            }
        }

        private static void StopTransfer()
        {
            
        }

    }
}
