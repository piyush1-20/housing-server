using API.Dtos;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Property = API.Models.Property;

namespace API.Controllers
{
    public class PropertyController:BaseController
    {
        private readonly IUnitofWork uow;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public PropertyController(IUnitofWork uow,IMapper mapper,IPhotoService photoService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet("list/{sellRent}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyList(int sellRent)
        {
            var properties =  await uow.PropertyRepository.GetPropertiesAsync(sellRent);
            var propertydto = mapper.Map<IEnumerable<PropertyListDto>>(properties);
            return Ok(propertydto);
        }

        [HttpGet("detail/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyDetail(int id)
        {
            var property = await uow.PropertyRepository.GetPropertyDetailAsync(id);
            var propertydetaildto = mapper.Map<PropertyDetailDto>(property);
            return Ok(propertydetaildto);
        }
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddProperty(PropertyDto propertyDto)
        {
            try
            {
                var property = mapper.Map<Property>(propertyDto);
                var user = GetUserId();
                property.PostedBy = user;
                property.LastUpdatedBy = user;// Example for setting PostedBy
                uow.PropertyRepository.AddProperty(property);
                await uow.SaveAsync();
                return Ok(property);
            }
            catch (DbUpdateException ex)
            {
                // Log the error details for troubleshooting
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"Database update error: {errorMessage}");
                return StatusCode(500, "An error occurred while saving the property.");
            }
            catch (Exception ex)
            {
                // General error handling
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost("add/photo/{propId}")]
        [Authorize]
        public async Task<ActionResult<PhotoDto>>AddPropertyPhoto(IFormFile file,int propId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var result = await photoService.UploadPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var property = await uow.PropertyRepository.GetPropertyById(propId);

            var photo = new Photo
            {
                ImageUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(property.Photos.Count == 0)
            {
                photo.IsPrimary = true;
            }
            property.Photos.Add(photo);
            if(await uow.SaveAsync()) return Ok(mapper.Map<PhotoDto>(photo));
            return BadRequest("Some Problem occurred while uploading the photo");
        }

        [HttpPost("set-primary-photo/{propId}/{photoPublicId}")]
        [Authorize]
        public async Task<IActionResult> SetPrimaryPhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();
            Console.Write(userId);
            var property = await uow.PropertyRepository.GetPropertyById(propId);
            if (property == null)
            {
                return BadRequest("No such property or photo exist");
            }
            if (property.PostedBy != userId)
            {
                return BadRequest("You are not authorized");
            }

            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);
            if (photo == null)
            {
                return BadRequest("No such property or image exists");
            }

            if (photo.IsPrimary)
            {
                return BadRequest("This is already a primary photo");
            }

            var currPrimary = property.Photos.FirstOrDefault(p => p.IsPrimary);
            if (currPrimary != null) currPrimary.IsPrimary = false;
            photo.IsPrimary = true;
            if (await uow.SaveAsync()) return NoContent();
            return BadRequest("Failed to set primary photo");
        }


        [HttpDelete("delete-photo/{propId}/{photoPublicId}")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();
            Console.Write(userId);
            var property = await uow.PropertyRepository.GetPropertyById(propId);
            if (property == null)
            {
                return BadRequest("No such property or photo exist");
            }
            if (property.PostedBy != userId)
            {
                return BadRequest("You are not authorized");
            }

            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);
            if (photo == null)
            {
                return BadRequest("No such property or image exists");
            }

            if (photo.IsPrimary)
            {
                return BadRequest("You cannot delete primary photo");
            }

            //remove photo from cloudinary
            var result = photoService.DeletePhotoAsync(photo.PublicId);
            //if (result.Error != null)
            //    return BadRequest("You cannot delete primary photo");

            property.Photos.Remove(photo);

            if (await uow.SaveAsync()) return Ok();
            return BadRequest("Failed to delete primary photo");
        }
    }
}
