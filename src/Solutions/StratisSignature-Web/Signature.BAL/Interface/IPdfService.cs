using System;
using System.Threading.Tasks;
using Signature.Shared.Models;

namespace Signature.BAL.Interface
{
    public interface IPdfService
    {
        Task<string> StampingFile(DocumentDetail documentDetail, Guid userId, string userFullName);        
    }
}
