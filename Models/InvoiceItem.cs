using System.ComponentModel.DataAnnotations;

namespace MLEFIN.Models
{
    public class InvoiceItem
    {
        [Key]
        public int Id { get; set; }

        public int VendorInvoiceId { get; set; }

        public string PartNumber { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Cost { get; set; }

        public decimal LineTotal { get; set; }
    }
}
