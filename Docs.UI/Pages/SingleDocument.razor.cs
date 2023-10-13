using Docs.UI.Interfaces;
using Docs.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Docs.UI.Pages
{
    public partial class SingleDocument : ComponentBase
    {
        [Inject]
        public IDocumentService DocumentService { get; set; }

        [Inject]
        public IDownloadService DownloadService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Id { get; set; }

        private Document document = new Document();

        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            document = await DocumentService.GetDocumentByIdAsync(Id);

            isLoading = false;
        }

        private async Task DeleteDocument()
        {
            await DocumentService.DeleteDocumentAsync(document.Id);

            NavigationManager.NavigateTo("/");
        }

        private async Task DownloadDocument()
        {
            var fileStream = await DownloadService.GetFileMemoryStream(document.Id);
            var fileName = $"{document.Title}.pdf";

            using var streamRef = new DotNetStreamReference(stream: fileStream);

            await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }

        private async Task UpdateDocument()
        {
            await DocumentService.UpdateDocumentAsync(document.Id, document);
        }
    }
}
