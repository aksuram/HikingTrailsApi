using System.Collections.Generic;

namespace HikingTrailsApi.Application.Comments
{
    public class FormattedCommentListVm
    {
        public int CommentCount { get; set; }
        public List<CommentWithUserRatingVm> Comments { get; set; }
    }
}
