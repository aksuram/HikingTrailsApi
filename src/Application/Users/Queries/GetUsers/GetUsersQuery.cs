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

namespace HikingTrailsApi.Application.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<Result<PaginatedList<UserVm>>>, IPaginatedListQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserVm>>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedList<UserVm>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var paginatedListValidator = new PaginatedListValidator();
            var validationResult = await paginatedListValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<PaginatedList<UserVm>>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var userVmList = await _applicationDbContext.Users
                .AsNoTracking()
                .ProjectTo<UserVm>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            if (userVmList.Items.Count == 0)
            {
                return Result<PaginatedList<UserVm>>.NotFound("PageNumber", "Nepavyko surasti naudotojų duotame puslapyje");    //404
            }

            return Result<PaginatedList<UserVm>>.Success(userVmList);   //200
        }
    }
}
