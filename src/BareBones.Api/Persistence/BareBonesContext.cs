using BareBones.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BareBones.Api.Persistence;

public class BareBonesContext : DbContext
{
    public DbSet<Document> Documents { get; set; }

    
    public BareBonesContext(DbContextOptions<BareBonesContext> options) : base(options)
    {
    }
}