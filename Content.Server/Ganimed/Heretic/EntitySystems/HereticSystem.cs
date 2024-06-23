/// Maded by Gorox for Enterprise. See CLA
using System.Numerics;
using Content.Server.Actions;
using Content.Server.GameTicking;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Ganimed.Heretic.Components;
using Content.Shared.Store.Components;
using Content.Shared.Tag;
using Content.Shared.Mind;
using Content.Shared.Actions;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Ganimed.Heretic;

namespace Content.Server.Ganimed.Heretic;

public sealed partial class HereticSystem : EntitySystem
{
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly ActionsSystem _action = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly StoreSystem _store = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly ActionContainerSystem _actionContainer = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string HereticShopId = "ActionHereticShop";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HereticComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<HereticComponent, HereticShopActionEvent>(OnShop);
    }

    private void OnMapInit(Entity<HereticComponent> ent, ref MapInitEvent args)
    {
        if (_mind.TryGetMind(ent, out var mind, out _))
        {
            _actionContainer.AddAction(mind, "ActionMansusGrasp");
            _actionContainer.AddAction(mind, "ActionHereticShop");
        }
    }

    private void OnShop(EntityUid uid, HereticComponent component, HereticShopActionEvent args)
    {
        if (!TryComp<StoreComponent>(uid, out var store))
            return;
        _store.ToggleUi(uid, uid, store);
    }
}
