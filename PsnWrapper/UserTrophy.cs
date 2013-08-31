using System;

namespace PsnWrapper
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
}