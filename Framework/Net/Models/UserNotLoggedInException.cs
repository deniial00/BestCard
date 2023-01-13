using System;

namespace Framework.Net.Models;

public class UserNotLoggedInException : Exception
{
	public UserNotLoggedInException()
		: base("User not logged in")
	{
	}

	public UserNotLoggedInException(Exception inner)
		: base("User not logged in", inner) {

	}
}

