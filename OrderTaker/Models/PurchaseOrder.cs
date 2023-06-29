using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderTaker.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace OrderTaker.Models
{
    public class PurchaseOrder
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }

        [Display(Name = "Date of Delivery"), DataType(DataType.Date)]
        public DateTime DateOfDelivery { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime TimeStamp { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        //[JsonIgnore]
        public Customer Customer { get; set; }

        public ICollection<PurchaseItem> PurchaseItems { get; set; }

    }

    public enum Status
    {
        New,
        Completed,
        Cancelled
    }
}
