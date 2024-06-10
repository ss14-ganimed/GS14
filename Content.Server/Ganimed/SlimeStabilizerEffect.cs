using Content.Server.Ganimed.XenoBiology.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReagentEffects
{
    public sealed partial class SlimeStabilize : ReagentEffect
    {
        [DataField]
        public float Amount;

        protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
            => Loc.GetString("reagent-effect-guidebook-adjust-slime-stabilize",
                ("chance", Probability),
                ("deltasign", MathF.Sign(Amount)),
                ("amount", MathF.Abs(Amount)));

        public override void Effect(ReagentEffectArgs args)
        {
            if (args.EntityManager.TryGetComponent(args.SolutionEntity, out XenoBiologyComponent? temp))
            {
                if (temp.Mutationchance >= 0);
                 {
                    temp.Mutationchance = 0;
                 }
                return;
            }
        }
    }
}
