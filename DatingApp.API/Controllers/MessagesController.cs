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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repository, IMapper mapper)
        {
            this._mapper = mapper;
            this._repository = repository;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repository.GetMessage(id);
            
            if(messageFromRepo==null)
                return NotFound();

            return Ok(messageFromRepo);

        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDTO messageForCreationDTO)
        {
            var sender = await _repository.GetUser(userId);

             if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDTO.SenderId = userId;

            var recipient = await _repository.GetUser(messageForCreationDTO.RecipientId);

            if(recipient==null)
                return BadRequest("Could not find user");
            
            var message = _mapper.Map<Message>(messageForCreationDTO);

            _repository.Add(message);

         
            if (await _repository.SaveAll())
            {
                    var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                    return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }

            throw new Exception("Message creation failed");

            //return BadRequest("Some error occured");
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUsers(int userId, [FromQuery]MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();


            messageParams.UserId = userId;

            var messagesfromRepo = await _repository.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesfromRepo);

            Response.AddPagination(messagesfromRepo.CurrentPage,messagesfromRepo.PageSize
            ,messagesfromRepo.TotalCount, messagesfromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messagesFromRepo = await _repository.GetMessageThread(userId,recipientId);
            var messageToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageToReturn);

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var messageFromRepo = await _repository.GetMessage(id);

                if(messageFromRepo.SenderId == userId){
                    messageFromRepo.SenderDeleted = true;
                }

                if(messageFromRepo.RecipientId == userId){
                    messageFromRepo.RecipientDeleted = true;
                }

                if(messageFromRepo.SenderDeleted == messageFromRepo.RecipientDeleted){
                    _repository.Delete(messageFromRepo);
                }

                if (await _repository.SaveAll()){
                    return NoContent();
                }

                throw new Exception("error deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _repository.GetMessage(id);
            if(message.RecipientId != userId)
            {
                return Unauthorized();
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _repository.SaveAll();

            return NoContent();

        }
    }
}