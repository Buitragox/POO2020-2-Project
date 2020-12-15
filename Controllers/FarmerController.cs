using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Models;
using Project.Services;
using Project.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Project.Controllers
{
    [Route("[controller]")]
    public class FarmerController : Controller
    {
        public readonly ProjectDBContext db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FarmerController(ProjectDBContext db, IWebHostEnvironment hostEnvironment)
        {
            this.db = db;
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet("Menu")]
        public async Task<IActionResult> Menu(long id)
        {
            MenuVM vm = new MenuVM();
            vm.userID = id;
            User u = await db.Users.FindAsync(id);
            vm.name = u.Username;
            return View(vm);
        }
        
        [HttpGet("Productos")]
        public async Task<IActionResult> ListProducts(long id)
        {

            FarmerProductsVM vm = new FarmerProductsVM();
            List<Product> newList = await db.Products.ToListAsync();
            vm.prodList = newList.Where(p => p.FarmerID == id).ToList();
            vm.prodList = vm.prodList.OrderBy(p => p.Name).ToList();
            vm.farmerID = id;
            return View(vm);
        }
        

        [HttpGet("EditarProducto")]
        public async Task<IActionResult> EditProduct(long id)
        {
            List<Product> newList = await db.Products.ToListAsync();
            Product prod = newList.First(p => p.ID == id);
            return View(prod);
        }

        [HttpPost("Actualizar")]
        public async Task<IActionResult> UpdateProduct(Product prod)
        {
            try
            {
                db.Entry(prod).State = EntityState.Modified;
                await db.SaveChangesAsync();
                MenuVM vm = new MenuVM();
                vm.userID = prod.FarmerID;
                return View(vm);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpGet("CrearProducto")]
        public IActionResult CreateProduct(long id)
        {
            Product newProd = new Product();
            newProd.FarmerID = id;
            return View(newProd);
        }

        [HttpPost("Creado")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProduct(Product prod)
        {
            try
            {
                if(prod.Price > 0 && prod.Stock >= 0 && prod.Name.Length > 0)
                {
                    prod.ID = 0;
                    // string wwwRootPath = _hostEnvironment.WebRootPath;
                    // string fileName = Path.GetFileNameWithoutExtension(prod.ImageFile.FileName);
                    // string extension = Path.GetExtension(prod.ImageFile.FileName);
                    // prod.ImageName=fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    // string path = Path.Combine(wwwRootPath + "/images/", fileName);
                    // using (var fileStream = new FileStream(path,FileMode.Create))
                    // {
                    //     await prod.ImageFile.CopyToAsync(fileStream);
                    // }
                    db.Products.Add(prod);
                    await db.SaveChangesAsync();
                    return View(prod.FarmerID);
                }
                else
                {
                    return Content("Ha ocurrido un error");
                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpGet("Eliminado")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                List<Product> newList = await db.Products.ToListAsync();
                Product prod = newList.First(p => p.ID == id);
                db.Products.Remove(prod);
                await db.SaveChangesAsync();
                return View(prod.FarmerID);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpGet("Historial")]
        public async Task<IActionResult> SalesHistory(long id)
        {
            try
            {
                SalesVM vm = new SalesVM();
                User seller = await db.Users.FindAsync(id);
                List<Sale> salesList = await db.SalesHistory.ToListAsync();
                salesList = salesList.Where(item => item.FarmerID == seller.ID).ToList();
                vm.SaleList = salesList;
                vm.SaleList = vm.SaleList.OrderByDescending(p => p.Date).ToList();
                vm.farmerID = seller.ID;
                return View(vm);
                
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }


    }
}
