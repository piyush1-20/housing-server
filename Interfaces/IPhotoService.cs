﻿using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile photo);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
