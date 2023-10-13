using AutoMapper;
using Docs.Api.Interfaces;
using Docs.Api.Models.Responses;

namespace Docs.Api.Services
{
    public class PagingService<DbModel, DtoModel> : IPagingService<DbModel, DtoModel> where DbModel : class where DtoModel : class
    {
        private readonly IMapper _mapper;

        public PagingService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public PageResponse<DtoModel> Page(List<DbModel> items, int startingIndex = 1, float pageTotal = 12)
        {
            var pageCount = Math.Ceiling(items.Count / pageTotal);

            items = items
                .Skip((startingIndex - 1) * (int)pageTotal)
                .Take((int)pageTotal)
                .ToList();

            return new PageResponse<DtoModel>
            {
                Payload = _mapper.Map<List<DtoModel>>(items),
                StartingIndex = startingIndex,
                PageTotal = (int)pageCount
            };
        }
    }
}
