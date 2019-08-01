using System;

namespace DatingApp.API.DTO
{
    public class PhotosForDetailDTO
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAddded { get; set; }
        public bool IsMain { get; set; }
    }
}