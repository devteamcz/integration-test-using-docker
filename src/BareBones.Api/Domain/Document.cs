using System.ComponentModel.DataAnnotations.Schema;

namespace BareBones.Api.Domain;

public class Document
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long DocumentId { get; protected set; }
    
    public string Name { get; protected set; }

    
    public Document(string name)
    {
        Name = name;
    }


    public void UpdateName(string name)
    {
        Name = name;
    }
}