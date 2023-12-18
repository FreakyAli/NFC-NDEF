using System.Text;
using System.Windows.Input;
using NDEF.MAUI.Interfaces;
using NDEF.MAUI.Platforms;

namespace NDEF.MAUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INfcService nfcAdapter;
        private string stringData;

        public string StringData
        {
            get => stringData;
            set => SetProperty(ref stringData, value);
        }

        public ICommand StartNfcTransmissionCommand { get; set; }

        public MainViewModel()
        {
            this.nfcAdapter = new NfcService();
            StartNfcTransmissionCommand = new Command(() => ExecuteNfc());
            stringData = string.Empty;
        }

        public override void ExecuteOnAppearing()
        {
            base.ExecuteOnAppearing();
            MainThread.InvokeOnMainThreadAsync(async() =>
            {
                if (await nfcAdapter.OpenNFCSettingsAsync())
                {
                    nfcAdapter.ConfigureNfcAdapter();
                    nfcAdapter.EnableForegroundDispatch();
                    //var tag = nfcAdapter.
                }
            });
        }

        public override void ExecuteOnDisappearing()
        {
            base.ExecuteOnDisappearing();
            nfcAdapter.DisableForegroundDispatch();
            nfcAdapter.UnconfigureNfcAdapter();
        }

        private Task ExecuteNfc()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(stringData);
            return nfcAdapter.SendAsync(bytes);
        }

    }
}

