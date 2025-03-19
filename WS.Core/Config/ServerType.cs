namespace WS.Core.Config;

[Flags]
public enum ServerType
{
    Gate = 2,
    Game = 4,
    All = Gate | Game,
}
