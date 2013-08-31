using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsnWrapper
{
	/// <summary>
	/// Login credentials when connecting to the API.
	/// </summary>
	public class Login
	{
		/// <summary>
		/// Username to use.
		/// </summary>
		public string Username { get; set; }
		/// <summary>
		/// Password to use.
		/// </summary>
		public string Password { get; set; }
		/// <summary>
		/// Return whether the login information has both a username and password assigned.
		/// </summary>
		public bool IsPopulated
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.Username) && !string.IsNullOrWhiteSpace(this.Password);
			}
		}

		public Login()
		{
		}

		public Login(string username, string password)
			: this()
		{
			this.Username = username;
			this.Password = password;
		}
	}
}
