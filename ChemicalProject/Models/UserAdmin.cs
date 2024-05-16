using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChemicalProject.Models
{
    public class UserAdmin
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AreaId")]
        public int? AreaId { get; set; }
        [ValidateNever]
        public Area Area { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
