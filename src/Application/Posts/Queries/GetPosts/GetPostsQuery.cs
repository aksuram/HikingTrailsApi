using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Mappings;
using HikingTrailsApi.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Posts.Queries.GetPosts
{
    public class GetPostsQuery : IRequest<Result<PaginatedList<PostVm>>>, IPaginatedListQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, Result<PaginatedList<PostVm>>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public GetPostsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<PostVm>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
        {
            var paginatedListValidator = new PaginatedListValidator();
            var validationResult = await paginatedListValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<PaginatedList<PostVm>>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var postVmList = await _applicationDbContext.Posts
                .AsNoTracking()
                .Include(x => x.User)
                .ProjectTo<PostVm>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            if (postVmList.Items.Count == 0)
            {
                return Result<PaginatedList<PostVm>>.NotFound("PageNumber", "Nepavyko surasti įrašų duotame puslapyje");    //404
            }

            return Result<PaginatedList<PostVm>>.Success(postVmList);   //200
        }
    }
}
