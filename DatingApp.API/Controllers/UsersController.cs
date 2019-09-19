using System.Collections;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DatingApp.API.Helpers;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using DatingApp.API.Models;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;
            if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            var users = await _repo.GetUsers(userParams);


            // var dtoList = users.Select(t=> new UserForListDTO(){
            //      Id = t.Id,
            //      Username = t.Username,
            //      Gender = t.Gender,
            //      Age = t.DateOfBirth.CalculateAge(),
            //      KnownAs = t.KnownAs,
            //      Created = t.Created,
            //      LastActive = t.LastActive,
            //      City = t.City,
            //      Country = t.Country,
            //      PhotoUrl = t.Photos.SingleOrDefault(y=>y.IsMain).Url
            //     }).OrderBy(t=>t.Created);
                 
            // return Ok(dtoList); 

            //auto mapper issue to resolve later
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            Response.AddPagination(users.CurrentPage,users.PageSize,
             users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            // var dtoUser = new UserForDetailedDTO(){
            //      Id =  user.Id,
            //      Username = user.Username,
            //      Gender = user.Gender,
            //      Age = user.DateOfBirth.CalculateAge(),
            //      KnownAs = user.KnownAs,
            //      Created = user.Created,
            //      LastActive = user.LastActive,
            //      City = user.City,
            //      Country = user.Country,
            //      PhotoUrl = user.Photos.SingleOrDefault(y=>y.IsMain).Url,
            //      Introduction = user.Introduction,
            //      Photos = user.Photos.Select(t=> new PhotosForDetailDTO()
            //      { 

            //      })
            //     };

            //return Ok(dtoUser);
            //auto mapper issue to resolve later
            var userToReturn = _mapper.Map<UserForDetailedDTO>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, 
        UserForUpdatesDTO userForUpdatesDTO)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);
            _mapper.Map(userForUpdatesDTO,userFromRepo);

            if( await _repo.SaveAll())
                return NoContent();
            
            throw new Exception($"Update of user with id {id} is failed");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id , int recipientId)
        {
             if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var like = await _repo.GetLike(id,recipientId);
                if(like!=null)
                    return BadRequest("You already liked this user");

                if(await _repo.GetUser(recipientId) == null){
                    return NotFound();
                }

                like = new Like(){LikerId = id, LikeeId = recipientId};

                _repo.Add(like);

                if(await _repo.SaveAll()) {
                    return Ok();
                }

                return BadRequest("Failed to like the user");
        }

        

    }
}