using SaleDeedRegistry.Lib.Client;
using System.Threading.Tasks;

namespace SaleDeedRegistry.Lib.Command
{
    public interface ISaleDeedRegistryCommand
    {
        Task<ReceiptResponse> Execute(SaleDeedRegistryBaseRequest requestObject);
    }
}
