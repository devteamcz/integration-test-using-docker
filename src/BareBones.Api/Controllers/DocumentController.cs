using System.Net;
using BareBones.Api.Application;
using BareBones.Api.Domain;
using BareBones.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BareBones.Api.Controllers;

[ApiController]
[Route("api/documents")]
[Produces("application/json")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    
    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    
    [HttpPost]
    [ProducesResponseType(typeof(DocumentDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> CreateAsync([FromBody] CreateOrUpdateDocumentDto documentDto)
    {
        var document = await _documentService.CreateDocumentAsync(documentDto.Name);

        return new JsonResult(ToDto(document));
    }
    
    [HttpPut("{documentId}")]
    [ProducesResponseType(typeof(DocumentDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> UpdateAsync([FromRoute] long documentId, [FromBody] CreateOrUpdateDocumentDto documentDto)
    {
        var document = await _documentService.UpdateDocumentAsync(documentId, documentDto.Name);
        if (document == null)
        {
            return NotFound();
        }

        return new JsonResult(ToDto(document));
    }
    
    [HttpGet("{documentId}")]
    [ProducesResponseType(typeof(DocumentDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetAsync([FromRoute] long documentId)
    {
        var document = await _documentService.GetDocumentAsync(documentId);
        if (document == null)
        {
            return NotFound();
        }

        return new JsonResult(ToDto(document));
    }

    private static DocumentDto ToDto(Document document)
    {
        return new DocumentDto { Name = document.Name, DocumentId = document.DocumentId};
    }
}