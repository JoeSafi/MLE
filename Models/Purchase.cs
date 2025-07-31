using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLEFIN.Models
{
    // Main vendor invoice header
    [Table("VendorInvoices", Schema = "joe")]
    public class VendorInvoice
    {
        public VendorInvoice()
        {
            InvoiceDate = DateTime.Today;
            DueDate = DateTime.Today.AddDays(30);
            Items = new List<VendorInvoiceItem>();
            Status = "Pending";
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Invoice number is required.")]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vendor is required.")]
        public int VendorCompanyID { get; set; }

        [Required(ErrorMessage = "Invoice date is required.")]
        [Column("InvoiceDate", TypeName = "date")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [Column("DueDate", TypeName = "date")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Partial, Overdue

        [StringLength(50)]
        public string? PONumber { get; set; }

        public bool HasWarranty { get; set; } = false;

        public bool HasCore { get; set; } = false;

        public bool HasParts { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? Reference { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("VendorCompanyID")]
        public virtual Company? VendorCompany { get; set; }

        [NotMapped]
        public List<VendorInvoiceItem> Items { get; set; }

        // Calculated properties
        [NotMapped]
        public decimal CalculatedTotal => HasParts ? SubTotal + TaxAmount + ShippingAmount - DiscountAmount : Amount;

        [NotMapped]
        public decimal AmountPaid { get; set; } // Calculated from payments

        [NotMapped]
        public decimal AmountDue => CalculatedTotal - AmountPaid;

        [NotMapped]
        public bool IsOverdue => Status != "Paid" && DueDate < DateTime.Today;

        [NotMapped]
        public int DaysOverdue => IsOverdue ? (DateTime.Today - DueDate).Days : 0;
    }

    // Invoice line items for parts
    [Table("VendorInvoiceItems", Schema = "joe")]
    public class VendorInvoiceItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VendorInvoiceID { get; set; }

        [Required(ErrorMessage = "Part number is required.")]
        [StringLength(50)]
        public string PartNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Cost is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        // Calculated property
        [NotMapped]
        public decimal LineTotal => Quantity * Cost;

        // Navigation property
        [ForeignKey("VendorInvoiceID")]
        public virtual VendorInvoice? VendorInvoice { get; set; }
    }

    // Warranty tracking
    [Table("VendorWarranties", Schema = "joe")]
    public class VendorWarranty
    {
        public VendorWarranty()
        {
            WarrantyDate = DateTime.Today;
            Status = "Pending";
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int VendorInvoiceID { get; set; }

        [Required]
        public int VendorCompanyID { get; set; }

        [Required(ErrorMessage = "Warranty date is required.")]
        [Column("WarrantyDate", TypeName = "date")]
        public DateTime WarrantyDate { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Credited, Paid (no credit received), Denied

        [Column("ResolutionDate", TypeName = "date")]
        public DateTime? ResolutionDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? WarrantyClaimNumber { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("VendorInvoiceID")]
        public virtual VendorInvoice? VendorInvoice { get; set; }

        [ForeignKey("VendorCompanyID")]
        public virtual Company? VendorCompany { get; set; }

        // Calculated properties
        [NotMapped]
        public bool IsOverdue => Status == "Pending" && WarrantyDate.AddDays(30) < DateTime.Today;

        [NotMapped]
        public int DaysPending => Status == "Pending" ? (DateTime.Today - WarrantyDate).Days : 0;
    }

    // Core returns tracking
    [Table("VendorCores", Schema = "joe")]
    public class VendorCore
    {
        public VendorCore()
        {
            CoreDate = DateTime.Today;
            Status = "Pending";
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int VendorInvoiceID { get; set; }

        [Required]
        public int VendorCompanyID { get; set; }

        [Required(ErrorMessage = "Core date is required.")]
        [Column("CoreDate", TypeName = "date")]
        public DateTime CoreDate { get; set; }

        [Required(ErrorMessage = "Core amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Core amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CoreAmount { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Shipped, Credited, Denied

        [Column("ShippedDate", TypeName = "date")]
        public DateTime? ShippedDate { get; set; }

        [Column("CreditedDate", TypeName = "date")]
        public DateTime? CreditedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditAmount { get; set; }

        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [StringLength(100)]
        public string? CoreNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("VendorInvoiceID")]
        public virtual VendorInvoice? VendorInvoice { get; set; }

        [ForeignKey("VendorCompanyID")]
        public virtual Company? VendorCompany { get; set; }

        // Calculated properties
        [NotMapped]
        public bool IsOverdue => Status == "Pending" && CoreDate.AddDays(45) < DateTime.Today;

        [NotMapped]
        public int DaysPending => Status == "Pending" ? (DateTime.Today - CoreDate).Days : 0;
    }

    // Vendor payments
    [Table("VendorPayments", Schema = "joe")]
    public class VendorPayment
    {
        public VendorPayment()
        {
            PaymentDate = DateTime.Today;
            PaymentMethod = "Check";
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vendor is required.")]
        public int VendorCompanyID { get; set; }

        [Required(ErrorMessage = "Payment date is required.")]
        [Column("PaymentDate", TypeName = "date")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public string PaymentMethod { get; set; } = "Check"; // Check, ACH, Credit Card, Cash

        [StringLength(50)]
        public string? CheckNumber { get; set; }

        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Link to bank transaction
        public int? BankTransactionID { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("VendorCompanyID")]
        public virtual Company? VendorCompany { get; set; }

        [ForeignKey("BankTransactionID")]
        public virtual BankTransaction? BankTransaction { get; set; }
    }

    // Vendor invoice payment allocations (to track which payments apply to which invoices)
    [Table("VendorInvoicePayments", Schema = "joe")]
    public class VendorInvoicePayment
    {
        public VendorInvoicePayment()
        {
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int VendorInvoiceID { get; set; }

        [Required]
        public int VendorPaymentID { get; set; }

        [Required(ErrorMessage = "Allocation amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Allocation amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AllocationAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("VendorInvoiceID")]
        public virtual VendorInvoice? VendorInvoice { get; set; }

        [ForeignKey("VendorPaymentID")]
        public virtual VendorPayment? VendorPayment { get; set; }
    }

    // Vendor returns
    [Table("VendorReturns", Schema = "joe")]
    public class VendorReturn
    {
        public VendorReturn()
        {
            ReturnDate = DateTime.Today;
            Items = new List<VendorReturnItem>();
            Status = "Pending";
            CreatedDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Return number is required.")]
        [StringLength(50)]
        public string ReturnNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vendor is required.")]
        public int VendorCompanyID { get; set; }

        [Required(ErrorMessage = "Return date is required.")]
        [Column("ReturnDate", TypeName = "date")]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingAmount { get; set; } = 0;

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Processed, Credited

        public bool HasParts { get; set; } = false;

        [StringLength(100)]
        public string? RMANumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [Column("ProcessedDate", TypeName = "date")]
        public DateTime? ProcessedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditAmount { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("VendorCompanyID")]
        public virtual Company? VendorCompany { get; set; }

        [NotMapped]
        public List<VendorReturnItem> Items { get; set; }

        // Calculated properties
        [NotMapped]
        public decimal CalculatedTotal => HasParts ? SubTotal + TaxAmount + ShippingAmount : Amount;
    }

    // Return line items for parts
    [Table("VendorReturnItems", Schema = "joe")]
    public class VendorReturnItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VendorReturnID { get; set; }

        [Required(ErrorMessage = "Part number is required.")]
        [StringLength(50)]
        public string PartNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Cost is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        // Calculated property
        [NotMapped]
        public decimal LineTotal => Quantity * Cost;

        // Navigation property
        [ForeignKey("VendorReturnID")]
        public virtual VendorReturn? VendorReturn { get; set; }
    }

    // Helper class for parts transactions (not a database entity)
    public class PartTransaction
    {
        public string PartNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public string? Reason { get; set; } // For returns
    }
}