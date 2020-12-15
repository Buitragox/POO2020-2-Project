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

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        public readonly ProjectDBContext db;
        public int counter;

        public HomeController(ProjectDBContext db)
        {
            counter = 0;
            this.db = db;
        }

        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }

        public IActionResult Register()
        {
            User u = new User();
            return View(u);
        }

        [HttpPost("Creado")]
        public async Task<IActionResult> SaveUser(User u)
        {
            try
            {
                if((u.Username != "") && (u.Password != "") && (u.FirstName != "") && (u.LastName != "")
                && (u.Address != "") && (u.AccountNumber != ""))
                {
                    bool validate = true;
                    List<User> userList = await db.Users.ToListAsync();
                    userList.ForEach(p => 
                    {
                        if(p.Username == u.Username && p.Password == u.Password)
                        {
                            validate = false;
                        }
                    });
                    if(validate)
                    {
                        db.Users.Add(u);
                        await db.SaveChangesAsync();
                        return View();
                    }
                    else
                    {
                        return Content("Ese usuario ya se encuentra registrado");
                    }
                }
                else
                {
                    return Content("No pueden haber campos vacios");
                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public async Task<IActionResult> Redirect(UserVM vm)
        {
            User newUser = new User();
            try
            {
                List<User> userList = await db.Users.ToListAsync();
                newUser = userList.First(u => (u.Username == vm.Username) && (u.Password == vm.Password));

                if (newUser.Type == "Farmer")
                {
                    return RedirectToAction("Menu", "Farmer", new { id = newUser.ID } );
                }
                else
                {
                    return RedirectToAction("Menu", "Client", new { id = newUser.ID });
                }
            }
            catch(Exception e)
            {
                // return Content(e.Message);
                return RedirectToAction("Failure", "Home");
            }
        }

        [HttpGet("LoginError")]
        public IActionResult Failure()
        {
            return View();
        }

        [HttpGet("EditarPerfil")]
        public async Task<IActionResult> EditUser(long id)
        {
            List<User> userList = await db.Users.ToListAsync();
            User newUser = userList.First(u => u.ID == id);
            return View(newUser);
        }

        [HttpPost("GuardarCambios")]
        public async Task<IActionResult> SaveChange(User u)
        { 
            try
            {
                db.Entry(u).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return View(u.ID);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
        public async Task<IActionResult> RedirectToMenu(long id)
        {
            User newUser = new User();
            try
            {
                List<User> userList = await db.Users.ToListAsync();
                newUser = userList.First(u => u.ID == id);

                if (newUser.Type == "Farmer")
                {
                    return RedirectToAction("Menu", "Farmer", newUser);
                }
                else
                {
                    return RedirectToAction("Menu", "Client", newUser);
                }
            }
            catch
            {
                return RedirectToAction("Failure", "HomeController");
            }
        }
    }
}
