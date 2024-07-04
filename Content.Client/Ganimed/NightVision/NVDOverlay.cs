using Content.Shared.Construction.Components;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.NVD;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;

namespace Content.Client.NVD;

public sealed class NVDOverlay : Overlay
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;
    [Dependency] private readonly IEyeManager _eye = default!;

    public override bool RequestScreenTexture => true;
    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    private readonly ShaderInstance _scanlineShader;
    private readonly ShaderInstance _greyscaleShader;

    public bool Enabled => _enabled;

    private bool _enabled = false;

    public NVDOverlay()
    {
        IoCManager.InjectDependencies(this);
        _scanlineShader = _prototypeManager.Index<ShaderPrototype>("Scanline").InstanceUnique();
        _greyscaleShader = _prototypeManager.Index<ShaderPrototype>("GreyscaleFullscreen").InstanceUnique();
    }

    public void SetEnabled(bool enabled)
    {
        _lightManager.DrawLighting = !enabled;
        _enabled = enabled;
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_enabled)
            return false;

        var draw = true;

        var playerEntity = _playerManager.LocalSession?.AttachedEntity;

        if (!_entityManager.TryGetComponent(_playerManager.LocalSession?.AttachedEntity, out EyeComponent? eyeComp)
            || args.Viewport.Eye != eyeComp.Eye
            || playerEntity is null)
            draw = false;

        return draw;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        var playerEntity = _playerManager.LocalSession?.AttachedEntity;

        if (playerEntity == null)
            return;
    }
}
