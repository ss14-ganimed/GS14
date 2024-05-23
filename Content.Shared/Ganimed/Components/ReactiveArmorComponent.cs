/// Maded by Gorox for Enterprise. See CLA
using Content.Shared.ReactiveArmor.Systems;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Content.Shared.Damage.Prototypes;

namespace Content.Shared.ReactiveArmor.Components;

[RegisterComponent, Access(typeof(ReactiveArmorSystem))]
public sealed partial class ReactiveArmorComponent : Component
{
    [DataField("minRandomRadius"), ViewVariables(VVAccess.ReadWrite)]
    public float MinRange = 2.0f;

    [DataField("maxRandomRadius"), ViewVariables(VVAccess.ReadWrite)]
    public float MaxRange = 3.0f;

    /// <summary>
    /// What damage types are accumulated for the trigger?
    /// </summary>
    [DataField("damageTypes", customTypeSerializer: typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
    public List<string>? DamageTypes;

    /// <summary>
    /// What threshold has to be reached before it is activated?
    /// </summary>
    [DataField("damageThreshold", required: true)]
    public float DamageThreshold;

    /// <summary>
    /// How much damage has been accumulated on the artifact so far
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float AccumulatedDamage = 0;
}