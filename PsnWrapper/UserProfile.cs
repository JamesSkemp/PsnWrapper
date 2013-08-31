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
		/// Profile information about a particular user.
		/// </summary>
		public class UserProfile
		{
			public string Id { get; set; }
			// todo removed until this library is cleaned up
			//public string Jid { get; set; }
			public string ImageUrl { get; set; }
			public string AboutMe { get; set; }
			public string Country { get; set; }
			public bool PSPlus { get; set; }
			public int EarnedPoints { get; set; }
			public int Level { get; set; }
			public int LevelProgress { get; set; }
			public int LevelBasePoints { get; set; }
			public int LevelNextPoints { get; set; }
			public int BronzeEarned { get; set; }
			public int SilverEarned { get; set; }
			public int GoldEarned { get; set; }
			public int PlatinumEarned { get; set; }
			public int TotalEarned { get; set; }
			public int TotalGames { get; set; }
			public DateTime? LastUpdate { get; set; }

			public UserProfile()
			{
			}
		}
	}
}