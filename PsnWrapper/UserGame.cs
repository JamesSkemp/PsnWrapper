using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsnWrapper
{
	public partial class Psn
	{
		/// <summary>
		/// Game for a particular user.
		/// </summary>
		public class UserGame
		{
			public string Id { get; set; }
			public string IdEurope { get; set; }
			public string Title { get; set; }
			public string ImageUrl { get; set; }
			public string Progress { get; set; }
			public int BronzeEarned { get; set; }
			public int SilverEarned { get; set; }
			public int GoldEarned { get; set; }
			public int PlatinumEarned { get; set; }
			public int TotalEarned { get; set; }
			public int OrderPlayed { get; set; }
			public DateTime? LastUpdated { get; set; }
			public int TotalPoints { get; set; }
			public int EarnedPoints { get; set; }
			public string Platform { get; set; }

			public UserGame()
			{
			}
		}
	}
}