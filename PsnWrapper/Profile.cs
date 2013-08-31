using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsnWrapper
{
	public class Profile
	{
		/// <summary>
		/// PlayStation Network id of the user to get data for.
		/// </summary>
		public string PsnId { get; set; }
		/// <summary>
		/// JID of the user. Formatted like an email.
		/// </summary>
		public string Jid { get; set; }
		/// <summary>
		/// Region of the user.
		/// </summary>
		public string Region { get; set; }

		public Profile()
		{
		}
	}
}