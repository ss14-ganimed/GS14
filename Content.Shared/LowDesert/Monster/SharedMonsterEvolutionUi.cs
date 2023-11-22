using Robust.Shared.Serialization;

namespace Content.Shared.LowDesert.Monster;

[Serializable, NetSerializable]
public sealed class MonsterEvolutionBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly List<MonsterEvolutionItem> Items;
	
	public readonly float EvoPoints;
	
	public readonly MonsterEvolutionOverview Overview;
	
	public MonsterEvolutionBoundUserInterfaceState(List<MonsterEvolutionItem> items, float evoPoints, MonsterEvolutionOverview overview)
	{
		Items = items;
		EvoPoints = evoPoints;
		Overview = overview;
	}
}

[Serializable, NetSerializable]
public sealed class MonsterEvolutionMessage : BoundUserInterfaceMessage
{
	public readonly MonsterEvolutionItem Item;
	
        public MonsterEvolutionMessage(MonsterEvolutionItem item)
        {
            Item = item;
        }
}

[Serializable, NetSerializable]
public class MonsterEvolutionItem
{
	public string Name { get; set; } = default!;
	public string Description { get; set; } = default!;
	public float Cost { get; set; } = default!;
	
	public MonsterEvolutionItem(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
	}
}

[Serializable, NetSerializable]
public class MonsterEvolutionOverview
{
	public string Name { get; set; } = default!;
	public float Health { get; set; } = default!;
	
	public MonsterEvolutionOverview(string? name, float? health)
	{
		Name = name ?? "???";
		Health = health ?? 0.0f;
	}
}

[Serializable, NetSerializable]
public enum MonsterEvolutionMenuKey : byte
{
	Key
}