class AsteroidBelt : Orbit
{
    public DwarfPlanet dwarf_planet = null;

    public AsteroidBelt(Star star, OrbitRange orbit_range) : base(star, orbit_range)
    {
        DetermineSatellites();
        DetermineDesirability();
        TestHabitation();
        GenerateBases();
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 5)
        {
            dwarf_planet = new DwarfPlanet(star, orbit_range, true, false, false, true);
        }
    }

    public override void AffectFromStarExpansion()
    {
        if (dwarf_planet != null)
        {
            dwarf_planet.AffectFromStarExpansion();
        }
    }

    public override void DetermineDesirability()
    {
        int dm = 0;
        if (star.flare_star) { dm += -RandomTools.RollD3(); }
        if (star.spectral_type == SpectralType.M && star.luminosity_class == 5) {dm += 1;}
        if ((star.spectral_type == SpectralType.M && star.luminosity_class != 5) ||
        star.spectral_type == SpectralType.L ||
        star.spectral_type == SpectralType.D ||
        star.spectral_type == SpectralType.K) {dm += 2;}
        desirability = RandomTools.RollD6() - RandomTools.RollD6() + dm;
    }

    protected override void GenerateBases()
    {
        if (habitation != Habitation.Uninhabited)
        {
            if (RandomTools.RollD6(2) >= 9) { bases.Add(Base.ResearchInstallation); }
        }
    }

    public override string ToString()
    {
        string to_append = "";
        //Outpost and colony are the same thing for asteroid belts
        if (habitation == Habitation.Outpost || habitation == Habitation.Colony) {to_append += " Outpost";}
        if (bases.Count > 0)
        {
            to_append += " Bases: ";
            bool place_comma = false;
            foreach (Base b in bases)
            {
                if (place_comma) { to_append += ", "; }
                else { place_comma = true; }
                to_append += b;
            }
        }
        if (dwarf_planet != null) { to_append += ":\n\t\t" + dwarf_planet.ToString(); }
        return "Asteroid Belt (Desire: " + desirability + ")" + to_append;
    }
}
