using Robust.Shared.Serialization;
using static Content.Shared.Storage.SharedStorageComponent;
namespace Content.Shared.Ganimed.StasisChamber;

[Serializable, NetSerializable]
public enum StasisStorageConsoleKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class StasisStorageConsoleState : BoundUserInterfaceState
{
    public bool HasAccess { get; }
    public StorageBoundUserInterfaceState StorageState { get; }
    public List<string> StasisChamberRecords { get; }

    public StasisStorageConsoleState (bool hasAccess, List<string> stasisChamberRecords, StorageBoundUserInterfaceState storageState)
    {
        HasAccess = hasAccess;
        StasisChamberRecords = stasisChamberRecords;
        StorageState = storageState;
    }
}