using Docs.UI.Interfaces;
using Docs.UI.Models;
using Microsoft.AspNetCore.Components;

namespace Docs.UI.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public IDocumentService DocumentService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private List<Document> documents = new List<Document>();

        protected override async Task OnInitializedAsync()
        {
            documents = await DocumentService.GetAllDocumentsAsync();
        }

        private void NavigateToSingleDocument(string id)
        {
            NavigationManager.NavigateTo($"/singledocument/{id}");
        }
    }
}
