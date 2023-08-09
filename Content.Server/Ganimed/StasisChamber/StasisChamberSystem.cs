using Content.Server.Mind.Components;
using Content.Shared.CCVar;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Ganimed.StasisChamber;
using Content.Shared.Verbs;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Server.Ganimed.StasisChamber;


/// <summary>
/// SS220
/// Implemented leaving from game via climbing in stasisChamber
/// </summary>
public sealed class StasisChamberSystem : SharedStasisChamberSystem
{
    [Dependency] private readonly StasisStorageConsoleSystem _StasisStorageConsoleSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    private ISawmill _sawmill = default!;
	
	private float _autoTransferDelay;

    public override void Initialize()
    {
        base.Initialize();

        _sawmill = Logger.GetSawmill("stasisChamber");
		
		_cfg.OnValueChanged(CCVars.AutoTransferToStasisDelay, SetAutoTransferDelay, true);

        SubscribeLocalEvent<StasisChamberComponent, ComponentInit>(OnComponentInit);

        SubscribeLocalEvent<StasisChamberComponent, GetVerbsEvent<AlternativeVerb>>(AddAlternativeVerbs);
        SubscribeLocalEvent<StasisChamberComponent, StasisChamberLeaveActionEvent>(OnStasisChamberLeaveAction);

        SubscribeLocalEvent<StasisChamberComponent, StasisChamberDragFinished>(OnDragFinished);
        SubscribeLocalEvent<StasisChamberComponent, DragDropTargetEvent>(HandleDragDropOn);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

		var timeToAutoTransfer = _gameTiming.CurTime - TimeSpan.FromSeconds(_autoTransferDelay);

        var entityEnumerator = EntityQueryEnumerator<StasisChamberComponent>();

        while (entityEnumerator.MoveNext(out var uid, out var stasisChamberComp))
        {
            if (stasisChamberComp.BodyContainer.ContainedEntity is null ||
                timeToAutoTransfer < stasisChamberComp.EntityLiedInStasisChamberTime)
            {
                continue;
            }

            TransferToStasisStorage(uid, stasisChamberComp);           
        }
    }

    /// <summary>
    /// Ejects body from stasisChamber
    /// </summary>
    /// <param name="uid"> EntityUid of the stasisChamber</param>
    /// <returns> EntityUid of the ejected body if it succeeded, otherwise returns null</returns>
    public override EntityUid? EjectBody(EntityUid uid, StasisChamberComponent? stasisChamberComponent)
    {
        if (!Resolve(uid, ref stasisChamberComponent))
        {
            return null;
        }

        if (stasisChamberComponent.BodyContainer.ContainedEntity is not { Valid: true } contained)
        {
            return null;
        }

        base.EjectBody(uid, stasisChamberComponent);
        return contained;
    }
	
	private void SetAutoTransferDelay(float value) => _autoTransferDelay = value;
	
    private void HandleDragDropOn(EntityUid uid, StasisChamberComponent stasisChamberComponent, ref DragDropTargetEvent args)
    {
        if (stasisChamberComponent.BodyContainer.ContainedEntity != null)
        {
            return;
        }

        if (!TryComp(args.Dragged, out MindContainerComponent? mind) || !mind.HasMind)
        {
            _sawmill.Error($"{ToPrettyString(args.User)} tries to put non-playable entity into stasisChamber {ToPrettyString(args.Dragged)}");
            return;
        }

        var doAfterArgs = new DoAfterArgs(args.User, stasisChamberComponent.EntryDelay, new StasisChamberDragFinished(), uid,
            target: args.Dragged, used: uid)
        {
            BreakOnDamage = true,
            BreakOnTargetMove = true,
            BreakOnUserMove = true,
            NeedHand = false,
        };
        _doAfterSystem.TryStartDoAfter(doAfterArgs);
        args.Handled = true;
    }

    private void OnDragFinished(EntityUid uid, StasisChamberComponent component, StasisChamberDragFinished args)
    {
        if (args.Cancelled || args.Handled || args.Args.Target is null)
        {
            return;
        }

        if (InsertBody(uid, args.Args.Target.Value, component))
        {
            _sawmill.Info($"{ToPrettyString(args.Args.User)} put {ToPrettyString(args.Args.Target.Value)} inside stasis сhamber.");
        }

        args.Handled = true;
    }

    private void OnStasisChamberLeaveAction(EntityUid uid, StasisChamberComponent component, StasisChamberLeaveActionEvent args)
    {
        if (component.BodyContainer.ContainedEntity is null)
        {
            _sawmill.Error("This action cannot be called if no one is in the stasis сhamber.");
            return;
        }
        TransferToStasisStorage(uid, component);
    }

    private void TransferToStasisStorage(EntityUid uid, StasisChamberComponent? component)
    {
        if (!Resolve(uid, ref component) || component.BodyContainer.ContainedEntity is null)
        {
            return;
        }

        var ev = new TransferredToStasisStorageEvent(uid, component.BodyContainer.ContainedEntity.Value);

        ev.Handled = false;

        RaiseLocalEvent(uid, ev, true);

        if (!ev.Handled)
        {
            _StasisStorageConsoleSystem.TransferToStasisStorage(uid, component.BodyContainer.ContainedEntity.Value);
            ev.Handled = true;
        }
        UpdateAppearance(uid, component);
    }
}