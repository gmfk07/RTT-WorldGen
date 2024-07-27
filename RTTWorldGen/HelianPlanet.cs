class HelianPlanet : Planetoid
{
    private List<Planetoid> satellites = new List<Planetoid>();
    private HelianPlanetType type;

    public HelianPlanet(Star star, OrbitRange orbit_range) : base(star, orbit_range)
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
        int satellite_roll = Math.Max(RandomTools.RollD6(1) - 3, 0);
        if (satellite_roll >= 0)
        {
            int sub_satellite_roll = RandomTools.RollD6(1);
            if (sub_satellite_roll == 6)
            {
                satellites.Add(new TerrestrialPlanet(star, orbit_range, true));
                satellite_roll--;
            }
            for (int i=0; i<satellite_roll; i++)
            {
                satellites.Add(new DwarfPlanet(star, orbit_range, false, true, false, true));
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
        GenerateWorld();
        foreach (Planetoid p in satellites)
        {
            p.AffectFromStarExpansion();
        }
    }

    public override void GenerateWorld()
    {
        size = RandomTools.RollD6() + 9;
        switch (type)
        {
            case HelianPlanetType.Asphodelian:
                atmosphere = 1;
                hydrosphere = 0;
                biosphere = 0;
                break;
            
            case HelianPlanetType.Helian:
                size = RandomTools.RollD6() + 9;
                atmosphere = 12;
                int hydro_roll = RandomTools.RollD6();
                if (hydro_roll <= 2) { hydrosphere = 0; }
                else if (hydro_roll <= 4) { hydrosphere = RandomTools.RollD6(2)-1; }
                else if (hydro_roll <= 6) { hydrosphere = 15; }
                biosphere = 0;
                break;
            
            case HelianPlanetType.Panthalassic:
                atmosphere = Math.Max(RandomTools.RollD6() + 8, 13);
                hydrosphere = 11;
                int bio_dm = star.spectral_type == SpectralType.K && star.luminosity_class == 5 ? 2 : 0;
                bio_dm += star.spectral_type == SpectralType.M && star.luminosity_class == 5 ? 4 : 0;
                bio_dm += star.spectral_type == SpectralType.L ? 5 : 0;

                if (star.age >= RandomTools.RollD3() + bio_dm) { biosphere = RandomTools.RollD3();}
                if (star.age >= 4 + bio_dm) { biosphere = RandomTools.RollD6(2);}
                break;
        }

        atmosphere = Math.Max(0, atmosphere);
    }

    public override string ToString()
    {
        string type_string = "Helian [";
        switch (type)
        {
            case HelianPlanetType.Asphodelian: type_string += "Asphodelian"; break;
            case HelianPlanetType.Helian: type_string += "Helian"; break;
            case HelianPlanetType.Panthalassic: type_string += "Panthalassic"; break;
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
            foreach (Planetoid satellite in satellites)
            {
                to_append += "\n\t\t" + satellite.ToString();
            }
        }
        return type_string + to_append;
    }
}
