using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.NVD;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class NVDComponent : Component
{
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId Action = "ActionToggleNVD";

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public EntityUid? ActionEntity;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public bool Enabled;
}
