using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using static WS.HTTP.HTTPTool;
using WS.DB;
using WS.HTTP;
using WS.WebSockets;
using WS.Core.Config;

namespace WS.Test
{


    [WebSocketHandlerAttribute]
    public class TestWS : IWebSocketBinaryQueue, IWebSocketMsgPacker, IWebSocketTokenCollection, IWebSocketTextQueue
    {



        public void OnBinaryMessage(byte[] buffer, int offset, int len)
        {
        }

        public void Init(int size)
        {
        }

        public void OnTextMessage(WebSocketToken token, bool endOfMessage, byte[] buffer, int offset, int len)
        {
            token.Send(new ArraySegment<byte>(buffer, 0, len), WebSocketMessageType.Text, endOfMessage);
        }

        List<WebSocketToken> tokens = new List<WebSocketToken>();
        public IEnumerable<WebSocketToken> GetTokens()
        {
            return tokens;
        }
        public void Refresh(WebSocketToken token, DateTime now)
        {
            if (!tokens.Contains(token))
                tokens.Add(token);
        }

        public void Remove(WebSocketToken token)
        {
            tokens.Remove(token);
        }

        public void Test(WebSocketToken token, int id, int sid, TodoItemDTO hha)
        {
            Console.WriteLine();
        }




        public void Release(ArraySegment<byte> bytes)
        {
        }

        public ArraySegment<byte> Encode(int id, int sid, object msg)
        {
            return null;
        }

        public ArraySegment<byte> Unpack()
        {
            return null;
        }

        public (int id, int sid, object msg, bool succeed) Decode(ArraySegment<byte> buffers)
        {
            return (0, 0, null, false);
        }
    }

    public static class RPC
    {
        public static async Task<HttpPostResult<TodoItemDTO>> RPCCreateTodo(TodoItemDTO todoItemDTO, int test)
        {

            return await HTTPTool.RpcHTTPPost<TodoItemDTO>(typeof(TestController), nameof(TestController.Test), new Dictionary<string, object> {
                { nameof(todoItemDTO),todoItemDTO},
                { nameof(test),test}
            });
        }

    }
    public class TodoItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public TodoItemDTO() { }
        public TodoItemDTO(Todo todoItem) =>
        (Id, Name, IsComplete) = (todoItem.Id, todoItem.Name, todoItem.IsComplete);

        public static bool TryParse(string value, out TodoItemDTO result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<TodoItemDTO>(value);
                return true;

            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

    }
    [Route("api/[controller]")]
    [ApiController]
    [RpcControllerAttribute(ServerType.Game)]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public async Task<IResult> Test([FromHeader] TodoItemDTO todoItemDTO, [FromHeader] int test)
        {
            await Task.Delay(100);
            return TypedResults.Ok(todoItemDTO);
        }
    }
}
