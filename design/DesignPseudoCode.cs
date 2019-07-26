ConnectNewPlug(string Username, string Mac)
{
	foreach (User u in UserManager.GetInstance().Users) {
		foreach (Plug p in Plugs) {
			if (p.Mac == Mac) {
				if (p.Approved) {
					throw new PlugAlreadyInUseException();
				} else {
					u.RemovePlug(p);
					Plug newPlug = p;
					u.AddPlug(p);
					return;
				}
			}
		}
	}

	Plug newPlug = new Plug(Mac);

	UserManager.GetInstance().GetUser(Username).AddPlug(newPlug);
}

ConnectNewUser(string Username, string Password)
{
	foreach (User u in UserManager.GetInstance().Users)
	{
		if (u.Username == Username)
		{
			throw new UsernameAlreadyInUseException();
		}
	}

	UserManager.GetInstance().AddUser(new User(Username, Password));
}

ConnectNewSample(string username, string mac, double current, double voltage, Date date) {
	UserManager.GetInstance().GetUser(username).GetPlug(mac).AddSample(new PowerUsageSample(...));
}