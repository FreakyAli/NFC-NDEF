using System;

namespace NDEFMAUI.Partials
{
    public partial class NfcService
    {
        /// <summary>
        /// Configuration for NFC service 
        /// </summary>
        public partial void ConfigureNfcAdapter();

        /// <summary>
        /// Enable NFC search
        /// </summary>
        public partial void EnableForegroundDispatch();

        /// <summary>
        /// Disable NFC search
        /// </summary>
        public partial void DisableForegroundDispatch();

        /// <summary>
        /// Unconfigure NFC services
        /// </summary>
        public partial void UnconfigureNfcAdapter();

        /// <summary>
        /// Send data over NFC 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public partial Task SendAsync(byte[] bytes);

        /// <summary>
        /// Open NFC settings, This is only for Android.
        /// </summary>
        /// <returns></returns>
        public partial Task<bool> OpenNFCSettingsAsync();
    }
}

