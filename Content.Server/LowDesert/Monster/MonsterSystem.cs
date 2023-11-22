using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.LowDesert.Monster;
using Content.Shared.LowDesert.Monster.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;

namespace Content.Server.LowDesert.Monster;

public sealed class MonsterSystem : SharedMonsterConsumeSystem
{
	
	[Dependency] private readonly SharedActionsSystem _actions = default!;
	[Dependency] private readonly MobThresholdSystem _thresholds = default!;
	[Dependency] private readonly DamageableSystem _damage = default!;
	
	public override void Initialize()
	{
		base.Initialize();
		
		SubscribeLocalEvent<MonsterComponent, ConsumeDoAfterEvent>(OnConsumeDoAfter);
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
	
}