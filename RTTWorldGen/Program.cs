using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

enum SpectralType {A, F, G, K, M, L, D}
enum CompanionOrbit {None, Tight, Close, Moderate, Distant}
enum OrbitRange {None, Epistellar, InnerZone, OuterZone}
enum Ring {None, Minor, Complex}
enum DwarfPlanetType {Arean, Hebean, Meltball, Promethean, Rockball, Snowball, Stygian, Vesperian}
enum TerrestrialPlanetType {Acheronian, Arid, JaniLithic, Oceanic, Tectonic, Telluric}
enum HelianPlanetType {Helian, Asphodelian, Panthalassic}
enum JovianPlanetType {Chthonian, Jovian}
enum Habitation {Uninhabited, Outpost, Colony}
enum Starport {None, Frontier, Poor, Routine, Good, Excellent}
enum Base {MerchantBase, CorporateBase, NavalBase, PirateBase, Embassy, SacredSite, Prison, RefugeeFacility, NaturePreserve, ResearchInstallation, ScoutBase, University, Terraforming, Shipyard}

static class RandomTools
{
    static Random rnd = new Random();

    public static double GetVarianceAbsolute(double _base, double _amount)
    {
        return _base - (_amount - _base) + rnd.NextDouble() * (_amount - _base)*2;
    }

    public static double GetVariancePercentage(double _base, double _percentage)
    {
        return GetVarianceAbsolute(_base, _base*_percentage);
    }

    public static double GetRandomRange(double _low, double _high)
    {
        return rnd.NextDouble() * (_high - _low) + _low;
    }

    public static int RollD6(int amount=1)
    {
        int result = 0;
        for (int i=0; i<amount; i++)
        {
            result += rnd.Next(1, 7);
        }
        return result;
    }

    public static int RollD3(int amount=1)
    {
        int result = 0;
        for (int i=0; i<amount; i++)
        {
            result += rnd.Next(1, 4);
        }
        return result;
    }
}

class StellarRegion
{
    int width;
    int height;
    Dictionary<Tuple<int, int>, StarSystem> star_system_coords = new Dictionary<Tuple<int, int>, StarSystem>();

    public StellarRegion(int width, int height)
    {
        this.width = width;
        this.height = height;

        for (int i=0; i<width; i++)
        {
            for (int j=0; j<height; j++)
            {
                Tuple<int, int> coords = new Tuple<int, int>(i, j);
                if (RandomTools.GetRandomRange(0, 1) > 0.5)
                {
                    star_system_coords.Add(coords, new StarSystem());
                }
            }
        }
    }

    public StarSystem GetStarSystem(int x, int y)
    {
        Tuple<int, int> coords = new Tuple<int, int>(x, y);
        if (star_system_coords.ContainsKey(coords))
        {
            return star_system_coords[coords];
        }
        else
        {
            return null;
        }
    }
}

class StarSystem
{
    public int stellar_multiplicity;
    public List<Star> stars = new List<Star>();
    public int age; //in billions
    public StarSystem()
    {
        int multiplicity_roll = RandomTools.RollD6(3);
        if (multiplicity_roll <= 10) { stellar_multiplicity = 1; }
        else if (multiplicity_roll <= 15) { stellar_multiplicity = 2; }
        else { stellar_multiplicity = 3; }

        age = RandomTools.RollD6(3) - 3;

        int spectral_roll = RandomTools.RollD6(2);
        SpectralType spectral_type;
        if (spectral_roll <= 2) {spectral_type = SpectralType.A;}
        else if (spectral_roll <= 3) {spectral_type = SpectralType.F;}
        else if (spectral_roll <= 4) {spectral_type = SpectralType.G;}
        else if (spectral_roll <= 5) {spectral_type = SpectralType.K;}
        else if (spectral_roll <= 13) {spectral_type = SpectralType.M;}
        else {spectral_type = SpectralType.L;}

        stars.Add(new Star(spectral_type, age, false));

        for (int i=1; i<stellar_multiplicity; i++)
        {
            SpectralType subspectral_type;
            int subspectral_roll = RandomTools.RollD6(1)-1 + spectral_roll;
            if (subspectral_roll <= 2) {subspectral_type = SpectralType.A;}
            else if (subspectral_roll <= 3) {subspectral_type = SpectralType.F;}
            else if (subspectral_roll <= 4) {subspectral_type = SpectralType.G;}
            else if (subspectral_roll <= 5) {subspectral_type = SpectralType.K;}
            else if (subspectral_roll <= 13) {subspectral_type = SpectralType.M;}
            else {subspectral_type = SpectralType.L;}

            stars.Add(new Star(subspectral_type, age, true));
        }

        bool has_close_companion = false;
        bool has_moderate_companion = false;

        foreach (Star star in stars)
        {
            if (star.companion_orbit == CompanionOrbit.Close) { has_close_companion = true; }
            if (star.companion_orbit == CompanionOrbit.Moderate) { has_moderate_companion = true; }
        }

        foreach (Star star in stars)
        {
            if (star.companion_orbit == CompanionOrbit.Distant) { star.GenerateOrbits(has_close_companion, has_moderate_companion); }
        }
        stars[0].GenerateOrbits(has_close_companion, has_moderate_companion);
    }
}

class Star
{
    public SpectralType spectral_type;
    public CompanionOrbit companion_orbit = CompanionOrbit.None;
    public List<Orbit> orbits = new List<Orbit>();
    public int luminosity_class;
    public double age;
    public bool flare_star = false;
    int epistellar_orbit_count = 0;
    int inner_zone_orbit_count = 0;
    int outer_zone_orbit_count = 0;

    public Star(SpectralType spectral_type, int system_age, bool companion_star)
    {
        switch (spectral_type)
        {
            case SpectralType.A:
                if (system_age <= 2)
                {
                    luminosity_class = 5;
                }
                else if (system_age <= 3)
                {
                    int subroll = RandomTools.RollD6(1);
                    if (subroll <= 2)
                    {
                        spectral_type = SpectralType.F;
                        luminosity_class = 4;
                    }
                    else if (subroll <= 3)
                    {
                        spectral_type = SpectralType.K;
                        luminosity_class = 3;
                    }
                    else if (subroll <= 6)
                    {
                        spectral_type = SpectralType.D;
                        luminosity_class = 0;
                    }
                }
                else
                {
                    spectral_type = SpectralType.D;
                    luminosity_class = 0;
                }
                break;
            
            case SpectralType.F:
                if (system_age <= 5)
                {
                    luminosity_class = 5;
                }
                else if (system_age <= 6)
                {
                    int subroll = RandomTools.RollD6(1);
                    if (subroll <= 3)
                    {
                        spectral_type = SpectralType.G;
                        luminosity_class = 4;
                    }
                    else if (subroll <= 6)
                    {
                        spectral_type = SpectralType.M;
                        luminosity_class = 3;
                    }
                }
                else
                {
                    spectral_type = SpectralType.D;
                    luminosity_class = 0;
                }
                break;
                
            case SpectralType.G:
                if (system_age <= 11)
                {
                    luminosity_class = 5;
                }
                else if (system_age <= 13)
                {
                    int subroll = RandomTools.RollD6(1);
                    if (subroll <= 3)
                    {
                        spectral_type = SpectralType.K;
                        luminosity_class = 4;
                    }
                    else if (subroll <= 6)
                    {
                        spectral_type = SpectralType.M;
                        luminosity_class = 3;
                    }
                }
                else
                {
                    spectral_type = SpectralType.D;
                    luminosity_class = 0;
                }
                break;
            
            case SpectralType.K:
                luminosity_class = 5;
                break;

            case SpectralType.L:
                luminosity_class = 0;
                break;

            case SpectralType.M:
                int m_subroll = RandomTools.RollD6(2) + (companion_star ? 2 : 0);

                if (m_subroll <= 9)
                {
                    luminosity_class = 5;
                }
                else if (m_subroll <= 12)
                {
                    luminosity_class = 5;
                    flare_star = true;
                }
                else
                {
                    spectral_type = SpectralType.L;
                }
                break;
        }

        this.spectral_type = spectral_type;
        this.age = system_age;

        if (companion_star)
        {
            int companion_star_roll = RandomTools.RollD6(1);
            if (companion_star_roll <= 2) { companion_orbit = CompanionOrbit.Tight; }
            else if (companion_star_roll <= 4) { companion_orbit = CompanionOrbit.Close; }
            else if (companion_star_roll <= 5) { companion_orbit = CompanionOrbit.Moderate; }
            else if (companion_star_roll <= 6)
            {
                companion_orbit = CompanionOrbit.Distant;
                GenerateOrbits(false, false);
            }
        }
    }

    public void GenerateOrbits(bool has_close_companion, bool has_moderate_companion)
    {
        epistellar_orbit_count = Math.Clamp(RandomTools.RollD6(1)-3-(spectral_type == SpectralType.M && luminosity_class == 5 ? 1 : 0), 0, 2);
        if (spectral_type == SpectralType.D || spectral_type == SpectralType.L || luminosity_class == 3) { epistellar_orbit_count = 0; }
        if (!has_close_companion && spectral_type != SpectralType.L) {inner_zone_orbit_count = Math.Max(RandomTools.RollD6(1)-1-(spectral_type == SpectralType.M && luminosity_class == 5 ? 1 : 0), 0);}
        if (!has_close_companion && spectral_type == SpectralType.L) {inner_zone_orbit_count = Math.Clamp(RandomTools.RollD3(1)-1, 0, 2);}
        if (!has_moderate_companion) {outer_zone_orbit_count = Math.Max(RandomTools.RollD6(1)-1-(spectral_type == SpectralType.L || (spectral_type == SpectralType.M && luminosity_class == 5) ? 1 : 0), 0);}
    
        Star star = this;
        for (int i=0; i<epistellar_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(star, OrbitRange.Epistellar)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(star, OrbitRange.Epistellar, false, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(star, OrbitRange.Epistellar, false)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(star, OrbitRange.Epistellar)); }
            else { orbits.Add(new JovianPlanet(star, OrbitRange.Epistellar)); }
        }
        for (int i=0; i<inner_zone_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(star, OrbitRange.InnerZone)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(star, OrbitRange.InnerZone, false, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(star, OrbitRange.InnerZone, false)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(star, OrbitRange.InnerZone)); }
            else { orbits.Add(new JovianPlanet(star, OrbitRange.InnerZone)); }
        }
        for (int i=0; i<outer_zone_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(star, OrbitRange.OuterZone)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(star, OrbitRange.OuterZone, false, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(star, OrbitRange.OuterZone, false)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(star, OrbitRange.OuterZone)); }
            else { orbits.Add(new JovianPlanet(star, OrbitRange.OuterZone)); }
        }

        if (spectral_type == SpectralType.D || luminosity_class == 3)
        {
            int orbits_affected = RandomTools.RollD6(1);
            for (int i=0; i < Math.Min(orbits_affected, orbits.Count); i++)
            {
                orbits[i].AffectFromStarExpansion();
            }
        }
    }
}

class Program
{
    static string MakeColumnReadable(int i)
    {
        switch (i)
        {
            case 0: return "A";
            case 1: return "B";
            case 2: return "C";
            case 3: return "D";
            case 4: return "E";
            case 5: return "F";
            case 6: return "G";
            case 7: return "H";
            case 8: return "I";
            case 9: return "J";
            case 10: return "K";
            case 11: return "L";
            default: return "";
        }
    }
    static void Main(string[] args)
    {
        StellarRegion region = new StellarRegion(12, 12);
        for (int i=0; i<12; i++)
        {
            for (int j=0; j<12; j++)
            {
                if (region.GetStarSystem(i, j) != null)
                {
                    if (!(i == 0 && j == 0))
                    {
                        Console.WriteLine("");
                    }

                    Console.WriteLine("(" + MakeColumnReadable(i) + "," + (j + 1) + ")");
                    foreach (Star star in region.GetStarSystem(i, j).stars)
                    {
                        Console.WriteLine(star.spectral_type.ToString() + star.luminosity_class.ToString() + (star.flare_star ? "<Flare>" : "") + ":");
                        foreach (Orbit orbit in star.orbits)
                        {
                            Console.WriteLine("\t" + orbit.ToString());
                        }
                    }
                }
            }
        }
    }
}