﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public DateTime? RecordDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ExpiredDate { get; set; }

    }
}
 