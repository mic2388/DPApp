using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
     //read this important
    // eager loading - needs to use include statement to explicitly load ddependencide
    // lazy loading just needs to configure in context class and 
    // add virtual keyword in entity classes where we reference other navigational 
    // properties , this will auto add whenever there is a need for dependent classes
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            var like = await _context.Likes.FirstOrDefaultAsync(u=>u.LikerId == userId && u.LikeeId == recipientId);
            return like;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            var photo =  await _context.Photos
            .Where(t=>t.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            return photo;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(t=> t.Id==id );
            return photo;

        }

        public async Task<User> GetUser(int id)
        {
            //_context.Users.Include(p=>p.Photos) - for eager
            var user = await _context.Users.FirstOrDefaultAsync(t=>t.Id == id);
            return user;
        }

        // public async Task<IEnumerable<User>> GetUsers()
        // {
        //     var users = await _context.Users.Include(p=>p.Photos).ToListAsync();
        //     return users;
        // }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
           //_context.Users.Include(p=>p.Photos) - for eager loading
            var users =  _context.Users.OrderByDescending(t=>t.LastActive).AsQueryable();
            users =  users.Where(user=>user.Id != userParams.UserId).Where(u=>u.Gender == userParams.Gender);

            if(userParams.Likers){
                var userLikers = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(u=> userLikers.Contains(u.Id));
            }
            if(userParams.Likees){
                var userLikees = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(u=> userLikees.Contains(u.Id));
            }

            if(userParams.MinAge!=18 || userParams.MaxAge!=99){
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(t=> t.DateOfBirth>=minDob && t.DateOfBirth <=maxDob);
            }
            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(t=>t.Created);
                        break;
                    default:
                        users = users.OrderByDescending(t=>t.LastActive);
                        break;
                }

            }

            return await PagedList<User>.CreateAsync(users, userParams.pageNumber,userParams.pageSize);
        }


        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            // _context.Users
            // .Include("Likers")
            // .Include("Likees") - for eager
            var user = await _context.Users
            .FirstOrDefaultAsync(u=>u.Id == id);

            if(likers)
            {
                return user.Likers.Where(u=>u.LikeeId == id).Select(t=>t.LikerId);
            }
            else
            {
                return user.Likees.Where(u=>u.LikerId == id).Select(r=>r.LikeeId);
            }

        }

        public async Task<bool> SaveAll()
        {
           return  await _context.SaveChangesAsync() > 0; 
        }

        public async Task<Message> GetMessage(int id)
        {
           return await _context.Messages.FirstOrDefaultAsync(t=>t.Id==id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            //eager loading 
            //  .Include(u=>u.Sender)
            //         .ThenInclude(t=>t.Photos)
            //         .Include(u=>u.Recipient)
            //         .ThenInclude(t=>t.Photos)
            var messages =  _context.Messages
                   .AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u=>u.RecipientId == messageParams.UserId 
                    && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(u=>u.SenderId == messageParams.UserId
                     && u.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(u=>u.RecipientId == messageParams.UserId &&
                    u.RecipientDeleted == false &&
                    u.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(d=>d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.pageNumber,messageParams.pageSize);

        
          //  throw new NotImplementedException();
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            //eager loading
            // .Include(u=>u.Sender)
            //         .ThenInclude(t=>t.Photos)
            //         .Include(u=>u.Recipient)
            //         .ThenInclude(t=>t.Photos)

            var messages = await _context.Messages
                    .Where(t=>t.RecipientId == userId 
                    && t.RecipientDeleted == false && t.SenderId == recipientId 
                    || t.RecipientId == recipientId && t.SenderDeleted == false && t.SenderId == userId).OrderByDescending(t=>t.MessageSent)
                    .ToListAsync();

            return messages;
        }
    }
}