using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLEFIN.Models
{
    [Table("CompanyCategory", Schema = "joe")]
    public class CompanyCategory
    {
        [Key]
        public int CompanyCategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        // Navigation property for related transactions
        public virtual ICollection<BankTransaction>? BankTransactions { get; set; }
    }
}