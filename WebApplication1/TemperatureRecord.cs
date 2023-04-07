using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TemperatureRecord
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public float Temperature { get; set; }
    }
}
