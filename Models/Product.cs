using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Project.Models
{
    public class Product
    {
        [Key]
        public long ID { get; set; }
        public long FarmerID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public string ImageName { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public Product()
        {
            ID = 0;
            FarmerID = 0;
            Name = "";
            Description = "";
            Price = 0;
            Stock = 0;
        }
    }
}