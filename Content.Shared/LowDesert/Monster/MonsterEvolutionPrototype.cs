using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

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
	
	[DataField("icon")]
    public SpriteSpecifier Icon { get; } = new SpriteSpecifier.Texture(new("/Textures/Objects/Devices/health_analyzer.rsi/unknown.png"));
	
	[DataField("species")]
	public string Species { get; } = "monster-evolution-species-none";
	
	[DataField("class")]
	public string Class { get; } = "monster-evolution-class-none";
	
	[DataField("specialization")]
	public string Specialization { get; } = "monster-evolution-specialization-none";
	
	[DataField("description")]
	public string Description { get; } = "monster-evolution-description-none";
	
	[DataField("isDevolution")]
	public bool IsDevolution { get; } = false;
}
