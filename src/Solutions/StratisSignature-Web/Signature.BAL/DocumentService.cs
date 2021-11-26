using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Signature.BAL.Interface;
using Signature.DAL.Interface;
using Signature.Shared.Models;

namespace Signature.BAL
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<IEnumerable<Document>> GetDocuments(Guid userId)
        {
            return await _documentRepository.GetDocuments(userId);
        }

        public async Task<string> AddDocument(Document document)
        {
            return await _documentRepository.AddDocument(document);
        }

        public async Task UpdateDocumentCreationHash(Guid documentId, string creationHash)
        {
            await _documentRepository.UpdateDocumentCreationHash(documentId, creationHash);
        }

        public async Task<IEnumerable<DocumentSigner>> GetDocumentSigners(Guid documentId)
        {
            return await _documentRepository.GetDocumentSigners(documentId);
        }

        public async Task<DocumentDetail> GetDocumentDetail(Guid documentId)
        {
            return await _documentRepository.GetDocumentDetail(documentId);
        }

        public async Task SignDocument(SignDocument signDocument)
        {
            await _documentRepository.SignDocument(signDocument);
        }

        public async Task<DocumentCounts> GetDocumentCounts(Guid userId)
        {
            return await _documentRepository.GetDocumentCounts(userId);
        }

        public async Task<Shape> GetShapeForSign(Guid documentId, Guid userId)
        {
            return await _documentRepository.GetShapeForSign(documentId, userId);
        }

        public async Task<Document> SearchDocument(string docSingedHash)
        {
            return await _documentRepository.SearchDocument(docSingedHash);
        }
    }
}
