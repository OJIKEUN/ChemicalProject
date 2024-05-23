using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ChemicalProject.Models
{
    public class ActualRecord
    {
        [Key]
        public int Id { get; set; }
        public int Badge { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public int ChemicalId { get; set; }
        [ForeignKey("ChemicalId")]
        [ValidateNever]
        public Chemical_FALab Chemical_FALab { get; set; }
    }
}
