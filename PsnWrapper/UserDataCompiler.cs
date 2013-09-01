using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PsnWrapper
{
	public partial class Psn
	{
		public class UserDataCompiler
		{
			/// <summary>
			/// Path to grab raw data files from.
			/// </summary>
			public string RawDataPath { get; set; }
			/// <summary>
			/// Path to grab trophy details from.
			/// </summary>
			public string TrophyDetailsPath { get; set; }
			/// <summary>
			/// Path to save user data to.
			/// </summary>
			public string OutputPath { get; set; }
			/// <summary>
			/// Format to use when creating compiled files. Example: compiled-{0}
			/// </summary>
			public string FileNameFormat { get; set; }
			/// <summary>
			/// Whether to backup generated files before updating.
			/// </summary>
			public bool BackupData { get; set; }
			public List<Object> DebugInfo { get; set; }

			public UserDataCompiler()
			{
				this.FileNameFormat = "{0}";
				this.DebugInfo = new List<object>();
			}

			/// <summary>
			/// Check whether the compiler can be run, or if data is missing.
			/// </summary>
			/// <returns>True if ready, otherwise an exception with information about what's missing.</returns>
			public bool CompilerReady()
			{
				// Verify the simple stuff.
				if (string.IsNullOrWhiteSpace(this.RawDataPath))
				{
					throw new NotImplementedException("Raw Data Path must be populated.");
				}
				else if (string.IsNullOrWhiteSpace(this.OutputPath))
				{
					throw new NotImplementedException("Output Path must be populated.");
				}
				else if (string.IsNullOrWhiteSpace(this.TrophyDetailsPath))
				{
					throw new NotImplementedException("Trophy Details Path must be populated.");
				}
				else if (string.IsNullOrWhiteSpace(this.FileNameFormat))
				{
					throw new NotImplementedException("You must specify the name format to use when saving the compiled files.");
				}

				var rawDataDirectory = new DirectoryInfo(this.RawDataPath);
				var rawDataFiles = rawDataDirectory.GetFiles("*.xml");

				var hasProfileFile = rawDataFiles.Any(f => f.Name.Equals("profile.xml"));
				if (!hasProfileFile)
				{
					throw new NotImplementedException("No profile file found.");
				}
				var hasTrophyFile = rawDataFiles.Any(f => f.Name.Equals("trophy.xml"));
				if (!hasTrophyFile)
				{
					throw new NotImplementedException("No trophy file found.");
				}
				var hasGamesFile = rawDataFiles.Any(f => f.Name.StartsWith("games_"));
				if (!hasGamesFile)
				{
					throw new NotImplementedException("No games file(s) found.");
				}
				var hasGameFiles = rawDataFiles.Any(f => f.Name.StartsWith("game-"));
				if (!hasGamesFile)
				{
					throw new NotImplementedException("No game file(s) found.");
				}
				return true;
			}

			public bool CompileData()
			{
				if (this.CompilerReady())
				{
					var currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");
					var filePath = Path.Combine(this.OutputPath, this.FileNameFormat + ".xml");
					var savePath = "";

					var userProfile = new UserProfile();
					var profileXml = XDocument.Load(Path.Combine(this.RawDataPath, "profile.xml")).Root;
					userProfile.Id = profileXml.Element("onlinename").Value;
					userProfile.ImageUrl = profileXml.Element("avatarurl").Value;
					userProfile.AboutMe = profileXml.Element("aboutme").Value;
					userProfile.Country = profileXml.Element("country").Value;
					userProfile.PSPlus = profileXml.Element("plusicon").Value == "1";
					var trophyXml = XDocument.Load(Path.Combine(this.RawDataPath, "trophy.xml")).Root;
					userProfile.EarnedPoints = int.Parse(trophyXml.Element("point").Value);
					userProfile.Level = int.Parse(trophyXml.Element("level").Value);
					userProfile.LevelProgress = int.Parse(trophyXml.Element("level").Attribute("progress").Value);
					userProfile.LevelBasePoints = int.Parse(trophyXml.Element("level").Attribute("base").Value);
					userProfile.LevelNextPoints = int.Parse(trophyXml.Element("level").Attribute("next").Value);
					userProfile.BronzeEarned = int.Parse(trophyXml.Element("types").Attribute("bronze").Value);
					userProfile.SilverEarned = int.Parse(trophyXml.Element("types").Attribute("silver").Value);
					userProfile.GoldEarned = int.Parse(trophyXml.Element("types").Attribute("gold").Value);
					userProfile.PlatinumEarned = int.Parse(trophyXml.Element("types").Attribute("platinum").Value);
					userProfile.TotalEarned = userProfile.BronzeEarned + userProfile.SilverEarned + userProfile.GoldEarned + userProfile.PlatinumEarned;
					var gamesFiles = new DirectoryInfo(this.RawDataPath).GetFiles("games_*.xml").Select(f => f.FullName);

					var apiGames = new List<ApiGame>();

					foreach (var gamesFile in gamesFiles)
					{
						var gamesXml = XDocument.Load(gamesFile).Root;
						var totalGames = int.Parse(gamesXml.Element("title").Value);
						if (userProfile.TotalGames == 0 || userProfile.TotalGames < totalGames)
						{
							userProfile.TotalGames = totalGames;
						}

						foreach (var game in gamesXml.Descendants("info"))
						{
							var gameInfo = new ApiGame();
							gameInfo.Id = game.Attribute("npcommid").Value;
							gameInfo.Platform = game.Attribute("pf").Value;
							gameInfo.Bronze = int.Parse(game.Element("types").Attribute("bronze").Value);
							gameInfo.Silver = int.Parse(game.Element("types").Attribute("silver").Value);
							gameInfo.Gold = int.Parse(game.Element("types").Attribute("gold").Value);
							gameInfo.Platinum = int.Parse(game.Element("types").Attribute("platinum").Value);
							gameInfo.Updated = DateTime.Parse(game.Element("last-updated").Value);

							var existingGame = apiGames.FirstOrDefault(g => g.Id == gameInfo.Id);
							if (existingGame != null && existingGame.Updated < gameInfo.Updated)
							{
								// This game is already added, but needs to be updated.
								apiGames.Remove(existingGame);
								apiGames.Add(gameInfo);
							}
							else
							{
								apiGames.Add(gameInfo);
							}
						}
					}
					apiGames = apiGames.OrderByDescending(g => g.Updated).ToList();

					userProfile.LastUpdate = apiGames.Select(g => g.Updated).FirstOrDefault();
					if (userProfile.LastUpdate == DateTime.MinValue)
					{
						userProfile.LastUpdate = null;
					}
					// Profile is done at this point.

					var userGames = new List<UserGame>();
					int orderPlayed = 1;
					// Get game data and add to games plus create trophies- file.
					foreach (var apiGame in apiGames)
					{
						var hasTrophyDetails = File.Exists(Path.Combine(this.TrophyDetailsPath, string.Format("{0}.xml", apiGame.Id)));
						var hasEarnedDetails = File.Exists(Path.Combine(this.RawDataPath, string.Format("game-{0}.xml", apiGame.Id)));
						if (hasTrophyDetails && hasEarnedDetails)
						{
							var trophyDetailsXml = XDocument.Load(Path.Combine(this.TrophyDetailsPath, string.Format("{0}.xml", apiGame.Id))).Root;
							var earnedDetailsXml = XDocument.Load(Path.Combine(this.RawDataPath, string.Format("game-{0}.xml", apiGame.Id))).Root;

							var gameDetails = trophyDetailsXml.Descendants("Game").FirstOrDefault();

							var userGame = new UserGame();
							userGame.Id = apiGame.Id;
							userGame.IdEurope = gameDetails.Element("IdEurope").Value;
							userGame.Title = gameDetails.Element("Title").Value;
							userGame.ImageUrl = gameDetails.Element("Image").Value;
							userGame.BronzeEarned = apiGame.Bronze;
							userGame.SilverEarned = apiGame.Silver;
							userGame.GoldEarned = apiGame.Gold;
							userGame.PlatinumEarned = apiGame.Platinum;
							userGame.TotalEarned = apiGame.Bronze + apiGame.Silver + apiGame.Gold + apiGame.Platinum;
							userGame.OrderPlayed = orderPlayed++;
							userGame.LastUpdated = apiGame.Updated;
							userGame.TotalPoints = int.Parse(gameDetails.Element("TotalPoints").Value);
							userGame.EarnedPoints = userGame.CalculateEarnedPoints();
							userGame.Progress = userGame.CalculateProgress().ToString();
							userGame.Platform = apiGame.Platform;

							var trophyDetails = trophyDetailsXml.Descendants("Trophy");
							userGame.PossibleTrophies = trophyDetails.Count();

							userGames.Add(userGame);

							// Compile trophy data.
							var userTrophies = new List<UserTrophy>();
							foreach (var trophyDetail in trophyDetails)
							{
								var userTrophy = new UserTrophy();
								userTrophy.Id = trophyDetail.Element("Id").Value;
								userTrophy.GameId = apiGame.Id;
								userTrophy.Title = trophyDetail.Element("Title").Value;
								userTrophy.ImageUrl = trophyDetail.Element("Image").Value;
								userTrophy.Description = trophyDetail.Element("Description").Value;
								userTrophy.Type = ParseTrophyType(trophyDetail.Element("Type").Value);
								userTrophy.Hidden = bool.Parse(trophyDetail.Element("Hidden").Value);
								userTrophy.Platform = apiGame.Platform;

								foreach (var earnedDetail in earnedDetailsXml.Descendants("trophy"))
								{
									if (earnedDetail.Attribute("id").Value == userTrophy.Id)
									{
										userTrophy.Earned = DateTime.Parse(earnedDetail.Value).ToUniversalTime().ToLocalTime();
										break;
									}
								}
								userTrophies.Add(userTrophy);
							}

							PerformBackup("trophies-" + apiGame.Id, "__{0}-" + currentTime);
							savePath = string.Format(filePath, "trophies-" + apiGame.Id);
							SaveTrophyXml(savePath, userTrophies);
						}
						else
						{
							DebugInfo.Add(apiGame.Id);
							DebugInfo.Add(hasTrophyDetails);
							DebugInfo.Add(hasEarnedDetails);
						}
					}

					#region Save data
					// Profile
					PerformBackup("profile", "__{0}-" + currentTime);
					savePath = string.Format(filePath, "profile");
					var psnProfile = new XmlSerializer(userProfile.GetType());
					using (StreamWriter writer = new StreamWriter(savePath))
					{
						psnProfile.Serialize(writer, userProfile);
					}
					// Games
					PerformBackup("games", "__{0}-" + currentTime);
					savePath = string.Format(filePath, "games");
					var psnGames = new XmlSerializer(userGames.GetType());
					using (StreamWriter writer = new StreamWriter(savePath))
					{
						psnGames.Serialize(writer, userGames);
					}
					// Trophies
					// done above
					#endregion
					return true;
				}
				return false;
			}

			/// <summary>
			/// Checks whether a backup needs to be performed, and backups the file if it does.
			/// </summary>
			/// <param name="fileName">Name of the file to backup, without the directory and extension.</param>
			/// <param name="backupFileFormat">String format to use when making the backup copy. Extension automatically appended.</param>
			/// <returns>True if the file was backed up.</returns>
			internal bool PerformBackup(string fileName, string backupFileFormat = "__{0}")
			{
				if (this.BackupData)
				{
					// Main directory for the user.
					var outputDirectory = this.OutputPath;
					// Format of the full file path.
					var filePathFormat = Path.Combine(outputDirectory, this.FileNameFormat + ".xml");
					// Actual path for this particular file.
					var filePath = string.Format(filePathFormat, fileName);
					if (File.Exists(filePath))
					{
						File.Copy(filePath, Path.Combine(outputDirectory, string.Format(backupFileFormat, string.Format(this.FileNameFormat, fileName)) + ".xml"));
					}
					return true;
				}
				return false;
			}

			/// <summary>
			/// Save a collection of user trophies to an XML file.
			/// </summary>
			/// <param name="path">Path to save the file to.</param>
			/// <param name="trophies">Collection of trophies to save.</param>
			/// <returns>True if data was saved.</returns>
			internal bool SaveTrophyXml(string path, List<UserTrophy> trophies)
			{
				var serializer = new XmlSerializer(trophies.GetType());
				using (var writer = new StreamWriter(path))
				{
					serializer.Serialize(writer, trophies);
					return true;
				}
				return false;
			}
		}
	}
}