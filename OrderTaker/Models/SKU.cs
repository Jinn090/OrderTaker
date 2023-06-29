using Microsoft.EntityFrameworkCore;
using OrderTaker.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderTaker.Models
{
    [Index(nameof(Name), nameof(Code), IsUnique = true)]
    public class SKU
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime TimeStamp { get; set;}
        public string UserID { get; set; }
        public User User { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [MaxLength]
        public byte[] Image { get; set; }
    }
}
