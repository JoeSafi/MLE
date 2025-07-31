using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLEFIN.Models
{
    [Table("Sales", Schema = "joe")]
    public class Sales
    {
        public Sales()
        {
            SalesDate = DateTime.Today;
            Lube = 0;
            AutoRepair = 0;
            Tires = 0;
            OtherSales = 0;
            Tax = 0;
            CarCount = 0;
            Bank = 0;
            Cash = 0;
            Checks = new List<CheckTransaction>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Sales date is required.")]
        [Column("SalesDate", TypeName = "date")]
        public DateTime SalesDate { get; set; }

        [Required(ErrorMessage = "Lube amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Lube amount must be 0 or greater")]
        public decimal Lube { get; set; } = 0;

        [Required(ErrorMessage = "Auto Repair amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Auto Repair amount must be 0 or greater")]
        public decimal AutoRepair { get; set; } = 0;

        [Required(ErrorMessage = "Tires amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Tires amount must be 0 or greater")]
        public decimal Tires { get; set; } = 0;

        [Required(ErrorMessage = "Other Sales amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Other Sales amount must be 0 or greater")]
        public decimal OtherSales { get; set; } = 0;

        [Required(ErrorMessage = "Tax amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Tax amount must be 0 or greater")]
        public decimal Tax { get; set; } = 0;

        [Required(ErrorMessage = "Car count is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Car count must be 0 or greater")]
        public int CarCount { get; set; } = 0;

        [Required(ErrorMessage = "Bank amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Bank amount must be 0 or greater")]
        public decimal Bank { get; set; } = 0;

        [Required(ErrorMessage = "Cash amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Cash amount must be 0 or greater")]
        public decimal Cash { get; set; } = 0;

        // Not mapped - for UI only, checks are stored separately in BankTransactions
        [NotMapped]
        public List<CheckTransaction> Checks { get; set; }

        // Calculated fields (not mapped to database)
        [NotMapped]
        public decimal Average => CarCount > 0 ? Lube / CarCount : 0;

        [NotMapped]
        public decimal TotalSales => Lube + AutoRepair + Tires + OtherSales + Tax;

        [NotMapped]
        public decimal TotalChecks => Checks?.Sum(c => c.Amount) ?? 0;

        // Bank column now includes both CC ACH and Check deposits
        [NotMapped]
        public decimal TotalBankDeposits => Bank + TotalChecks;

        [NotMapped]
        public decimal TotalDeposits => TotalBankDeposits + Cash;

        [NotMapped]
        public decimal ShortOver => TotalDeposits - TotalSales;
    }

    // Helper class for check transactions (not a database entity)
    public class CheckTransaction
    {
        public string CheckNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PayerName { get; set; } = string.Empty;
    }
}