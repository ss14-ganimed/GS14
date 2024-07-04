using Content.Shared.Actions;
using Content.Shared.Clothing;
using Content.Shared.Toggleable;

namespace Content.Shared.NVD;

public sealed class SharedNVDSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly ActionContainerSystem _actionContainer = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NVDComponent, GetItemActionsEvent>(OnGetItemActions);
        SubscribeLocalEvent<NVDComponent, MapInitEvent>(OnMapInit);
    }

    private void OnGetItemActions(Entity<NVDComponent> uid, ref GetItemActionsEvent args)
    {
        if (uid.Comp.ActionEntity is {} actionEntity)
            args.AddAction(actionEntity);
    }

    private void OnMapInit(Entity<NVDComponent> uid, ref MapInitEvent args)
    {
        _actionContainer.EnsureAction(uid, ref uid.Comp.ActionEntity, uid.Comp.Action);
    }
}
