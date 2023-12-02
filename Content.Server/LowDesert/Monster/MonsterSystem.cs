using Content.Server.Mind;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.LowDesert.Monster;
using Content.Shared.LowDesert.Monster.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Weapons.Melee;
using Robust.Server.GameObjects;
using Content.Shared.Eye.Blinding.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.LowDesert.Monster;

public sealed class MonsterSystem : SharedMonsterConsumeSystem
{
	
	[Dependency] private readonly SharedActionsSystem _actions = default!;
	[Dependency] private readonly MobThresholdSystem _thresholds = default!;
	[Dependency] private readonly DamageableSystem _damage = default!;
	[Dependency] private readonly UserInterfaceSystem _userInterfaceSystem = default!;
	[Dependency] private readonly IPrototypeManager _prototypeManager = default!;
	[Dependency] private readonly IEntityManager _entityManager = default!;
	[Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
	
	public override void Initialize()
	{
		base.Initialize();
		
		SubscribeLocalEvent<MonsterComponent, BoundUIOpenedEvent>(SubscribeUpdateUiState);
		
        SubscribeLocalEvent<MonsterComponent, MonsterEvolutionMessage>(OnMutation);
		
        SubscribeLocalEvent<MonsterComponent, MonsterEvolutionEvolveMessage>(OnEvolution);
	}
	
	private void OnConsumeDoAfter(EntityUid uid, MonsterComponent component, ConsumeDoAfterEvent args)
	{
		if (args.Handled || args.Cancelled || args.Args.Target is null)
            return;
		
		if (!EntityManager.TryGetComponent<MonsterConsumableComponent>(args.Args.Target, out var consumable))
			return;
		
		if (!consumable.IsConsumable)
			return;
		
		consumable.IsConsumable = false;
		
		if (EntityManager.TryGetComponent<DamageableComponent>(args.Args.Target, out var damageable))
		{
			if (_thresholds.TryGetThresholdForState(args.Args.Target.Value, MobState.Dead, out var threshold) && threshold is not null && threshold > 0.0f)
			{
				component.EvoPoints += consumable.EvoPointsGranted * (component.ConsumeDamageValue / (float)(threshold.Value));
			}
			else
			{
				component.EvoPoints += consumable.EvoPointsGranted;
			}
			
			DamageSpecifier damage = new()
			{
				DamageDict = new() { { component.ConsumeDamageID, component.ConsumeDamageValue } }
			};
			_damage.TryChangeDamage(args.Args.Target, damage, true, true, damageable, uid);
			
		}
		else
		{
			component.EvoPoints += consumable.EvoPointsGranted;
		}
	}
	
	private void SubscribeUpdateUiState<T>(Entity<MonsterComponent> ent, ref T ev)
    {
        UpdateUiState(ent);
    }
	
	private void UpdateUiState(Entity<MonsterComponent>	monster)
	{
		var items = new List<MonsterEvolutionItem>();
		
		var item = new MonsterEvolutionItem("Мутация", "Нажми сюда, чтобы мутировать", 10.0f);
		
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		items.Add(item);
		
		_thresholds.TryGetThresholdForState(monster, MobState.Dead, out var health);
		
		var name = "???";
		if (EntityManager.TryGetComponent<MetaDataComponent>(monster, out var metaData))
			name = metaData.EntityName;
		
		var attackSpeed = 0.0f;
		var attackDamage = 0.0f;
		var attackRange = 0.0f;
		if (EntityManager.TryGetComponent<MeleeWeaponComponent>(monster, out var meleeWeapon))
		{
			attackSpeed = meleeWeapon.AttackRate * 10.0f;
			attackDamage = (float) meleeWeapon.Damage.GetTotal();
			attackRange = meleeWeapon.Range * 10.0f;
		}
		
		var walkSpeed = 0.0f;
		var runSpeed = 0.0f;
		if (EntityManager.TryGetComponent<MovementSpeedModifierComponent>(monster, out var movement))
		{
			walkSpeed = movement.BaseWalkSpeed * 7f;
			runSpeed = movement.BaseSprintSpeed * 7f;
		}
		
		var staminaCooldown = 0.0f;
		var staminaThreshold = 0.0f;
		var staminaReplenish = 0.0f;
		var staminaTime = 0.0f;
		if (EntityManager.TryGetComponent<StaminaComponent>(monster, out var stamina))
		{
			staminaCooldown = stamina.Cooldown;
			staminaThreshold = stamina.CritThreshold;
			staminaReplenish = stamina.Decay;
			staminaTime = (float) stamina.StunTime.TotalSeconds;
		}
		
		var overview = new MonsterEvolutionOverview(
			name, 
			health is null ? 0.0f : (float) health,
			attackSpeed,
			attackDamage,
			attackRange,
			monster.Comp.ConsumeDamageValue / 10.0f,
			50.0f / monster.Comp.ConsumeTime,
			monster.Comp.EvoPoints,
			walkSpeed,
			runSpeed,
			staminaCooldown,
			staminaThreshold,
			staminaReplenish,
			staminaTime,
			monster.Comp.Species,
			monster.Comp.Class,
			monster.Comp.EvoPointsSpent,
			monster.Comp.EvoPointsRequired);
		
		var evolutions = new List<MonsterEvolutionPrototype>();
		
		foreach (var prototype in monster.Comp.Evolutions)
		{
			if (_prototypeManager.TryIndex<MonsterEvolutionPrototype>(prototype, out var evolution))
				evolutions.Add(evolution);
		}
			
		var state = new MonsterEvolutionBoundUserInterfaceState(items, monster.Comp.EvoPoints, overview, evolutions);
		_userInterfaceSystem.TrySetUiState(monster, MonsterEvolutionMenuKey.Key, state);
	}
	
	private void OnMutation(EntityUid uid, MonsterComponent component, MonsterEvolutionMessage args)
	{
		
	}
	
	private void OnEvolution(EntityUid uid, MonsterComponent component, MonsterEvolutionEvolveMessage args)
	{
		if (component.EvoPoints < args.Evolution.Cost)
			return;
		
		component.EvoPoints -= args.Evolution.Cost;
		
		if (_mindSystem.TryGetMind(uid, out var mindId, out var mind) && !Deleted(uid))
		{
			var eggEntity = _entityManager.SpawnEntity("MobMonsterEgg", Transform(uid).Coordinates);
			
			_mindSystem.TransferTo(mindId, eggEntity, mind: mind);
			_entityManager.DeleteEntity(uid);
			
			var evolutionComp = EnsureComp<MonsterEggEvolutionComponent>(eggEntity);
			evolutionComp.EvolutionTime = _gameTiming.CurTime + TimeSpan.FromSeconds(10f);
			evolutionComp.Prototype = args.Evolution.Prototype;
			evolutionComp.StoredPoints = component.EvoPoints;
		}
	}
	
}