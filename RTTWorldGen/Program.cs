enum SpectralType {A, F, G, K, M, L, D}
enum CompanionOrbit {None, Tight, Close, Moderate, Distant}
enum OrbitRange {None, Epistellar, InnerZone, OuterZone}
enum Rings {None, Minor, Complex}
enum DwarfPlanetType {Arean, Hebean, Meltball, Promethean, Rockball, Snowball, Stygian, Vesperian}

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
    int luminosity_class;
    bool flare_star = false;
    int epistellar_orbit_count = 0;
    int inner_zone_orbit_count = 0;
    int outer_zone_orbit_count = 0;

    public Star(SpectralType spectral_type, int system_age, bool companion_star)
    {
        this.spectral_type = spectral_type;

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
    
        for (int i=0; i<epistellar_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(OrbitRange.Epistellar)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(OrbitRange.Epistellar, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(OrbitRange.Epistellar)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(OrbitRange.Epistellar)); }
            else { orbits.Add(new JovianPlanet(OrbitRange.Epistellar)); }
        }
        for (int i=0; i<inner_zone_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(OrbitRange.InnerZone)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(OrbitRange.InnerZone, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(OrbitRange.InnerZone)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(OrbitRange.InnerZone)); }
            else { orbits.Add(new JovianPlanet(OrbitRange.InnerZone)); }
        }
        for (int i=0; i<outer_zone_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(OrbitRange.OuterZone)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(OrbitRange.OuterZone, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(OrbitRange.OuterZone)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(OrbitRange.OuterZone)); }
            else { orbits.Add(new JovianPlanet(OrbitRange.OuterZone)); }
        }
    }
}

class Orbit
{
    public OrbitRange orbit_range = OrbitRange.None;
    public Orbit(OrbitRange orbit_range)
    {
        this.orbit_range = orbit_range;
        DetermineSatellites();
    }

    public virtual void DetermineSatellites()
    {
        return;
    }
}

class Planetoid : Orbit
{
    int size;

    public Planetoid(OrbitRange orbit_range) : base(orbit_range)
    {
    }
}

class AsteroidBelt : Orbit
{
    public DwarfPlanet dwarf_planet = null;

    public AsteroidBelt(OrbitRange orbit_range) : base(orbit_range)
    {
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 5)
        {
            dwarf_planet = new DwarfPlanet(orbit_range, true, false, false);
        }
    }
}

class DwarfPlanet : Planetoid
{
    public DwarfPlanet companion = null;
    public DwarfPlanetType type;
    
    public DwarfPlanet(OrbitRange orbit_range, bool asteroid_belt_member, bool helian_sat, bool jovian_sat) : base(orbit_range)
    {
        return;
    }
    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 6)
        {
            companion = new DwarfPlanet(orbit_range, false, false, false);
            companion.companion = this;
        }
    }
    public void DetermineType(bool asteroid_belt_member, bool helian_sat, bool jovian_sat)
    {
        int roll = RandomTools.RollD6(1);
        if (orbit_range == OrbitRange.Epistellar)
        {
            if (roll <= 3) { type = DwarfPlanetType.Rockball; }
            else if (roll <= 5) { type = DwarfPlanetType.Meltball; }
            else if (roll <= 6)
            {
                int subroll = RandomTools.RollD6(1);
                if (subroll <= 4) { type = DwarfPlanetType.Hebean; }
                else { type = DwarfPlanetType.Promethean }
            }
        }
        else if (orbit_range == OrbitRange.InnerZone)
        {
            roll += (asteroid_belt_member ? -2 : 0) + (helian_sat ? 1 : 0) + (jovian_sat ? 2 : 0);
        }
    }
}

class TerrestrialPlanet : Planetoid
{
    public DwarfPlanet satellite = null;

    public TerrestrialPlanet(OrbitRange orbit_range) : base(orbit_range)
    {
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 5)
        {
            satellite = new DwarfPlanet(orbit_range, false, false, false);
        }
    }
}

class HelianPlanet : Planetoid
{
    public List<Planetoid> satellites = new List<Planetoid>();

    public HelianPlanet(OrbitRange orbit_range) : base(orbit_range)
    {
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = Math.Max(RandomTools.RollD6(1) - 3, 0);
        if (satellite_roll >= 0)
        {
            int sub_satellite_roll = RandomTools.RollD6(1);
            if (sub_satellite_roll == 6)
            {
                satellites.Add(new TerrestrialPlanet(orbit_range));
                satellite_roll--;
            }
            for (int i=0; i<satellite_roll; i++)
            {
                satellites.Add(new DwarfPlanet(orbit_range, false, true, false));
            }
        }
    }
}

class JovianPlanet : Planetoid
{
    public List<Planetoid> satellites = new List<Planetoid>();
    public Rings rings = Rings.None;

    public JovianPlanet(OrbitRange orbit_range) : base(orbit_range)
    {
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        int sub_satellite_roll = RandomTools.RollD6(1);
        if (sub_satellite_roll == 6)
        {
            int sub_sub_satellite_roll = RandomTools.RollD6(1);
            if (sub_sub_satellite_roll <= 5)
            {
                satellites.Add(new TerrestrialPlanet(orbit_range));
            }
            else
            {
                satellites.Add(new HelianPlanet(orbit_range));
            }
            satellite_roll--;
        }
        for (int i=0; i<satellite_roll; i++)
        {
            satellites.Add(new DwarfPlanet(orbit_range, false, false, true));
        }

        int rings_roll = RandomTools.RollD6(1);
        if (rings_roll <= 4) {rings = Rings.Minor;}
        else {rings = Rings.Complex;}
    }
}

class Program
{
    static void Main(string[] args)
    {
        StellarRegion region = new StellarRegion(6, 6);
        for (int i=0; i<6; i++)
        {
            for (int j=0; j<6; j++)
            {
                if (region.GetStarSystem(i, j) != null)
                {
                    Console.WriteLine("(" + i + "," + j + ")");
                    foreach (Star star in region.GetStarSystem(i, j).stars)
                    {
                        Console.WriteLine(star.spectral_type);
                        Console.WriteLine(star.orbits.Count);
                        foreach (Orbit orbit in star.orbits)
                        {
                            Console.WriteLine(orbit);
                        }
                    }
                }
            }
        }
    }
}