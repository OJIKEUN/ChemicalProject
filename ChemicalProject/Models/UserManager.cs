using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChemicalProject.Models
{
    public class UserManager
    {
        [Key]
        public int Id { get; set; }
        public string Role { get; set; }

        [ForeignKey("AreaId")]
        public int? AreaId { get; set; }
        [ValidateNever]
        public Area Area { get; set; }
        [Required]
        public string Name { get; set; }
        public string Username { get; set; }
    }
}

