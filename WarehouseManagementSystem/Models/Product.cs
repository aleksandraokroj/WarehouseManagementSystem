using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public string RefCode { get; set; }

        public System.Uri Image { get; set; }
    }
}
