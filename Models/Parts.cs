using System.ComponentModel.DataAnnotations;

namespace MLEFIN.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Part number is required")]
        [StringLength(50, ErrorMessage = "Part number cannot exceed 50 characters")]
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Current Cost")]
        [Range(0, double.MaxValue, ErrorMessage = "Current cost must be greater than or equal to 0")]
        public decimal? CurrentCost { get; set; }

        [Display(Name = "Average Cost")]
        [Range(0, double.MaxValue, ErrorMessage = "Average cost must be greater than or equal to 0")]
        public decimal? AverageCost { get; set; }

        [Display(Name = "List Price")]
        [Range(0, double.MaxValue, ErrorMessage = "List price must be greater than or equal to 0")]
        public decimal? ListPrice { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }
}