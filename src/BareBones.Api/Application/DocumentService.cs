using BareBones.Api.Domain;

namespace BareBones.Api.Application;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;


    public DocumentService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public ValueTask<Document?> GetDocumentAsync(long documentId)
    {
        return _documentRepository.GetAsync(documentId);
    }

    public async Task<Document> CreateDocumentAsync(string name)
    {
        var document = new Document(name);
        await _documentRepository.SaveAsync(document);
        return document;
    }

    public async Task<Document?> UpdateDocumentAsync(long documentId, string name)
    {
        var document = await _documentRepository.GetAsync(documentId);
        if (document == null)
        {
            return null;
        }
        
        document.UpdateName(name);
        await _documentRepository.UpdateAsync(document);

        return document;
    }
}