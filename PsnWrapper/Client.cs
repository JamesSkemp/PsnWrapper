using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PsnWrapper
{
	public class Client
	{
		public string Firmware { get; set; }
		public string BasicLogin { get; set; }
		public string TrophyLogin { get; set; }
		public string UserAgent { get; set; }
		public Profile User { get; set; }
		public string Test { get; set; }
		public string TestFirmwareData { get; set; }
		public string TestJid { get; set; }
		public string TestProfileData { get; set; }
		public string TestTrophyData { get; set; }
		public string TestGamesData { get; set; }
		public string TestGameData { get; set; }

		public Client()
		{
			var configSettings = System.Configuration.ConfigurationManager.AppSettings;
			this.Firmware = configSettings["Firmware"];
			this.BasicLogin = configSettings["BasicLogin"];
			this.TrophyLogin = configSettings["TrophyLogin"];
			this.UserAgent = configSettings["UserAgent"];
		}

		public string ApiRequest(string uri, ICredentials credentials = null, string postData = null)
		{
			var request = WebRequest.CreateHttp(uri);
			request.UserAgent = this.UserAgent;
			request.Timeout = 10000;

			if (credentials != null)
			{
				request.Credentials = credentials;
				if (!string.IsNullOrWhiteSpace(postData))
				{
					request.ContentType = "text/xml; charset=UTF-8";
					request.Method = "POST";
					using (var s = request.GetRequestStream())
					{
						using (StreamWriter stw = new StreamWriter(s))
						{
							stw.Write(postData);
						}
					}
				}
			}

			using (var response = request.GetResponse())
			{
				var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Attempt to update the firmware version with the latest information.
		/// </summary>
		/// <returns></returns>
		public bool GetCurrentFirmware()
		{
			this.TestFirmwareData = ApiRequest("http://fus01.ps3.update.playstation.net/update/ps3/list/us/ps3-updatelist.txt");
			if (!string.IsNullOrWhiteSpace(this.Test))
			{
				return true;
			}
			return false;
		}

		public bool GetJid()
		{
			if (string.IsNullOrWhiteSpace(this.User.PsnId))
			{
				// todo
				return false;
			}

			var credentials = this.BasicLogin.Split(':');
			var networkCredentials = new System.Net.NetworkCredential(credentials[0], credentials[1]);
			var postData = string.Format("<?xml version='1.0' encoding='utf-8'?><searchjid platform='ps3' sv='{0}'><online-id>{1}</online-id></searchjid>", this.Firmware, this.User.PsnId);
			foreach (KeyValuePair<string, string> searchUri in Uris.JidSearch)
			{
				this.TestJid = ApiRequest(searchUri.Value, networkCredentials, postData);
				var xml = XDocument.Parse(this.TestJid);
				this.User.Jid = xml.Root.Element("jid").Value;

				if (!string.IsNullOrWhiteSpace(this.User.Jid))
				{
					return true;
				}
			}
			return false;
		}

		public bool GetProfile()
		{
			if (string.IsNullOrWhiteSpace(this.User.Jid))
			{
				// todo
				return false;
			}
			else if (string.IsNullOrWhiteSpace(this.User.Region))
			{
				// todo
				return false;
			}

			var searchUri = string.Format("http://getprof.{0}.np.community.playstation.net/basic_view/func/get_profile", this.User.Region);
			var credentials = this.BasicLogin.Split(':');
			var networkCredentials = new System.Net.NetworkCredential(credentials[0], credentials[1]);
			var postData = string.Format("<profile platform='ps3' sv='{0}'><jid>{1}</jid></profile>", this.Firmware, this.User.Jid);

			this.TestProfileData = ApiRequest(searchUri, networkCredentials, postData);

			return true;
		}

		public bool GetTrophyCount()
		{
			if (string.IsNullOrWhiteSpace(this.User.Jid))
			{
				// todo
				return false;
			}

			var searchUri = "http://trophy.ww.np.community.playstation.net/trophy/func/get_user_info";
			// todo, move into static?
			var credentials = this.TrophyLogin.Split(':');
			// Yuck.
			var networkCredentials = new System.Net.NetworkCredential(credentials[0], this.TrophyLogin.Replace(credentials[0] + ":", ""));
			// todo, start and end range - doesn't need to be 64?
			var postData = string.Format("<nptrophy platform='ps3' sv='{0}'><jid>{1}</jid><start>1</start><max>64</max><pf>ps3</pf><pf>psp2</pf></nptrophy>", this.Firmware, this.User.Jid);

			this.TestTrophyData = ApiRequest(searchUri, networkCredentials, postData);

			return true;
		}

		public bool GetGames()
		{
			if (string.IsNullOrWhiteSpace(this.User.Jid))
			{
				return false;
			}

			var searchUri = "http://trophy.ww.np.community.playstation.net/trophy/func/get_title_list";
			var credentials = this.TrophyLogin.Split(':');
			var networkCredentials = new System.Net.NetworkCredential(credentials[0], this.TrophyLogin.Replace(credentials[0] + ":", ""));
			var postData = string.Format("<nptrophy platform='ps3' sv='{0}'><jid>{1}</jid><start>1</start><max>64</max></nptrophy>", this.Firmware, this.User.Jid);

			this.TestGamesData = ApiRequest(searchUri, networkCredentials, postData);

			return true;
		}

		public bool GetTrophies(string gameId)
		{
			if (string.IsNullOrWhiteSpace(this.User.Jid))
			{
				return false;
			}
			else if (string.IsNullOrWhiteSpace(gameId))
			{
				return false;
			}

			var searchUri = "http://trophy.ww.np.community.playstation.net/trophy/func/get_trophies";
			var credentials = this.TrophyLogin.Split(':');
			var networkCredentials = new System.Net.NetworkCredential(credentials[0], this.TrophyLogin.Replace(credentials[0] + ":", ""));
			var postData = string.Format("<nptrophy platform='ps3' sv='{0}'><jid>{1}</jid><list><info npcommid='{2}'><target>FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF</target></info></list></nptrophy>", this.Firmware, this.User.Jid, gameId);

			this.TestGameData = ApiRequest(searchUri, networkCredentials, postData);

			return true;
		}
	}
}

// Possibly helpful information?
/*
https://github.com/JamesSkemp/PlayStationProfile
http://psnapi.org/?page=forum&forum=4&topic=69#post:607

https://github.com/KrobothSoftware/psn-lib

https://github.com/thiagopa/PlaystationNetworkAPI
https://github.com/thiagopa/PlaystationNetworkPythonAPI
https://github.com/tomchuk/psn
https://github.com/noorus/psn/blob/master/psn.inc.php

http://psnapi.codeplex.com/discussions/226698
http://www.cameronjtinker.com/post/2012/01/02/First-version-of-PSN-trophy-collector-finished.aspx

https://github.com/cubehouse/PSNjs
https://github.com/search?p=4&q=playstation&ref=commandbar&type=Repositories
*/
