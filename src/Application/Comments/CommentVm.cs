using System;

namespace HikingTrailsApi.Application.Comments
{
    public class CommentVm
    {
        public Guid Id { get; set; }
        public string Body { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }

        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public Guid PostId { get; set; }
        //TODO: Add PostTitle?

        public Guid? ReplyToId { get; set; }

        public int Rating { get; set; }
    }
}
