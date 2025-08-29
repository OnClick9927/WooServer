namespace WS.Core.HTTP;

public interface IHttpRPCContext
{
    string GetHttpRpcUrl(string serverName);
    Task<bool> RegisterServer();

    bool Exist(string serverName);
    List<string> FindServerNames(int serverType);




    List<TimerRecord> ReadRecords();
    void OnRecordChange(List<TimerRecord> records);

    void CallRecord(TimerRecord record);
}
