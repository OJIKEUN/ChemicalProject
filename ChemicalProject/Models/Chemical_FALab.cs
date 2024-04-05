﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChemicalProject.Models
{
	public class Chemical_FALab
	{
		[Key]
		public int Id { get; set; }
		public int Badge { get; set; }
		public string ChemicalName { get; set; }
		public string Brand { get; set; }
		public int Packaging { get; set; }
		public string Unit { get; set; }
		public int MinimumStock { get; set; }
		public double Price { get; set; }
		public string Justify { get; set; }
		public DateTime? RequestDate { get; set; }


		public bool? StatusManager { get; set; }
		public string? RemarkManager { get; set; }
		public DateTime? ApprovalDateManager { get; set; }

        public bool? StatusESH { get; set; }
        public string? RemarkESH { get; set; }
        public DateTime? ApprovalDateESH { get; set; }
    }
}

