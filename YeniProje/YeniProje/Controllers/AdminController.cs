using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YeniProje.Models;


namespace YeniProje.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        
        public AdminController() { 
        
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

        }
        public ActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer(UserModel model,string Role)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user.Id,Role);
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Errors.First());
            }

            return View(model);
        }
      

         public ActionResult UpdateCustomer(string mail)
         {
            var user = userManager.FindByName(mail);
            
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult UpdateCustomer(ApplicationUser updatedUser)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindById(updatedUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Kullanıcı bilgilerini güncelle
                user.UserName = updatedUser.UserName;
                user.Email = updatedUser.Email;
                // Role güncelleme işlemleri burada yapılabilir

                // Kullanıcıyı veritabanında güncelle
                var result = userManager.Update(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin"); 
                }
               
            }

            return View(updatedUser);
        }
        [HttpPost]
        public async Task<ActionResult> SaveCustomer(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByEmail(model.Email);

                if (user == null)
                {
                    // Kullanıcı bulunamadı durumunda hata işlemleri
                }

                user.UserName = model.Username;
                user.Email = model.Email;
                userManager.AddToRole(user.Id, model.Roles.ToString());

                // Diğer alanları güncelleme

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                  
                    return RedirectToAction("Index");
                }

            }
            return RedirectToAction("Index");
           
        }
        // GET: Admin
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var users = userManager.Users.ToList();

            var userList = new List<UserModel>();

            foreach (var user in users)
            {
                var userModel = new UserModel
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = userManager.GetRoles(user.Id).ToList() 
                };


                userList.Add(userModel);
            }

            return View(userList);
        }

       
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult DeleteUser(string username)
        {
            var user = userManager.FindByName(username);
            if (user != null)
            {
                var result = userManager.Delete(user);
                if (result.Succeeded)
                {
                   
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı silinirken bir hata oluştu.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUserRole(string username, string newRole)
        {
            var user = userManager.FindByName(username);
            if (user != null)
            {
                var currentRoles = userManager.GetRoles(user.Id);
                foreach (var role in currentRoles)
                {
                    userManager.RemoveFromRole(user.Id, role);
                }
                userManager.AddToRole(user.Id, newRole);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
            }

            return RedirectToAction("Index");
        }


        public ActionResult UserList()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var users = userManager.Users.ToList();

            var userList = new List<UserModel>();
            var userRoles = new List<string>();
            foreach (var user in users)
            {
                var roles = userManager.GetRoles(user.Id);
                if (!roles.Contains("Admin"))
                {
                    userRoles.Add("User");
                }
            }

            
            foreach (var user in users)
            {
                var userModel = new UserModel
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = userManager.GetRoles(user.Id).ToList() /
                };

                userList.Add(userModel);
            }
            ViewBag.UserRoles = userRoles;
            return View(userList);
        }

        [HttpPost]
        public async Task<ActionResult> AssignRole(string userId, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await userManager.AddToRoleAsync(user.Id, roleName);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Rol atama işlemi başarısız oldu.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index"); 
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Delete(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
    
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Kullanıcı silme işlemi başarısız oldu.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }
        }

    }

}