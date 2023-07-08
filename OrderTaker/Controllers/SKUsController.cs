using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class SKUsController : Controller
    {
        private readonly OrderTakerContext _context;
        private readonly UserManager<User> _userManager;

        public SKUsController(OrderTakerContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("api/skus/get")]
        public async Task<IActionResult> GetSKUs()
        {
            var Response = await this._context.SKUs.ToListAsync();
            return Json(Response);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            SKU sku = new();
            return PartialView("_ModalPartial", sku);
        }

        [HttpPost("api/skus/create")]
        public async Task<IActionResult> Create(SKU sku)
        {
            sku.DateCreated = DateTime.Now;
            sku.CreatedBy =
                _userManager.GetUserAsync(User).Result!.FirstName + " " +
                _userManager.GetUserAsync(User).Result!.LastName;
            sku.Timestamp = DateTime.Now;

            try
            {
                _context.Add(sku);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Json(new CustomResponse() { Code = 409, Message = "Duplicate Name/ Code" });
                //return Json(new CustomResponse() { Code = 409, Message = ex.InnerException.Message });
            }
            finally
            {
                ModelState.Clear();
            }

            return Json(Ok());
        }

        [HttpGet("api/skus/edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SKUs == null)
            {
                return NotFound();
            }

            var sku = await _context.SKUs.FindAsync(id);
            if (sku == null)
            {
                return NotFound();
            }
            return Json(sku);
        }

        [HttpPost("api/skus/edit/{id}")]
        public async Task<IActionResult> Edit(int id, SKU sku)
        {
            if (id != sku.ID)
            {
                return NotFound();
            }

            try
            {
                sku.CreatedBy =
                    _userManager.GetUserAsync(User).Result!.FirstName + " " +
                    _userManager.GetUserAsync(User).Result!.LastName;
                sku.Timestamp = DateTime.Now;

                var checkName = await _context.SKUs
                    .Where(c => c.Name == sku.Name && c.ID != id)
                    .FirstOrDefaultAsync();
                if (checkName != null)
                {
                    return Json(new CustomResponse() { Code = 409, Message = "Duplicate SKU Name" }); //2601
                }

                var checkCode = await _context.SKUs
                    .Where(c => c.Code == sku.Code && c.ID != id)
                    .FirstOrDefaultAsync();
                if (checkCode != null)
                {
                    return Json(new CustomResponse() { Code = 409, Message = "Duplicate SKU Code" }); //2601
                }

                _context.Update(sku);
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
