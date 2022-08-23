using System;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Application = Microsoft.Maui.Controls.Application;
using NDEF.MAUI.Enums;
using static Android.Nfc.NfcAdapter;
using NDEF.MAUI.Interfaces;

namespace NDEF.MAUI.Platforms
{
    public class NfcService : INfcService
    {
        private readonly MainActivity mainActivity = (MainActivity)Platform.CurrentActivity;
        private Lazy<NfcAdapter> lazynfcAdapter = new Lazy<NfcAdapter>(() => NfcAdapter.GetDefaultAdapter(Platform.CurrentActivity));
        private NfcAdapter NfcAdapter => lazynfcAdapter.Value;
        private PendingIntent pendingIntent;
        private IntentFilter[] writeTagFilters;
        private string[][] techList;
        private ReaderCallback readerCallback;
        private NfcStatus NfcStatus => NfcAdapter == null ?
                                      NfcStatus.Unavailable : NfcAdapter.IsEnabled ?
                                      NfcStatus.Enabled : NfcStatus.Disabled;

        public static Tag DetectedTag { get; set; }

        public NfcService()
        {
            Platform.ActivityStateChanged += Platform_ActivityStateChanged;
        }

        private void Platform_ActivityStateChanged(object sender, ActivityStateChangedEventArgs e)
        {
            switch (e.State)
            {
                case ActivityState.Resumed:
                    EnableForegroundDispatch();
                    break;

                case ActivityState.Paused:
                    DisableForegroundDispatch();
                    break;
            }
        }

        public void ConfigureNfcAdapter()
        {
            IntentFilter tagdiscovered = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            IntentFilter ndefDiscovered = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
            IntentFilter techDiscovered = new IntentFilter(NfcAdapter.ActionTechDiscovered);
            tagdiscovered.AddCategory(Intent.CategoryDefault);
            ndefDiscovered.AddCategory(Intent.CategoryDefault);
            techDiscovered.AddCategory(Intent.CategoryDefault);

            var intent = new Intent(mainActivity, mainActivity.Class).AddFlags(ActivityFlags.SingleTop);
            pendingIntent = PendingIntent.GetActivity(mainActivity, 0, intent, 0);

            techList = new string[][]
            {
                new string[] { nameof(NfcA) },
                new string[] { nameof(NfcB) },
                new string[] { nameof(NfcF) },
                new string[] { nameof(NfcV) },
                new string[] { nameof(IsoDep) },
                new string[] { nameof(NdefFormatable) },
                new string[] { nameof(MifareClassic) },
                new string[] { nameof(MifareUltralight) },
            };
            readerCallback = new ReaderCallback();
            writeTagFilters = new IntentFilter[] { tagdiscovered, ndefDiscovered, techDiscovered };
        }

        public void DisableForegroundDispatch()
        {
            NfcAdapter?.DisableForegroundDispatch(Platform.CurrentActivity); //Foreground dispatch API disabled
            NfcAdapter?.DisableReaderMode(Platform.CurrentActivity); //Reader mode API disabled
        }

        public void EnableForegroundDispatch()
        {
            NfcAdapter?.EnableForegroundDispatch(Platform.CurrentActivity, pendingIntent, writeTagFilters, techList); //Foreground dispatch API enabled
            NfcAdapter?.EnableReaderMode(Platform.CurrentActivity, readerCallback, NfcReaderFlags.NfcA, null); //Reader mode API enabled
        }

        public void UnconfigureNfcAdapter()
        {
            Platform.ActivityStateChanged -= Platform_ActivityStateChanged;
        }

        public async Task SendAsync(byte[] bytes)
        {
            Ndef ndef = null;
            try
            {
                if (null == DetectedTag)
                    DetectedTag = await GetDetectedTag();

                ndef = Ndef.Get(DetectedTag);
                if (ndef == null)
                    return;

                if (!ndef.IsWritable)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Tag is readonly", "Ok");
                    return;
                }

                if (!ndef.IsConnected)
                {
                    await ndef.ConnectAsync();
                }

                await WriteToTag(ndef, bytes);
            }
            catch (IOException)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "There was an error while transmission of data, this could be caused because the device was moved away from the tag", "Ok");
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "There was an error while making this request", "Ok");
            }
            finally
            {
                if (ndef?.IsConnected == true)
                    ndef.Close();

                ndef = null;
                DetectedTag = null;
            }
        }

        private async Task<Tag> GetDetectedTag()
        {
            mainActivity.NfcTag = new TaskCompletionSource<Tag>();
            readerCallback.NFCTag = new TaskCompletionSource<Tag>();
            var tagDetectionTask = await Task.WhenAny(mainActivity.NfcTag.Task, readerCallback.NFCTag.Task);//.TimeoutAfter(TimeSpan.FromSeconds(60));
            return await tagDetectionTask;
        }

        private async Task WriteToTag(Ndef ndef, byte[] chunkedBytes)
        {
            var ndefRecord = new NdefRecord(NdefRecord.TnfWellKnown, NdefRecord.RtdText?.ToArray(), Array.Empty<byte>(), chunkedBytes);
            NdefRecord[] records = { ndefRecord };
            NdefMessage message = new NdefMessage(records);
            ndef.WriteNdefMessage(message);
            await Application.Current.MainPage.DisplayAlert("NFC", "Write Successful", "Ok");
        }

        public async Task<bool> OpenNFCSettingsAsync()
        {
            if (NfcStatus == NfcStatus.Unavailable)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "NFC is not supported", "Ok");
                return false;
            }

            if (NfcStatus == NfcStatus.Disabled)
            {
                await Application.Current.MainPage.DisplayAlert("Disabled", "NFC is disabled", "Ok");

                var intent = Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean ?
                             new Intent(Android.Provider.Settings.ActionNfcSettings) :
                             new Intent(Android.Provider.Settings.ActionWirelessSettings);

                mainActivity?.StartActivity(intent);
            }

            return true;

        }
    }
}

