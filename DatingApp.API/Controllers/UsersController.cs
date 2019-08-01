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

namespace DatingApp.API.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();


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
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
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

    }
}