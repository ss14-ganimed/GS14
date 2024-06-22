/// Maded by Gorox for Enterprise. See CLA
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Server.Ganimed.Heretic.Components;

[RegisterComponent]
public sealed partial class MansusGraspComponent : Component
{

    [DataField("knifeTag", customTypeSerializer: typeof(PrototypeIdSerializer<TagPrototype>))]
    public string Knife = "Knife";

    [DataField("weapon", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string Weapon = "MobSlimesPet";

}