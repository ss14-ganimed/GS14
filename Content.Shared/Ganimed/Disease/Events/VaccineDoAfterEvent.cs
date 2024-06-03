using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Ganimed.Disease.Events;

[Serializable, NetSerializable]
public sealed partial class VaccineDoAfterEvent : SimpleDoAfterEvent
{
}
