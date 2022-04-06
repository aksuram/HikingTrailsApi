using System;

namespace HikingTrailsApi.Application.Comments
{
    public class CommentCreateDto
    {
        public string Body { get; set; }
        public Guid? ReplyToId { get; set; }
    }
}
