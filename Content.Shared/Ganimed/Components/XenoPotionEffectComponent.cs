/// Maded by Gorox for Enterprise. See CLA

namespace Content.Shared.Ganimed.XenoPotionEffected.Components;

[RegisterComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class XenoPotionEffectedComponent : Component
{

    [DataField, AutoNetworkedField]
    public Color Color = Color.FromHex("#c62121");
    
    [DataField, AutoNetworkedField]
    public string ShaderName = "Greyscale";
}