class JovianPlanet : Planetoid
{
    private List<Planetoid> satellites = new List<Planetoid>();
    private Ring rings = Ring.None;
    private JovianPlanetType type = JovianPlanetType.Jovian;

    public JovianPlanet(Star star, OrbitRange orbit_range) : base(star, orbit_range)
    {
        DetermineSatellites();
        DetermineType();
        GenerateWorld();
        CheckTerraforming();
        DetermineDesirability();
        GenerateBases();
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
                satellites.Add(new TerrestrialPlanet(star, orbit_range, true));
            }
            else
            {
                satellites.Add(new HelianPlanet(star, orbit_range));
            }
            satellite_roll--;
        }
        for (int i=0; i<satellite_roll; i++)
        {
            satellites.Add(new DwarfPlanet(star, orbit_range, false, false, true, true));
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
        GenerateWorld();
        foreach (Planetoid p in satellites)
        {
            p.AffectFromStarExpansion();
        }
    }

    public override void GenerateWorld()
    {
        switch (type)
        {
            case JovianPlanetType.Chthonian:
                size = 16;
                atmosphere = 1;
                hydrosphere = 0;
                biosphere = 0;
                break;
            
            case JovianPlanetType.Jovian:
                size = 16;
                atmosphere = 16;
                hydrosphere = 16;
                if (RandomTools.RollD6() + (orbit_range == OrbitRange.InnerZone ? 2 : 0) >= 6)
                {
                    if (star.age >= RandomTools.RollD6()) { biosphere = RandomTools.RollD3(); }
                    if (star.age >= 7) { biosphere = RandomTools.RollD6(2) + (star.spectral_type == SpectralType.D ? -3 : 0);}
                }
                else
                {
                    biosphere = 0;
                }
                break;
        }
    }

    public override string ToString()
    {
        string type_string = "Jovian [";
        switch (type)
        {
            case JovianPlanetType.Chthonian: type_string += "Chthonian"; break;
            case JovianPlanetType.Jovian: type_string += "Jovian"; break;
        }
        type_string += "]";
        type_string += " (S" + size + ", A" + atmosphere + ", H" + hydrosphere + ", B" + biosphere + ", OR-" + orbit_range + ", D" + desirability + ")";
        string to_append = "";
        if (habitation == Habitation.Outpost) {to_append += " Outpost (P" + population + ", G" + government + ", L" + law + ", I" + industry + ", SP-" + starport + ")";}
        else if (habitation == Habitation.Colony) {to_append += " Colony (P" + population + ", G" + government + ", L" + law + ", I" + industry + ", SP-" + starport + ")";}
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
        if (satellites.Count != 0)
        {
            to_append += ":";
            to_append += "\n\t\tRings: " + rings.ToString();
            foreach (Planetoid satellite in satellites)
            {
                to_append += "\n\t\t" + satellite.ToString();
            }
        }
        return type_string + to_append;
    }
}
