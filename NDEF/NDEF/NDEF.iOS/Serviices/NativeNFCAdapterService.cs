using System.Threading.Tasks;
using CoreNFC;
using NDEF.Interfaces;
using NDEF.iOS.Enums;
using NDEF.iOS.Services;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeNFCAdapterService))]
namespace NDEF.iOS.Services
{
    public class NativeNFCAdapterService : INfcAdapter
    {
        public NativeNFCAdapterService()
        {
        }

        #region Unused methods
        public void ConfigureNfcAdapter()
        {

        }

        public void EnableForegroundDispatch()
        {

        }

        public void DisableForegroundDispatch()
        {

        }

        public void UnconfigureNfcAdapter()
        {

        }

        #endregion

        public async Task ReadAsync()
        {
            var isNfcAvailable = UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
            if (isNfcAvailable && NFCNdefReaderSession.ReadingAvailable)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        var sessionDelegate = new SessionDelegate();
                        var session = new NFCNdefReaderSession(sessionDelegate, null, true);
                        session.BeginSession();

                        var status = await sessionDelegate.WasDataTransmitted.Task;
                        if (status != NfcTransmissionStatus.Success)
                        {
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Suitable error message", "Ok");
                        }

                    }
                    catch
                    {
                        await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "There was an error while trying to create a NFC session", "Ok");
                    }
                });
            }
            else
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Read is not supported by this tag", "Ok");
            }
        }

        public Task<bool> OpenNFCSettingsAsync() => Task.FromResult(true);

        public Task<bool> GetCurrentStatusAsync()
        {
            bool status = false;
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (NFCNdefReaderSession.ReadingAvailable)
                {
                    status = true;
                }
            }
            return Task.FromResult(status);
        }
    }
}


