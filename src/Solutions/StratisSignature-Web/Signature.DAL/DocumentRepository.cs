using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Signature.DAL.Interface;
using Signature.Shared.Models;
using Signature.Utility.Mapping;

namespace Signature.DAL
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IDatabaseSettings _databaseSettings;

        public DocumentRepository(IDatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public async Task<IEnumerable<Document>> GetDocuments(Guid userId)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[GetDocuments]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var list = new List<Document>();

                        while (await reader.ReadAsync())
                            list.Add(SqlMapper.Map<Document>(reader));

                        return list;
                    }
                }
            }
        }

        public async Task<string> AddDocument(Document document)
        {
            using (var con = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "[dbo].[AddDocument]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DocumentName", SqlDbType.VarChar).Value = document.DocumentName;
                    cmd.Parameters.Add("@FileName", SqlDbType.VarChar).Value = document.FileName;
                    cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = document.Description;
                    cmd.Parameters.Add("@DocumentHash", SqlDbType.VarChar).Value = document.DocumentHash;
                    cmd.Parameters.Add("@DocumentData", SqlDbType.VarBinary).Value = document.Data;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = document.CreatedBy;
                    cmd.Parameters.Add("@DocumentStatus", SqlDbType.Bit).Value = (int)document.Status;
                    cmd.Parameters.Add("@SignersUserIds", SqlDbType.VarChar).Value =
                        string.Join(',', document.SignerIds);

                    SqlParameter outPutPara = new SqlParameter
                    {
                        ParameterName = "@DocumentId",
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outPutPara);

                    await cmd.ExecuteNonQueryAsync();
                    var newDocumentId = outPutPara.Value;
                    return newDocumentId.ToString();
                }
            }
        }

        public async Task UpdateDocumentCreationHash(Guid documentId, string creationHash)
        {
            using (var con = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "[dbo].[UpdateDocCreationHash]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = documentId;
                    cmd.Parameters.Add("@CreationHash", SqlDbType.VarChar).Value = creationHash;

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<DocumentSigner>> GetDocumentSigners(Guid documentId)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[GetDocumentSigners]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = documentId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var list = new List<DocumentSigner>();

                        while (await reader.ReadAsync())
                            list.Add(SqlMapper.Map<DocumentSigner>(reader));

                        return list;
                    }
                }
            }
        }

        public async Task<DocumentDetail> GetDocumentDetail(Guid documentId)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[GetDocumentDetail]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = documentId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var docDetail = new DocumentDetail();

                        while (await reader.ReadAsync())
                            docDetail.Document = SqlMapper.Map<Document>(reader);

                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                            docDetail.Signers.Add(SqlMapper.Map<DocumentSigner>(reader));

                        return docDetail;
                    }
                }
            }
        }

        public async Task SignDocument(SignDocument signDocument)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[SignDocument]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = signDocument.DocumentId;
                    cmd.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = signDocument.UserId;
                    cmd.Parameters.Add("@SignTxHash", SqlDbType.VarChar).Value = signDocument.SignTransactionHash;
                    cmd.Parameters.Add("@SignedDocHash", SqlDbType.VarChar).Value = signDocument.SignedDocHash;
                    cmd.Parameters.Add("@SignedDocData", SqlDbType.VarBinary).Value = signDocument.SignedDocumentData;
                    cmd.Parameters.Add("@BlockNumber", SqlDbType.BigInt).Value = signDocument.BlockNumber;

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<DocumentCounts> GetDocumentCounts(Guid userId)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[GetDocumentCounts]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dashboard = new DocumentCounts();

                        while (await reader.ReadAsync())
                            dashboard.AwaitingSignCount = Convert.ToInt32(reader["AwaitingSignCount"]);

                        await reader.NextResultAsync();

                        while (await reader.ReadAsync())
                            dashboard.CompletedCount = Convert.ToInt32(reader["CompletedCount"]);

                        return dashboard;
                    }
                }
            }
        }

        public async Task<Shape> GetShapeForSign(Guid documentId, Guid userId)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[GetDocumentShapes]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = documentId;
                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var shape = new Shape();

                        while (await reader.ReadAsync())
                            shape = SqlMapper.Map<Shape>(reader);

                        return shape;
                    }
                }
            }
        }

        public async Task<Document> SearchDocument(string docSingedHash)
        {
            using (var connection = new SqlConnection(_databaseSettings.ConnectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("[dbo].[SearchDocument]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SignedDocHash", SqlDbType.VarChar).Value = docSingedHash;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var document = new Document();

                        while (await reader.ReadAsync())
                            document = SqlMapper.Map<Document>(reader);                        

                        return document;
                    }
                }
            }
        }
    }
}
