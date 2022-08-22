using System;
using CoreNFC;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.VisualBasic;
using NDEFMAUI.Enums;
using UIKit;

namespace NDEFMAUI.Partials
{
    public partial class NfcService
    {

        public NfcService()
        {
        }

        public partial void ConfigureNfcAdapter()
        {

        }

        public partial void EnableForegroundDispatch()
        {

        }

        public partial void DisableForegroundDispatch()
        {

        }

        public partial void UnconfigureNfcAdapter()
        {

        }

        public async partial Task SendAsync(byte[] bytes)
        {
            var isNfcAvailable = UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
            if (isNfcAvailable && NFCNdefReaderSession.ReadingAvailable)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {

                        var sessionDelegate = new SessionDelegate(bytes);
                        var session = new NFCNdefReaderSession(sessionDelegate, null, true);
                        session.BeginSession();

                        var status = await sessionDelegate.WasDataTransmitted.Task;
                        if (status != NfcTransmissionStatus.Success)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "Suitable error message", "Ok");
                        }

                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "There was an error while trying to create a NFC session", "Ok");
                    }
                });
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Read is not supported by this tag", "Ok");
            }
        }

        public partial Task<bool> OpenNFCSettingsAsync() => Task.FromResult(true);
    }

}

