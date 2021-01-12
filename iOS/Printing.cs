namespace Zebble
{
    using CoreGraphics;
    using Foundation;
    using System.Threading.Tasks;
    using UIKit;
    using Olive;

    public partial class Printing
    {
        static UIPrinter Printer;
        static string PrinterUrl;

        /// <summary>
        /// Trys to print the given file without any question to select the printer.
        /// </summary>
        /// <param name="path">Image file or url</param>
        /// <param name="printerUrl">The printer url</param>
        /// <returns>Used printer`s absolute url for the next use.</returns>
        public static async Task<string> TrySilentPrint(string path, string printerUrl = "")
        {
            var imageFile = GetFile(path);

            if (!imageFile.Exists)
            {
                Log.For<Printing>().Error(null, "The file is not exist");
                return null;
            }

            var printerUI = UIPrintInteractionController.SharedPrintController;

            if (printerUI == null)
            {
                Log.For<Printing>().Error(null, "Unable to print at this time.");
                return null;
            }
            else
            {
                var printInfo = UIPrintInfo.PrintInfo;
                printInfo.OutputType = UIPrintInfoOutputType.General;
                printInfo.JobName = "Image is printing";
                printInfo.Orientation = UIPrintInfoOrientation.Landscape;

                printerUI.PrintInfo = printInfo;
                printerUI.PrintingItem = NSUrl.FromFilename(imageFile.FullName);
                printerUI.ShowsPageRange = true;

                UIPrinter printer = await GetPrinter(printerUrl);

                await printerUI.PrintToPrinterAsync(printer);
                return printer.Url.AbsoluteString;
            }
        }

        static async Task<UIPrinter> GetPrinter(string printerUrl)
        {
            if (Device.OS.IsBeforeiOS(13))
            {
                if (PrinterUrl == printerUrl && Printer != null)
                    return Printer;
            }

            var printer = UIPrinter.FromUrl(new NSUrl(printerUrl));
            var isthere = await printer.ContactPrinterAsync();
            if (isthere)
            {
                PrinterUrl = printerUrl;
                return Printer = printer;
            }

            var printerPicker = UIPrinterPickerController.FromPrinter(null);

            var callback = await printerPicker.PresentFromRectAsync(
                new CGRect(0, 0, View.Root.ActualWidth, View.Root.ActualHeight / 4),
                View.Root.Native(),
                true);
            //var callback = await printerPicker.PresentAsync(true);

            printer = callback.PrinterPickerController.SelectedPrinter;
            PrinterUrl = printer.Url.AbsoluteString;

            return Printer = printer;
        }
    }
}