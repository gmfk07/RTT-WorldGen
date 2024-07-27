class Planetoid : Orbit
{
    protected int size;
    protected int atmosphere;
    protected int hydrosphere;
    protected int biosphere;
    protected int population;
    protected int government;
    protected int industry;
    protected int law=0;
    bool seeded;
    protected Starport starport = Starport.None;

    public Planetoid(Star star, OrbitRange orbit_range) : base(star, orbit_range)
    {
    }
    
    public virtual void GenerateWorld()
    {
    }

    public override void DetermineDesirability()
    {
        int dm = 0;
        if (hydrosphere == 0) { dm -= 1; }
        if (size >= 13 || (atmosphere >= 12 && atmosphere <= 16)) { dm -= 2; }
        if (star.flare_star) { dm -= RandomTools.RollD3(); }
        if ((size >= 1 && size <= 11) && (atmosphere >= 2 && atmosphere <= 9) && (hydrosphere >= 1 && hydrosphere <= 11))
        {
            if ((size >= 5 && size <= 10) && (atmosphere >= 4 && atmosphere <= 9) && (hydrosphere >= 4 && hydrosphere <= 8)) {dm += 5;}
            else if (hydrosphere >= 10 && hydrosphere <= 11) {dm += 3;}
            else if (atmosphere >= 2 && atmosphere <= 6 && hydrosphere >= 0 && hydrosphere <= 3) {dm += 2;}
            else {dm += 4;}
        }
        if (size >= 10 && atmosphere <= 15) {dm -= 1;}
        if (orbit_range == OrbitRange.InnerZone)
        {
            if (star.spectral_type == SpectralType.M && star.luminosity_class == 5) {dm += 1;}
            else {dm += 2;}
        }
        if (size == 0) {dm -= 1;}
        if (atmosphere == 6 || atmosphere == 8) {dm += 1;}
        desirability = dm;

        TestHabitation();
    }

    protected override void HandleColonization()
    {
        int tl = 14;
        int settlement = 4;
        //RTT WorldGen has a maximum that potentially goes under the minimum, so I'm assuming the minimum takes precedence
        int pop_max = desirability + RandomTools.RollD3() - RandomTools.RollD3();
        population = Math.Min(tl + settlement - 9, pop_max);
        population = Math.Max(population, 4);
        government = population + RandomTools.RollD6(2) - 7;

        if (size >= 1 && size <= 11 && atmosphere >= 2 && atmosphere <= 9)
        {
            biosphere = Math.Min(biosphere, RandomTools.RollD6() + 5);
            seeded = true;
        }

        if (government != 0) {law = government + RandomTools.RollD6(2) - 7;}
        int dm = law >= 1 && law <= 3 ? 1 : 0;
        dm += law >= 6 && law <= 9 ? 1 : 0;
        dm += law >= 10 && law <= 3 ? 12 : 0;
        dm += law >= 13 ? 1 : 0;
        dm += (atmosphere >= 0 && atmosphere <= 4) || atmosphere == 7 || atmosphere >= 9 || hydrosphere == 15 ? 1 : 0;
        dm += (tl >= 12 && tl <= 14) ? 1 : 0;
        dm += tl >= 15 ? 1 : 0;
        if (population != 0) {industry = population + RandomTools.RollD6(2) - 7 + dm;}

        if (industry == 0) {population -= 1;}
        if (industry >= 4 && industry <= 9) {
            if (atmosphere == 3) { atmosphere = 2; }
            if (atmosphere == 5) { atmosphere = 4; }
            if (atmosphere == 6) { atmosphere = 7; }
            if (atmosphere == 8) { atmosphere = 9; }
        }
        if (industry >= 10)
        {
            if (RandomTools.RollD6() >= 4) {population += 1;}
            else {
                population += 2;
                if (atmosphere == 3) { atmosphere = 2; }
                if (atmosphere == 5) { atmosphere = 4; }
                if (atmosphere == 6) { atmosphere = 7; }
                if (atmosphere == 8) { atmosphere = 9; }
            }
        }

        int roll = RandomTools.RollD6(2) + industry - 7 + 1;
        if (roll <= 2) { starport = Starport.None; }
        else if (roll <= 4) { starport = Starport.Frontier; }
        else if (roll <= 6) { starport = Starport.Poor; }
        else if (roll <= 8) { starport = Starport.Routine; }
        else if (roll <= 10) { starport = Starport.Good; }
        else { starport = Starport.Excellent; }

        if ((population >= 1 || industry >= 5) && starport == Starport.None) { starport = Starport.Frontier; }
    }

    protected override void HandleOutpostPlaced()
    {
        population = Math.Clamp(RandomTools.RollD3() + desirability, 0, 4);
        government = population == 0 ? 0 : Math.Clamp(population + RandomTools.RollD6() - 7, 0, 6);

        if (population >= 0 && starport == Starport.None)
        {
            starport = Starport.Frontier;
        }
    }

    protected void CheckTerraforming()
    {
        int tl = 14;

        if (size >= 1 && size <= 11 && atmosphere >= 1 && atmosphere <= 13 && hydrosphere != 15 && orbit_range == OrbitRange.InnerZone)
        {
            int tp = Math.Max(tl + 4 - 15, tl + RandomTools.RollD6() - 15);

            bases.Add(Base.Terraforming);

            //If not habitable world, get into habitable range
            bool habitable_size = size >= 1 && size <= 11;
            bool habitable_atmos = atmosphere >= 2 && atmosphere <= 9;
            bool habitable_hydro = hydrosphere >= 0 && hydrosphere <= 11;
            bool habitable = habitable_size && habitable_atmos && habitable_hydro;
            if (!habitable)
            {
                //Special hydro removal rule for worlds with atmos 11-12
                if (atmosphere >= 11 && atmosphere <= 12 && hydrosphere >= 2)
                {
                    while (hydrosphere > 1 && tp > 0) { hydrosphere--; tp--; }
                }
                while (atmosphere < 2 && tp > 0) { atmosphere++; tp--; }
                while (atmosphere > 9 && tp > 0) { atmosphere--; tp--; }
            }

            //If habitable, try to reach garden world status
            while (atmosphere < 5 && tp > 0) { atmosphere++; tp--; }
            while (hydrosphere < 4 && tp > 0) { hydrosphere++; tp--; }
            while (hydrosphere > 8 && hydrosphere != 11 && tp > 0) { hydrosphere--; tp--; }

            //Spend the rest on biosphere
            while (tp > 0) { biosphere++; tp--; }
        }
    }

    protected override void GenerateBases()
    {
        if (habitation == Habitation.Uninhabited)
        {
            return;
        }

        if (habitation == Habitation.Outpost)
        {
            if (RandomTools.RollD6(2) >= 9) { bases.Add(Base.ResearchInstallation); }
            return;
        }
        
        int merchant_base_roll;
        int research_roll;
        switch (starport)
        {
            case Starport.Excellent:
                if (RandomTools.RollD6(2) >= 6) { bases.Add(Base.Embassy); }

                merchant_base_roll = RandomTools.RollD6(2);
                if (merchant_base_roll >= 6) { bases.Add(Base.MerchantBase); }
                if (merchant_base_roll >= 9) { bases.Add(Base.Shipyard); }
                if (merchant_base_roll >= 10) { bases.Add(Base.CorporateBase); }

                research_roll = RandomTools.RollD6(2);
                if (research_roll >= 8) { bases.Add(Base.ResearchInstallation); }
                if (research_roll >= 11) { bases.Add(Base.University); }

                if (RandomTools.RollD6(2) >= 10) { bases.Add(Base.ScoutBase); }
                break;

            case Starport.Good:
                if (RandomTools.RollD6(2) >= 8) { bases.Add(Base.Embassy); }

                merchant_base_roll = RandomTools.RollD6(2);
                if (merchant_base_roll >= 8) { bases.Add(Base.MerchantBase); }
                if (merchant_base_roll >= 11) { bases.Add(Base.Shipyard); }

                research_roll = RandomTools.RollD6(2);
                if (research_roll >= 10) { bases.Add(Base.ResearchInstallation); }

                if (RandomTools.RollD6(2) >= 12) { bases.Add(Base.PirateBase); }
                if (RandomTools.RollD6(2) >= 8) { bases.Add(Base.ScoutBase); }
                break;

            case Starport.Routine:
                if (RandomTools.RollD6(2) >= 10) { bases.Add(Base.Embassy); }
                if (RandomTools.RollD6(2) >= 10) { bases.Add(Base.MerchantBase); }
                if (RandomTools.RollD6(2) >= 10) { bases.Add(Base.ResearchInstallation); }
                if (RandomTools.RollD6(2) >= 10) { bases.Add(Base.PirateBase); }
                if (RandomTools.RollD6(2) >= 8) { bases.Add(Base.ScoutBase); }
                break;
        
            case Starport.Poor:
                if (RandomTools.RollD6(2) >= 7) { bases.Add(Base.ScoutBase); }
                if (RandomTools.RollD6(2) >= 12) { bases.Add(Base.PirateBase); }
                break;
        }

        if ((population >= 1 || biosphere >= 1) && RandomTools.RollD6(2) >= 10)
        {
            int roll = RandomTools.RollD6();
            if (roll <= 2) { bases.Add(Base.Prison); }
            if (roll <= 4) { bases.Add(Base.RefugeeFacility); }
            if (roll <= 6) { bases.Add(Base.NaturePreserve); }
        }

        int naval_base_roll = RandomTools.RollD6(2);
        if (naval_base_roll >= 6) { bases.Add(Base.NavalBase); }
        if (naval_base_roll >= 9) { bases.Add(Base.Shipyard); }
        
        if (RandomTools.RollD6(2) <= population) { bases.Add(Base.SacredSite); }
    }
}
