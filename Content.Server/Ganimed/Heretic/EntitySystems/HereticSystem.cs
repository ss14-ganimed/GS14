/// Maded by Gorox for Enterprise. See CLA
using System.Numerics;
using Content.Server.Temperature.Components;
using Content.Server.Body.Components;
using Content.Server.Actions;
using Content.Server.Atmos.Components;
using Content.Server.GameTicking;
using Content.Server.Ganimed.Heretic.Components;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Chemistry.Reagent;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared.Ganimed.Heretic.Components;
using Content.Shared.Ganimed.Heretic;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Store.Components;
using Content.Shared.Tag;
using Content.Shared.Mind;
using Content.Shared.Actions;
using Content.Shared.Hands.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Ganimed.Heretic;

public sealed partial class HereticSystem : EntitySystem
{
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
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
        SubscribeLocalEvent<HereticComponent, AristocratDoAfterEvent>(OnAristocrat);
        SubscribeLocalEvent<HereticComponent, DemonDoAfterEvent>(OnDemon);
        SubscribeLocalEvent<HereticComponent, CreateColdThrowingStarEvent>(OnCreateThrowingStar);
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

    private void OnAristocrat(EntityUid uid, HereticComponent component, AristocratDoAfterEvent args)
    {
       if (TryComp<TemperatureComponent>(uid, out var tempComponent))
        {
            tempComponent.ColdDamageThreshold = 0;
        }

        _entManager.RemoveComponent<RespiratorComponent>(uid);
    }

    private void OnDemon(EntityUid uid, HereticComponent component, DemonDoAfterEvent args)
    {
       if (TryComp<TemperatureComponent>(uid, out var tempComponent))
        {
            tempComponent.HeatDamageThreshold = 50000;
        }

        _entManager.RemoveComponent<FlammableComponent>(uid);
    }

    private void OnCreateThrowingStar(EntityUid uid, HereticComponent component, CreateColdThrowingStarEvent args)
    {
        args.Handled = true;
        var user = args.Performer;

        var star = Spawn("ThrowingStarHeretic", Transform(user).Coordinates);
        _hands.TryPickupAnyHand(user, star);
    }
}
