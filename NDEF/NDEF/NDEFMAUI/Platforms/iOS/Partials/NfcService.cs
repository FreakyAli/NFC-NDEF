using System;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.VisualBasic;
using NDEFMAUI.Enums;

namespace NDEFMAUI.Partials
{
    public partial class NfcAdapter
    {

        public NfcAdapter()
        {
            //Platform.ActivityStateChanged += Platform_ActivityStateChanged;
        }

        public partial void ConfigureNfcAdapter()
        {

        }

        public partial void EnableForegroundDispatch()
        {

        }

        public partial void DisableForegroundDispatch()
        {

        }

        public partial void UnconfigureNfcAdapter()
        {

        }

        public async partial Task SendAsync(List<byte[]> bytes, int delay)
        {

        }

        public partial Task<bool> OpenNFCSettingsAsync()
        {
            return Task.FromResult(true);
        }
    }

}

