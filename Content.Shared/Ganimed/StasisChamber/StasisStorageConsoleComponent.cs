using Content.Shared.Whitelist;
using Robust.Shared.Serialization;

namespace Content.Shared.Ganimed.StasisChamber;

[RegisterComponent]
public sealed class StasisStorageConsoleComponent : Component
{
    /// <summary>
    /// List for IC knowing who went in stasis
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public List<string> StoredEntities = new List<string>();

    /// <summary>
    /// All items that are not whitelisted will be
    /// irretrievably lost after the essence is transferred to stasisstorage.
    /// </summary>
    [DataField("whitelist"), ViewVariables(VVAccess.ReadWrite)]
    public EntityWhitelist? Whitelist = null;

    /// <summary>
    /// Radius to check if the console can handle the TransferredEntity event
    /// of the stasisChambers
    /// </summary>
    [DataField("radiusToConnect"), ViewVariables(VVAccess.ReadWrite)]
    public float RadiusToConnect = 500f;

    /// <summary>
    /// We want the stasisChamber to have ConsoleComponent for
    /// situations where there are no available console entity nearly
    /// that variable for knowing is it really console or stasisChamber
    /// </summary>
    [DataField("isItReallyStasisChamber"), ViewVariables(VVAccess.ReadOnly)]
    public bool IsStasisChamber = false;
}

[Serializable, NetSerializable]
public sealed class StasisChamberStorageInteractWithItemEvent : BoundUserInterfaceMessage
{
    public readonly EntityUid InteractedItemUid;
    public StasisChamberStorageInteractWithItemEvent(EntityUid interactedItemUid)
    {
        InteractedItemUid = interactedItemUid;
    }
}