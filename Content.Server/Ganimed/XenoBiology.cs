/// Maded by Gorox for Enterprise. See CLA
using System.Numerics;
using Content.Server.Ganimed.XenoBiology.Components;
using Content.Server.Ganimed.XenoFood.Components;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.IoC;
using Content.Shared.Polymorph.Systems;
using Content.Server.Polymorph.Systems;
using Content.Server.Polymorph.Components;
using Robust.Server.GameObjects;

namespace Content.Server.Ganimed.XenoBiology.Systems;

public sealed class XenoBiologySystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;

    private const string PolymorphId = "RandomSlimePerson"; // Прототип полиморфа
    private const int PointsPerAttack = 10; // Очки за атаку
    private const int PointsThreshold = 200; // Сколько необходимо для деления

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<XenoBiologyComponent, MeleeHitEvent>(OnSlimeAttack);
    }

    private void OnSlimeAttack(EntityUid uid, XenoBiologyComponent component, ref MeleeHitEvent args)
    {
        foreach (var hitEntity in args.HitEntities)
        {
            if (EntityManager.HasComponent<XenoFoodComponent>(hitEntity))
            {
                // Атакующий получает очки
                component.Points += PointsPerAttack;

                // Проверяем, достиг ли компонент порога очков
                if (component.Points >= PointsThreshold)
                {

                    if (TryComp<MindContainerComponent>(uid, out var mindContainer) && mindContainer.HasMind)
                    {
                       _polymorph.PolymorphEntity(uid, PolymorphId);
                    }
                    else

                    // С шансом 30% мутирует при делении
                    if (_robustRandom.Prob(component.Mutationchance))
                    {
                        Spawn(component.Mutagen, Transform(uid).Coordinates);
                    }
                    else
                    {
                        // Иначе делится на исходный(щиткод уэээ)
                        Spawn(component.Antimutagen, Transform(uid).Coordinates);
                    }

                    if (_robustRandom.Prob(component.Mutationchance))
                    {
                        Spawn(component.Mutagen, Transform(uid).Coordinates);
                    }
                    else
                    {
                        Spawn(component.Antimutagen, Transform(uid).Coordinates);
                    }

                    if (_robustRandom.Prob(component.Mutationchance))
                    {
                        Spawn(component.Mutagen, Transform(uid).Coordinates);
                    }
                    else
                    {
                        Spawn(component.Antimutagen, Transform(uid).Coordinates);
                    }
                    EntityManager.DeleteEntity(uid);

                    // После достижения порога очков и выполнения действий, выходим из метода
                    return;
                }
                break;
            }
        }
    }
}