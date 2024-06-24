using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Ganimed.Heretic;

[Serializable, NetSerializable]
public sealed partial class SucrificeDoAfterEvent : SimpleDoAfterEvent
{
}

[Serializable, NetSerializable]
public sealed partial class AristocratDoAfterEvent : SimpleDoAfterEvent
{
}