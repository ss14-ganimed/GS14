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
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Ganimed.Heretic;

namespace Content.Server.Ganimed.Heretic;

public sealed partial class HereticSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _action = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly StoreSystem _store = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string HereticShopId = "ActionHereticShop";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HereticComponent, HereticShopActionEvent>(OnShop);
    }

    private void OnShop(EntityUid uid, HereticComponent component, HereticShopActionEvent args)
    {
        if (!TryComp<StoreComponent>(uid, out var store))
            return;
        _store.ToggleUi(uid, uid, store);
    }


}
