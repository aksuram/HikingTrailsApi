using HikingTrailsApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Interfaces
{
    public interface IImageStorageService
    {
        public Task<Image> SaveImage(IFormFile formFile);
    }
}
