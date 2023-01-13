using System;
using Newtonsoft.Json;
namespace Framework.Data.Models;

public class CardModel
{
	[JsonProperty("Id")]
	public string CardId { get; set; }

    [JsonProperty("Name")]
	public string CardName { get; set; }

    [JsonProperty("Damage")]
	public float CardDamage { get; set; }

	public CardModel(string name, float damage)
	{
		CardId = "asdasd";
		CardName = name;
		CardDamage = damage;
	}

}

