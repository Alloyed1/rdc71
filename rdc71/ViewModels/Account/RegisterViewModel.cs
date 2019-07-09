using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace rdc71.ViewModels.Account
{
	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Имя")]
		public string FirstName { get; set; }

		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Фамилия")]
		public string LastName { get; set; }

		[Required(AllowEmptyStrings = false)]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Введите пароль")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		[Display(Name = "Подтвердить пароль")]
		public string PasswordConfirm { get; set; }
	}
}
