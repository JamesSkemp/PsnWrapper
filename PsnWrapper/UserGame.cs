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

			/// <summary>
			/// Return the total earned points for a game, based upon earned trophies.
			/// </summary>
			/// <param name="includePlatinum">Whether to include platinum trophies in the count, which are not included in the number of points a game is worth.</param>
			/// <returns>Total points earned by trophies.</returns>
			public int CalculateEarnedPoints(bool includePlatinum = false)
			{
				var platinumPoints = includePlatinum ? 180 : 0;

				return (15 * this.BronzeEarned)
					+ (30 * this.SilverEarned)
					+ (90 * this.GoldEarned)
					+ (platinumPoints * this.PlatinumEarned)
					;
			}

			/// <summary>
			/// Return the progress of a game, based upon points.
			/// </summary>
			/// <returns>Percent, as int, of game progress.</returns>
			public int CalculateProgress()
			{
				if (this.TotalPoints == 0)
				{
					return 0;
				}
				return 100 * this.EarnedPoints / this.TotalPoints;
			}
		}
	}
}