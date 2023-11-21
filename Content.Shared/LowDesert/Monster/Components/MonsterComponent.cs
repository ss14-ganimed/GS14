using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;


namespace Content.Shared.LowDesert.Monster.Components;


[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class MonsterComponent : Component
{
	[DataField("evoPoints"), ViewVariables(VVAccess.ReadWrite)]
	public float EvoPoints = 10.0f;
	
	[DataField("toggleAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string EvolutionAction = "ActionMonsterEvolution";
	
	[DataField("consumeAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? ConsumeAction = "ActionMonsterConsume";

    [DataField("consumeActionEntity")]
    public EntityUid? ConsumeActionEntity;
	
	[DataField("consumeTime"), ViewVariables(VVAccess.ReadWrite)]
	public float ConsumeTime = 10.0f;
	
	[DataField("consumeDamageID")]
	public string ConsumeDamageID = "Cellular";
	
	[DataField("consumeDamageValue")]
	public float ConsumeDamageValue = 50.0f;

    [DataField, AutoNetworkedField] public EntityUid? EvolutionActionEntity;
}

public sealed partial class MonsterEvolutionActionEvent : InstantActionEvent
{

}

public sealed partial class MonsterConsumeActionEvent : EntityTargetActionEvent
{

}