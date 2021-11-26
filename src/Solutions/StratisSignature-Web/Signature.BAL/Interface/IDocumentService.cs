using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Signature.Shared.Models;

namespace Signature.BAL.Interface
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetDocuments(Guid userId);

        Task<string> AddDocument(Document document);

        Task UpdateDocumentCreationHash(Guid documentId, string creationHash);

        Task<IEnumerable<DocumentSigner>> GetDocumentSigners(Guid documentId);

        Task<DocumentDetail> GetDocumentDetail(Guid documentId);

        Task SignDocument(SignDocument signDocument);

        Task<DocumentCounts> GetDocumentCounts(Guid userId);

        Task<Shape> GetShapeForSign(Guid documentId, Guid userId);

        Task<Document> SearchDocument(string docSingedHash);
    }
}
