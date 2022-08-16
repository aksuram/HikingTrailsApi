namespace HikingTrailsApi.Application.Common.Interfaces
{
    public interface IPaginatedListQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
