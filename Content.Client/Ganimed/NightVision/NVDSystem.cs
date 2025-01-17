using Robust.Client.Graphics;
using Robust.Client.Player;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.NVD;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.NVD;

public sealed class NVDSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;

    private NVDOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        _overlay = new();
        _overlayMan.AddOverlay(_overlay);
    }

    public override void Update(float frameTime)
    {
        if (_playerManager.LocalEntity is {} ourUid &&
            _inventory.TryGetSlotEntity(ourUid, "eyes", out EntityUid? slotEntity) &&
            TryComp(slotEntity, out NVDComponent? NVDComponent) &&
            NVDComponent.Enabled)
            _overlay.SetEnabled(true);
        else if (_overlay.Enabled)
            _overlay.SetEnabled(false);
    }
}
