using System;
using Xamarin.Forms;
using Plugin.Logger;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Kala
{
    public class BarcodePage : ContentPage
    {
        public BarcodePage(string barcodeButtonId, MobileBarcodeScanningOptions mobileBarcodeScanningOptions)
        {
            ZXingScannerPage scanPage = new ZXingScannerPage(mobileBarcodeScanningOptions);
            Navigation.PushAsync(scanPage);

            scanPage.OnScanResult += (result) => {
                scanPage.IsScanning = false;
                CrossLogger.Current.Debug("Barcode", "Barcode: " + result.Text);

                #pragma warning disable CS4014
                new RestService().SendCommand(barcodeButtonId, result.Text);
                #pragma warning restore CS4014

                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });                
            };

            scanPage.Disappearing += RestoreMain;
        }

        private void RestoreMain(object sender, EventArgs e)
        {
            Application.Current.MainPage = Widgets.PreviousPage;
        }
    }
}
