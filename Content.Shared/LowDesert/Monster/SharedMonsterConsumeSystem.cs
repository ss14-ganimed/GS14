using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.LowDesert.Monster.Components;
using Content.Shared.Popups;
using Robust.Shared.Serialization;

namespace Content.Shared.LowDesert.Monster;

public abstract class SharedMonsterConsumeSystem : EntitySystem
{
	[Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
	
	public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MonsterComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<MonsterComponent, MonsterConsumeActionEvent>(OnConsumeAction);
        SubscribeLocalEvent<MonsterConsumableComponent, ExaminedEvent>(OnExamined);
    }
	
	protected void OnStartup(EntityUid uid, MonsterComponent component, ComponentStartup args)
    {
        if (component.ConsumeAction != null)
            _actionsSystem.AddAction(uid, ref component.ConsumeActionEntity, component.ConsumeAction);
    }
	
	protected void OnConsumeAction(EntityUid uid, MonsterComponent component, MonsterConsumeActionEvent args)
	{
		if (args.Handled)
            return;
		
		if (!EntityManager.TryGetComponent<MonsterConsumableComponent>(args.Target, out var consumable))
			return;
		
		if (!consumable.IsConsumable)
		{
			_popupSystem.PopupClient(Loc.GetString("monster-consume-action-popup-message-fail-target-consumed"), uid, uid);
			return;
		}
		
		var doAfterEventArgs = new DoAfterArgs(EntityManager, uid, component.ConsumeTime, new ConsumeDoAfterEvent(), uid, args.Target, used: uid)
		{
			BreakOnTargetMove = true,
			BreakOnUserMove = true,
            BreakOnDamage = true,
		};
		
		_doAfterSystem.TryStartDoAfter(doAfterEventArgs);
	}
	
	private void OnExamined(EntityUid uid, MonsterConsumableComponent component, ExaminedEvent args)
    {
        if (!component.IsConsumable)
			args.PushText(Loc.GetString("monster-consume-examine-consumed"));
    }
}

[Serializable, NetSerializable]
public sealed partial class ConsumeDoAfterEvent : SimpleDoAfterEvent
{
	
}