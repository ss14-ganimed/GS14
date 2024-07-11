using Content.Shared.Ganimed.Disease;

namespace Content.Server.Ganimed.Disease.Components;

public sealed class DiseaseInfectionSpreadEvent : EntityEventArgs
{
    public EntityUid Owner { get; init; } = default!;
    public DiseasePrototype Disease { get; init; } = default!;
    public float Range { get; init; } = default!;
}
