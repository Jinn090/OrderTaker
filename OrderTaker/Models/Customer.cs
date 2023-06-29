using Microsoft.EntityFrameworkCore;
using OrderTaker.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

namespace OrderTaker.Models
{

    [Index(nameof(FullName), nameof(MobileNumber), IsUnique = true)]
    public class Customer
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile number must be numeric"), StringLength(10)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        public string City { get; set; }

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

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
