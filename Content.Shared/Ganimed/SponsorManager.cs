using System.Linq;
using Robust.Shared.Player;
using Content.Shared.Administration;
using Content.Shared.Administration.Managers;

namespace Content.Shared.Ganimed.SponsorManager;

public sealed class SponsorManager
{
    private Dictionary<string, DateTime> sponsors =
		new Dictionary<string, DateTime>(){
			{"JustHuman", new DateTime(2024, 06, 13)},
			{"Safno_S", new DateTime(2024, 07, 03)},
			{"Gorox", new DateTime(2024, 06, 30)},
			{"watrushechnik", new DateTime(2024, 07, 05)},
			{"Bezdnaa", new DateTime(2024, 06, 29)},
            {"Hyper_B", new DateTime(2024, 07, 09)},  
		};

    public bool IsSponsor(ICommonSession? session)
    {
        if (session is null)
			return false;
		if (session.ConnectedClient is null)
			return false;
		
		var nickname = session.ConnectedClient.UserName;
		
		if (nickname is null)
			return false;
		
		if (!sponsors.TryGetValue(nickname, out var sponsorTime))
			return false;
		
		return IsStillSponsor(sponsorTime);
    }
	
	public bool IsSponsor(string? name)
    {
        if (name is null)
			return false;
		
		if (!sponsors.TryGetValue(name, out var sponsorTime))
			return false;
		
		return IsStillSponsor(sponsorTime);
    }
	
	public bool IsHost(ICommonSession? session)
	{
		if (session is null)
			return false;
		
		var adminManager = IoCManager.Resolve<ISharedAdminManager>();
		return adminManager.HasAdminFlag(session, AdminFlags.Host);
	}
	
	public bool AllowSponsor(ICommonSession? session)
	{
		if (session is null)
			return false;
		if (session.ConnectedClient is null)
			return false;
		
		return IsHost(session) || IsSponsor(session);
	}
	
	public bool IsStillSponsor(DateTime sponsorDate)
	{
		return (sponsorDate.Add(TimeSpan.Parse("30:00:00:00")) > DateTime.Now) 
			&& (sponsorDate <= DateTime.Now);
	}
}
