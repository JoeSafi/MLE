using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLEFIN.Models
{
    [Table("Company", Schema = "joe")]
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyID { get; set; }

        [Required]
        [StringLength(50)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(20)]
        public string? CellPhone { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? ContactName { get; set; }

        [StringLength(25)]
        public string? PaymentTerms { get; set; }

        public bool CardOnFile { get; set; }

        // CardNumber is stored as numeric(16,0) in database but we want to work with it as string
        [Column("CardNumber")]
        public decimal? CardNumberDecimal { get; set; }

        [NotMapped]
        public string? CardNumber
        {
            get => CardNumberDecimal?.ToString("F0");
            set => CardNumberDecimal = string.IsNullOrEmpty(value) ? null : (decimal.TryParse(value, out var result) ? result : null);
        }

        public bool Check { get; set; }
        public bool Cash { get; set; }

        [Required]
        public int CompanyCategoryID { get; set; }

        [ForeignKey("CompanyCategoryID")]
        public CompanyCategory? Category { get; set; }
    }
}