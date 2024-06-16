/// Maded by Gorox for Enterprise. See CLA
namespace Content.Shared.Ganimed.XenoPotion.Components;

[RegisterComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class XenoPotionComponent : Component
{
    [DataField("color"), AutoNetworkedField]
    public Color Color = Color.FromHex("#c62121");

    [DataField("effect"), AutoNetworkedField]
    public string Effect = "Speed";
}