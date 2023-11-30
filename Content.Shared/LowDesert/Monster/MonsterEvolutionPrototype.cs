using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.LowDesert.Monster;

[Serializable, NetSerializable, Prototype("monsterEvolution")]
public sealed partial class MonsterEvolutionPrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; } = default!;
	
	[DataField("prototype")]
    public string Prototype { get; } = default!;
	
	[DataField("cost")]
	public float Cost { get; } = 1;
	
	[DataField("name")]
	public string Name { get; } = "monster-evolution-name-none";
	
	[DataField("description")]
	public string Description { get; } = "monster-evolution-description-none";
}
