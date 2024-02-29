using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniShop.Entity.Concrete.Identity;
using MiniShop.Shared.ViewModels.IdentityModels;

namespace MiniShop.UI.Areas.Admin.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    [Area("Admin")]
    public class UserController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var users = await _userManager.Users.ToListAsync();
        //    return View(users);
        //}
        public async Task<IActionResult> Index() => View( await _userManager.Users.ToListAsync());

        public async Task<IActionResult> AssignRoles(string id)
        {
            //id'si gönderilen, rol ataması yapılacak user'ı buluyoruz.
            var user = await _userManager.FindByIdAsync(id);
            var userRoles = await _userManager.GetRolesAsync(user);
            //ilgili user'ın rollerini de içerecek şekilde rol listesini yaratıyoruz.
            var roles = await _roleManager.Roles.Select(r=> new AssignRoleViewModel
            {
                RoleId = r.Id,
                RoleName = r.Name,
                IsAssigned = userRoles.Any(x=> x==r.Name)
            }).ToListAsync();
            var userRolesViewModel = new UserRolesViewModel
            {
                Id = user.Id,
                Name = $"{user.FirstName + " " + user.LastName}",
                UserName = user.UserName,
                Roles = roles
            };
            return View(userRolesViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRoles(UserRolesViewModel userRolesViewModel)
        {
            if(ModelState.IsValid) 
            { 
                var user = await _userManager.FindByIdAsync(userRolesViewModel.Id);
                foreach(var role in userRolesViewModel.Roles)
                {
                    if(role.IsAssigned)
                    {
                        await _userManager.AddToRoleAsync(user, role.RoleName);
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(userRolesViewModel);
        }
        public async Task<IActionResult> DeleteUser(string id) 
        {
            var user = await _userManager.FindByIdAsync(id);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Select(r => new AssignRoleViewModel
            {
                RoleId = r.Id,
                RoleName = r.Name,
                IsAssigned = userRoles.Any(x => x == r.Name)
            }).ToListAsync();
            var deleteUserViewModel = new DeleteUserViewModel
            { Id = user.Id,
                Name = user.FirstName + " " + user.LastName,
                UserName = user.UserName,
                Roles = roles
            };
            return View(deleteUserViewModel);
        }
    }
}
