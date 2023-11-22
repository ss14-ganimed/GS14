using Robust.Shared.Serialization;

namespace Content.Shared.LowDesert.Monster;

public sealed class MonsterEvolutionBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly List<MonsterEvolutionItem>? Items;
	
	public MonsterEvolutionBoundUserInterfaceState(List<MonsterEvolutionItem>? items)
	{
		Items = items;
	}
}

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
public enum MonsterEvolutionMenuKey : byte
{
	Key
}