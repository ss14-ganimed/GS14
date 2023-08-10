using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Content.Server.Chat.Systems;
using Content.Server.Forensics;
using Content.Server.Hands.Systems;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Objectives.Conditions;
using Content.Server.Objectives.Interfaces;
using Content.Server.Station.Systems;
using Content.Server.StationRecords.Systems;
using Content.Server.Storage.Components;
using Content.Shared.Access.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Item;
using Content.Shared.Ganimed.StasisChamber;
using Content.Shared.StationRecords;
using Content.Shared.Whitelist;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using static Content.Shared.Storage.SharedStorageComponent;

namespace Content.Server.Ganimed.StasisChamber;


public sealed class StasisStorageConsoleSystem : EntitySystem
{
    [Dependency] private readonly StationRecordsSystem _stationRecordsSystem = default!;
    [Dependency] private readonly ActionBlockerSystem _actionBlockerSystem = default!;
    [Dependency] private readonly AccessReaderSystem _accessReaderSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly IObjectivesManager _objectivesManager = default!;
    [Dependency] private readonly MindTrackerSystem _mindTrackerSystem = default!;
    [Dependency] private readonly StationJobsSystem _stationJobsSystem = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly HandsSystem _handsSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;

    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();

        _sawmill = Logger.GetSawmill("StasisStorageConsole");

        SubscribeLocalEvent<StasisStorageConsoleComponent, StasisChamberStorageInteractWithItemEvent>(OnInteractWithItem);
        SubscribeLocalEvent<StasisStorageConsoleComponent, EntRemovedFromContainerMessage>(OnStorageItemRemoved);
        SubscribeLocalEvent<StasisStorageConsoleComponent, BoundUIOpenedEvent>(UpdateUserInterface);

        SubscribeLocalEvent<TransferredToStasisStorageEvent>(OnTransferredToStasis);

        SubscribeLocalEvent<StasisStorageConsoleComponent, GotEmaggedEvent>(OnEmagAct);
    }

    private void OnEmagAct(EntityUid uid, StasisStorageConsoleComponent component, ref GotEmaggedEvent args)
    {
        args.Handled = true;
    }


    /// <summary>
    /// Method for transferring entity to stasis storage
    /// </summary>
    /// <param name="uid"> Entity that has StasisStorageConsoleComponent</param>
    /// <param name="target"> Entity to transfer</param>
    /// <param name="stasisChamberConsoleComp"> component</param>
    public void TransferToStasisStorage(EntityUid uid, EntityUid target, StasisStorageConsoleComponent? stasisChamberConsoleComp = null)
    {
        if (Resolve(uid, ref stasisChamberConsoleComp))
        {
            TransferToStasisStorage(uid, stasisChamberConsoleComp, target);
        }
    }

    private void OnInteractWithItem(EntityUid uid, StasisStorageConsoleComponent component, StasisChamberStorageInteractWithItemEvent args)
    {
        if (args.Session.AttachedEntity is not EntityUid player)
            return;

        if (!Exists(args.InteractedItemUid))
        {
            _sawmill.Error($"Player {args.Session} interacted with non-existent item {args.InteractedItemUid} stored in {ToPrettyString(uid)}");
            return;
        }

        if (!TryComp<ServerStorageComponent>(uid, out var storageComp))
        {
            return;
        }

        if (!_actionBlockerSystem.CanInteract(player, args.InteractedItemUid) || storageComp.Storage == null || !storageComp.Storage.Contains(args.InteractedItemUid))
            return;

        if (!TryComp(player, out HandsComponent? hands) || hands.Count == 0)
            return;

        if (!_accessReaderSystem.IsAllowed(player, uid))
        {
            _sawmill.Info($"Player {ToPrettyString(player)} possibly exploits UI, trying to take item from {ToPrettyString(uid)} without access");
            return;
        }

        if (hands.ActiveHandEntity == null)
        {
            if (_handsSystem.TryPickupAnyHand(player, args.InteractedItemUid, handsComp: hands)
                && storageComp.StorageRemoveSound != null)
                _sawmill.Info($"{ToPrettyString(player)} takes {ToPrettyString(args.InteractedItemUid)} from {ToPrettyString(uid)}");
        }
    }

    /// <summary>
    /// System reacts to broadcast event
    /// first suitable StasisStorageConsole component will handle it
    /// </summary>
    /// <param name="args"> Event contains information about the stasisChamber and the entity,
    /// which we must transfer to the stasis storage</param>
    private void OnTransferredToStasis(TransferredToStasisStorageEvent args)
    {
        if (args.Handled)
        {
            return;
        }

        var entityEnumerator = EntityQueryEnumerator<StasisStorageConsoleComponent>();

        while (entityEnumerator.MoveNext(out var uid, out var stasisStorageConsoleComp))
        {
            if (stasisStorageConsoleComp.IsStasisChamber)
            {
                continue;
            }

            var consoleCoord = Transform(uid).Coordinates;
            var stasisChamberCoord = Transform(args.StasisChamber).Coordinates;

            if (consoleCoord.InRange(_entityManager, _transformSystem, stasisChamberCoord, stasisStorageConsoleComp.RadiusToConnect))
            {
                args.Handled = true;
                TransferToStasisStorage(uid, stasisStorageConsoleComp, args.EntityToTransfer);
                return;
            }
        }
    }

    private void TransferToStasisStorage(EntityUid uid, StasisStorageConsoleComponent component, EntityUid entityToTransfer)
    {
        _sawmill.Info($"{ToPrettyString(entityToTransfer)} moved to stasis storage");

        var station = _stationSystem.GetOwningStation(uid);

        if (station is not null)
        {
            DeleteEntityRecord(entityToTransfer, station.Value, out var job);
			
			var jobName = job != null 
				? job.JobTitle ?? Loc.GetString("job-title-no-occupation")
				: Loc.GetString("job-title-no-occupation");
				
			jobName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(jobName);
            
            _chatSystem.DispatchStationAnnouncement(station.Value, 
                Loc.GetString(
                    "stasisChamber-entered-stasis",
                    ("character", MetaData(entityToTransfer).EntityName),
                    ("job", jobName)),
                Loc.GetString("stasisChamber-sender"));
            
			string formatRTime = string.Format("{0:hh\\:mm\\:ss}", _gameTiming.RealTime);
            component.StoredEntities.Add($"[{jobName}] {MetaData(entityToTransfer).EntityName} - {formatRTime}");
        }

        UndressEntity(uid, component, entityToTransfer);

        _entityManager.QueueDeleteEntity(entityToTransfer);

        ReplaceKillEntityObjectives(entityToTransfer);
    }

    /// <summary>
    /// Looks through all objectives in game,
    /// All KillPersonObjective where target equals to uid
    /// would be replaced with new random objective 
    /// </summary>
    /// <param name="uid"> target uid</param>
    private void ReplaceKillEntityObjectives(EntityUid uid)
    {
        foreach (var mind in _mindTrackerSystem.AllMinds)
        {
            if (mind.OwnedEntity is null)
            {
                continue;
            }

            IEnumerable<Objective> objectiveToReplace = mind.AllObjectives.Where(objective =>
                objective.Conditions.Any(condition => (condition as KillPersonCondition)?.IsTarget(uid) ?? false));

            foreach (var objective in objectiveToReplace)
            {
                _mindSystem.TryRemoveObjective(mind, mind.Objectives.IndexOf(objective));
                var newObjective = _objectivesManager.GetRandomObjective(mind, "TraitorObjectiveGroups");
                if (newObjective is null || !_mindSystem.TryAddObjective(mind, newObjective))
                {
                    _sawmill.Error($"{ToPrettyString(mind.OwnedEntity.Value)}'s target is in stasisChamber, so he lost his objective and didn't get a new one");
                    continue;
                }

                _sawmill.Info($"{ToPrettyString(mind.OwnedEntity.Value)}'s target is in stasisChamber, so he got a new one");
            }
        }
    }

    /// <summary>
    /// Looking through all Entity's items,
    /// and if item is not in storage whitelist - deletes it,
    /// otherwise transfers it to stasis storage
    /// </summary>
    /// <param name="uid"> EntityUid of the storage</param>
    /// <param name="component"></param>
    /// <param name="target"> Entity to undress</param>

    private void UndressEntity(EntityUid uid, StasisStorageConsoleComponent component, EntityUid target)
    {
        if (!TryComp<ServerStorageComponent>(uid, out var storageComponent)
            || storageComponent.Storage is null)
        {
            return;
        }

        /*
        * It would be great if we could instantly delete items when we know they are not whitelisted.
        * However, this could lead to a situation where we accidentally delete the uniform,
        * resulting in all items inside the pockets being dropped before we add them to the itemsToTransfer list.
        * So we should have itemsToDelete list.
        */

        List<EntityUid> itemsToTransfer = new();
        List<EntityUid> itemsToDelete = new();

        // Looking through all 
        SortContainedItems(in target,ref itemsToTransfer,ref itemsToDelete, in component.Whitelist);

        foreach (var item in itemsToTransfer)
        {
            storageComponent.Storage.Insert(item);
        }

        foreach (var item in itemsToDelete)
        {
            _entityManager.DeleteEntity(item);
        }
    }

    /// <summary>
    /// Recursively goes through all child entities of our entity
    /// and if entity is item - adds it to whiteListedItems,
    /// otherwise adds it to itemsToDelete 
    /// </summary>
    /// <param name="storageToLook"></param>
    /// <param name="whitelistedItems"></param>
    /// <param name="itemsToDelete"></param>
    /// <param name="whitelist"></param>
    private void SortContainedItems(in EntityUid storageToLook, ref List<EntityUid> whitelistedItems,
        ref List<EntityUid> itemsToDelete, in EntityWhitelist? whitelist)
    {
        if (TryComp<TransformComponent>(storageToLook, out var transformComponent))
        {
            foreach (var childUid in transformComponent.ChildEntities)
            {
                if (!HasComp<ItemComponent>(childUid))
                {
                    continue;
                }

                if (whitelist is null || whitelist.IsValid(childUid))
                {
                    whitelistedItems.Add(childUid);
                }
                else
                {
                    itemsToDelete.Add(childUid);
                }

                // As far as I know, ChildEntities cannot be recursive 
                SortContainedItems(in childUid, ref whitelistedItems, ref itemsToDelete, in whitelist);
            }
        }
    }

    /// <summary>
    /// Delete entity records from station general records
    /// using DNA to match record
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="station"></param>
    /// <param name="job"> returns job of entity </param>
    private bool DeleteEntityRecord(EntityUid uid, EntityUid station,[NotNullWhen(true)] out GeneralStationRecord? deletedRecord)
    {
        var stationRecord = FindEntityStationRecordKey(station, uid);
		
		deletedRecord = null;
		
        if (stationRecord is null)
        {
            return false;
        }

        deletedRecord = stationRecord.Value.Item2;

        _stationRecordsSystem.RemoveRecord(station, stationRecord.Value.Item1);
		
		return true;
    }

    private (StationRecordKey, GeneralStationRecord)? FindEntityStationRecordKey(EntityUid station, EntityUid uid)
    {
        if (TryComp<DnaComponent>(uid, out var dnaComponent))
        {
            var stationRecords = _stationRecordsSystem.GetRecordsOfType<GeneralStationRecord>(station);
            var result = stationRecords.FirstOrNull(records => records.Item2.DNA == dnaComponent.DNA);
            if (result is not null)
            {
                return result.Value;
            }
        }

        return null;
    }

    private void OnStorageItemRemoved(EntityUid uid, StasisStorageConsoleComponent storageComp, EntRemovedFromContainerMessage args)
    {

        UpdateUserInterface(uid, storageComp, args.Entity, true);
    }

    private void UpdateUserInterface(EntityUid uid, StasisStorageConsoleComponent component, BoundUIOpenedEvent args)
    {
        if (args.Session.AttachedEntity is null)
        {
            return;
        }
        UpdateUserInterface(uid, component, args.Session.AttachedEntity.Value);
    }

    private void UpdateUserInterface(EntityUid uid, StasisStorageConsoleComponent? component, EntityUid user,
        bool forseAccess = false)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }

        if (TryComp<ServerStorageComponent>(uid, out var storageComponent) && storageComponent.StoredEntities is not null)
        {
            var hasAccess = HasComp<EmaggedComponent>(uid) || _accessReaderSystem.IsAllowed(user, uid) || forseAccess;
            var storageState = hasAccess ?  
                new StorageBoundUserInterfaceState((List<EntityUid>) storageComponent.StoredEntities, 
                    storageComponent.StorageUsed, 
                    storageComponent.StorageCapacityMax)
                : new StorageBoundUserInterfaceState(new List<EntityUid>(),
                    0,
                    storageComponent.StorageCapacityMax);

            var state = new StasisStorageConsoleState(hasAccess, component.StoredEntities, storageState);
            SetStateForInterface(uid, state);
        }
    }

    private void SetStateForInterface(EntityUid uid, StasisStorageConsoleState storageConsoleState)
    {
        var ui = _userInterface.GetUiOrNull(uid, StasisStorageConsoleKey.Key);
        if (ui is not null)
        {
            UserInterfaceSystem.SetUiState(ui, storageConsoleState);
        }
    }
}