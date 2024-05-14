using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ChemicalProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        [ValidateNever]
        public Role Role { get; set; }

        [ForeignKey("AreaId")]
        public int? AreaId { get; set; }
        [ValidateNever]
        public Area Area { get; set; }
        [Required]
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
