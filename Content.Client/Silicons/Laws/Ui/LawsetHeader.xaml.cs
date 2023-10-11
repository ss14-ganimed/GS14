using Content.Client.Chat.Managers;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Content.Shared.Silicons.Laws;
using Content.Shared.Speech;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.Silicons.Laws.Ui;

[GenerateTypedNameReferences]
public sealed partial class LawsetHeader : Control
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly EntityManager _entityManager = default!;

    public event Action<BaseButton.ButtonEventArgs>? OnLawsetAnnouncementButtonPressed;

    public LawsetHeader(EntityUid uid, string name, string description, HashSet<string>? radioChannels)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

		var lawsetName = $"Набор законов: {Loc.GetString(name)}";
        var lawsetDescription = Loc.GetString(description);

        LawsetName.Text = lawsetName;
        LawsetDescription.SetMessage(lawsetDescription);

        // If you can't talk, you can't state your laws...
        if (!_entityManager.TryGetComponent<SpeechComponent>(uid, out var speech) || speech.SpeechSounds is null)
        {    
			StateLawsLabel.Text = "";
			return;
		}
		StateLawsLabel.Text = Loc.GetString("laws-ui-state-lawset");

        var localButton = new Button
        {
            Text = Loc.GetString("hud-chatbox-select-channel-Local"),
            Modulate = Color.DarkGray,
            StyleClasses = { "chatSelectorOptionButton" },
            MinHeight = 35,
            MinWidth = 75,
        };

        localButton.OnPressed += _ =>
        {
            _chatManager.SendMessage($"Набор законов: {Loc.GetString(name)}. {Loc.GetString(description)}", ChatSelectChannel.Local);
        };

        LawsetAnnouncementButtons.AddChild(localButton);

        if (radioChannels == null)
			return;

        foreach (var radioChannel in radioChannels)
        {
            if (!_prototypeManager.TryIndex<RadioChannelPrototype>(radioChannel, out var radioChannelProto))
                continue;

            var radioChannelButton = new Button
            {
                Text = Loc.GetString(radioChannelProto.Name),
                Modulate = radioChannelProto.Color,
                StyleClasses = { "chatSelectorOptionButton" },
                MinHeight = 35,
                MinWidth = 75,
            };

            radioChannelButton.OnPressed += _ =>
            {
                switch (radioChannel)
                {
                    case SharedChatSystem.CommonChannel:
                        _chatManager.SendMessage($"{SharedChatSystem.RadioCommonPrefix} Набор законов: {Loc.GetString(name)}. {Loc.GetString(description)}", ChatSelectChannel.Radio); break;
                    default:
                        _chatManager.SendMessage($"{SharedChatSystem.RadioChannelPrefix}{radioChannelProto.KeyCode} Набор законов: {Loc.GetString(name)}. {Loc.GetString(description)}", ChatSelectChannel.Radio); break;
                }
            };

            LawsetAnnouncementButtons.AddChild(radioChannelButton);
        }
    }
}
