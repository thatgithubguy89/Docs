using Docs.UI.Interfaces;
using Docs.UI.Models;
using Microsoft.AspNetCore.Components;

namespace Docs.UI.Pages
{
    public partial class SingleDocument : ComponentBase
    {
        [Inject]
        public IDocumentService DocumentService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Id { get; set; }

        private Document document = new Document();

        protected override async Task OnInitializedAsync()
        {
            document = await DocumentService.GetDocumentByIdAsync(Id);
        }

        private async Task DeleteDocument()
        {
            await DocumentService.DeleteDocumentAsync(document.Id);

            NavigationManager.NavigateTo("/");
        }

        private async Task UpdateDocument()
        {
            await DocumentService.UpdateDocumentAsync(document.Id, document);
        }
    }
}
