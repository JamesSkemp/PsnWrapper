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
		/// Game returned by the unofficial API.
		/// </summary>
		public class ApiGame
		{
			public string Id { get; set; }
			public string Platform { get; set; }
			public int Bronze { get; set; }
			public int Silver { get; set; }
			public int Gold { get; set; }
			public int Platinum { get; set; }
			public DateTime Updated { get; set; }

			public ApiGame()
			{
			}
		}
	}
}
