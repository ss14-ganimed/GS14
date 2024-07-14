using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.EntityEffects;
using Content.Shared.Ganimed.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityEffects.Effects;

public sealed partial class FelinidCoolDownReaction : EntityEffect
{
		protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
            => Loc.GetString("reagent-effect-guidebook-extinguish-reaction", ("chance", Probability));

        public override void Effect(EntityEffectBaseArgs args)
        {	
			var doAfterSystem = EntitySystem.Get<SharedDoAfterSystem>();
            if (!args.EntityManager.TryGetComponent(args.TargetEntity, out FelinidComponent? felinid)) return;
			if (!args.EntityManager.TryGetComponent(args.TargetEntity, out DoAfterComponent? comp)) return;
			doAfterSystem.CancelAll(comp);
        }
}
