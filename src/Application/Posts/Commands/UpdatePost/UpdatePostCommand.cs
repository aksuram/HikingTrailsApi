using AutoMapper;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Domain.Entities;
using HikingTrailsApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace HikingTrailsApi.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommand : IRequest<Result<PostVm>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result<PostVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTime _dateTime;

        public UpdatePostCommandHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IDateTime dateTime)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _dateTime = dateTime;
        }

        //TODO: Make a different admin edit endpoint
        public async Task<Result<PostVm>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var updatePostCommandValidator = new UpdatePostCommandValidator();
            var validationResult = await updatePostCommandValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<PostVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var post = await _applicationDbContext.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (post == null)
            {
                return Result<PostVm>.NotFound("Id", "Nepavyko surasti įrašo"); //404
            }

            var userRole = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            var isNotAdmin = userRole != Role.Administrator.ToString();
            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);

            if (userId != post.UserId && isNotAdmin)
            {
                return Result<PostVm>.Forbidden("Authorization", "Naudotojas negali keisti kitų naudotojų įrašų");  //403
            }

            if (post.IsDeleted)
            {
                return Result<PostVm>.NotFound("Post deleted", "Įrašas yra ištrintas"); //404
            }

            if (isNotAdmin && post.CreationDate.AddHours(1) < _dateTime.Now)
            {
                return Result<PostVm>.Conflict("Time constraint", "Negalima redaguoti įrašo praėjus daugiau nei valandai nuo sukūrimo"); //409
            }

            await _applicationDbContext.Posts
                .Where(x => x.Id == request.Id)
                .UpdateAsync(x =>
                    new Post
                    {
                        Title = request.Title,
                        Body = request.Body,
                        EditDate = _dateTime.Now
                    },
                cancellationToken);

            post.Title = request.Title;
            post.Body = request.Body;
            post.EditDate = _dateTime.Now;

            //TODO: Might need to fetch Post creator(User) to properly map to PostVm
            return Result<PostVm>.Success(_mapper.Map<Post, PostVm>(post)); //200
        }
    }
}
