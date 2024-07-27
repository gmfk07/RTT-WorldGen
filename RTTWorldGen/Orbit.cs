class Orbit
{
    public OrbitRange orbit_range = OrbitRange.None;
    public Star star;
    public int desirability;
    public Habitation habitation = Habitation.Uninhabited;
    protected List<Base> bases = new List<Base>();
    public Orbit(Star star, OrbitRange orbit_range)
    {
        this.star = star;
        this.orbit_range = orbit_range;
    }

    public virtual void DetermineSatellites()
    {
        return;
    }

    public virtual void AffectFromStarExpansion()
    {

    }

    public virtual void DetermineDesirability()
    {

    }

    public virtual string ToString()
    {
        return "Orbit";
    }

    protected void TestHabitation()
    {
        if (RandomTools.RollD6(2) - 2 <= desirability)
        {
            habitation = Habitation.Colony;
            HandleColonization();
        }
        else if (RandomTools.RollD6() <= 5)
        {
            habitation = Habitation.Outpost;
            HandleOutpostPlaced();
        }
    }

    protected virtual void HandleColonization()
    {

    }

    protected virtual void HandleOutpostPlaced()
    {

    }

    protected virtual void GenerateBases()
    {
        
    }
}
