using Content.Shared.LowDesert.Monster;
using Content.Shared.LowDesert.Monster.Components;
using Robust.Client.GameObjects;

namespace Content.Client.LowDesert.Monster.Ui;

public sealed partial class MonsterEvolutionBoundUserInterface : BoundUserInterface
{
	private MonsterEvolutionMenu? _window;
	
	public MonsterEvolutionBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }
	
	protected override void Open()
	{
		base.Open();
		
		_window = new MonsterEvolutionMenu();
		_window.OnClose += Close;
		_window.OpenCentered();
		
		_window.OnMonsterEvolutionItemButtonPressed += (args, button) => SendMessage(new MonsterEvolutionMessage(button.Item));
	}
	
	protected override void UpdateState(BoundUserInterfaceState state)
	{
		base.UpdateState(state);
		
		var castState = (MonsterEvolutionBoundUserInterfaceState) state;
		
		_window?.UpdateState(castState);
	}
	
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		
		if (disposing)
			_window?.Dispose();
		
	}
}