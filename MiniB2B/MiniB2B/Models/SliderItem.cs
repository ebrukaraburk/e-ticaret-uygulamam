using System.ComponentModel.DataAnnotations;

namespace MiniB2B.Models
{
    public class SliderItem
    {
        public int Id { get; set; }
        public int? ProductId { get; set; } 

        public string? Title { get; set; }      
        public string? Description { get; set; } 

        public string ImageUrl { get; set; } = string.Empty; 
        public string? LinkUrl { get; set; }  

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}