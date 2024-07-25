using System.Collections;
using System.Security.Cryptography.X509Certificates;

enum SpectralType {A, F, G, K, M, L, D}
enum CompanionOrbit {None, Tight, Close, Moderate, Distant}
enum OrbitRange {None, Epistellar, InnerZone, OuterZone}
enum Ring {None, Minor, Complex}
enum DwarfPlanetType {Arean, Hebean, Meltball, Promethean, Rockball, Snowball, Stygian, Vesperian}
enum TerrestrialPlanetType {Acheronian, Arid, JaniLithic, Oceanic, Tectonic, Telluric}
enum HelianPlanetType {Helian, Asphodelian, Panthalassic}
enum JovianPlanetType {Chthonian, Jovian}

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
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(OrbitRange.Epistellar, false, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(OrbitRange.Epistellar, false)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(OrbitRange.Epistellar)); }
            else { orbits.Add(new JovianPlanet(OrbitRange.Epistellar)); }
        }
        for (int i=0; i<inner_zone_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(OrbitRange.InnerZone)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(OrbitRange.InnerZone, false, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(OrbitRange.InnerZone, false)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(OrbitRange.InnerZone)); }
            else { orbits.Add(new JovianPlanet(OrbitRange.InnerZone)); }
        }
        for (int i=0; i<outer_zone_orbit_count; i++)
        {
            int orbit_roll = RandomTools.RollD6(1) - (spectral_type == SpectralType.L ? 1 : 0);
            if (orbit_roll <= 1) { orbits.Add(new AsteroidBelt(OrbitRange.OuterZone)); }
            else if (orbit_roll <= 2) { orbits.Add(new DwarfPlanet(OrbitRange.OuterZone, false, false, false, false)); }
            else if (orbit_roll <= 3) { orbits.Add(new TerrestrialPlanet(OrbitRange.OuterZone, false)); }
            else if (orbit_roll <= 4) { orbits.Add(new HelianPlanet(OrbitRange.OuterZone)); }
            else { orbits.Add(new JovianPlanet(OrbitRange.OuterZone)); }
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

class Orbit
{
    public OrbitRange orbit_range = OrbitRange.None;
    public Orbit(OrbitRange orbit_range)
    {
        this.orbit_range = orbit_range;
    }

    public virtual void DetermineSatellites()
    {
        return;
    }

    public virtual void AffectFromStarExpansion()
    {

    }

    public virtual string ToString()
    {
        return "Orbit";
    }
}

class Planetoid : Orbit
{
    public int size;

    public Planetoid(OrbitRange orbit_range) : base(orbit_range)
    {
    }
    
    public virtual void GenerateWorld()
    {
    }
}

class AsteroidBelt : Orbit
{
    public DwarfPlanet dwarf_planet = null;

    public AsteroidBelt(OrbitRange orbit_range) : base(orbit_range)
    {
        DetermineSatellites();
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 5)
        {
            dwarf_planet = new DwarfPlanet(orbit_range, true, false, false, true);
        }
    }

    public override void AffectFromStarExpansion()
    {
        if (dwarf_planet != null)
        {
            dwarf_planet.AffectFromStarExpansion();
        }
    }

    public override string ToString()
    {
        string to_append = "";
        if (dwarf_planet != null) { to_append += ":\n   " + dwarf_planet.ToString(); }
        return "Asteroid Belt" + to_append;
    }
}

class DwarfPlanet : Planetoid
{
    private DwarfPlanet companion = null;
    private DwarfPlanetType type;
    
    public DwarfPlanet(OrbitRange orbit_range, bool asteroid_belt_member, bool helian_sat, bool jovian_sat, bool is_satellite) : base(orbit_range)
    {
        if (!is_satellite)
        {
            DetermineSatellites();
        }
        DetermineType(asteroid_belt_member, helian_sat, jovian_sat);
    }
    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 6)
        {
            companion = new DwarfPlanet(orbit_range, false, false, false, true);
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
                else { type = DwarfPlanetType.Promethean; }
            }
        }
        else if (orbit_range == OrbitRange.InnerZone)
        {
            roll += (asteroid_belt_member ? -2 : 0) + (helian_sat ? 1 : 0) + (jovian_sat ? 2 : 0);

            if (roll <= 4) { type = DwarfPlanetType.Rockball; }
            else if (roll <= 6) { type = DwarfPlanetType.Arean; }
            else if (roll <= 7) { type = DwarfPlanetType.Meltball; }
            else if (roll <= 8)
            {
                int subroll = RandomTools.RollD6(1);
                if (subroll <= 4) { type = DwarfPlanetType.Hebean; }
                else { type = DwarfPlanetType.Promethean; }
            }
        }
        else if (orbit_range == OrbitRange.OuterZone)
        {
            roll += (asteroid_belt_member ? -1 : 0) + (helian_sat ? 1 : 0) + (jovian_sat ? 2 : 0);

            if (roll <= 0) { type = DwarfPlanetType.Rockball; }
            else if (roll <= 4) { type = DwarfPlanetType.Snowball; }
            else if (roll <= 6) { type = DwarfPlanetType.Rockball; }
            else if (roll <= 7) { type = DwarfPlanetType.Meltball; }
            else if (roll <= 8)
            {
                int subroll = RandomTools.RollD6(1);
                if (subroll <= 3) { type = DwarfPlanetType.Hebean; }
                else if (subroll <= 5) { type = DwarfPlanetType.Arean; }
                else { type = DwarfPlanetType.Promethean; }
            }
        }
    }

    public override void GenerateWorld()
    {
        switch (type)
        {
            case DwarfPlanetType.Arean:
                this.size = RandomTools.RollD6()-1;
                break;
        }
    }

    public override string ToString()
    {
        string type_string = "";
        switch (type)
        {
            case DwarfPlanetType.Arean: type_string = "Arean"; break;
            case DwarfPlanetType.Hebean: type_string = "Hebean"; break;
            case DwarfPlanetType.Meltball: type_string = "Meltball"; break;
            case DwarfPlanetType.Promethean: type_string = "Promethean"; break;
            case DwarfPlanetType.Rockball: type_string = "Rockball"; break;
            case DwarfPlanetType.Snowball: type_string = "Snowball"; break;
            case DwarfPlanetType.Stygian: type_string = "Stygian"; break;
            case DwarfPlanetType.Vesperian: type_string = "Vesperian"; break;
        }
        string to_append = "";
        if (companion != null) { to_append += ":\n   " + companion.ToString(); }
        return type_string + to_append;
    }

    public override void AffectFromStarExpansion()
    {
        type = DwarfPlanetType.Stygian;
        if (companion != null)
        {
            companion.AffectFromStarExpansion();
        }
    }
}

class TerrestrialPlanet : Planetoid
{
    private DwarfPlanet satellite = null;
    private TerrestrialPlanetType type;

    public TerrestrialPlanet(OrbitRange orbit_range, bool is_satellite) : base(orbit_range)
    {
        if (!is_satellite) { DetermineSatellites(); }
        DetermineType();
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 5)
        {
            satellite = new DwarfPlanet(orbit_range, false, false, false, true);
        }
    }

    public void DetermineType()
    {
        if (orbit_range == OrbitRange.Epistellar)
        {
            int roll = RandomTools.RollD6(1);
            if (roll <= 4) { type = TerrestrialPlanetType.JaniLithic; }
            //RTT World Gen has Vesperian here, which is for dwarf planets only so idk why it's there I'm ignoring it
            else if (roll <= 6) { type = TerrestrialPlanetType.Tectonic; }
        }
        else if (orbit_range == OrbitRange.InnerZone)
        {
            int roll = RandomTools.RollD6(2);
            if (roll <= 4) { type = TerrestrialPlanetType.Telluric; }
            else if (roll <= 6) { type = TerrestrialPlanetType.Arid; }
            else if (roll <= 7) { type = TerrestrialPlanetType.Tectonic; }
            else if (roll <= 9) { type = TerrestrialPlanetType.Oceanic; }
            else if (roll <= 10) { type = TerrestrialPlanetType.Tectonic; }
            else if (roll <= 12) { type = TerrestrialPlanetType.Telluric; }
        }
        else if (orbit_range == OrbitRange.OuterZone)
        {
            int roll = RandomTools.RollD6(1);
            if (roll <= 4) { type = TerrestrialPlanetType.Arid; }
            else if (roll <= 6) { type = TerrestrialPlanetType.Tectonic; }
            else if (roll <= 8) { type = TerrestrialPlanetType.Oceanic; }
        }
    }

    public override void AffectFromStarExpansion()
    {
        type = TerrestrialPlanetType.Acheronian;
        if (satellite != null)
        {
            satellite.AffectFromStarExpansion();
        }
    }

    public override string ToString()
    {
        string type_string = "";
        switch (type)
        {
            case TerrestrialPlanetType.Acheronian: type_string = "Acheronian"; break;
            case TerrestrialPlanetType.Arid: type_string = "Arid"; break;
            case TerrestrialPlanetType.JaniLithic: type_string = "JaniLithic"; break;
            case TerrestrialPlanetType.Oceanic: type_string = "Oceanic"; break;
            case TerrestrialPlanetType.Tectonic: type_string = "Tectonic"; break;
            case TerrestrialPlanetType.Telluric: type_string = "Telluric"; break;
        }
        string to_append = "";
        if (satellite != null) { to_append += ":\n   " + satellite.ToString(); }
        return type_string + to_append;
    }
}

class HelianPlanet : Planetoid
{
    private List<Planetoid> satellites = new List<Planetoid>();
    private HelianPlanetType type;

    public HelianPlanet(OrbitRange orbit_range) : base(orbit_range)
    {
        DetermineSatellites();
        DetermineType();
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = Math.Max(RandomTools.RollD6(1) - 3, 0);
        if (satellite_roll >= 0)
        {
            int sub_satellite_roll = RandomTools.RollD6(1);
            if (sub_satellite_roll == 6)
            {
                satellites.Add(new TerrestrialPlanet(orbit_range, true));
                satellite_roll--;
            }
            for (int i=0; i<satellite_roll; i++)
            {
                satellites.Add(new DwarfPlanet(orbit_range, false, true, false, true));
            }
        }
    }

    public void DetermineType()
    {
        if (orbit_range == OrbitRange.Epistellar)
        {
            int roll = RandomTools.RollD6(1);
            if (roll <= 5) { type = HelianPlanetType.Helian; }
            else { type = HelianPlanetType.Asphodelian; }
        }
        else if (orbit_range == OrbitRange.InnerZone)
        {
            int roll = RandomTools.RollD6(1);
            if (roll <= 4) { type = HelianPlanetType.Helian; }
            else { type = HelianPlanetType.Panthalassic; }
        }
        else if (orbit_range == OrbitRange.OuterZone)
        {
            type = HelianPlanetType.Helian;
        }
    }

    public override void AffectFromStarExpansion()
    {
        type = HelianPlanetType.Asphodelian;
        foreach (Planetoid p in satellites)
        {
            p.AffectFromStarExpansion();
        }
    }

    public override string ToString()
    {
        string type_string = "";
        switch (type)
        {
            case HelianPlanetType.Asphodelian: type_string = "Asphodelian"; break;
            case HelianPlanetType.Helian: type_string = "Helian"; break;
            case HelianPlanetType.Panthalassic: type_string = "Panthalassic"; break;
        }
        string to_append = "";
        if (satellites.Count != 0)
        {
            to_append = ":";
            foreach (Planetoid satellite in satellites)
            {
                to_append += "\n   " + satellite.ToString();
            }
        }
        return type_string + to_append;
    }
}

class JovianPlanet : Planetoid
{
    private List<Planetoid> satellites = new List<Planetoid>();
    private Ring rings = Ring.None;
    private JovianPlanetType type;

    public JovianPlanet(OrbitRange orbit_range) : base(orbit_range)
    {
        DetermineSatellites();
        DetermineType();
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
                satellites.Add(new TerrestrialPlanet(orbit_range, true));
            }
            else
            {
                satellites.Add(new HelianPlanet(orbit_range));
            }
            satellite_roll--;
        }
        for (int i=0; i<satellite_roll; i++)
        {
            satellites.Add(new DwarfPlanet(orbit_range, false, false, true, true));
        }

        int rings_roll = RandomTools.RollD6(1);
        if (rings_roll <= 4) {rings = Ring.Minor;}
        else {rings = Ring.Complex;}
    }

    public void DetermineType()
    {
        if (orbit_range == OrbitRange.Epistellar)
        {
            int roll = RandomTools.RollD6(1);
            if (roll <= 5) { type = JovianPlanetType.Jovian; }
            else { type = JovianPlanetType.Chthonian; }
        }
    }

    public override void AffectFromStarExpansion()
    {
        type = JovianPlanetType.Chthonian;
        foreach (Planetoid p in satellites)
        {
            p.AffectFromStarExpansion();
        }
    }

    public override string ToString()
    {
        string type_string = "";
        switch (type)
        {
            case JovianPlanetType.Chthonian: type_string = "Chthonian"; break;
            case JovianPlanetType.Jovian: type_string = "Jovian"; break;
        }
        string to_append = "";
        if (satellites.Count != 0)
        {
            to_append = ":";
            to_append += "\n   " + rings.ToString();
            foreach (Planetoid satellite in satellites)
            {
                to_append += "\n   " + satellite.ToString();
            }
        }
        return type_string + to_append;
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
                    if (!(i == 0 && j == 0))
                    {
                        Console.WriteLine("");
                    }

                    Console.WriteLine("(" + i + "," + j + ")");
                    foreach (Star star in region.GetStarSystem(i, j).stars)
                    {
                        Console.WriteLine(star.spectral_type);
                        Console.WriteLine(star.orbits.Count);
                        foreach (Orbit orbit in star.orbits)
                        {
                            Console.WriteLine(orbit.ToString());
                        }
                    }
                }
            }
        }
    }
}