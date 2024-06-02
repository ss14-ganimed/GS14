using Content.Server.Humanoid;
using Content.Server.Repairable;
using Content.Shared.Ganimed.Disease;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Server.Ganimed.Disease.Effects;

public sealed partial class DiseaseCatConversion : DiseaseEffect
{
    public override object GenerateEvent(Entity<DiseaseCarrierComponent> ent, ProtoId<DiseasePrototype> disease)
    {
        return new DiseaseEffectArgs<DiseaseCatConversion>(ent, disease, this);
    }
}

public sealed partial class DiseaseEffectSystem
{
    [Dependency] private readonly HumanoidAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly MetaDataSystem _metaDataSystem = default!;

    private void DiseaseCatConversion(Entity<DiseaseCarrierComponent> ent,
        ref DiseaseEffectArgs<DiseaseCatConversion> args)
    {
        if (args.Handled)
            return;
        args.Handled = true;

        if (TryComp<MobThresholdsComponent>(ent, out var thresholdsComponent))
        {
            (thresholdsComponent as dynamic).AllowRevives = true;
        }

        _disease.CureDisease(ent, args.Disease);
        if (TryComp<HumanoidAppearanceComponent>(ent, out var appearanceComponent))
        { 

            if (appearanceComponent.MarkingSet.Markings.TryGetValue(MarkingCategories.HeadTop, out var headtop))
            {
                foreach (var marking in headtop.ToArray())
                {
                    _appearanceSystem.RemoveMarking(ent, marking.MarkingId);
                }
            }

            _appearanceSystem.AddMarking(ent, "CatEars", Color.White, true, true, appearanceComponent);
            _appearanceSystem.AddMarking(ent, "CatTail", Color.White, true, true, appearanceComponent);
        }
    }
}
