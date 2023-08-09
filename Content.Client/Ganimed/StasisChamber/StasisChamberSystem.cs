using Content.Shared.Ganimed.StasisChamber;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;

namespace Content.Client.Ganimed.StasisChamber;

public sealed class StasisChamberSystem : SharedStasisChamberSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StasisChamberComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<StasisChamberComponent, GetVerbsEvent<AlternativeVerb>>(AddAlternativeVerbs);
        SubscribeLocalEvent<StasisChamberComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(EntityUid uid, StasisChamberComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite is null)
        {
            return;
        }

        if (!_appearanceSystem.TryGetData<bool>(uid, StasisChamberComponent.StasisChamberVisuals.ContainsEntity, out var isOpen, args.Component))
        {
            return;
        }

        args.Sprite.LayerSetState(StasisChamberVisualLayers.Cover, isOpen ? "chamber-open" : "chamber-closed");
    }
}

public enum StasisChamberVisualLayers : byte
{
    Cover
}