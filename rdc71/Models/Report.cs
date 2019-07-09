using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.Models
{
	public class Report
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string UserEmail { get; set; }
		public string FirstName { get; set; }
		public DateTime SendDate { get; set; }
		public string LastName { get; set; }
		public string BodyReport { get; set; }
	}
}
