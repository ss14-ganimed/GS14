using Content.Shared.LowDesert.Monster.Components;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.Mind;
using Robust.Shared.Timing;

namespace Content.Server.LowDesert.Monster;

public sealed class MonsterEggEvolutionSystem : EntitySystem
{
	
	[Dependency] private readonly BlindableSystem _blindableSystem = default!;
	[Dependency] private readonly IEntityManager _entityManager = default!;
	[Dependency] private readonly IGameTiming _gameTiming = default!;
	[Dependency] private readonly SharedMindSystem _mindSystem = default!;
	
	public override void Update(float frameTime)
	{
		base.Update(frameTime);

		var query = EntityQueryEnumerator<MonsterEggEvolutionComponent>();
		while (query.MoveNext(out var uid, out var evolutionComp))
		{
			if (_gameTiming.CurTime >= evolutionComp.EvolutionTime)
			{
				ResolveHatching((uid, evolutionComp));
			}
		}
	}
	
	private void ResolveHatching(Entity<MonsterEggEvolutionComponent> ent)
	{
		if (_mindSystem.TryGetMind(ent, out var mindId, out var mind) && !Deleted(ent))
		{
			var evolvedEntity = _entityManager.SpawnEntity(ent.Comp.Prototype, Transform(ent).Coordinates);
			_mindSystem.TransferTo(mindId, evolvedEntity, mind: mind);
			_entityManager.DeleteEntity(ent);
			
			var monsterComp = EnsureComp<MonsterComponent>(evolvedEntity);
			monsterComp.EvoPoints = ent.Comp.StoredPoints;
			
			var blindableComp = EnsureComp<BlindableComponent>(evolvedEntity);
			blindableComp.IsBlind = true;
			Dirty(blindableComp);
			
		}
	}
	
}