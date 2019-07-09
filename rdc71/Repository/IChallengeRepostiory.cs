using Dapper;
using Newtonsoft.Json;
using rdc71.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using rdc71.ViewModels.Profile;
using Microsoft.Extensions.Configuration;
using rdc71.ViewModels.Home;

namespace rdc71.Repository
{
	public interface IChallengeRepository
	{
		List<Challenge> GetUserChallenge(string userEmail);
		Task StartChallenge(string userEmail, List<string> userTask, int countDay);
		ProfileViewModel GetProfileViewModel(string userEmail);
		bool IsStart(string userEmail);
		int MarkTask(string userEmail, int taskNumber, int DayNumber);
		Challenge GetLastChallenge(string userEmail);
		List<int> GetDayProgress(string userEmail, int dayComplete);
		void SendReport(string firstName, string lastName, string body, DateTime time);
		List<TableUser> WhoEndChallange();
		bool CanUserMarkDay(string userEmail, DateTime time);
	}
	public class ChallengeRepository : IChallengeRepository
	{
		private string connectionString;
		public ChallengeRepository(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}
		public void SendReport(string firstName, string lastName, string body, DateTime time)
		{
			using (IDbConnection dbDapper = new SqlConnection(connectionString))
			{
				var query = "INSERT INTO Report (FirstName, LastName, BodyReport, SendDate) VALUES(@firstName, @lastName, @body, @time)";
				dbDapper.Execute(query, new { firstName, lastName, body, time});
			}
		}
		public List<Challenge> GetUserChallenge(string userEmail)
		{
			using (IDbConnection dbDapper = new SqlConnection(connectionString))
			{
				string query = "SELECT * FROM Challenge WHERE UserEmail = @userEmail";
				return dbDapper.Query<Challenge>(query, new { userEmail }).ToList();
			}
		}
		public List<TableUser> WhoEndChallange()
		{
			using(IDbConnection dbDapper = new SqlConnection(connectionString))
			{
				TableUser model = new TableUser();

				var query = "SELECT DISTINCT u.Email, u.FirstName, u.LastName FROM Challenge c INNER JOIN AspNetUsers u ON c.UserEmail = u.Email WHERE c.DaysComplete = 71";
				return  dbDapper.Query<TableUser>(query).ToList();
			}
		}
		public bool CanUserMarkDay(string userEmail, DateTime time)
		{
			Challenge challenge = GetLastChallenge(userEmail);
			if (challenge != null)
			{
				TimeSpan span = (time - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
				TimeSpan span2 = (Convert.ToDateTime(challenge.LastDayComleteTime) - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

				int res = span.Days - span2.Days;

				if (res > 0)
					return true;
				else
					return false;
			}
			return true;
		}
		public List<int> GetDayProgress(string userEmail, int dayComplete)
		{
			Challenge challenge = GetLastChallenge(userEmail);
			List<List<int>> taskProgress = JsonConvert.DeserializeObject<List<List<int>>>(challenge.DayProgress);
			List<int> day = taskProgress[dayComplete];
			return day;
		}
		public Challenge GetLastChallenge(string userEmail)
		{
			using (IDbConnection dbDapper = new SqlConnection(connectionString))
			{
				var query = "SELECT TOP 1 * FROM Challenge WHERE UserEmail = @userEmail ORDER BY StartDate DESC ";
				return dbDapper.Query<Challenge>(query, new { userEmail }).FirstOrDefault();
			}
		}

		public ProfileViewModel GetProfileViewModel(string userEmail)
		{
			ProfileViewModel profileViewModel = new ProfileViewModel();
			profileViewModel.Challenges = GetUserChallenge(userEmail);
			if (profileViewModel.Challenges.Count != 0)
				profileViewModel.LastChallenge = GetLastChallenge(userEmail);
			else
				profileViewModel.LastChallenge = null;


			if (profileViewModel.LastChallenge != null)
			{
				if (profileViewModel.LastChallenge.DaysComplete != 71)
					profileViewModel.DayProgress = GetDayProgress(userEmail, profileViewModel.LastChallenge.DaysComplete);
			}
				
				
			else
				profileViewModel.DayProgress = new List<int> { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };


			return profileViewModel;
		}
		public bool IsStart(string userEmail)
		{
			List<Challenge> list = GetUserChallenge(userEmail);

			if (list.Count == 0)
			{
				return false;
			}
			else
			{
				if (list[list.Count - 1].DaysComplete != 71)
				{
					return true;
				}
			}
			return false;
		}
		public async Task StartChallenge(string userEmail, List<string> userTask, int countDay)
		{
			string userTaskJSON = JsonConvert.SerializeObject(userTask);
			Challenge challenge = new Challenge { UserEmail = userEmail, DaysComplete = countDay, StartDate = DateTime.Now, UserTask = userTaskJSON };

			List<List<int>> daysProgress = new List<List<int>>();
			for (int i = 1; i < 72; i++)
			{
				List<int> mass = new List<int>();
				mass.AddRange(new int[4] { i, 0, 0, 0 });

				for (int j = 1; j < userTask.Count; j++)
				{
					mass.Add(0);
				}
				daysProgress.Add(mass);
			}
			string dayJSON = JsonConvert.SerializeObject(daysProgress);

			using (IDbConnection dbDapper = new SqlConnection(connectionString))
			{

				var query = "INSERT INTO Challenge (UserEmail, DaysComplete, StartDate, UserTask, DayProgress) VALUES(@UserEmail, @DaysComplete, @StartDate, @UserTask, @dayJSON)";
				await dbDapper.ExecuteAsync(query, new { challenge.UserEmail, challenge.DaysComplete, challenge.StartDate, challenge.UserTask, dayJSON });
			}
		}
		public int MarkTask(string userEmail, int taskNumber, int DayNumber)
		{
			Challenge userChallenge = GetLastChallenge(userEmail);

			List<List<int>> taskProgress = JsonConvert.DeserializeObject<List<List<int>>>(userChallenge.DayProgress);
			List<int> dayProgress = taskProgress[DayNumber - 1];

			if (dayProgress[taskNumber] == 0)
				dayProgress[taskNumber] = 1;
			else
				return 0;

			var hash = new HashSet<int>(dayProgress);
			if (!hash.Contains(0))
			{
				userChallenge.DaysComplete++;
				using (IDbConnection dbDapper = new SqlConnection(connectionString))
				{
					int id = userChallenge.Id;
					int day = userChallenge.DaysComplete;
					DateTime time = DateTime.Now;

					string dayJSONN = JsonConvert.SerializeObject(taskProgress);
					var query = "UPDATE Challenge SET DayProgress = @dayJSONN WHERE Id = @id";
					dbDapper.Execute(query, new { dayJSONN, id });

					query = "UPDATE Challenge SET DaysComplete = @day, LastDayComleteTime = @time WHERE Id = @id";
					dbDapper.Execute(query, new { day, time, id });
					if(userChallenge.DaysComplete == 71)
						return 3;

				}
				return 1;
			}

			dayProgress[taskNumber] = 1;
			taskProgress[DayNumber - 1] = dayProgress;

			string dayJSON = JsonConvert.SerializeObject(taskProgress);


			using (IDbConnection dbDapper = new SqlConnection(connectionString))
			{
				int id = userChallenge.Id;
				var query = "UPDATE Challenge SET DayProgress = @dayJSON WHERE Id = @id";
				dbDapper.Execute(query, new { dayJSON, id });
				return 2;
			}
		}

	}
}
