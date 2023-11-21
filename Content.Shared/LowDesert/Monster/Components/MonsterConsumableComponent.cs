using Robust.Shared.GameStates;

namespace Content.Shared.LowDesert.Monster.Components;

[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class MonsterConsumableComponent : Component
{
	[DataField("evoPointsGranted"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public float EvoPointsGranted = 1.0f;
	
	[DataField("isConsumable"), ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
	public bool IsConsumable = true;
}