using JetBrains.Annotations;
using Robust.Shared.Random;

namespace Content.Server.Maps.NameGenerators;

[UsedImplicitly]
public sealed partial class ExoNameGenerator : StationNameGenerator
{
    /// <summary>
    ///     Where the map comes from. Should be a two or three letter code, for example "VG" for Packedstation.
    /// </summary>
    private string[] PreffixCodes => new []{ "-01", "", "", "", "", "", "EXO", "MASS2", "MASS7", "NT", "PR", "DH" };
	private string[] SuffixCodes => new []{ "", "8", "",  "", "a", "b", "exh", "Alpha", "Beta", "Gamma", "Delta", "Vultaum", "Cebri-0", "Omega" };

    public override string FormatName(string input)
    {
        var random = IoCManager.Resolve<IRobustRandom>();

        // No way in hell am I writing custom format code just to add nice names. You can live with {0}
		return string.Format(input, $"{random.Next(0, 99)}{random.Pick(PreffixCodes)}", $"{random.Pick(SuffixCodes)}");
	}
}
