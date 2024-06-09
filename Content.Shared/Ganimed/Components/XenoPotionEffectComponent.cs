/// Maded by Gorox for Enterprise. See CLA
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Ganimed.XenoPotionEffected.Components;

[RegisterComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class XenoPotionEffectedComponent : Component
{

    [DataField, AutoNetworkedField]
    public Color Color = Color.FromHex("#c62121");

    [DataField, AutoNetworkedField]
    public Color BeforeColor = Color.FromHex("#c62121");

    [DataField, AutoNetworkedField]
    public bool Enabled;
    
    [DataField, AutoNetworkedField]
    public string ShaderName = "Greyscale";
}

[Serializable, NetSerializable]
public enum XenoPotionEffectedVisualizer : byte
{
    Effected,
}