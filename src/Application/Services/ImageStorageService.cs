using HikingTrailsApi.Application.Common.Helpers;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly IDateTime _dateTime;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageStorageService(IDateTime dateTime, IWebHostEnvironment webHostEnvironment)
        {
            _dateTime = dateTime;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Image> SaveImage(IFormFile formFile)
        {
            //TODO: server side image resizing
            if (formFile == null) return null;

            var imageFileName = GuidHelper.ToUrlFriendlyString(Guid.NewGuid()) + ".jpeg";

            var image = new Image
            {
                CreationDate = _dateTime.Now,
                Path = imageFileName
            };

            using (var fileStream = new FileStream(
                Path.Combine(_webHostEnvironment.ContentRootPath, "Images", imageFileName),
                FileMode.Create, FileAccess.Write))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return image;
        }
    }
}
