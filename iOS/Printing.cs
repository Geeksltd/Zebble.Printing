namespace Zebble
{
    using Foundation;
    using System.Threading.Tasks;
    using UIKit;

    public partial class Printing
    {
        /// <summary>
        /// Print specefic image from a file or an url.
        /// </summary>
        /// <param name="path">image file or url</param>
        public static Task PrintImage(string path)
        {
            var imageFile = GetFile(path);

            if (!imageFile.Exists)
            {
                Device.Log.Error("The file is not exist");
                return Task.CompletedTask;
            }

            var printer = UIPrintInteractionController.SharedPrintController;

            if (printer == null)
            {
                Device.Log.Error("Unable to print at this time.");
            }
            else
            {
                var printInfo = UIPrintInfo.PrintInfo;
                printInfo.OutputType = UIPrintInfoOutputType.General;
                printInfo.JobName = "Image is printing";

                printer.PrintInfo = printInfo;
                printer.PrintingItem = NSUrl.FromFilename(imageFile.FullName);
                printer.ShowsPageRange = true;

                var handler = new UIPrintInteractionCompletionHandler((printInteractionController, completed, error) =>
                {
                    if (completed)
                    {
                        Device.Log.Message("Print Completed.");
                    }
                    else if (!completed && error != null)
                    {
                        Device.Log.Message("Error Printing.");
                    }
                });

                printer.Present(true, handler);
            }

            return Task.CompletedTask;
        }
    }
}
