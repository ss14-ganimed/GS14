/// Maded by Gorox for Enterprise. See CLA
using Content.Server.Ganimed.XenoBiology.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Server.Ganimed.XenoBiology.Components;

[RegisterComponent]
public sealed partial class XenoBiologyComponent : Component
{
    [DataField("points"), ViewVariables(VVAccess.ReadWrite)]
    public int Points = 0;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Mutationchance = 0.3f;

    [DataField("mutagen", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Mutagen = "MobSlimesPet";

    [DataField("antimutagen", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Antimutagen = "MobAdultSlimesGreen";
}