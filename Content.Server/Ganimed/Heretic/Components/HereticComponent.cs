/// Maded by Gorox for Enterprise. See CLA

namespace Content.Server.Ganimed.Heretic.Components;

[RegisterComponent]
public sealed partial class HereticComponent : Component
{

    [DataField("points"), ViewVariables(VVAccess.ReadWrite)]
    public int Points = 0;
    
}