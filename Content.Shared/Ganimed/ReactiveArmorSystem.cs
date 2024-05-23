/// Maded by Gorox for Enterprise. See CLA
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Alert;
using Content.Shared.Audio;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Standing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Player;
using Content.Shared.Mobs.Components;
using Content.Shared.Teleportation.Components;
using Content.Shared.Teleportation.Systems;
using Content.Shared.Damage;
using Content.Shared.ReactiveArmor.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;


namespace Content.Shared.ReactiveArmor.Systems;

public sealed class ReactiveArmorSystem : EntitySystem
{
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ReactiveArmorComponent, DamageChangedEvent>(OnReactiveDamaged);
    }

    private void OnReactiveDamaged(EntityUid uid, ReactiveArmorComponent component, ref DamageChangedEvent args)
    {
        if (!args.DamageIncreased)
            return;

        if (args.DamageDelta == null)
            return;

        foreach (var (type, amount) in args.DamageDelta.DamageDict)
        {
            if (component.DamageTypes != null && !component.DamageTypes.Contains(type))
                continue;

            component.AccumulatedDamage += (float) amount;
        }
        if (component.AccumulatedDamage <= component.DamageThreshold)
            return;

         var xform = Transform(uid);
         _xform.SetCoordinates(uid, xform, xform.Coordinates.Offset(_random.NextVector2(component.MinRange, component.MaxRange)));
         float AccumulatedDamage = 0;
    }
}