using Docs.UI.Interfaces;
using Docs.UI.Models;
using Microsoft.AspNetCore.Components;

namespace Docs.UI.Pages
{
    public partial class CreateDocument : ComponentBase
    {
        [Inject]
        public IDocumentService DocumentService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private Document document = new Document();

        private async Task AddDocument()
        {
            await DocumentService.AddDocumentAsync(document);

            NavigationManager.NavigateTo("/");
        }
    }
}
