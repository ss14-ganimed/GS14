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

[Serializable, NetSerializable]
public class MonsterEvolutionData
{
	public string Prototype { get; } = default!;
	public float Cost { get; } = 1;
	public SpriteSpecifier Icon { get; } = new SpriteSpecifier.Texture(new("/Textures/Objects/Devices/health_analyzer.rsi/unknown.png"));
	public string Species { get; } = "monster-evolution-species-none";
	public string Class { get; } = "monster-evolution-class-none";
	public string Specialization { get; } = "monster-evolution-specialization-none";
	public string Description { get; } = "monster-evolution-description-none";
	public bool IsDevolution { get; } = false;
	
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