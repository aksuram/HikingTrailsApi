using AutoMapper;
using AutoMapper.QueryableExtensions;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Posts.Commands.CreatePost
{
    public class CreatePostCommand : IRequest<Result<PostVm>>
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<PostVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTime _dateTime;

        public CreatePostCommandHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IDateTime dateTime)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _dateTime = dateTime;
        }

        public async Task<Result<PostVm>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var createPostCommandValidator = new CreatePostCommandValidator();
            var validationResult = await createPostCommandValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<PostVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);

            var post = new Post
            {
                Title = request.Title,
                Body = request.Body,
                UserId = userId,
                CreationDate = _dateTime.Now
            };

            //TODO: Might need changing. Mapping from Post to PostVm without User entity the PostVm user fullName can't be formed

            _applicationDbContext.Posts.Add(post);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            //TODO: Remove if unused
            //var postWithAdditionalData = await _applicationDbContext.Posts
            //    .AsNoTracking()
            //    .ProjectTo<PostWithUserRatingVm>(_mapper.ConfigurationProvider)
            //    .FirstOrDefaultAsync(x => x.Id)

            return Result<PostVm>.Created(_mapper.Map<Post, PostVm>(post)); //201
        }
    }
}
