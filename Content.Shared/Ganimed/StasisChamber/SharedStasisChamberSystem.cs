using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Ganimed.StasisChamber
{
    public abstract class SharedStasisChamberSystem : EntitySystem
    {
        [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly StandingStateSystem _standingStateSystem = default!;
        [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
        [Dependency] private readonly EntityManager _entityManager = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<StasisChamberComponent, CanDropTargetEvent>(OnStasisChamberCanDropTarget);
        }

        /// <summary>
        /// Inserts target inside stasisChamber
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="target"></param>
        /// <param name="stasisChamberComponent"></param>
        /// <returns> true if we successfully inserted target inside stasisChamber, otherwise returns false</returns>
        public bool InsertBody(EntityUid uid, EntityUid target, StasisChamberComponent stasisChamberComponent)
        {
            if (stasisChamberComponent.BodyContainer.ContainedEntity != null)
                return false;

            if (!HasComp<MobStateComponent>(target))
                return false;

            var xform = Transform(target);
            stasisChamberComponent.BodyContainer.Insert(target, transform: xform);

            if (_prototypeManager.TryIndex<InstantActionPrototype>("StasisChamberLeave", out var leaveAction))
            {
                _actionsSystem.AddAction(target, new InstantAction(leaveAction), uid);
            }


            _standingStateSystem.Stand(target, force: true);

            stasisChamberComponent.EntityLiedInStasisChamberTime = _gameTiming.CurTime;

            UpdateAppearance(uid, stasisChamberComponent);
            return true;
        }

        public void TryEjectBody(EntityUid uid, EntityUid userId, StasisChamberComponent? stasisChamberComponent)
        {
            if (!Resolve(uid, ref stasisChamberComponent))
            {
                return;
            }

            var ejected = EjectBody(uid, stasisChamberComponent);
            if (ejected != null)
                _adminLogger.Add(LogType.Action, LogImpact.Medium,
                    $"{ToPrettyString(ejected.Value)} ejected from {ToPrettyString(uid)} by {ToPrettyString(userId)}");
        }

        public virtual EntityUid? EjectBody(EntityUid uid, StasisChamberComponent? stasisChamberComponent)
        {
            if (!Resolve(uid, ref stasisChamberComponent))
            {
                return null;
            }

            if (stasisChamberComponent.BodyContainer.ContainedEntity is not { Valid: true } contained)
            {
                return null;
            }

            stasisChamberComponent.BodyContainer.Remove(contained);

            if (HasComp<KnockedDownComponent>(contained) || _mobStateSystem.IsIncapacitated(contained))
            {
                _standingStateSystem.Down(contained);
            }
            else
            {
                _standingStateSystem.Stand(contained);
            }

            _actionsSystem.RemoveProvidedActions(contained, uid);

            UpdateAppearance(uid, stasisChamberComponent);
            return contained;
        }

        private void OnStasisChamberCanDropTarget(EntityUid uid, StasisChamberComponent component,
            ref CanDropTargetEvent args)
        {
            if (args.Handled)
                return;

            args.CanDrop = HasComp<BodyComponent>(args.Dragged);
            args.Handled = true;
        }

        protected void OnComponentInit(EntityUid uid, StasisChamberComponent stasisChamberComponent, ComponentInit args)
        {
            stasisChamberComponent.BodyContainer = _containerSystem.EnsureContainer<ContainerSlot>(uid, "Chamber-body");
        }

        protected void UpdateAppearance(EntityUid uid, StasisChamberComponent? stasisChamber = null,
            AppearanceComponent? appearance = null)
        {
            if (!Resolve(uid, ref stasisChamber))
            {
                return;
            }

            if (!Resolve(uid, ref appearance))
            {
                return;
            }

            _appearanceSystem.SetData(uid, StasisChamberComponent.StasisChamberVisuals.ContainsEntity,
                stasisChamber.BodyContainer.ContainedEntity is null || _entityManager.IsQueuedForDeletion(stasisChamber.BodyContainer.ContainedEntity.Value), appearance);
        }

        protected void AddAlternativeVerbs(EntityUid uid, StasisChamberComponent stasisChamberComponent,
            GetVerbsEvent<AlternativeVerb> args)
        {
            if (!args.CanAccess || !args.CanInteract || args.User != stasisChamberComponent.BodyContainer.ContainedEntity)
                return;

            if (stasisChamberComponent.BodyContainer.ContainedEntity != null)
            {
                args.Verbs.Add(new AlternativeVerb
                {
                    Text = Loc.GetString("stasisChamber-verb-noun-occupant"),
                    Category = VerbCategory.Eject,
                    Priority = 1,
                    Act = () => TryEjectBody(uid, args.User, stasisChamberComponent)
                });
            }
        }



        [Serializable, NetSerializable]
        public sealed class StasisChamberDragFinished : SimpleDoAfterEvent
        {
        }
    }
}

public sealed class StasisChamberLeaveActionEvent : InstantActionEvent
{
}