PsnWrapper
==========

Wrapper for unofficial PlayStation Network calls.

Usage (WIP)
=====

First you need grab user data from the unofficial PSN API. Calling this still needs a bit of work, but basically you'll setup a client:

	var client = new Client();
	client.Firmware = "<insert number>";
	client.BasicLogin = "<find online if you do not have this>";
	client.TrophyLogin = "<find online if you do not have this>";
	client.UserAgent = "<insert appropriate user agent depending upon device (?)>";
	
	client.User = new Profile {
		PsnId = "strivinglife",
		Region = "us",
		Jid = "<jid to match user>"
	};

Again, very rough, but generate the following XML files (calling client methods before this):

	int start = 1;
	int count = 64;
	var gameId = "NPWR04479_00";

	XDocument xml;
	if (!string.IsNullOrWhiteSpace(client.TestProfileData)) {
		xml = XDocument.Parse(client.TestProfileData);
		xml.Save(@"C:\Users\James\Projects\GitHub\VideoGamesSpa\_output\strivinglife\psn official\" + "profile.xml");
		xml.Dump();
	}
	if (!string.IsNullOrWhiteSpace(client.TestTrophyData)) {
		xml = XDocument.Parse(client.TestTrophyData);
		xml.Save(@"C:\Users\James\Projects\GitHub\VideoGamesSpa\_output\strivinglife\psn official\" + "trophy.xml");
		xml.Dump();
	}
	if (!string.IsNullOrWhiteSpace(client.TestGamesData)) {
		xml = XDocument.Parse(client.TestGamesData);
		// you'll have to do this multiple times, one for each group of 64 games played
		xml.Save(@"C:\Users\James\Projects\GitHub\VideoGamesSpa\_output\strivinglife\psn official\" + "games_" + start.ToString() + "-" + count.ToString() + ".xml");
		xml.Dump();
	}
	if (!string.IsNullOrWhiteSpace(client.TestGameData)) {
		xml = XDocument.Parse(client.TestGameData);
		// you'll have to do this multiple times, one for each game played
		xml.Save(@"C:\Users\James\Projects\GitHub\VideoGamesSpa\_output\strivinglife\psn official\" + "game-" + gameId + ".xml");
		xml.Dump();
	}

Next run the wrapper to actually compile the data.

Currently you'll need a trophy file, from , for each game that you want to compile. In the future this may not be necessary.

	var compiler = new PsnWrapper.Psn.UserDataCompiler();
	compiler.BackupData = true;
	compiler.FileNameFormat = "{0}";
	compiler.OutputPath = @"C:\Users\James\Projects\GitHub\VideoGamesSpa\_output\strivinglife\psnwrapper\";
	compiler.RawDataPath = @"C:\Users\James\Projects\GitHub\VideoGamesSpa\_output\strivinglife\psn official\";
	compiler.TrophyDetailsPath = @"C:\Users\James\Projects\GitHub\PsnTrophies\games\";
	try
	{
		compiler.CompileData();
	}
	catch (Exception ex)
	{
		ex.Dump();
	}

Easier documentation coming soon.
