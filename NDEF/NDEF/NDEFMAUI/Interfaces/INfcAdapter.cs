using System;

namespace NDEFMAUI.Interfaces
{
    public interface INfcAdapter
    {
        void ConfigureNfcAdapter()
        {
        }

        void EnableForegroundDispatch()
        {
        }

        void DisableForegroundDispatch()
        {
        }

        void UnconfigureNfcAdapter()
        {
        }

        Task SendAsync(List<byte[]> bytes, int delay);

        Task TestNfcAsync();

        Task<bool> OpenNFCSettingsAsync();
    }
}

