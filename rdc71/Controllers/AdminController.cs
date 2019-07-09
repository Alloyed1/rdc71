using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rdc71.Models;
using rdc71.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController: Controller
	{
		RoleManager<IdentityRole> roleManager;
		IAdminRepository adminRepository;
		public AdminController(RoleManager<IdentityRole> roleManager, IAdminRepository adminRepository)
		{
			this.roleManager = roleManager;
			this.adminRepository = adminRepository;
		}
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public List<Report> GetUserReport()
		{
			return adminRepository.GetUserReporst();
		}
		public async Task<IActionResult> AddRoles()
		{
			IdentityResult result = await roleManager.CreateAsync(new IdentityRole("Admin"));
			return RedirectToAction("Index", "Home");
		}
	}
}
