/// Maded by Gorox for Enterprise. See CLA
using Content.Shared.DamageTeleport.Systems;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Content.Shared.Damage.Prototypes;

namespace Content.Shared.DamageTeleport.Components;

[RegisterComponent, Access(typeof(DamageTeleportSystem))]
public sealed partial class DamageTeleportComponent : Component
{
    [DataField("minRandomRadius"), ViewVariables(VVAccess.ReadWrite)]
    public float MinRange = 1.0f;

    [DataField("maxRandomRadius"), ViewVariables(VVAccess.ReadWrite)]
    public float MaxRange = 2.0f;
}