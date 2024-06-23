/// Maded by Gorox for Enterprise. See CLA
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Ganimed.Heretic.Components;
using Content.Shared.Actions;
using Content.Shared.Store.Components;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Content.Server.Antag;
using Content.Shared.Ganimed.Heretic.Components;
using Content.Shared.Mobs.Systems;
using Content.Server.Objectives.Components;
using Content.Shared.NPC.Systems;
using System.Text;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Random;
using Content.Server.Roles;

namespace Content.Server.GameTicking.Rules;

public sealed class HereticRuleSystem : GameRuleSystem<HereticRuleComponent>
{
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedRoleSystem _roleSystem = default!;
    [Dependency] private readonly ObjectivesSystem _objectives = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly NpcFactionSystem _npcFactionSystem = default!;
    [Dependency] private readonly SharedJobSystem _jobs = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HereticRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagSelected);
    }

    private void AfterAntagSelected(Entity<HereticRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        if (args.Session == null)
            return;

        MakeHeretic(args.EntityUid, ent);
    }

    public bool MakeHeretic(EntityUid uid, HereticRuleComponent component)
    {
        if (!_mindSystem.TryGetMind(uid, out var mindId, out var mind))
            return false;

        EnsureComp<HereticComponent>(uid, out HereticComponent? hereticComp);
        EnsureComp<StoreComponent>(uid, out StoreComponent? storeComp);
        
        storeComp.Categories.Add("HereticAbilities");
        storeComp.CurrencyWhitelist.Add("HereticKnowledge");


        _antag.SendBriefing(uid, GetBriefing(), Color.Green, component.GreetingSound);

        _npcFactionSystem.RemoveFaction(mindId, "Nanotrasen", false);
        _npcFactionSystem.AddFaction(mindId, "Syndicate");

        return true;
    }

    public string GetBriefing(bool shortBrief = false)
    {
        var sb = new StringBuilder();

        sb.Append(Loc.GetString("heretic-briefing-start"));

        if (shortBrief)
            return sb.ToString(); // that's it.

        sb.Append(Loc.GetString("heretic-briefing-fluff"));

        return sb.ToString();
    }
}
