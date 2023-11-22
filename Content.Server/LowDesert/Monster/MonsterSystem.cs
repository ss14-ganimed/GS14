using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.LowDesert.Monster;
using Content.Shared.LowDesert.Monster.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameObjects;

namespace Content.Server.LowDesert.Monster;

public sealed class MonsterSystem : SharedMonsterConsumeSystem
{
	
	[Dependency] private readonly SharedActionsSystem _actions = default!;
	[Dependency] private readonly MobThresholdSystem _thresholds = default!;
	[Dependency] private readonly DamageableSystem _damage = default!;
	[Dependency] private readonly UserInterfaceSystem _userInterfaceSystem = default!;
	
	public override void Initialize()
	{
		base.Initialize();
		
		SubscribeLocalEvent<MonsterComponent, ConsumeDoAfterEvent>(OnConsumeDoAfter);
		
		SubscribeLocalEvent<MonsterComponent, BoundUIOpenedEvent>(SubscribeUpdateUiState);
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
		
		var item = new MonsterEvolutionItem("Эволюция", "Нажми сюда, чтобы эволюционировать", 10.0f);
		
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
		
		var overview = new MonsterEvolutionOverview(name, 
			health is null ? 0.0f : (float) health);
			
		var state = new MonsterEvolutionBoundUserInterfaceState(items, monster.Comp.EvoPoints, overview);
		_userInterfaceSystem.TrySetUiState(monster, MonsterEvolutionMenuKey.Key, state);
	}
	
}