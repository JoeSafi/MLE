using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLEFIN.Models
{
    [Table("BankTransactions", Schema = "joe")]
    public class BankTransaction
    {
        public BankTransaction()
        {
            TransactionDate = DateTime.Today;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Account is required.")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction date is required.")]
        [Column("TransactionDate", TypeName = "date")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        public string Payee { get; set; } = string.Empty;

        // New fields for check support
        [StringLength(50)]
        public string? CheckNumber { get; set; }

        [StringLength(255)]
        public string? Payer { get; set; }

        // Foreign key to Company for Payee
        public int? CompanyID { get; set; }

        // Nullable to match your database structure
        public int? CompanyCategoryID { get; set; }

        // New DepositType field
        [StringLength(20)]
        public string? DepositType { get; set; }

        [NotMapped]
        public decimal Balance { get; set; }

        // Navigation properties
        [ForeignKey("CompanyCategoryID")]
        public virtual CompanyCategory? CompanyCategory { get; set; }

        [ForeignKey("CompanyID")]
        public virtual Company? Company { get; set; }

        // Custom validation for DepositType when Type is Deposit
        public bool IsDepositTypeRequired => Type == "Deposit";

        // Helper property for display
        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(CheckNumber) && !string.IsNullOrEmpty(Payer)
            ? $"Check #{CheckNumber} - {Payer}"
            : Payee;
    }
}