using Auth.Web.Data;
using Auth.Web.Models;
using Auth.Web.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;
        public UserController(ApplicationDbContext db,
                              UserManager<User> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            // Code With Using AutoMapper (This Is The Best)

            var users = _db.Users.Where(x => !x.IsDelete).OrderByDescending(x => x.CreatedAt).ToList();
            var usersVm = _mapper.Map<List<User>, List<UserViewModel>>(users);

            return View(usersVm);

            // Code Without Using AutoMapper

            //var users = _db.Users.Where(x => !x.IsDelete).Select(x => new UserViewModel()
            //{
            //    Id = x.Id,
            //    UserName = x.UserName,
            //    Email = x.Email,
            //    PhoneNumber = x.PhoneNumber,
            //    CreatedAt = x.CreatedAt

            //}).ToList();

            //return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel input)
        {
            if (ModelState.IsValid)
            {
                // Code With Using AutoMapper (This Is The Best)

                var user = _mapper.Map<User>(input);
                //user.CreatedAt = DateTime.Now;  // This is initilize in user constructor
                user.UserName = input.Email;

                // Code Without Using AutoMapper

                //var user = new User()
                //{
                //    CreatedAt = DateTime.Now,
                //    Email = input.Email,
                //    UserType = input.UserType,
                //    UserName = input.Email,
                //    PhoneNumber = input.PhoneNumber
                //};

                var result = await _userManager.CreateAsync(user, input.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result);
                    // or 
                    //foreach (var error in result.Errors)
                    //{
                    //    ModelState.AddModelError("", error.Description);
                    //}
                }

                if (input.UserType == Enums.UserType.Admin)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else if (input.UserType == Enums.UserType.Employee)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
                }
                return RedirectToAction("Index");
            }

            return View(input);
        }

        public IActionResult Delete(string Id)
        {
            var user = _db.Users.SingleOrDefault(x => x.Id == Id && !x.IsDelete);
            user.IsDelete = true;
            _db.Users.Update(user);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> InitRoles()
        {
            if (!_db.Roles.Any())
            {
                var roles = new List<string>();
                roles.Add("Admin");
                roles.Add("Employee");

                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            return RedirectToAction("Index");
        }
    }
}
