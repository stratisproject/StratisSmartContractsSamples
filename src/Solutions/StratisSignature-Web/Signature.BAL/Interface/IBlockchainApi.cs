using System.Threading.Tasks;
using Signature.Shared.Models;

namespace Signature.BAL.Interface
{
    public interface IBlockchainApi
    {
        Task<TransactionResponse> CreateDocument(string agreementId, string fileHash, string senderWalletAddress);
        Task<ApiLocalCallResponse<Agreement>> GetDocument(string agreementId, string senderWalletAddress);

        Task<TransactionResponse> AddSigner(string agreementId, string senderWalletAddress, string signerWalletAddress);

        Task<string> SignMessage(string message, string signerWalletAddress, string signerWalletPassword);

        Task<TransactionResponse> SignDocument(string agreementId, string signerWalletAddress, string digitalSign);

        Task<ApiLocalCallResponse<Stamp>> GetStamp(string agreementId, string signerWalletAddress);

        Task<TransactionReceipt> GetReceipt(string txHash);
    }
}
