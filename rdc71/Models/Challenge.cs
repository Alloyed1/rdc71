using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.Models
{
	public class Challenge
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Key]
		public string UserEmail { get; set; }
		public int DaysComplete { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? LastDayComleteTime { get; set; }
		public string DayProgress { get; set; }
		public string UserTask { get; set; }
	}
}
