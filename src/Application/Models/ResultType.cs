namespace HikingTrailsApi.Application.Models
{
    public enum ResultType
    {
        Success = 0,        //200
        Created = 1,        //201
        NoContent = 2,      //204
        Unauthorized = 3,   //401
        Forbidden = 4,      //403
        NotFound = 5,       //404
        Conflict = 6,       //409
        BadRequest = 7,     //400
        Unknown = 100       //500
    }
}
