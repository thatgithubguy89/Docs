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

        private List<int> pageList = new List<int>();

        private int pageTotal = 1;

        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await GetDocuments();

            for (int i = 0; i < pageTotal; i++)
            {
                pageList.Add(i + 1);
            }

            isLoading = false;
        }

        private void NavigateToSingleDocument(string id)
        {
            NavigationManager.NavigateTo($"/singledocument/{id}");
        }

        private async Task GetDocuments(int page = 1)
        {
            var response = await DocumentService.GetAllDocumentsAsync(page);

            pageTotal = response.PageTotal;
            documents = response.Payload;
        }
    }
}
