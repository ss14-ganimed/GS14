﻿using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Ganimed.Disease.Swab;

[Serializable, NetSerializable]
public sealed partial class DiseaseSwabDoAfterEvent : SimpleDoAfterEvent
{
}

[Serializable, NetSerializable]
public sealed partial class BotanySwabDoAfterEvent : SimpleDoAfterEvent
{
}
