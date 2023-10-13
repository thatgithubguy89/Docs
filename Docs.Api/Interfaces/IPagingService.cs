using Docs.Api.Models.Responses;

namespace Docs.Api.Interfaces
{
    public interface IPagingService<DbModel, DtoModel>
    {
        PageResponse<DtoModel> Page(List<DbModel> items, int startingIndex = 1, float pageTotal = 12);
    }
}
