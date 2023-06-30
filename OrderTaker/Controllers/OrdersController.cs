using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderTaker.Data;
using OrderTaker.Models;
using OrderTaker.Models.ViewModel;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.DotNet.MSIdentity.Shared;
using System.Linq;
using Azure;

namespace OrderTaker.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {

        private readonly OrderTakerDbContext _context;
        private readonly UserManager<User> _userManager;
        public OrdersController(OrderTakerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("api/orders/get")]
        public async Task<IActionResult> GetOrders()
        {
            var Response = await this._context.PurchaseOrders
                .Include(po => po.Customer)
                .ToListAsync();
            return Json(Response);
        }

        [HttpPost("api/orders/skus/gePurchasedItems")]
        public async Task<IActionResult> GetPurchasedItems(int? id, OrderTakingViewModel vm)
        {
            Debug.WriteLine($"id: {id}");
            //OrderID = id
            if (id != null)
            {
                var Response = await this._context.PurchaseItems
                    .Include(x => x.SKU)
                    .Where(i => i.PurchaseOrderID == id)
                    .Select(item => new
                    {
                        id = item.ID,
                        skuId = item.SKU.ID,
                        name = item.SKU.Name,
                        quantity = item.Quantity,
                        price = item.Price
                    })
                    .ToListAsync();
                return Json(Response);
            }

            return Json(null);
        }


        [HttpPost("api/orders/skus/get")]
        public async Task<IActionResult> GetSKUs(string skus)
        {
            JArray jArray = JArray.Parse(skus);
            var names = new List<string>();
            foreach (JObject jObject in jArray.Cast<JObject>())
            {
                names.Add((string)jObject["name"]);
                //Debug.WriteLine($"{(string)jObject["skuId"]} -> {(string)jObject["name"]}");
            }
            if (names.Count > 0)
            {
                var response = await _context.SKUs
                    .Where(sku => !names.Contains(sku.Name))
                    .Where(sku => sku.IsActive == true)
                    .ToListAsync();
                return Json(response);
            }
            else
            {
                var response = await this._context.SKUs
                .Where(sku => sku.IsActive == true)
                .ToListAsync();
                return Json(response);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateCustomerDropDownList();
            PopulateStatusDropDownList();

            var vm = new OrderTakingViewModel()
            {
                PurchaseItem = new PurchaseItem() { Quantity = 1 },
                PurchaseOrder = new PurchaseOrder()
                {
                    DateOfDelivery = DateTime.Today.AddDays(1)
                }
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(OrderTakingViewModel vm)
        {
            try
            {
                vm.PurchaseOrder.UserID = _userManager.GetUserAsync(User).Result!.Id;
                vm.PurchaseOrder.DateCreated = DateTime.Now;
                vm.PurchaseOrder.CreatedBy =
                    _userManager.GetUserAsync(User).Result!.FirstName + " " +
                    _userManager.GetUserAsync(User).Result!.LastName;
                vm.PurchaseOrder.TimeStamp = DateTime.Now;
                vm.PurchaseOrder.CustomerID = vm.Customer.ID;

                if (vm.PurchaseItems != null)
                {
                    vm.PurchaseOrder.PurchaseItems = new List<PurchaseItem>();

                    foreach (PurchaseItem item in vm.PurchaseItems)
                    {
                        item.TimeStamp = DateTime.Now;
                        item.UserID = _userManager.GetUserAsync(User).Result!.Id;

                        vm.PurchaseOrder.AmountDue += item.Price;

                        vm.PurchaseOrder.PurchaseItems.Add(item);
                    }
                }


                //vm.PurchaseOrder.PurchaseItems.Add()

                _context.Add(vm.PurchaseOrder);

                await _context.SaveChangesAsync();

            }
            catch (DbException ex)
            {
                Debug.WriteLine(ex);
                ;
            }

            return Json(new { redirectToUrl = Url.Action("Index", "Orders") });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.PurchaseItems)
                .Include(po => po.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(po => po.ID == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }


            var vm = new OrderTakingViewModel()
            {
                PurchaseItem = new PurchaseItem() { Quantity = 1 },
                PurchaseOrder = purchaseOrder,
            };

            PopulateCustomerDropDownList(purchaseOrder.Customer.ID);
            PopulateStatusDropDownList((Status)Enum.Parse(typeof(Status), purchaseOrder.Status));

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, OrderTakingViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }
            Console.WriteLine(vm);
            var purchaseOrderToUpdate = await _context.PurchaseOrders
                .Include(po => po.PurchaseItems)
                .Include(po => po.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(po => po.ID == id);

            if (purchaseOrderToUpdate != null)
            {
                try
                {

                    //_context.Update(purchaseOrderToUpdate);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }


                PopulateCustomerDropDownList(purchaseOrderToUpdate.Customer.ID);
                PopulateStatusDropDownList((Status)Enum.Parse(typeof(Status), purchaseOrderToUpdate.Status));

                return View(vm);
            }

            return NotFound();
        }

        private void PopulateCustomerDropDownList(object selectedCustomer = null)
        {
            var customersQuery = from customer in _context.Customers
                                 where customer.IsActive == true
                                 select customer;

            ViewBag.CustomerID = new SelectList(customersQuery.AsNoTracking(), "ID", "FullName", selectedCustomer);
        }

        private void PopulateStatusDropDownList(Status selectedStatus = Status.New)
        {
            var list = (Enum.GetValues(typeof(Status)).Cast<Status>()
                .Select(s => new SelectListItem() { Text = s.ToString(), Value = s.ToString() }))
                .ToList();

            ViewBag.StatusList = new SelectList(list, "Value", "Text", selectedStatus);
        }
    }
}
