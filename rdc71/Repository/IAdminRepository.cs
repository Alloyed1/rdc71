using Microsoft.Extensions.Configuration;
using rdc71.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.Repository
{
	public interface IAdminRepository
	{
		List<Report> GetUserReporst();
	}
	public class AdminRepository : IAdminRepository
	{
		string connectionString;
		public AdminRepository(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}
		public List<Report> GetUserReporst()
		{
			using(IDbConnection dbDapper = new SqlConnection(connectionString))
			{
				var query = "SELECT * FROM Report";
				return dbDapper.Query<Report>(query).ToList();
			}
		}
	}

}
