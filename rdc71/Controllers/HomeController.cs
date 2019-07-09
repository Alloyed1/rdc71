using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rdc71.Models;
using rdc71.Repository;
using rdc71.ViewModels.Home;

namespace rdc71.Controllers
{
	public class HomeController : Controller
	{
		IChallengeRepository challengeRepository;
		public HomeController(IChallengeRepository challengeRepository)
		{
			this.challengeRepository = challengeRepository;
		}
		public IActionResult Index()
		{

			return View(challengeRepository.WhoEndChallange());
		}

		public IActionResult AboutRD()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		[HttpPut]
		public void SendReport(string firstName, string lastName, string body)
		{
			challengeRepository.SendReport(firstName, lastName, body, DateTime.Now);
		}
	}
}
