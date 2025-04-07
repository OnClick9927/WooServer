namespace WS.Core.Tool;

public static class TimeTool
{
    public interface ITimeEntityContext
    {
        void Invoke();
    }

    private static List<TimeEntity> entities = new List<TimeEntity>();
    public class TimeEntity
    {
        private ITimeEntityContext context;
        internal void Invoke()
        {
            context.Invoke();
        }
        internal bool completed { get; private set; }
        public void InvokeComplete()
        {
            completed = true;
        }
        internal static TimeEntity Create(ITimeEntityContext context)
        {
            var entity = new TimeEntity()
            {
                context = context,
                completed = false,
            };
            return entity;
        }
    }
    public static TimeEntity Add(ITimeEntityContext context)
    {
        var entity = TimeEntity.Create(context);
        entities.Add(entity);
        return entity;
    }

    internal static void Update()
    {
        entities.RemoveAll(x => x.completed);
        for (var i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            entity.Invoke();
        }
    }

    public static long GetTimeStamp(DateTime dateTime)
    {
        DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);
        long timestamp = dateTimeOffset.ToUnixTimeSeconds();
        return timestamp;
    }
    public static long GetTimeStamp_Now()
    {
        DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
        long timeStamp = dateTimeOffset.ToUnixTimeSeconds();
        return timeStamp;
    }
    public static DateTime TimeStampToDateTime(long timeStamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
        DateTime dateTime = dateTimeOffset.DateTime;
        return dateTime;
    }
    public static DateTime TimeStampToLocalDateTime(long timeStamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
        DateTime dateTime = dateTimeOffset.LocalDateTime;
        return dateTime;
    }
}

