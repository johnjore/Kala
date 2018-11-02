using System;
using Xamarin.Forms;
using Plugin.Logger;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using System.Threading.Tasks;

namespace Kala.Pages
{
    public class BarcodePage : ContentPage
    {
        public BarcodePage(string barcodeButtonId, MobileBarcodeScanningOptions mobileBarcodeScanningOptions)
        {
            ZXingScannerPage scanPage = new ZXingScannerPage(mobileBarcodeScanningOptions);
            Navigation.PushAsync(scanPage);

            scanPage.OnScanResult += (result) => {
                scanPage.IsScanning = false;
                
                CrossLogger.Current.Debug("Barcode", "Barcode: '" + result.Text + "', Format: " + result.BarcodeFormat);

                Task.Run(async () =>
                {
                    await new RestService().SendCommand(barcodeButtonId, result.Text);
                });

                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });                
            };

            scanPage.Disappearing += RestoreMain;
        }

        private void RestoreMain(object sender, EventArgs e)
        {
            Application.Current.MainPage = Widgets.GetPreviousPage();
        }
    }
}
