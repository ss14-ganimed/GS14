using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Ganimed.StasisChamber;


/// <summary>
/// Component for In-game leaving or AFK
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed class StasisChamberComponent : Component
{
    /// <summary>
    /// Delay before climbing in stasisChamber
    /// </summary>
    [DataField("entryDelay")] public float EntryDelay = 6f;

    [ViewVariables(VVAccess.ReadWrite)] public TimeSpan EntityLiedInStasisChamberTime;

    [ViewVariables(VVAccess.ReadWrite)] public ContainerSlot BodyContainer = default!;

    [Serializable, NetSerializable]
    public enum StasisChamberVisuals : byte
    {
        ContainsEntity
    }
}

/// <summary>
/// Raises when somebody transfers to stasis storage 
/// </summary>
public sealed class TransferredToStasisStorageEvent : HandledEntityEventArgs
{
    public EntityUid StasisChamber { get; }
    public EntityUid EntityToTransfer { get; }

    public TransferredToStasisStorageEvent(EntityUid stasisChamber, EntityUid entityToTransfer)
    {
        StasisChamber = stasisChamber;
        EntityToTransfer = entityToTransfer;
    }
}