using Robust.Shared.Serialization;

namespace Content.Shared.LowDesert.Monster;

[Serializable, NetSerializable]
public sealed class MonsterEvolutionBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly List<MonsterEvolutionItem> Items;
	
	public readonly float EvoPoints;
	
	public readonly MonsterEvolutionOverview Overview;
	
	public readonly List<MonsterEvolutionPrototype> Evolutions;
	
	public MonsterEvolutionBoundUserInterfaceState(List<MonsterEvolutionItem> items, float evoPoints, MonsterEvolutionOverview overview, List<MonsterEvolutionPrototype> evolutions)
	{
		Items = items;
		EvoPoints = evoPoints;
		Overview = overview;
		Evolutions = evolutions;
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
public sealed class MonsterEvolutionEvolveMessage : BoundUserInterfaceMessage
{
	
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
	public float AttackSpeed { get; set; } = default!;
	public float AttackDamage { get; set; } = default!;
	public float AttackRange { get; set; } = default!;
	public float ConsumeDamage { get; set; } = default!;
	public float ConsumeTime { get; set; } = default!;
	public float EvoPoints { get; set; } = default!;
	public float WalkSpeed { get; set; } = default!;
	public float RunSpeed { get; set; } = default!;
	public float StaminaCooldown { get; set; } = default!;
	public float StaminaThreshold { get; set; } = default!;
	public float StaminaReplenish { get; set; } = default!;
	public float StaminaTime { get; set; } = default!;
	public string Species { get; set; } = default!;
	public string Class { get; set; } = default!;
	public float EvoPointsSpent { get; set; } = default!;
	public float EvoPointsRequired { get; set; } = default!;
	
	public MonsterEvolutionOverview(string? name, float? health, float? attackSpeed, float? attackDamage, float? attackRange, float? consumeDamage, float? consumeTime, float? evoPoints, float? walkSpeed, float? runSpeed, float? staminaCooldown, float? staminaThreshold, float? staminaReplenish, float? staminaTime, string? monsterSpecies, string? monsterClass, float? evoPointsSpent, float? evoPointsRequired)
	{
		Name = name ?? "???";
		Health = health ?? 0.0f;
		AttackSpeed = attackSpeed ?? 0.0f;
		AttackDamage = attackDamage ?? 0.0f;
		AttackRange = attackRange ?? 0.0f;
		ConsumeDamage = consumeDamage ?? 0.0f;
		ConsumeTime = consumeTime ?? 0.0f;
		EvoPoints = evoPoints ?? 0.0f;
		WalkSpeed = walkSpeed ?? 0.0f;
		RunSpeed = runSpeed ?? 0.0f;
		StaminaCooldown = staminaCooldown ?? 0.0f;
		StaminaThreshold = staminaThreshold ?? 0.0f;
		StaminaReplenish = staminaReplenish ?? 0.0f;
		StaminaTime = staminaTime ?? 0.0f;
		Species = monsterSpecies ?? "???";
		Class = monsterClass ?? "???";
		EvoPointsSpent = evoPointsSpent ?? 0.0f;
		EvoPointsRequired = evoPointsRequired ?? 0.0f;
	}
}

[Serializable, NetSerializable]
public enum MonsterEvolutionMenuKey : byte
{
	Key
}