/// Maded by Gorox for Enterprise. See CLA
using Content.Shared.Antag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Ganimed.Heretic.Components;

[RegisterComponent]
public sealed partial class HereticComponent : Component
{

    [DataField("points"), ViewVariables(VVAccess.ReadWrite)]
    public int Points = 0;
    
    public List<EntityUid> Objectives;

    public override bool SessionSpecific => true;

    [DataField("path"), ViewVariables(VVAccess.ReadWrite)]
    public string Path = "None";

    [DataField("effect"), ViewVariables(VVAccess.ReadWrite)]
    public string Effect = "None";
}