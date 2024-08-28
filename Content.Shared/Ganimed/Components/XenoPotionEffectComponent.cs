/// Maded by Gorox CC-BY-SA 3.0
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Ganimed.XenoPotionEffected.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState(raiseAfterAutoHandleState: true)]
public sealed partial class XenoPotionEffectedComponent : Component
{

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public Color Color = Color.FromHex("#c62121");

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public Color BeforeColor = Color.FromHex("#c62121");

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool Enabled;
    
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string ShaderName = "Greyscale";
}
