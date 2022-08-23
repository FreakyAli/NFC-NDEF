using Android.Nfc;
using NfcAdapter = Android.Nfc.NfcAdapter;
using System.Threading.Tasks;

namespace NDEF.Droid.Services
{
    public class ReaderCallback : Java.Lang.Object, NfcAdapter.IReaderCallback
    {
        public TaskCompletionSource<Tag> NFCTag { get; set; }

        public void OnTagDiscovered(Tag tag)
        {
            var isSuccess = NFCTag?.TrySetResult(tag);
            if (null == NFCTag || !isSuccess.Value)
                NativeNFCAdapterService.DetectedTag = tag;
        }
    }
}

