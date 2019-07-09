using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rdc71.Models;
using rdc71.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.Controllers
{
	public class AccountController : Controller
	{
		private SignInManager<User> signInManager;
		//private IUserRepository userRepository;
		private UserManager<User> userManager;
		RoleManager<IdentityRole> roleManager;
		public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.roleManager = roleManager;
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{

			if (ModelState.IsValid)
			{
				User user = new User { Email = model.Email, UserName = model.Email, FirstName = model.FirstName, LastName = model.LastName };
				// добавляем пользователя
				var result = await userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					if(user.Email == "jenya.moxov@gmail.com")
						await userManager.AddToRoleAsync(user, "Admin");

					// установка куки
					await signInManager.SignInAsync(user, true);
					return RedirectToAction("Index", "Profile");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
					Report report = new Report();
				}
			}
			return View(model);

		}
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Profile");
			}
			else
			{
				ModelState.AddModelError("", "Неправильный логин и (или) пароль");
			}
			return View(model);
		}
		[HttpPut]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			// удаляем аутентификационные куки
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
