using System.ComponentModel.DataAnnotations;

namespace ChemicalProject.Models
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
