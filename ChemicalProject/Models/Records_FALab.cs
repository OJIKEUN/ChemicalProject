using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChemicalProject.Models
{
    public class Records_FALab
    {
        [Key]
        public int Id { get; set; }
        public int ChemicalId { get; set; }
        [ForeignKey("ChemicalId")]
        [ValidateNever]
        public Chemical_FALab Chemical_FALab { get; set; }
        public int Badge { get; set; }
        public int ReceivedQuantity { get; set; }
        public int Consumption { get; set; }
        public string Justify { get; set; }

        [NotMapped]
        public int CurrentStock { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? RecordDate { get; set; }
        public DateTime? ExpiredDate { get; set; }

        public int? WasteId { get; set; }
        [ForeignKey("WasteId")]
        [ValidateNever]
        public Waste_FALab? Waste { get; set; }
    }
}
