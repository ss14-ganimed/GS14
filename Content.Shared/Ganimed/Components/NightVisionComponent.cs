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

    [Access(Other = AccessPermissions.ReadWriteExecute)]
    public bool DrawShadows = false; // shitty code btw


    [Access(Other = AccessPermissions.ReadWriteExecute)]
    public bool GraceFrame = false;
}
