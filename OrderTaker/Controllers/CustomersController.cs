using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using OrderTaker.Data;
using OrderTaker.Models;
using OrderTaker.Models.ViewModel;

namespace OrderTaker.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly OrderTakerContext _context;
        private readonly UserManager<User> _userManager;
        public CustomersController(OrderTakerContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("api/customers/get")]
        public async Task<IActionResult> GetCustomers([FromBody] DtParameters dtParameters)
        {
            //Debug.WriteLine($"Draw: {dtParameters.Draw}");
            //Debug.WriteLine($"Length: {dtParameters.Length}");
            //Debug.WriteLine($"Order: column index - {dtParameters.Order[0].Column} column name - {dtParameters.Columns[dtParameters.Order[0].Column].Name} dir -  {dtParameters.Order[0].Dir}");
    
            var searchBy = dtParameters.Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "id";
            var orderDirection = dtParameters.Order[0].Dir.ToString().ToLower();
            
            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                //orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchBy))
            {
                query = query.Where(cust =>
                    cust.FullName != null && cust.FullName.ToLower().Contains(searchBy.ToLower()) ||
                    cust.MobileNumber != null && cust.MobileNumber.ToLower().Contains(searchBy.ToLower()) ||
                    cust.City != null && cust.City.ToLower().Contains(searchBy.ToLower()));
            }

            orderCriteria = $"{orderCriteria}_{orderDirection}";
            Debug.WriteLine($"Order: {orderCriteria}");

            query = orderCriteria switch
            {
                "fullName_asc" => query.OrderBy(x => x.FullName),
                "fullName_desc" => query.OrderByDescending(x => x.FullName),
                "mobileNumber_asc" => query.OrderBy(x => x.MobileNumber),
                "mobileNumber_desc" => query.OrderByDescending(x => x.MobileNumber),
                "city_asc" => query.OrderBy(x => x.City),
                "city_desc" => query.OrderByDescending(x => x.City),
                "isActive_asc" => query.OrderBy(x => x.IsActive),
                "isActive_desc" => query.OrderByDescending(x => x.IsActive),
                _ => query.OrderBy(x => x.ID),
            };

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = await query.CountAsync();
            var totalResultsCount = await _context.Customers.CountAsync();

            return Json(new DtResult<Customer>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = await query
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToListAsync()
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            Customer customer = new();
            return PartialView("_ModalPartial", customer);
        }

  
        [HttpPost("api/customers/create")]
        public async Task<IActionResult> Create(Customer customer)
        {
            customer.FullName = $"{customer.LastName}, {customer.FirstName}";
            customer.DateCreated = DateTime.Now;
            customer.CreatedBy = 
                _userManager.GetUserAsync(User).Result!.FirstName + " " +
                _userManager.GetUserAsync(User).Result!.LastName;
            customer.Timestamp = DateTime.Now;

            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                //return Json(new CustomResponse() { Code = 409, Message = ex.InnerException.Message });
                return Json(new CustomResponse() { Code = 409, Message = "Duplicate Full Name/ Mobile Number" }); //2601
            }
            finally
            {
                ModelState.Clear();
            }

            return Json(Ok());
        }


        [HttpGet("api/customers/edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Json(customer);
        }

        [HttpPost("api/customers/edit/{id}")]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.ID)
            {
                return NotFound();
            }

            try
            {
                customer.FullName = $"{customer.LastName}, {customer.FirstName}";
                customer.Timestamp = DateTime.Now;
                customer.CreatedBy =
                    _userManager.GetUserAsync(User).Result!.FirstName + " " +
                    _userManager.GetUserAsync(User).Result!.LastName;

                var checkMobile = await _context.Customers
                    .Where(c => c.MobileNumber == customer.MobileNumber && c.ID != id)
                    .FirstOrDefaultAsync();
                if(checkMobile != null)
                {
                    return Json(new CustomResponse() { Code = 409, Message = "Duplicate Mobile Number" }); //2601
                }

                var checkFullName = await _context.Customers
                    .Where(c => c.FullName == customer.FullName && c.ID != id)
                    .FirstOrDefaultAsync();
                if (checkFullName != null)
                {
                    return Json(new CustomResponse() { Code = 409, Message = "Duplicate Full Name" }); //2601
                }

                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (SqlException ex)
            {
                return Json(new CustomResponse() { Code = ex.ErrorCode, Message = ex.Message }); //2601
            }
            finally
            {
                ModelState.Clear();
            }

            return Json(Ok());
        }

    }
}
