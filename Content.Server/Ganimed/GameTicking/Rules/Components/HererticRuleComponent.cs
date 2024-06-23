/// Maded by Gorox for Enterprise. See CLA
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Content.Shared.Roles;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(HereticRuleSystem))]
public sealed partial class HereticRuleComponent : Component
{
    [DataField("prototypeId")]
    public ProtoId<AntagPrototype> PrototypeId = "Heretic";

    /// <summary>
    ///     Path to the heretic greeting sound.
    /// </summary>
    [DataField]
    public SoundSpecifier GreetingSound = new SoundPathSpecifier("/Audio/Ambience/Antag/traitor_start.ogg");

    [DataField] public EntityUid? ActionContainer;
}
