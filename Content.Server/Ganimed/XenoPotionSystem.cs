/// Maded by Gorox CC-BY-SA 3.0
using System.Linq;
using Content.Shared.Ganimed.XenoPotion.Components;
using Content.Server.Ganimed.XenoFood.Components;
using Content.Shared.Ganimed.XenoPotionEffected.Components;
using Content.Server.Atmos.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Clothing;
using Content.Shared.IdentityManagement;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.Ganimed.XenoPotion;

public sealed class XenoPotionSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<XenoPotionComponent, AfterInteractEvent>(OnAfterInteract);
    }

    /// <summary>
    /// Trigger the doafter for scanning
    /// </summary>
    private void OnAfterInteract(EntityUid uid, XenoPotionComponent component, ref AfterInteractEvent args)
    {
      if (args.Handled)
         return;



      if (args.Target != null && component.Effect == "Speed" && !EntityManager.HasComponent<XenoPotionEffectedComponent>(args.Target.Value))
      {

        if (args.Target != null && EntityManager.HasComponent<ClothingSpeedModifierComponent>(args.Target.Value))
        {
            var meta = MetaData(args.Target.Value);
            var name = meta.EntityName;

            EnsureComp<XenoPotionEffectedComponent>(args.Target.Value, out XenoPotionEffectedComponent? color);

            _metaData.SetEntityName(args.Target.Value, Loc.GetString("potion-speed-name-prefix", ("target", name)));

            EntityManager.RemoveComponent<ClothingSpeedModifierComponent>(args.Target.Value);

            color.Color = component.Color;
         
            EntityManager.DeleteEntity(args.Used);         
        }
      }

      else if (args.Target != null && component.Effect == "Pressure" && !EntityManager.HasComponent<XenoPotionEffectedComponent>(args.Target.Value))
      {
        if (args.Target != null && !EntityManager.HasComponent<PressureProtectionComponent>(args.Target.Value) && EntityManager.HasComponent<ClothingComponent>(args.Target.Value))
        {
          var meta = MetaData(args.Target.Value);
          var name = meta.EntityName;

          EnsureComp<XenoPotionEffectedComponent>(args.Target.Value, out XenoPotionEffectedComponent? color);

          _metaData.SetEntityName(args.Target.Value, Loc.GetString("potion-pressure-name-prefix", ("target", name)));

          EnsureComp<PressureProtectionComponent>(args.Target.Value, out PressureProtectionComponent pressure);

          color.Color = component.Color;
         
          pressure.LowPressureMultiplier = 1000f;
        
          EntityManager.DeleteEntity(args.Used);
        }
      }

      args.Handled = true;
    }
}
