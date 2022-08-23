using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreNFC;
using Foundation;
using NDEF.iOS.Enums;
using UIKit;
using Xamarin.Essentials;

namespace NDEF.iOS.Services
{
    public class SessionDelegate : NFCNdefReaderSessionDelegate
    {
        private readonly byte[] bytes;
        private readonly string WriteMessage;

        public TaskCompletionSource<NfcTransmissionStatus> WasDataTransmitted { get; set; }

        public SessionDelegate(byte[] bytes)
        {
            this.bytes = bytes;
            WasDataTransmitted = new TaskCompletionSource<NfcTransmissionStatus>();
        }

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {

        }

        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                session.AlertMessage = "Tag detected";
                try
                {
                    if (tags.Length != 1)
                    {
                        session.InvalidateSession(errorMessage: "Cannot write on multiple tags at the same time");
                        WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                        return;
                    }

                    var NdefTag = tags.First();
                    session.ConnectToTag(NdefTag, (error) =>
                    {
                        if (error != null)
                        {
                            WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                            session.InvalidateSession("There was an error while making this request");
                            return;
                        }
                    });

                    NdefTag?.QueryNdefStatus((status, capacity, error) =>
                    {
                        if (error != null)
                        {
                            WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                            session.InvalidateSession("Could not query status of tag");
                        }

                        switch (status)
                        {
                            case NFCNdefStatus.NotSupported:
                                WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                                session.InvalidateSession("This is an unsupported tag");
                                break;

                            case NFCNdefStatus.ReadOnly:
                                WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                                session.InvalidateSession("This tag is readonly");
                                break;

                            case NFCNdefStatus.ReadWrite:
                                var isNfcWriteAvailable = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
                                if (isNfcWriteAvailable)
                                {
                                    session.AlertMessage = WriteMessage;
                                    var chunkString = Encoding.UTF8.GetString(bytes);
                                    var textPayload = NFCNdefPayload.CreateWellKnownTypePayload(chunkString);
                                    var ndefpayloadArray = new NFCNdefPayload[] { textPayload };
                                    var ndefMessage = new NFCNdefMessage(ndefpayloadArray);
                                    NdefTag.WriteNdef(ndefMessage, (tagError) =>
                                    {
                                        if (tagError != null)
                                        {
                                            WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                                            session.InvalidateSession("Falied to write this message");
                                        }
                                        else
                                        {
                                            session.AlertMessage = "Write successful";
                                            session.InvalidateSession();
                                            WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Success);
                                        }
                                    });
                                }
                                else
                                {
                                    WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                                    session.InvalidateSession("There was an error while trying to write on this tag");
                                }
                                break;

                            default:
                                WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Unknown);
                                session.InvalidateSession("Tag status unkoown");
                                break;
                        }
                    });
                }
                catch (Exception ex)
                {
                    session.InvalidateSession();
                    WasDataTransmitted.TrySetResult(NfcTransmissionStatus.Failed);
                }
            });
        }

        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            if (error.Code == (int)NFCReaderError.ReaderSessionInvalidationErrorSessionTimeout)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error ", "Tag search timed out", "Ok");
                });
            }
        }
    }

}

