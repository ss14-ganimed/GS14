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
using Content.Shared.DamageTeleport.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Reflect;

namespace Content.Shared.DamageTeleport.Systems;

public sealed class DamageTeleportSystem : EntitySystem
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

        SubscribeLocalEvent<DamageTeleportComponent, AttackedEvent>(OnReactiveDamaged);
        SubscribeLocalEvent<DamageTeleportComponent, ProjectileReflectAttemptEvent>(OnReactiveDamagedProjectile);
        SubscribeLocalEvent<DamageTeleportComponent, HitScanReflectAttemptEvent>(OnReactiveDamagedHitscan);
    }

    private void OnReactiveDamaged(EntityUid uid, DamageTeleportComponent component, ref AttackedEvent args)
    {
         var xform = Transform(uid);
         _xform.SetCoordinates(uid, xform, xform.Coordinates.Offset(_random.NextVector2(component.MinRange, component.MaxRange)));
    }

    private void OnReactiveDamagedProjectile(EntityUid uid, DamageTeleportComponent component, ref ProjectileReflectAttemptEvent args)
    {
         var xform = Transform(uid);
         _xform.SetCoordinates(uid, xform, xform.Coordinates.Offset(_random.NextVector2(component.MinRange, component.MaxRange)));
    }

    private void OnReactiveDamagedHitscan(EntityUid uid, DamageTeleportComponent component, ref HitScanReflectAttemptEvent args)
    {
         var xform = Transform(uid);
         _xform.SetCoordinates(uid, xform, xform.Coordinates.Offset(_random.NextVector2(component.MinRange, component.MaxRange)));
    }
}