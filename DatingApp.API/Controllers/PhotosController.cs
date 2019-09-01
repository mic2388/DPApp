using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary cloudinary;
        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._mapper = mapper;
            this._repo = repo;

            Account account = new Account
            (
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );

             cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
             var photoFromRepo = await _repo.GetPhoto(id);
             var photos = Mapper.Map<PhotoForReturnDto>(photoFromRepo);
             return Ok(photos);
        }

        [HttpPost]
        
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromFormAttribute] PhotoForCreationDto photoforCreationDto )
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);
            var file = photoforCreationDto.File;

            var uploadResult = new ImageUploadResult();
            if(file.Length > 0){

                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation =  new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = cloudinary.Upload(uploadParams);
                }

            }

            photoforCreationDto.Url = uploadResult.Uri.ToString();
            photoforCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoforCreationDto);

            if(!userFromRepo.Photos.Any(t=>t.IsMain)){
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto",new {id = photo.Id},photoToReturn);
            }

            return BadRequest("Could not add the photo");

        }

        [HttpPost("{id}/SetMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var user = await _repo.GetUser(userId);
            
            if(!user.Photos.Any(t=>t.Id == id)){
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll()){
                return NoContent();
            }

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
           if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var user = await _repo.GetUser(userId);
            
            if(!user.Photos.Any(t=>t.Id == id)){
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo");

            if(photoFromRepo.PublicId !=null){
                var deleteParams  = new DeletionParams(photoFromRepo.PublicId);
                var result = cloudinary.Destroy(deleteParams);

                if(result.Result == "ok"){
                _repo.Delete(photoFromRepo);
                }
            }
            else{
                 _repo.Delete(photoFromRepo);
            }
         

            if(await _repo.SaveAll()){
                return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }

    }
}