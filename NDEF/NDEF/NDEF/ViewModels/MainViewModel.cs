using System;
using NDEF.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NDEF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INfcAdapter nfcAdapter;
        private string stringData;

        public string StringData
        {
            get => stringData;
            set => SetProperty(ref stringData, value);
        }

        public ICommand StartNfcTransmissionCommand { get; set; }

        public MainViewModel()
        {
            this.nfcAdapter = DependencyService.Get<INfcAdapter>();
            StartNfcTransmissionCommand = new AsyncCommand(ExecuteNfc);
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

