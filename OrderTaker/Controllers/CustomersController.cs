using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OrderTaker.Data;
using OrderTaker.Models;

namespace OrderTaker.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly OrderTakerDbContext _context;
        private readonly UserManager<User> _userManager;
        public CustomersController(OrderTakerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("api/customers/get")]
        public async Task<IActionResult> GetCustomers()
        {
            var Response = await this._context.Customers.ToListAsync();
            return Json(Response);
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
            customer.TimeStamp = DateTime.Now;

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
                customer.TimeStamp = DateTime.Now;
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
