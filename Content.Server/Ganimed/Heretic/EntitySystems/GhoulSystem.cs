/// Maded by Gorox for Enterprise. See CLA
using System.Linq;
using Content.Server.Ganimed.Ghoul.Components;
using Content.Server.Atmos.Components;
using Content.Server.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Slippery;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Clothing;
using Content.Shared.Mind;
using Content.Shared.Actions;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.Ganimed.Ghoul;

public sealed class GhoulSystem : EntitySystem
{
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly ActionsSystem _action = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly ActionContainerSystem _actionContainer = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<GhoulComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(EntityUid uid, GhoulComponent component, ref AfterInteractEvent args)
    {
      if (args.Handled)
         return;

      if (args.Target != null && EntityManager.HasComponent<NoSlipComponent>(args.Target.Value))
      {
         Spawn("ClothingBackpackDebug2", Transform(uid).Coordinates);
         Spawn("MeleeDebugGib", Transform(uid).Coordinates);
         Spawn("NuclearGrenade", Transform(uid).Coordinates);
         Spawn("NuclearGrenade", Transform(uid).Coordinates);

        if (_mind.TryGetMind(args.User, out var mind, out _))
        {
            _actionContainer.AddAction(mind, "ActionGhoul");
        }
      }
    }
}
