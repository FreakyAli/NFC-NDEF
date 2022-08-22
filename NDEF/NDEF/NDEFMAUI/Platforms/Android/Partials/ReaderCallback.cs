using Android.Nfc;
using NfcAdapter = Android.Nfc.NfcAdapter;

namespace NDEFMAUI.Partials
{
    public class ReaderCallback : Java.Lang.Object, NfcAdapter.IReaderCallback
    {
        public TaskCompletionSource<Tag> NFCTag { get; set; }


        public ReaderCallback()
        {

        }

        public void OnTagDiscovered(Tag tag)
        {
            var isSuccess = NFCTag?.TrySetResult(tag);
            if (null == NFCTag || !isSuccess.Value)
                NfcService.DetectedTag = tag;
        }
    }
}

