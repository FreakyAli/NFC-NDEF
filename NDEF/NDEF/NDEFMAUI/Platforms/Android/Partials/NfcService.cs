using System;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using NDEFMAUI.Enums;
using NfcAdapter = Android.Nfc.NfcAdapter;
using Application = Microsoft.Maui.Controls.Application;
using static Android.Nfc.NfcAdapter;
using Android.Nfc.Tech;
using Org.Apache.Commons.Logging;
using System.Runtime.Serialization;

namespace NDEFMAUI.Partials
{
    public partial class NfcService
    {
        private readonly MainActivity mainActivity = (MainActivity)Platform.CurrentActivity;
        private Lazy<NfcAdapter> lazynfcAdapter = new Lazy<NfcAdapter>(() => NfcAdapter.GetDefaultAdapter(Platform.CurrentActivity));

        private NfcAdapter NfcAdapter => lazynfcAdapter.Value;
        private PendingIntent pendingIntent;
        private IntentFilter[] writeTagFilters;
        private string[][] techList;
        private ReaderCallback readerCallback;
        public static Tag DetectedTag { get; set; }

        private NfcStatus NfcStatus => NfcAdapter == null ? NfcStatus.Unavailable :
                                                            NfcAdapter.IsEnabled ? NfcStatus.Enabled : NfcStatus.Disabled;
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

        public partial void ConfigureNfcAdapter()
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

        public partial void DisableForegroundDispatch()
        {
            NfcAdapter?.DisableForegroundDispatch(Platform.CurrentActivity);
            NfcAdapter?.DisableReaderMode(Platform.CurrentActivity);
        }

        public partial void EnableForegroundDispatch()
        {
            NfcAdapter?.EnableForegroundDispatch(Platform.CurrentActivity, pendingIntent, writeTagFilters, techList);
            Task.Delay(10000);

            if (null == DetectedTag)
                NfcAdapter?.EnableReaderMode(Platform.CurrentActivity, readerCallback, NfcReaderFlags.NfcA, null);
        }

        public partial void UnconfigureNfcAdapter()
        {
            Platform.ActivityStateChanged -= Platform_ActivityStateChanged;
        }

        public async partial Task SendAsync(byte[] bytes)
        {
            Ndef ndef = null;
            try
            {
                if (null == DetectedTag)
                    DetectedTag = await GetDetectedTag();
                else
                    loggerService.TrackMessage("SendAsync", "Tag has beed already detected");



                dialogTextView.Text = Strings.NfcPage_TagDetected;

                ndef = Ndef.Get(DetectedTag);
                if (ndef == null)
                    return;

                if (!ndef.IsWritable)
                {
                    loggerService.TrackMessage(LoggingConstants.NFCNonWriteable);

                    await alertService.DisplayAlertAsync(Strings.Global_Error, Strings.NfcPage_TagIsReadonly);
                    return;
                }

                if (!ndef.IsConnected)
                {
                    loggerService.TrackMessage(LoggingConstants.NFCTagConnectingBegin);

                    await ndef.ConnectAsync();

                    loggerService.TrackMessage(LoggingConstants.NFCTagConnected);
                }

                await WriteToTag(ndef, chunkedBytes, delay);
            }
            catch (IOException)
            {
                await alertService.DisplayAlertAsync(Strings.Global_Error, Strings.NfcPage_TransmissionError);
            }
            catch (Exception)
            {
                await alertService.DisplayAlertAsync(Strings.Global_Error, Strings.Error_Message);
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

        private async Task WriteToTag(Ndef ndef, byte[] chunkedBytes, int delay)
        {


            var ndefRecord = new NdefRecord(NdefRecord.TnfWellKnown, NdefRecord.RtdText?.ToArray(), new byte[0], chunk);
            NdefRecord[] records = { ndefRecord };
            NdefMessage message = new NdefMessage(records);
            ndef.WriteNdefMessage(message);
            await Task.Delay(delay);
            await Application.Current.MainPage.DisplayAlert("NFC", "Write Successful", "Ok");
        }

        public async partial Task<bool> OpenNFCSettingsAsync()
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

