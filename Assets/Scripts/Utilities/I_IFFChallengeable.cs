
/// <summary>
/// Faction a unit belongs to
/// </summary>
public enum Team
{
    None = 0,       // Unaligned
    Blue,           //Blue,
    Red,            //Red,   
    Count           // Not a team, just number of teams that exist for use when creating arrays etc.
}

/// <summary>
/// Primary domain a unit belongs to, used for targeting by Air, Land, Sea, Space, Underwater etc.
/// </summary>
public enum Domain_Tag
{
    None = 0,
    Air,
    Land,
    Count
}

/// <summary>
/// Used for responses between factions IFF challenging each other. You challenge an IFF_Challengeable, and receive an IFF_Tag response.
/// </summary>
public enum IFF_Tag
{
    None,           // Ignore
    Neutral,        // Show up as an ID'd neutral target
    Friendly,       // Target for friendly actions
    Frenemy,        // Target, but do not shoot
    Enemy,          // Target and kill on sight
    Unknown,        // Show up as unknown contact
    Count   // Number of IFF tags possible for use when creating arrays etc.
}

public interface I_IFFChallengeable
{
    public Team Team { get; set; } //
    public Domain_Tag Domain { get; set; }

    public abstract string GetUnitName();

    /// <summary>
    /// Returns the response to an IFF challenge by whoIsAsking
    /// </summary>
    /// <param name="whoIsAsking"> The IFF challenger's team i.e. the one who is asking "Who goes there? Are you friendly" </param> 
    /// <returns></returns>
    public abstract IFF_Tag IFF_GetResponse(Team whoIsAsking);

    public static IFF_Tag[] DefaultResponseToNeutral = new IFF_Tag[(int)Team.Count]
                 {
                      IFF_Tag.None,         //None = 0,       
                      IFF_Tag.Neutral,      //Blue,
                      IFF_Tag.Neutral,      //Red,        
                };

    public static IFF_Tag[] DefaultResponseToOne = new IFF_Tag[(int)Team.Count]
                 {
                      IFF_Tag.None,         //None = 0,       
                      IFF_Tag.Friendly,     //Blue,
                      IFF_Tag.Enemy,        //Red,          
                 };


    public static IFF_Tag[] DefaultResponseToTwo = new IFF_Tag[(int)Team.Count]
                 {
                     IFF_Tag.None,          //None = 0,       
                     IFF_Tag.Enemy,         //Blue,
                     IFF_Tag.Friendly,      //Red,           
                 };


    public static IFF_Tag[] GetDefaultAlignment(Team myTeam)
    {
        switch (myTeam)
        {
            case Team.None:
            return DefaultResponseToNeutral;

            case Team.Blue:
                return DefaultResponseToOne;

            case Team.Red:
                return DefaultResponseToTwo;

            default:
                return new IFF_Tag[(int)Team.Count]
                {
                      IFF_Tag.None,  //None = 0,       
                      IFF_Tag.None,  //Blue,
                      IFF_Tag.None,  //Red,   
                };
        }
    }
}

