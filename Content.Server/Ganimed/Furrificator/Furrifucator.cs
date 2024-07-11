using System.Linq;
using Content.Server.Humanoid;
using Content.Server.GameTicking;
using Robust.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Server.Speech.Components;

namespace Content.Server.Furrificator
{
    public sealed class FurrificatorSystem : EntitySystem
    {
        [Dependency] private readonly HumanoidAppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawned);
        }

        private void OnPlayerSpawned(PlayerSpawnCompleteEvent ev)
        {
            var player = ev.Player;
            var entity = player.AttachedEntity;

            if (entity == null)
                return;

            if (player.Name == "Safno_San")
            {
                if (!_entityManager.HasComponent<OwOAccentComponent>(entity.Value))
                {
                    _entityManager.AddComponent<OwOAccentComponent>(entity.Value);
                    Logger.InfoS("Furrificator", $"Added OwOAccentComponent to player {player.Name}");
                }

                if (TryComp<HumanoidAppearanceComponent>(entity.Value, out var appearanceComponent))
                { 
                  _appearanceSystem.AddMarking(entity.Value, "CatEars", Color.White, true, true, appearanceComponent);
                  _appearanceSystem.AddMarking(entity.Value, "CatTail", Color.White, true, true, appearanceComponent);
                }
            }
        }
    }
}
