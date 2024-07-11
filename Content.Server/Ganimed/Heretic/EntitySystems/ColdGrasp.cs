/// Maded by Gorox for Enterprise. See CLA
using System.Linq;
using Content.Server.Ganimed.Heretic.Components;
using Content.Server.Temperature.Components;
using Content.Shared.Ganimed.Heretic;
using Content.Shared.Ganimed.Heretic.Components;
using Content.Shared.Tag;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Clothing;
using Content.Shared.IdentityManagement;
using Content.Shared.FixedPoint;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.Ganimed.Heretic;

public sealed class ColdGraspSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;

    public override void Initialize()
    {

        base.Initialize();

        SubscribeLocalEvent<HereticComponent, ColdGraspEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(Entity<HereticComponent> ent, ref ColdGraspEvent args)
    {
      if (args.Target != null && TryComp<TemperatureComponent>(args.Target, out var tempComp))
      {
           tempComp.CurrentTemperature -= 10.0f;
      }
    }
}
