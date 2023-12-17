using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.Content;
using AndroidX.Core.Content;

namespace NDEF.MAUI;

[MetaData(NfcAdapter.ActionTechDiscovered, Resource = "@xml/nfc_tech_filter")]
[IntentFilter(new[] { NfcAdapter.ActionTechDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
[IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
[IntentFilter(new[] { NfcAdapter.ActionTagDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, Exported = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
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
            Tag tag;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
            {
                var cSharpType = typeof(Tag);
                var javaType = Java.Lang.Class.FromType(cSharpType);
                tag = (Tag)IntentCompat.GetParcelableExtra(intent,NfcAdapter.ExtraTag, javaType);
            }
            else
            {
                tag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
            }
            if (tag != null)
            {
                var isSuccess = NfcTag?.TrySetResult(tag);
                if (null == NfcTag || !isSuccess.Value) { }
                Platforms.NfcService.DetectedTag = tag;
            }
        }
    }
}