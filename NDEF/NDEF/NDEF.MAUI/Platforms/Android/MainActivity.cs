using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Android.Content;
using Android.Util;


namespace NDEF.MAUI;

[MetaData(NfcAdapter.ActionTechDiscovered, Resource = "@xml/nfc_tech_filter")]
[IntentFilter(new[] { NfcAdapter.ActionTechDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
[IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
[IntentFilter(new[] { NfcAdapter.ActionTagDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

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
                NDEF.MAUI.Platforms.NfcService.DetectedTag = tag;
            }
        }
    }
}

