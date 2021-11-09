using System;
using System.IO;
using System.Threading.Tasks;
using Spyder.Client;
using Spyder.Client.Net.Notifications;

namespace DrawingDataRawCaptureUtility
{
    class Program
    {
        private static readonly int maxLogsPerServer = 10;
        private static readonly string resultsPath = @"c:\temp\drawingDataCaptures";

        private static SpyderClientManager spyderClientManager;

        static async Task Main(string[] args)
        {
            if (!Directory.Exists(resultsPath))
                Directory.CreateDirectory(resultsPath);

            Console.WriteLine("Beginning to listen for DrawingData");

            spyderClientManager = new SpyderClientManager();
            spyderClientManager.RaiseDrawingDataChanged = true;
            spyderClientManager.DrawingDataReceived += SpyderClientManager_DrawingDataReceived;
            if(!await spyderClientManager.StartupAsync())
            {
                Console.WriteLine("Failed to start");
                return;
            }

            Console.WriteLine("Captured data will be written to " + resultsPath);
            Console.WriteLine("Initialized.  Press return to stop.");
            Console.ReadLine();

            Console.WriteLine("Shutting down...");
            await spyderClientManager.ShutdownAsync();
        }

        private static async void SpyderClientManager_DrawingDataReceived(object sender, DrawingDataReceivedEventArgs e)
        {
            var spyder = await spyderClientManager.GetServerAsync(e.ServerIP);
            string logFolder = Path.Combine(resultsPath, $"{e.ServerIP} - {spyder.Version.ToShortString()}");
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            int fileIndex = Directory.GetFiles(logFolder).Length;
            if (fileIndex <= maxLogsPerServer)
            {
                string fullFileName = Path.Combine(logFolder, $"{e.ServerIP} - {spyder.Version.ToShortString()} - {fileIndex}.bin");
                File.WriteAllBytes(fullFileName, e.RawMessage);
                Console.WriteLine("Wrote file " + fullFileName);
            }
        }

    }
}
