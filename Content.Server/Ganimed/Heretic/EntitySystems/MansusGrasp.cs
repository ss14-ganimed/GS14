/// Maded by Gorox for Enterprise. See CLA
using System.Linq;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Server.Ganimed.Heretic.Components;
using Content.Shared.Ganimed.Heretic;
using Content.Shared.Ganimed.Heretic.Components;
using Content.Shared.Damage;
using Content.Shared.Tag;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Clothing;
using Content.Shared.Store.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.FixedPoint;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.Ganimed.Heretic;

public sealed class MansusGraspSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly StoreSystem _store = default!;

    public override void Initialize()
    {

        base.Initialize();

        SubscribeLocalEvent<MansusGraspComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<MansusGraspComponent, SucrificeDoAfterEvent>(OnSucrifice);
    }

    private void OnAfterInteract(EntityUid uid, MansusGraspComponent component, ref AfterInteractEvent args)
    {
      if (args.Target != null && !EntityManager.HasComponent<HereticSucrificiedComponent>(args.Target.Value) && EntityManager.HasComponent<HereticCanSucrificiedComponent>(args.Target.Value))
      {
         PrepSucrifice(uid, component, args.Target.Value, args.User);
      }
      else if (args.Target != null && _tag.HasTag(args.Target.Value, component.Knife))
      {
           Spawn(component.Weapon, Transform(args.Target.Value).Coordinates);
           EntityManager.DeleteEntity(args.Target.Value);
      }
    }

    //Sucriface

    private void PrepSucrifice(EntityUid uid, MansusGraspComponent component, EntityUid target, EntityUid user)
    {
      if (target != null && TryComp<DamageableComponent>(target, out var damage))
      {


        if (damage.TotalDamage >= 200)
        {
          var doAfterEventArgs = new DoAfterArgs(EntityManager, user, 5, new SucrificeDoAfterEvent(), uid, target: target, used: uid)
          {
              BreakOnWeightlessMove = true,
              BreakOnDamage = true,
              NeedHand = true,
              BreakOnHandChange = true
          };

          if (!_doAfterSystem.TryStartDoAfter(doAfterEventArgs))
              return;
        }
      }
    }

    private void OnSucrifice(EntityUid uid, MansusGraspComponent component, ref SucrificeDoAfterEvent args)
    {
       if (args.User != null && args.Target != null && TryComp<HereticComponent>(args.User, out var heretic))
       {
          heretic.Points += 1;

          if (args.User != null && TryComp<StoreComponent>(args.User, out var storeComp))
          {
              _store.TryAddCurrency(new Dictionary<string, FixedPoint2>
                  { {"HereticKnowledge", heretic.Points} }, args.User);
          }

          EntityManager.AddComponent<HereticSucrificiedComponent>(args.Target.Value);
       }
    }
}
