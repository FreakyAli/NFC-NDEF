using System;
using Android.Content;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Nfc;
using System.Threading.Tasks;

namespace NDEF.Droid
{
    [MetaData(NfcAdapter.ActionTechDiscovered, Resource = "@xml/nfc_tech_filter")]
    [IntentFilter(new[] { NfcAdapter.ActionTechDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
    [IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
    [IntentFilter(new[] { NfcAdapter.ActionTagDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
    [Activity(Label = "NDEF", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public TaskCompletionSource<Tag> NfcTag { get; set; }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (NfcAdapter.ActionTagDiscovered.Equals(intent.Action) ||
                NfcAdapter.ActionTechDiscovered.Equals(intent.Action) ||
                NfcAdapter.ActionNdefDiscovered.Equals(intent.Action))
            {
                var tag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
                if (tag != null)
                {
                    var isSuccess = NfcTag?.TrySetResult(tag);
                    if (null == NfcTag || !isSuccess.Value) { }
                    Services.NativeNFCAdapterService.DetectedTag = tag;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
