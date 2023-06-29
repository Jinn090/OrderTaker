using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using OrderTaker.Data;

namespace OrderTaker.Models
{
    public class PurchaseItem
    {
        public int ID { get; set; }

        public int PurchaseOrderID { get; set; }

        public int SkuID { get; set; }

        public int Quantity { get; set; }

        //Computed Value
        //qty * sku price
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } 

        [Display(Name = "Date Modified")]
        public DateTime TimeStamp { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }
        public SKU SKU { get; set; }
    }
}
