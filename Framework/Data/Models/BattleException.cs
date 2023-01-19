using System;
namespace Framework.Data.Models;

public class BattleException : Exception
{
	public BattleException() : base() { }

	public BattleException(string msg) : base(msg) { }

	public BattleException(string msg, Exception innerEx) : base(msg, innerEx) { }
}

