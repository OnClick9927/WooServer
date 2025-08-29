namespace WS.Core.HTTP;

public class TimerRecord
{
    public string key { get; set; }
    public string serverName { get; set; }

    public long time { get; set; }
    //public Action<Entity> call;
}
