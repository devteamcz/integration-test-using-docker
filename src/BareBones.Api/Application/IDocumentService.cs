using BareBones.Api.Domain;

namespace BareBones.Api.Application;

public interface IDocumentService
{
    ValueTask<Document?> GetDocumentAsync(long documentId);
    
    Task<Document> CreateDocumentAsync(string name);
    
    Task<Document?> UpdateDocumentAsync(long documentId, string name);
}