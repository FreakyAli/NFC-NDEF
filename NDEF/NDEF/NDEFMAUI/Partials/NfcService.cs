using System;

namespace NDEFMAUI.Partials
{
    public partial class NfcService
    {
        public partial void ConfigureNfcAdapter();

        public partial void EnableForegroundDispatch();

        public partial void DisableForegroundDispatch();

        public partial void UnconfigureNfcAdapter();

        public partial Task SendAsync(byte[] bytes);

        public partial Task<bool> OpenNFCSettingsAsync();
    }
}

