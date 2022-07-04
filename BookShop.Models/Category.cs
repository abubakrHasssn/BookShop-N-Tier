using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Display(Name ="Display Order")]
        [Range(1,100,ErrorMessage = "Display Order Range from 1-100 Only.")]
        public int DisplyOrder { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
