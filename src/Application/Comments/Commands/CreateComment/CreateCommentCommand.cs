using AutoMapper;
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

namespace HikingTrailsApi.Application.Comments.Commands.CreateComment
{
    public class CreateCommentCommand : IRequest<Result<CommentVm>>
    {
        public Guid PostId { get; set; }
        public string Body { get; set; }
        public Guid? ReplyToId { get; set; }
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTime _dateTime;

        public CreateCommentCommandHandler(IApplicationDbContext applicationDbContext,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, IDateTime dateTime)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _dateTime = dateTime;
        }

        public async Task<Result<CommentVm>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var createCommentCommandValidator = new CreateCommentCommandValidator();
            var validationResult = await createCommentCommandValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<CommentVm>.BadRequest(validationResult.Errors.Select(x =>
                    new FieldError(x.PropertyName, x.ErrorMessage)));   //400
            }

            var post = await _applicationDbContext.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.PostId, cancellationToken);

            if (post == null)
            {
                return Result<CommentVm>.NotFound("PostId", "Nerastas nurodytas įrašas");   //404
            }

            if (post.IsDeleted)
            {
                return Result<CommentVm>.NotFound("PostId", "Nurodytas įrašas yra ištrintas");   //404
            }

            //TODO: Test if a comment can reply to itself and if another verification needs to be written here
            if (request.ReplyToId.HasValue)
            {
                var replyToComment = await _applicationDbContext.Comments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.ReplyToId.Value, cancellationToken);

                if (replyToComment == null)
                {
                    return Result<CommentVm>.NotFound("ReplyToId", "Nerastas nurodytas komentaras");    //404
                }

                if (replyToComment.PostId != request.PostId)
                {
                    return Result<CommentVm>.BadRequest("PostId", "Nesutampa komentaro į kurį atsakoma įrašas ir kuriamo komentaro įrašas");    //400
                }

                if (replyToComment.ReplyToId.HasValue)
                {
                    return Result<CommentVm>.Conflict("ReplyToId", "Negalima atsakyti į komentaro atsakymą");   //409
                }
            }

            Guid.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirstValue("id"), out var userId);

            var comment = new Comment
            {
                Body = request.Body,
                CreationDate = _dateTime.Now,
                UserId = userId,
                PostId = request.PostId,
                ReplyToId = request.ReplyToId
            };

            //TODO: Might need changing. Mapping from Comment to CommentVm without User entity the CommentVm user fullName can't be formed

            _applicationDbContext.Comments.Add(comment);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result<CommentVm>.Created(_mapper.Map<Comment, CommentVm>(comment)); //201
        }
    }
}
