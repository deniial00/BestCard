using System;
namespace Framework.Data.Models;

public class UserModel
{
	public int UserId;
	public string Username;
	public bool IsAdmin;
	public int Credits;

	public UserModel(int userId, string username, bool isAdmin, int credits)
	{
		UserId = userId;
		Username = username;
		IsAdmin = isAdmin;
		Credits = credits;
	}

	public UserModel() { }
}

