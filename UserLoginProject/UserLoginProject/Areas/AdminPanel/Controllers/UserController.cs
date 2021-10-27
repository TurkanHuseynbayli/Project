using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserLoginProject.DAL;
using UserLoginProject.Extensions;
using UserLoginProject.Models;
using UserLoginProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserLoginProject.DAL;
using UserLoginProject.Models;
using UserLoginProject.ViewModels;
using static UserLoginProject.Extensions.Extension;

namespace BackEnd.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<AppUser> users = _userManager.Users.ToList();
            List<UserVM> usersVM = new List<UserVM>();
            foreach (AppUser user in users)
            {
                UserVM userVM = new UserVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Username = user.UserName,
                    IsDelete = user.isDelete,
                    Role = (await _userManager.GetRolesAsync(user))[0],
                };
                usersVM.Add(userVM);
            }
            return View(usersVM);
        }
        #region User ChangeStatus
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeStatus")]

        public async Task<IActionResult> ChangeStatusPost(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.isDelete)
            {
                user.isDelete = false;
            }
            else
            {
                user.isDelete = true;
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
        #endregion  User ChangeStatus
        #region User ResetPassword
        public IActionResult ResetPassword(string id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, ResetPasswordVM reg)
        {
            if (!ModelState.IsValid) return Content("Some Problem exists");

            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ModelState.AddModelError("Password", "We coudnt find specified User");
                return View();
            }
            string passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, passwordToken, reg.Password);
            return RedirectToAction(nameof(Index));
        }
        #endregion  User ResetPassword
        #region User ChangeRole
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.UserName == User.Identity.Name)
            {
                return Content("Ozun bil");
            }
            UserVM userVM = await getUserVM(user);
            return View(userVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            if (id == null || role == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.UserName == User.Identity.Name)
            {
                return Content("Ozun bil");
            }
            string oldRole = (await _userManager.GetRolesAsync(user))[0];
            IdentityResult addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Some problem exist");
                UserVM userVM = await getUserVM(user);
                return View(userVM);
            }

            IdentityResult removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Some problem exist");
                UserVM userVM = await getUserVM(user);
                return View(userVM);
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }

        #endregion User ChangeRole

        public async Task<IActionResult> Detail(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        #region User UpdateUser
        public IActionResult UpdateUser(string id)
        {
            if (id == null) return NotFound();
            AppUser user = _userManager.Users.FirstOrDefault(c => c.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string id, AppUser userNewParam)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();


            UserVM userVM = await getUserVM(user);



            user.Name = userNewParam.Name;
            user.Surname = userNewParam.Surname;
            user.Email = userNewParam.Email;
            user.UserName = userNewParam.UserName;
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, true);


            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion User UpdateUser

        #region getUserVM
        private async Task<UserVM> getUserVM(AppUser user)
        {
            List<string> roles = new List<string>();
            foreach (var item in Enum.GetValues(typeof(Roles)))
            {
                roles.Add(item.ToString());
            }
            UserVM userVM = new UserVM
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.UserName,
                IsDelete = user.isDelete,
                Role = (await _userManager.GetRolesAsync(user))[0],
                Roles = roles
            };
            return userVM;
        }
        #endregion getUserVM
    }
}
