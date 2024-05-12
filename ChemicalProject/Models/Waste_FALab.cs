using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChemicalProject.Models
{
    public class Waste_FALab
    {
        [Key]
        public int Id { get; set; }
        public string WasteType { get; set; }
        public int WasteQuantity { get; set; }
        public DateTime? WasteDate { get; set; }
        [NotMapped]
        public int Balance { get; set; }
    }
}
