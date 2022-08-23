using System;
using Android.Nfc;
using System.Windows.Input;
using System.Text;

namespace NDEFMAUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly NDEFMAUI.Partials.NfcService nfcAdapter;
        private string stringData;

        public string StringData
        {
            get => stringData;
            set => SetProperty(ref stringData, value);
        }

        public ICommand StartNfcTransmissionCommand { get; set; }

        public MainViewModel()
        {
            this.nfcAdapter = new NDEFMAUI.Partials.NfcService();
            StartNfcTransmissionCommand = new Command(()=> ExecuteNfc());
            stringData = string.Empty;
        }

        public async override void ExecuteOnAppearing()
        {
            base.ExecuteOnAppearing();
            if (await nfcAdapter.OpenNFCSettingsAsync())
            {
                nfcAdapter.ConfigureNfcAdapter();
                nfcAdapter.EnableForegroundDispatch();
            }
        }

        public override void ExecuteOnDisappearing()
        {
            base.ExecuteOnDisappearing();
            Partials.NfcService.DisableForegroundDispatch();
            nfcAdapter.UnconfigureNfcAdapter();
        }

        private Task ExecuteNfc()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(stringData);
            return nfcAdapter.SendAsync(bytes);
        }

    }

}

