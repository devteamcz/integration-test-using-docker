namespace BareBones.Api.Domain;

public interface IDocumentRepository
{
    ValueTask<Document?> GetAsync(long documentId);

    Task SaveAsync(Document document);
    
    Task UpdateAsync(Document document);
}