using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsnWrapper
{
	/// <summary>
	/// Functionality to make unofficial PSN calls for data.
	/// </summary>
	public partial class Psn
	{
		/// <summary>
		/// Timeout (in milliseconds) to use when making a request.
		/// </summary>
		public int Timeout { get; set; }
		/// <summary>
		/// Login to use for basic information.
		/// </summary>
		public Login BasicLogin { get; set; }
		/// <summary>
		/// Login to use for trophy information.
		/// </summary>
		public Login TrophyLogin { get; set; }
		/// <summary>
		/// Firmware to report when making requests.
		/// </summary>
		public string Firmware { get; set; }
		/// <summary>
		/// User agent to report when making requests.
		/// </summary>
		public string UserAgent { get; set; }

		public Psn()
		{
		}

		public bool CompileUserData()
		{



			return false;
		}
	}
}
