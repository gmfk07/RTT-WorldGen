class TerrestrialPlanet : Planetoid
{
    private DwarfPlanet satellite = null;
    private TerrestrialPlanetType type;

    public TerrestrialPlanet(Star star, OrbitRange orbit_range, bool is_satellite) : base(star, orbit_range)
    {
        if (!is_satellite) { DetermineSatellites(); }
        DetermineType();
        GenerateWorld();
        CheckTerraforming();
        DetermineDesirability();
        GenerateBases();
    }

    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 5)
        {
            satellite = new DwarfPlanet(star, orbit_range, false, false, false, true);
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
        GenerateWorld();
        if (satellite != null)
        {
            satellite.AffectFromStarExpansion();
        }
    }

    public override void GenerateWorld()
    {
        int atm_dm = 0, hydro_dm = 0, bio_dm = 0;
        int atm_roll = 0, hydro_roll = 0, bio_roll = 0;
        size = RandomTools.RollD6() + 4;
        switch (type)
        {
            case TerrestrialPlanetType.Acheronian:
                atmosphere = 1;
                hydrosphere = 0;
                biosphere = 0;
                break;

            case TerrestrialPlanetType.Arid:
                bio_dm += star.spectral_type == SpectralType.K && star.luminosity_class == 5 ? 2 : 0;
                bio_dm += star.spectral_type == SpectralType.M && star.luminosity_class == 5 ? 4 : 0;
                bio_dm += star.spectral_type == SpectralType.L ? 5 : 0;
                bio_dm += orbit_range == OrbitRange.OuterZone ? 2 : 0;
                if (star.age >= RandomTools.RollD3() + bio_dm) { biosphere = RandomTools.RollD3(); }
                if (star.age >= 4 + bio_dm + (star.spectral_type == SpectralType.D ? -3 : 0)) { biosphere = RandomTools.RollD6(2); }

                if (biosphere >= 3) { atmosphere = Math.Clamp(atmosphere = RandomTools.RollD6(2)-7+size, 2, 9); }
                else { atmosphere = 10; }

                hydrosphere = RandomTools.RollD3();
                break;
            
            case TerrestrialPlanetType.JaniLithic:
                atmosphere = RandomTools.RollD6() <= 3 ? 1 : 10;
                hydrosphere = 0;
                biosphere = 0;
                break;
            
            case TerrestrialPlanetType.Oceanic:
                bio_dm += star.spectral_type == SpectralType.K && star.luminosity_class == 5 ? 2 : 0;
                bio_dm += star.spectral_type == SpectralType.M && star.luminosity_class == 5 ? 4 : 0;
                bio_dm += star.spectral_type == SpectralType.L ? 5 : 0;
                bio_dm += orbit_range == OrbitRange.OuterZone ? 2 : 0;
                if (star.age >= RandomTools.RollD3() + bio_dm) { biosphere = RandomTools.RollD3(); }
                if (star.age >= 4 + bio_dm + (star.spectral_type == SpectralType.D ? -3 : 0)) { biosphere = RandomTools.RollD6(2); }

                atm_dm += star.spectral_type == SpectralType.K && star.luminosity_class == 5 ? -1 : 0;
                atm_dm += star.spectral_type == SpectralType.M && star.luminosity_class == 5 ? -2 : 0;
                atm_dm += star.spectral_type == SpectralType.L ? -3 : 0;
                atm_dm += star.luminosity_class == 4 ? -1 : 0;
                if (biosphere >= 0) { atmosphere = Math.Clamp(RandomTools.RollD6(2) + size - 6, 1, 12); }
                else
                {
                    atm_roll = RandomTools.RollD6();
                    if (atm_roll <= 1) {atmosphere = 1;}
                    else if (atm_roll <= 4) {atmosphere = 10;}
                    else if (atm_roll <= 6) {atmosphere = 12;}
                }

                hydrosphere = 11;
                break;
            
            case TerrestrialPlanetType.Tectonic:
                bio_dm += star.spectral_type == SpectralType.K && star.luminosity_class == 5 ? 2 : 0;
                bio_dm += star.spectral_type == SpectralType.M && star.luminosity_class == 5 ? 4 : 0;
                bio_dm += star.spectral_type == SpectralType.L ? 5 : 0;
                bio_dm += orbit_range == OrbitRange.OuterZone ? 2 : 0;
                if (star.age >= RandomTools.RollD3() + bio_dm) { biosphere = RandomTools.RollD3(); }
                if (star.age >= 4 + bio_dm + (star.spectral_type == SpectralType.D ? -3 : 0)) { biosphere = RandomTools.RollD6(2); }

                if (biosphere >= 3) {atmosphere = Math.Clamp(RandomTools.RollD6(2) + size - 7, 2, 9);}
                else {atmosphere = 10;}

                hydrosphere = RandomTools.RollD6(2)-2;
                break;
            
            case TerrestrialPlanetType.Telluric:
                atmosphere = 12;
                if (RandomTools.RollD6() <= 4) { hydrosphere = 0; }
                else if (RandomTools.RollD6() <= 6) { hydrosphere = 15;}
                biosphere = 0;
                break;
        }

        biosphere = Math.Max(0, biosphere);
        hydrosphere = Math.Max(0, hydrosphere);
        atmosphere = Math.Max(0, atmosphere);
    }

    public override string ToString()
    {
        string type_string = "Terrestrial [";
        switch (type)
        {
            case TerrestrialPlanetType.Acheronian: type_string += "Acheronian"; break;
            case TerrestrialPlanetType.Arid: type_string += "Arid"; break;
            case TerrestrialPlanetType.JaniLithic: type_string += "JaniLithic"; break;
            case TerrestrialPlanetType.Oceanic: type_string += "Oceanic"; break;
            case TerrestrialPlanetType.Tectonic: type_string += "Tectonic"; break;
            case TerrestrialPlanetType.Telluric: type_string += "Telluric"; break;
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
        if (satellite != null) { to_append += ":\n\t\t" + satellite.ToString(); }
        return type_string + to_append;
    }
}
