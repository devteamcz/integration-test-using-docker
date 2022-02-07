using System.Threading.Tasks;
using BareBones.Client;
using BareBones.Tests.Fixture;
using FluentAssertions;
using Xunit;

namespace BareBones.Tests;

public class DocumentIntegrationTest : IntegrationTestBase
{
    
    public DocumentIntegrationTest(TestingWebApplicationFactory factory) : base(factory)
    {
    }
    
    
    [Fact]
    public async Task TestCreate()
    {
        var httpClient = CreateIntegrationTestClient();
        
        // Arrange
        const string newName = "A";

        // Act
        var createdDocument = await httpClient.DocumentsPOSTAsync(new CreateOrUpdateDocumentDto { Name = newName });

        // Assert
        var document = await httpClient.DocumentsGETAsync(createdDocument.DocumentId.Value);
        
        document.DocumentId.Should().BePositive();
        document.Name.Should().Be(newName);
    }
    
    [Fact]
    public async Task TestUpdate()
    {
        var httpClient = CreateIntegrationTestClient();
        
        // Arrange
        var createdDocument = await httpClient.DocumentsPOSTAsync(new CreateOrUpdateDocumentDto { Name = "A" });
        const string updatedName = "B";

        // Act
        await httpClient.DocumentsPUTAsync(createdDocument.DocumentId.Value, new CreateOrUpdateDocumentDto { Name = updatedName });

        // Assert
        var document = await httpClient.DocumentsGETAsync(createdDocument.DocumentId.Value);

        document.DocumentId.Should().Be(createdDocument.DocumentId);
        document.Name.Should().Be(updatedName);
    }
}