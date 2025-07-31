using Microsoft.AspNetCore.Mvc.Rendering;
using MLEFIN.Models;

namespace MLEFIN.ViewModels
{
    public class VendorInvoiceModalViewModel
    {
        public VendorInvoice Invoice { get; set; }
        public SelectList Vendors { get; set; }
        public SelectList Categories { get; set; }
        public IEnumerable<Part> Parts { get; set; }
    }
}