using BareBones.Api.Domain;

namespace BareBones.Api.Persistence;

public class DocumentRepository : EfRepositoryBase<Document,long>, IDocumentRepository
{
    public DocumentRepository(BareBonesContext context) : base(context)
    {
    }
}