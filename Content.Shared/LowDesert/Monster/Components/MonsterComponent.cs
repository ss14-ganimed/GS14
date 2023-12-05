using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;


namespace Content.Shared.LowDesert.Monster.Components;


[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class MonsterComponent : Component
{
	[DataField("species"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public string Species = "???";
	
	[DataField("class"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public string Class = "???";
	
	[DataField("evoPoints"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public float EvoPoints = 10.0f;
	
	[DataField("evoPointsSpent"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public float EvoPointsSpent = 0.0f;
	
	[DataField("evoPointsRequired"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public float EvoPointsRequired = 50.0f;
	
	[DataField("consumeAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>)), AutoNetworkedField]
    public string? ConsumeAction = "ActionMonsterConsume";

    [DataField("consumeActionEntity"), AutoNetworkedField]
    public EntityUid? ConsumeActionEntity;
	
	[DataField("consumeTime"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public float ConsumeTime = 10.0f;
	
	[DataField("consumeDamageID"), AutoNetworkedField]
	public string ConsumeDamageID = "Cellular";
	
	[DataField("consumeDamageValue"), AutoNetworkedField]
	public float ConsumeDamageValue = 50.0f;
	
	[DataField("evolutions"), AutoNetworkedField]
	public List<MonsterEvolutionData> Evolutions = new();
}

public sealed partial class MonsterEvolutionActionEvent : InstantActionEvent
{

}

public sealed partial class MonsterConsumeActionEvent : EntityTargetActionEvent
{

}

[DataDefinition, Serializable, NetSerializable]
public partial struct MonsterEvolutionData
{
	[DataField("prototype")]
	public string Prototype { get; set; } = default!;
	
	[DataField("cost")]
	public float Cost { get; set; } = 1;
	
	[DataField("icon")]
	public SpriteSpecifier Icon { get; set; } = new SpriteSpecifier.Texture(new("/Textures/Objects/Devices/health_analyzer.rsi/unknown.png"));
	
	[DataField("species")]
	public string Species { get; set; } = "monster-evolution-species-none";
	
	[DataField("class")]
	public string Class { get; set; } = "monster-evolution-class-none";
	
	[DataField("specialization")]
	public string Specialization { get; set; } = "monster-evolution-specialization-none";
	
	[DataField("description")]
	public string Description { get; set; } = "monster-evolution-description-none";
	
	[DataField("isDevolution")]
	public bool IsDevolution { get; set; } = false;
	
	public MonsterEvolutionData(string prototype, float cost, SpriteSpecifier icon, string species, string monsterClass, string specialization, string description, bool isDevolution)
	{
		Prototype = prototype;
		Cost = cost;
		Icon = icon;
		Species = species;
		Class = monsterClass;
		Specialization = specialization;
		Description = description;
		IsDevolution = isDevolution;
	}
}