using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Project.Models;
using Project.Services;
using Project.ViewModels;

namespace Project.Controllers
{
    [Route("[controller]")]
    public class ClientController : Controller
    {
        public readonly ProjectDBContext db;
        private IMemoryCache _cache;

        public ClientController(ProjectDBContext db, IMemoryCache memoryCache)
        {
            this.db = db;
            _cache = memoryCache;
        }

        [HttpGet("Menu")]
        public async Task<IActionResult> Menu(long id)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            MenuVM vm = new MenuVM();
            vm.userID = id;
            User u = await db.Users.FindAsync(id);
            vm.name = u.Username;
            string keymsg = "msg" + id.ToString();
            _cache.Set(keymsg, "", cacheEntryOptions);
            return View(vm);
        }

        [HttpGet("GetProducts/")]
        public async Task<IActionResult> GetProducts(long id)
        {
            Console.WriteLine(id);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            
            Dictionary<Product, int> cart;
            string key = "cart" + id.ToString();
            if (!_cache.TryGetValue(key, out cart))
            {
                cart = new Dictionary<Product, int>();
                _cache.Set(key, cart, cacheEntryOptions);
             
            }
            string keymsg = "msg" + id.ToString();
            string msg;
            if (!_cache.TryGetValue(keymsg, out msg))
            {
                msg = "";
                _cache.Set(keymsg, "", cacheEntryOptions);
             
            }
            ProdListVM vm = new ProdListVM();
            vm.prodList = await db.Products.ToListAsync();
            vm.prodList = vm.prodList.OrderBy(p => p.Name).ToList();
            vm.clientID = id;
            vm.cart = cart;
            vm.message = msg;
            return View(vm);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(ProdListVM vm)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()              
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                Dictionary<Product, int> cart;
                string key = "cart" + vm.clientID.ToString();
                if (!_cache.TryGetValue(key, out cart))
                {
                    cart = new Dictionary<Product, int>();
                    _cache.Set(key, cart, cacheEntryOptions);
                
                }
                string keymsg = "msg" + vm.clientID.ToString();
                vm.prodList = await db.Products.ToListAsync();
                
                vm.cart = cart;
                Product prod = vm.prodList.First(p => p.ID == vm.prodID);
                Product keyProd = new Product();
                List<Product> keyList = new List<Product>(cart.Keys);
                bool validate = true;
                keyList.ForEach(p => {
                    if(p.ID == prod.ID)
                    {
                        validate = false;
                        keyProd = p;
                    }
                });
                
                if(validate)
                {
                    if(vm.amount <= prod.Stock)
                    {
                        _cache.Set(keymsg, "Se ha añadido el producto al carrito", cacheEntryOptions);
                        vm.cart.Add(prod, vm.amount);
                    }
                    else
                    {
                        _cache.Set(keymsg, "La cantidad ingresada supera la disponibilidad", cacheEntryOptions);
                    }
                }
                else
                {
                    if(vm.cart[keyProd] + vm.amount <= prod.Stock)
                    {
                        vm.cart[keyProd] += vm.amount;
                        _cache.Set(keymsg, "Se ha añadido el producto al carrito", cacheEntryOptions);
                        
                        
                    }
                    else
                    {
                        _cache.Set(keymsg, "La cantidad ingresada supera la disponibilidad", cacheEntryOptions);
                    }
                }
                _cache.Set(key, cart, cacheEntryOptions);
                return RedirectToAction("GetProducts", new { id = vm.clientID });
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpGet("Carro")]
        public IActionResult Cart(long id)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                Dictionary<Product, int> cart;
                string key = "cart" + id.ToString();
                if (!_cache.TryGetValue(key, out cart))
                {
                    cart = new Dictionary<Product, int>();
                    _cache.Set(key, cart, cacheEntryOptions);
                
                }
                ProdListVM vm = new ProdListVM();
                vm.clientID = id;
                vm.cart = cart;
                return View(vm);
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpGet("Checkout")]
        public async Task<IActionResult> Checkout(long id)
        {
            try
            {
                string key = "cart" + id.ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                Dictionary<Product, int> cart;
                if (!_cache.TryGetValue(key, out cart))
                    {
                        cart = new Dictionary<Product, int>();
                        _cache.Set(key, cart, cacheEntryOptions);
                    }
                User buyer = await db.Users.FindAsync(id);
                List<Product> keyList = new List<Product>(cart.Keys);
                Purchase newPurchase;
                Sale newSale;
                foreach(Product p in keyList)
                {
                    newPurchase = new Purchase(id, p.Name, cart[p]);
                    db.PurchaseHistory.Add(newPurchase);
                    await db.SaveChangesAsync();
                    newSale = new Sale(p.FarmerID, p.Name, cart[p], buyer.Address, buyer.Username);
                    db.SalesHistory.Add(newSale);
                    await db.SaveChangesAsync();
                    p.Stock -= cart[p];
                    db.Entry(p).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
    
                cart = new Dictionary<Product, int>();
                _cache.Set(key, cart, cacheEntryOptions);
                return View(id);
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpGet("Historial")]
        public async Task<IActionResult> PurchaseHistory(long id)
        {
            try
            {
                PurchaseVM vm = new PurchaseVM();
                User buyer = await db.Users.FindAsync(id);
                List<Purchase> purList = await db.PurchaseHistory.ToListAsync();
                purList = purList.Where(item => item.ClientID == buyer.ID).ToList();
                vm.PurchaseList = purList;
                vm.PurchaseList = vm.PurchaseList.OrderByDescending(p => p.Date).ToList();
                vm.clientID = buyer.ID;
                return View(vm);
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
            
        }
    }
}
