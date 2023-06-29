using OrderTaker.Models;
using System.ComponentModel.DataAnnotations;

namespace OrderTaker.Models.ViewModel
{
    public class OrderTakingViewModel
    { 
        public PurchaseOrder PurchaseOrder { get; set; }
        public Customer Customer { get; set; }
        public IEnumerable<SKU> SKUs { get; set; }
        public SKU SelectedSKU { get; set; }
        public PurchaseItem PurchaseItem { get; set; }
        public IEnumerable<PurchaseItem> PurchaseItems { get; set; }
        public IEnumerable<Status> StatusList  { get; set; }
    }
}
