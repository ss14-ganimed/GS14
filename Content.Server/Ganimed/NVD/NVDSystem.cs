using Content.Shared.NVD;
using Content.Shared.Toggleable;

namespace Content.Server.NVD;

public sealed class NVDSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NVDComponent, ToggleActionEvent>(Toggle);
    }

    private void Toggle(Entity<NVDComponent> uid, ref ToggleActionEvent args)
    {
        if (uid.Comp.Enabled)
            Disable(uid);
        else
            Enable(uid);
        Dirty(uid);
    }

    public void Disable(Entity<NVDComponent> uid)
    {
        uid.Comp.Enabled = false;
        _appearance.SetData(uid, ToggleVisuals.Toggled, false);
    }

    public void Enable(Entity<NVDComponent> uid)
    {
        uid.Comp.Enabled = true;
        _appearance.SetData(uid, ToggleVisuals.Toggled, true);
    }
}
