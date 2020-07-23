namespace SRUL
{
    public interface IReadWrite
    {
        // dynamic IRead(string type, string varName);
        // dynamic IWrite { get; set; }
        dynamic Read(string type, string varName, bool round = true);
        bool Write(string type, string varName, dynamic value);
        dynamic Freeze(string type, string varName);

        // dynamic InvokeDelegation { get; set; }
        dynamic FinTreasury { get; set; }
        dynamic FinGDP { get; set; }
        dynamic DomPopulation { get; set; }
        dynamic DomImmigration { get; set; }
        dynamic DomEmigration { get; set; }
        dynamic DomBirth { get; set; }
        dynamic DomDeath { get; set; }
        dynamic DomLiteracy { get; set; }
        dynamic DomTourism { get; set; }
        dynamic DomCreditRating { get; set; }
        dynamic DomUNSubsidy { get; set; }
        dynamic DomUNOpinion { get; set; }
        dynamic DomTreatyIntegrity { get; set; }
        dynamic DomDomApproval { get; set; }
        dynamic DomMilApproval { get; set; }
        dynamic FinInflation { get; set; }
        dynamic DomUnemployment { get; set; }
        
        // 1 Day Click Feature
        dynamic ADayClickBuild { get; set; }
        dynamic ADayClickArmy { get; set; }
        dynamic ADayClickResearch { get; set; }
        // dynamic readpopo
    }
}