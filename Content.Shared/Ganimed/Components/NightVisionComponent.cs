using Content.Shared.Actions;
using Content.Shared.Eye.NightVision.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Eye.NightVision.Components;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(NightVisionSystem))]
public sealed partial class NightVisionComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("isOn"), AutoNetworkedField]
    public bool IsNightVision;

    [DataField("color")]
    public Color NightVisionColor = Color.Green;

    [DataField]
    public bool IsToggle = false;

    [DataField] public EntityUid? ActionContainer;

    [Access(Other = AccessPermissions.ReadWriteExecute)]
    public bool DrawShadows = false;

    [Access(Other = AccessPermissions.ReadWriteExecute)]
    public bool GraceFrame = false;
}

public sealed partial class NVInstantActionEvent : InstantActionEvent { }