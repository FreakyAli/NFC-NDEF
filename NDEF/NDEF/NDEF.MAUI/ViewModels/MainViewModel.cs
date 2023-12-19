using System.Windows.Input;
using NDEF.MAUI.Interfaces;
using NDEF.MAUI.Platforms;

namespace NDEF.MAUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INfcService nfcAdapter;
        private string stringData;
        private bool showIndicator;

        public string StringData
        {
            get => stringData;
            set => SetProperty(ref stringData, value);
        }

        public bool ShowIndicator
        {
            get => showIndicator;
            set => SetProperty(ref showIndicator, value);
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
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (await nfcAdapter.OpenNFCSettingsAsync())
                {
                    nfcAdapter.ConfigureNfcAdapter();
                    nfcAdapter.EnableForegroundDispatch();
                }
            });
        }

        public override void ExecuteOnDisappearing()
        {
            base.ExecuteOnDisappearing();
            nfcAdapter.DisableForegroundDispatch();
            nfcAdapter.UnconfigureNfcAdapter();
        }

        private async Task ExecuteNfc()
        {
            ShowIndicator = true;
            await nfcAdapter.ReadAsync();
            ShowIndicator = false;
        }

    }
}