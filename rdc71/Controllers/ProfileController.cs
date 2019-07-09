using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using rdc71.Models;
using rdc71.Repository;
using rdc71.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.Controllers
{
	public class ProfileController : Controller
	{
		IChallengeRepository challengeRepository;
		public ProfileController(IChallengeRepository challengeRepository)
		{
			this.challengeRepository = challengeRepository;
		}
		[HttpGet]
		[Authorize]
		public IActionResult Index(string userEmail)
		{
			if (userEmail == null)
				userEmail = User.Identity.Name;

			ViewBag.IsStart = challengeRepository.IsStart(userEmail);

			ProfileViewModel model = challengeRepository.GetProfileViewModel(userEmail);


			if (model.LastChallenge == null)
			{
				Challenge challenge = new Challenge { DaysComplete = 0 };
				model.LastChallenge = challenge;
			}

			return View(model);
		}
		[HttpPost]
		public void StartChallenge(string userEmail, List<string> userTask, int countDay)
		{
			challengeRepository.StartChallenge(userEmail, userTask, countDay);
		}
		[HttpGet]
		public List<string> GetUserTaskChallenge(string userEmail)
		{
			Challenge challenge = challengeRepository.GetLastChallenge(userEmail);
			if(challenge == null)
				return null;

			return JsonConvert.DeserializeObject<List<string>>(challenge.UserTask);
			
		}
		[HttpGet]
		public List<int> GetUserDayProgress(string userEmail)
		{
			Challenge challenge = challengeRepository.GetLastChallenge(userEmail);
			if (challenge == null)
				return null;

			return JsonConvert.DeserializeObject<List<int>>(challenge.DayProgress);
		}
		[HttpPost]
		public int MarkTask(string userEmail, int taskNumber, int DayNumber)
		{
			int result = challengeRepository.MarkTask(userEmail, taskNumber, DayNumber);
			return result;
		}
		[HttpPost]
		public int CanMark(string userEmail)
		{
			if (challengeRepository.CanUserMarkDay(userEmail, DateTime.Now))
				return 0;
			else
				return 1;
		}
	}
}
