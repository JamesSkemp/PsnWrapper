using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsnWrapper
{
	public class Uris
	{
		public static Dictionary<string, string> JidSearch
		{
			get
			{
				var uris = new Dictionary<string, string>(3);
				uris.Add("us", "http://searchjid.usa.np.community.playstation.net/basic_view/func/search_jid");
				uris.Add("gb", "http://searchjid.eu.np.community.playstation.net/basic_view/func/search_jid");
				uris.Add("jp", "http://searchjid.jpn.np.community.playstation.net/basic_view/func/search_jid");
				return uris;
			}
		}


	}
}