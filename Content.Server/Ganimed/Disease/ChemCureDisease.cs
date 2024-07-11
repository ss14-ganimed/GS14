using Content.Shared.Chemistry.Reagent;
using Content.Server.Ganimed.Disease;
using Content.Shared.EntityEffects;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityEffects.Effects
{
    /// <summary>
    /// Default metabolism for medicine reagents.
    /// </summary>
    public sealed partial class ChemCureDisease : EntityEffect
    {
        protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
            => Loc.GetString("reagent-effect-guidebook-adjust-cure",
                ("chance", Probability));

        /// <summary>
        /// Chance it has each tick to cure a disease, between 0 and 1
        /// </summary>
        [DataField("cureChance")]
        public float CureChance = 0.15f;

        public override void Effect(EntityEffectBaseArgs args)
        {
            var cureChance = CureChance;

            if (args is EntityEffectReagentArgs reagentArgs) {
            cureChance *= reagentArgs.Scale.Float();
            }

            var ev = new CureDiseaseAttemptEvent(cureChance);
            args.EntityManager.EventBus.RaiseLocalEvent(args.TargetEntity, ev, false);
        }
    }
}
