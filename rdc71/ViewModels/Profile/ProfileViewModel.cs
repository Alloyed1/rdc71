using rdc71.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.ViewModels.Profile
{
	public class ProfileViewModel
	{
		public Challenge LastChallenge { get; set; }
		public List<Challenge> Challenges { get; set; }
		public List<int> DayProgress { get; set; }
	}
}
