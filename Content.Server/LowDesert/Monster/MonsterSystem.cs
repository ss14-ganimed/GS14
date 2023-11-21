using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.LowDesert.Monster;
using Content.Shared.LowDesert.Monster.Components;

namespace Content.Server.LowDesert.Monster;

public sealed class MonsterSystem : SharedMonsterConsumeSystem
{
	
	[Dependency] private readonly SharedActionsSystem _actions = default!;
	[Dependency] private readonly DamageableSystem _damage = default!;
	
	public override void Initialize()
	{
		base.Initialize();
		
		SubscribeLocalEvent<MonsterComponent, MapInitEvent>(OnMapInit);
		SubscribeLocalEvent<MonsterComponent, ComponentShutdown>(OnComponentShutdown);
		SubscribeLocalEvent<MonsterComponent, ConsumeDoAfterEvent>(OnConsumeDoAfter);
	}
	
	private void OnMapInit(EntityUid uid, MonsterComponent component, MapInitEvent args)
	{
		_actions.AddAction(uid, ref component.EvolutionActionEntity, component.EvolutionAction);
	}
	
	private void OnComponentShutdown(EntityUid uid, MonsterComponent component, ComponentShutdown args)
	{
		if (component.EvolutionActionEntity != null)
			_actions.RemoveAction(uid, component.EvolutionActionEntity);
	}
	
	private void OnConsumeDoAfter(EntityUid uid, MonsterComponent component, ConsumeDoAfterEvent args)
	{
		if (args.Handled || args.Cancelled || args.Args.Target is null)
            return;
		
		if (!EntityManager.TryGetComponent<MonsterConsumableComponent>(args.Args.Target, out var consumable))
			return;
		
		if (!consumable.IsConsumable)
			return;
		
		component.EvoPoints += consumable.EvoPointsGranted;
		consumable.IsConsumable = false;
		
		if (EntityManager.TryGetComponent<DamageableComponent>(args.Args.Target, out var damageable))
		{
			DamageSpecifier damage = new()
			{
				DamageDict = new() { { component.ConsumeDamageID, component.ConsumeDamageValue } }
			};
			_damage.TryChangeDamage(args.Args.Target, damage, true, true, damageable, uid);
		}
	}
	
}