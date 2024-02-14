using System;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using NfcAdapter = Android.Nfc.NfcAdapter;
using Application = Xamarin.Forms.Application;
using Android.Nfc.Tech;
using NDEF.Interfaces;
using Xamarin.Essentials;
using NDEF.Droid.Enums;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using NDEF.Droid.Services;
using Debug = System.Diagnostics.Debug;

[assembly: Dependency(typeof(NativeNFCAdapterService))]
namespace NDEF.Droid.Services
{
    class NativeNFCAdapterService : INfcAdapter
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

        public NativeNFCAdapterService()
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

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                pendingIntent = PendingIntent.GetActivity(Platform.CurrentActivity, 0, intent, PendingIntentFlags.Mutable | PendingIntentFlags.UpdateCurrent);
            }
            else
            {
                pendingIntent = PendingIntent.GetActivity(Platform.CurrentActivity, 0, intent, 0);
            }

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
            if(pendingIntent== null ||writeTagFilters == null || techList==null || readerCallback== null)
            {
                return;
            }
            NfcAdapter?.EnableForegroundDispatch(Platform.CurrentActivity, pendingIntent, writeTagFilters, techList); //Foreground dispatch API enabled
            NfcAdapter?.EnableReaderMode(Platform.CurrentActivity, readerCallback, NfcReaderFlags.NfcA, null); //Reader mode API enabled
        }

        public void UnconfigureNfcAdapter()
        {
            Platform.ActivityStateChanged -= Platform_ActivityStateChanged;
        }

        public async Task ReadAsync()
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

                // this is where you get your data 
                Debug.WriteLine(ndef.NdefMessage);
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
            var tagDetectionTask = await Task.WhenAny(mainActivity.NfcTag.Task, readerCallback.NFCTag.Task);
            return await tagDetectionTask;
        }

        public Task<bool> OpenNFCSettingsAsync()
        {
            if (NfcStatus == NfcStatus.Unavailable)
            {
                return Task.FromResult(false);
            }

            if (NfcStatus == NfcStatus.Disabled)
            {
                var intent = Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean ?
                             new Intent(Android.Provider.Settings.ActionNfcSettings) :
                             new Intent(Android.Provider.Settings.ActionWirelessSettings);

                mainActivity?.StartActivity(intent);
            }
            return Task.FromResult(true);
        }

        public Task<bool> GetCurrentStatusAsync()
        {
            var status = false;

            if (NfcAdapter != null)
            {
                if (NfcAdapter.IsEnabled)
                {
                    status = true;
                }
            }

            return Task.FromResult(status);
        }
    }
}