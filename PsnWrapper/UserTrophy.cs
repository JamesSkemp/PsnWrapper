using System;

namespace PsnWrapper
{
	public partial class Psn
	{
		/// <summary>
		/// Trophy information for a particular game and user.
		/// </summary>
		public class UserTrophy
		{
			public string Id { get; set; }
			public string GameId { get; set; }
			public string Title { get; set; }
			public string ImageUrl { get; set; }
			public string Description { get; set; }
			public DateTime? Earned { get; set; }
			public TrophyType? Type { get; set; }
			public bool Hidden { get; set; }
			public string Platform { get; set; }

			public UserTrophy()
			{
			}
		}

		/// <summary>
		/// Type of trophy
		/// </summary>
		public enum TrophyType
		{
			Bronze,
			Silver,
			Gold,
			Platinum
		}

		/// <summary>
		/// Parse a string and return the applicable trophy type.
		/// </summary>
		/// <param name="trophyType">String of text to parse.</param>
		public static TrophyType? ParseTrophyType(string trophyType)
		{
			switch (trophyType.ToLower())
			{
				case "bronze":
					return TrophyType.Bronze;
				case "silver":
					return TrophyType.Silver;
				case "gold":
					return TrophyType.Gold;
				case "platinum":
					return TrophyType.Platinum;
				default:
					break;
			}
			return null;
		}
	}
}