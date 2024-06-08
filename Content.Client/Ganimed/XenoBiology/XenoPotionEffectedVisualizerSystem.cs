using System.Linq;
using Robust.Client.GameObjects;
using static Robust.Client.GameObjects.SpriteComponent;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Ganimed.XenoPotionEffected.Components;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client.Ganimed.XenoPotionEffected
{
    public sealed class XenoPotionEffectedVisualizerSystem : VisualizerSystem<XenoPotionEffectedComponent>
    {
        /// <summary>
        /// Visualizer for Paint which applies a shader and colors the entity.
        /// </summary>

        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly IPrototypeManager _protoMan = default!;

        public ShaderInstance? Shader;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<XenoPotionEffectedComponent, HeldVisualsUpdatedEvent>(OnHeldVisualsUpdated);
            SubscribeLocalEvent<XenoPotionEffectedComponent, EquipmentVisualsUpdatedEvent>(OnEquipmentVisualsUpdated);
        }

        protected override void OnAppearanceChange(EntityUid uid, XenoPotionEffectedComponent component, ref AppearanceChangeEvent args)
        {
            Shader = _protoMan.Index<ShaderPrototype>(component.ShaderName).Instance();

            if (args.Sprite == null)
                return;


            var sprite = args.Sprite;


            foreach (var spriteLayer in sprite.AllLayers)
            {
                if (spriteLayer is not Layer layer)
                    continue;

                if (layer.Shader == null) // If shader isn't null we dont want to replace the original shader.
                {
                    layer.Shader = Shader;
                    layer.Color = component.Color;
                }
            }
        }

        private void OnHeldVisualsUpdated(EntityUid uid, XenoPotionEffectedComponent component, HeldVisualsUpdatedEvent args)
        {
            if (args.RevealedLayers.Count == 0)
                return;

            if (!TryComp(args.User, out SpriteComponent? sprite))
                return;

            foreach (var revealed in args.RevealedLayers)
            {
                if (!sprite.LayerMapTryGet(revealed, out var layer) || sprite[layer] is not Layer notlayer)
                    continue;

                sprite.LayerSetShader(layer, component.ShaderName);
                sprite.LayerSetColor(layer, component.Color);
            }
        }

        private void OnEquipmentVisualsUpdated(EntityUid uid, XenoPotionEffectedComponent component, EquipmentVisualsUpdatedEvent args)
        {
            if (args.RevealedLayers.Count == 0)
                return;

            if (!TryComp(args.Equipee, out SpriteComponent? sprite))
                return;

            foreach (var revealed in args.RevealedLayers)
            {
                if (!sprite.LayerMapTryGet(revealed, out var layer) || sprite[layer] is not Layer notlayer)
                    continue;

                sprite.LayerSetShader(layer, component.ShaderName);
                sprite.LayerSetColor(layer, component.Color);
            }
        }
    }
}
