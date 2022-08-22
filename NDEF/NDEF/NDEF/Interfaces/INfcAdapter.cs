using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NDEF.Interfaces
{
    public interface INfcAdapter
    {
        void ConfigureNfcAdapter();

        void EnableForegroundDispatch();

        void DisableForegroundDispatch();

        Task SendAsync(byte[] bytes);

        Task<bool> OpenNFCSettingsAsync();

        void UnconfigureNfcAdapter();
    }
}

