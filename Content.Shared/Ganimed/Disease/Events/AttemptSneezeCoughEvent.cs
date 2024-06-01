namespace Content.Shared.Ganimed.Disease.Events;

public sealed class AttemptSneezeCoughEvent(string? EmoteId) : CancellableEntityEventArgs
{
    public string? EmoteId { get; } = EmoteId;
}
