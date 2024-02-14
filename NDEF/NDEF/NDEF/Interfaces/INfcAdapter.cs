using System.Threading.Tasks;

namespace NDEF.Interfaces
{
    public interface INfcAdapter
    {
        void ConfigureNfcAdapter();

        void EnableForegroundDispatch();

        void DisableForegroundDispatch();

        Task ReadAsync();

        Task<bool> OpenNFCSettingsAsync();

        void UnconfigureNfcAdapter();

        Task<bool> GetCurrentStatusAsync();
    }
}