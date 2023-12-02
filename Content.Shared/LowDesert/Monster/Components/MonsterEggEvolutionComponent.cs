using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;


namespace Content.Shared.LowDesert.Monster.Components;


[NetworkedComponent, RegisterComponent]
public sealed partial class MonsterEggEvolutionComponent : Component
{
	[DataField("evolutionTime")]
	public TimeSpan EvolutionTime { get; set; } = default!;
	
	[DataField("prototype")]
	public string Prototype { get; set; } = default!;
	
	[DataField("storedPoints")]
	public float StoredPoints { get; set; } = default!;
}