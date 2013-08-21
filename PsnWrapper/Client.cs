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
		public string TestProfileData { get; set; }
		public string TestTrophyData { get; set; }

		public Client()
		{
			var configSettings = System.Configuration.ConfigurationManager.AppSettings;
			this.Firmware = configSettings["Firmware"];
			this.BasicLogin = configSettings["BasicLogin"];
			this.TrophyLogin = configSettings["TrophyLogin"];
			this.UserAgent = configSettings["UserAgent"];
		}

		/// <summary>
		/// Attempt to update the firmware version with the latest information.
		/// </summary>
		/// <returns></returns>
		public bool GetCurrentFirmware()
		{
			var request = WebRequest.CreateHttp("http://fus01.ps3.update.playstation.net/update/ps3/list/us/ps3-updatelist.txt");
			request.UserAgent = this.UserAgent;
			request.Timeout = 10000;

			using (var response = request.GetResponse())
			{
				var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				this.Test = reader.ReadToEnd();
			}
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

			foreach (KeyValuePair<string, string> searchUri in Uris.JidSearch)
			{
				var request = WebRequest.CreateHttp(searchUri.Value);
				request.UserAgent = this.UserAgent;
				request.Timeout = 10000;
				request.ContentType = "text/xml; charset=UTF-8";
				request.Method = "POST";

				var credentials = this.BasicLogin.Split(':');
				request.Credentials = new System.Net.NetworkCredential(credentials[0], credentials[1]);

				var postData = string.Format("<?xml version='1.0' encoding='utf-8'?><searchjid platform='ps3' sv='{0}'><online-id>{1}</online-id></searchjid>", this.Firmware, this.User.PsnId);

				using (var s = request.GetRequestStream())
				{
					using (StreamWriter stw = new StreamWriter(s))
					{
						stw.Write(postData);
					}
				}

				using (var response = request.GetResponse())
				{
					var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
					this.Test = reader.ReadToEnd();
					var xml = XDocument.Parse(this.Test);
					this.User.Jid = xml.Root.Element("jid").Value;
				}
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

			var request = WebRequest.CreateHttp(searchUri);
			request.UserAgent = this.UserAgent;
			request.Timeout = 10000;
			request.ContentType = "text/xml; charset=UTF-8";
			request.Method = "POST";

			var credentials = this.BasicLogin.Split(':');
			request.Credentials = new System.Net.NetworkCredential(credentials[0], credentials[1]);

			var postData = string.Format("<profile platform='ps3' sv='{0}'><jid>{1}</jid></profile>", this.Firmware, this.User.Jid);

			using (var s = request.GetRequestStream())
			{
				using (StreamWriter stw = new StreamWriter(s))
				{
					stw.Write(postData);
				}
			}

			using (var response = request.GetResponse())
			{
				var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				this.TestProfileData = reader.ReadToEnd();
				//var xml = XDocument.Parse(this.Test);
				//this.User.Jid = xml.Root.Element("jid").Value;
			}

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

			var request = WebRequest.CreateHttp(searchUri);
			request.UserAgent = this.UserAgent;
			request.Timeout = 10000;
			request.ContentType = "text/xml; charset=UTF-8";
			request.Method = "POST";

			var credentials = this.TrophyLogin.Split(':');
			// Yuck.
			request.Credentials = new System.Net.NetworkCredential(credentials[0], this.TrophyLogin.Replace(credentials[0] + ":", ""));

			// todo, start and end range - doesn't need to be 64?
			var postData = string.Format("<nptrophy platform='ps3' sv='{0}'><jid>{1}</jid><start>1</start><max>64</max><pf>ps3</pf><pf>psp2</pf></nptrophy>", this.Firmware, this.User.Jid);

			using (var s = request.GetRequestStream())
			{
				using (StreamWriter stw = new StreamWriter(s))
				{
					stw.Write(postData);
				}
			}

			using (var response = request.GetResponse())
			{
				var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				this.TestTrophyData = reader.ReadToEnd();
				//var xml = XDocument.Parse(this.Test);
				//this.User.Jid = xml.Root.Element("jid").Value;
			}

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
