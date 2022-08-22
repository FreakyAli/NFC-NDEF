using System;

namespace NDEFMAUI.Interfaces
{
    public partial class NfcService
    {
        partial void ConfigureNfcAdapter() { }

        partial void EnableForegroundDispatch()
        {
        }

        partial void DisableForegroundDispatch()
        {
        }

        partial void UnconfigureNfcAdapter()
        {
        }

        partial Task SendAsync(List<byte[]> bytes, int delay);

        partial Task TestNfcAsync();

        partial Task<bool> OpenNFCSettingsAsync();
    }
}

