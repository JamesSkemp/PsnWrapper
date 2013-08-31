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

					var userGames = new List<ApiGame>();

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

							var existingGame = userGames.FirstOrDefault(g => g.Id == gameInfo.Id);
							if (existingGame != null && existingGame.Updated < gameInfo.Updated)
							{
								// This game is already added, but needs to be updated.
								userGames.Remove(existingGame);
								userGames.Add(gameInfo);
							}
							else
							{
								userGames.Add(gameInfo);
							}
						}
					}
					userProfile.LastUpdate = userGames.Select(g => g.Updated).OrderByDescending(g => g).FirstOrDefault();
					if (userProfile.LastUpdate == DateTime.MinValue)
					{
						userProfile.LastUpdate = null;
					}
					// Profile is done at this point.




					// todo - games (x1) and trophies (x?)

					foreach (var apiGame in userGames)
					{
						// todo get game data and add to games plus create trophies- file.
					}




					/*var gamesXml = new List<XElement>();



					userProfile.LastUpdate = null;


					
					var gameXml = new List<XElement>();
					var gameFiles = new DirectoryInfo(this.RawDataPath).GetFiles("game-*.xml").Select(f => f.FullName);
					foreach (var gameFile in gamesFiles)
					{
						gameXml.Add(XDocument.Load(gameFile).Root);
					}*/

					#region Save data
					var filePath = Path.Combine(this.OutputPath, this.FileNameFormat + ".xml");
					var path = "";
					// Profile
					PerformBackup("profile", "__{0}-" + currentTime);
					path = string.Format(filePath, "profile");
					var psnProfile = new XmlSerializer(userProfile.GetType());
					using (StreamWriter writer = new StreamWriter(path))
					{
						psnProfile.Serialize(writer, userProfile);
					}
					// Games
					// todo
					// Trophies
					// todo
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
		}
	}
}