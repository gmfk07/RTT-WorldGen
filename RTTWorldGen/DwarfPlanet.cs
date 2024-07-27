class DwarfPlanet : Planetoid
{
    private DwarfPlanet companion = null;
    private DwarfPlanetType type;
    
    public DwarfPlanet(Star star, OrbitRange orbit_range, bool asteroid_belt_member, bool helian_sat, bool jovian_sat, bool is_satellite) : base(star, orbit_range)
    {
        if (!is_satellite)
        {
            DetermineSatellites();
        }
        DetermineType(asteroid_belt_member, helian_sat, jovian_sat);
        GenerateWorld();
        CheckTerraforming();
        DetermineDesirability();
        GenerateBases();
    }
    public override void DetermineSatellites()
    {
        int satellite_roll = RandomTools.RollD6(1);
        if (satellite_roll >= 6)
        {
            companion = new DwarfPlanet(star, orbit_range, false, false, false, true);
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
        int atm_dm = 0, hydro_dm = 0, bio_dm = 0;
        int atm_roll = 0, hydro_roll = 0, bio_roll = 0;
        switch (type)
        {
            case DwarfPlanetType.Arean:
                size = RandomTools.RollD6()-1;

                atm_dm = star.spectral_type == SpectralType.D ? -2 : 0;
                atm_roll = RandomTools.RollD6() + atm_dm;
                if (atm_roll <= 3) {atmosphere = 1;}
                else {atmosphere = 10;}

                hydro_dm = atmosphere == 1 ? -4 : 0;
                hydrosphere = RandomTools.RollD6(2) + size - 7 + hydro_dm;

                bio_dm = (star.spectral_type == SpectralType.L ? 2 : 0) + (orbit_range == OrbitRange.OuterZone ? 2 : 0);
                if (star.age >= RandomTools.RollD3() + bio_dm && atmosphere == 1) {biosphere = RandomTools.RollD6() - 4;}
                else if (star.age >= RandomTools.RollD3() + bio_dm && atmosphere == 10) {biosphere = RandomTools.RollD3();}
                else if (star.age >= RandomTools.RollD3() + bio_dm && atmosphere == 1) {biosphere = RandomTools.RollD6() + size - 2;}
                else {biosphere = 0;}
                break;
            
            case DwarfPlanetType.Hebean:
                size = RandomTools.RollD6()-1;
                atmosphere = RandomTools.RollD6() + size - 6;
                if (atmosphere >= 2) { atmosphere = 10; }
                hydrosphere = RandomTools.RollD6(2) + size - 11;
                biosphere = 0;
                break;

            case DwarfPlanetType.Meltball:
                size = RandomTools.RollD6()-1;
                atmosphere = 1;
                hydrosphere = 15;
                biosphere = 0;
                break;

            case DwarfPlanetType.Promethean:
                this.size = RandomTools.RollD6()-1;

                hydrosphere = RandomTools.RollD6(2) - 2;

                bio_dm = (star.spectral_type == SpectralType.L ? 2 : 0) + (orbit_range == OrbitRange.Epistellar ? -2 : 0) + (orbit_range == OrbitRange.OuterZone ? 2 : 0);
                if (star.age >= RandomTools.RollD3() + bio_dm) {biosphere = RandomTools.RollD3();}
                else if (star.age >= 4 + bio_dm + (star.spectral_type == SpectralType.D ? -3 : 0)) {biosphere = RandomTools.RollD6(2);}
                else {biosphere = 0;}

                if (biosphere >= 3) {atmosphere = Math.Clamp(RandomTools.RollD6(2) + size - 7, 2, 9);}
                else {atmosphere = 10;}
                break;

            case DwarfPlanetType.Rockball:
                size = RandomTools.RollD6()-1;
                atmosphere = 0;
                hydrosphere = RandomTools.RollD6(2) + size - 11 + (star.spectral_type == SpectralType.L ? 1 : 0) + (orbit_range == OrbitRange.Epistellar ? -2 : 0) + (orbit_range == OrbitRange.OuterZone ? 2 : 0);
                biosphere = 0;
                break;

            case DwarfPlanetType.Snowball:
                size = RandomTools.RollD6()-1;
                atmosphere = (RandomTools.RollD6() <= 4 ? 0 : 1);
                bool subsurface_oceans = RandomTools.RollD6() <= 4;
                hydrosphere = (subsurface_oceans ? 10 : RandomTools.RollD6(2)-2);
                if (subsurface_oceans && star.age >= RandomTools.RollD6()) { biosphere = RandomTools.RollD6() - 3;}
                else if (subsurface_oceans && star.age >= 6 + (star.spectral_type == SpectralType.L ? 2 : 0) + (orbit_range == OrbitRange.OuterZone ? 2 : 0)) { biosphere = RandomTools.RollD6() + size - 2;}
                break;
            
            case DwarfPlanetType.Stygian:
                size = RandomTools.RollD6()-1;
                atmosphere = 0;
                hydrosphere = 0;
                biosphere = 0;
                break;
            
            case DwarfPlanetType.Vesperian:
                size = RandomTools.RollD6()+4;
                
                if (star.age >= RandomTools.RollD3()) {biosphere = RandomTools.RollD3();}
                if (star.age >= 4) {biosphere = RandomTools.RollD6(2);}

                if (biosphere >= 3) {atmosphere = Math.Clamp(RandomTools.RollD6(2)+size-7, 2, 9);}
                else {atmosphere = 10;}

                hydrosphere = RandomTools.RollD6(2)-2;
                break;
        }

        biosphere = Math.Max(0, biosphere);
        hydrosphere = Math.Max(0, hydrosphere);
    }

    public override string ToString()
    {
        string type_string = "Dwarf [";
        switch (type)
        {
            case DwarfPlanetType.Arean: type_string += "Arean"; break;
            case DwarfPlanetType.Hebean: type_string += "Hebean"; break;
            case DwarfPlanetType.Meltball: type_string += "Meltball"; break;
            case DwarfPlanetType.Promethean: type_string += "Promethean"; break;
            case DwarfPlanetType.Rockball: type_string += "Rockball"; break;
            case DwarfPlanetType.Snowball: type_string += "Snowball"; break;
            case DwarfPlanetType.Stygian: type_string += "Stygian"; break;
            case DwarfPlanetType.Vesperian: type_string += "Vesperian"; break;
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
        if (companion != null) { to_append += ":\n\t\t" + companion.ToString(); }
        return type_string + to_append;
    }

    public override void AffectFromStarExpansion()
    {
        type = DwarfPlanetType.Stygian;
        GenerateWorld();
        if (companion != null)
        {
            companion.AffectFromStarExpansion();
        }
    }
}
