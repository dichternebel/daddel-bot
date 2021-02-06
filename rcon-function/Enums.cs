namespace Rcon.Function
{
    // https://totalcsgo.com/commands

    public enum GameTypes
    {
        casual,
        competitive,
        wingman,
        dangerzone,
        deathmatch,
        armsrace
    }

    /*
    https://steamcommunity.com/sharedfiles/filedetails/?id=437426939

    Mercury----0.378 G----302.4
    Venus----0.907 G----725.6
    Earth----1 G----800
    Moon----0.166 G----132.8
    Mars----0.377 G----301.6
    Jupiter----2.36 G----1888
    Saturn----0.916 G----732.8
    Uranus----0.889 G----711.2
    Neptune----1.12 G----896
    Pluto----0.059 G----47.2
    */

    public enum GravityTypes
    {
        mercury = 302,
        venus = 726,
        earth = 800,
        moon = 133,
        mars = 302,
        jupiter = 1888,
        saturn = 733,
        uranus = 711,
        neptune = 896,
        pluto = 47
    }
}