/// Maded by Gorox for Enterprise. See CLA
using Content.Shared.Antag;

namespace Content.Shared.Ganimed.Heretic.Components;

[RegisterComponent]
public sealed partial class HereticComponent : Component
{

    [DataField("points"), ViewVariables(VVAccess.ReadWrite)]
    public int Points = 0;
    
    public List<EntityUid> Objectives;

    public override bool SessionSpecific => true;    
}