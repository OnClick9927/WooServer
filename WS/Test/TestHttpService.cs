using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using static WS.Core.HTTP.HTTPTool;
using WS.DB;
using WS.Core.Config;
using WS.Core.HTTP;
using WS.Core.WebSockets;

namespace WS.Test
{


    [WebSocketHandlerAttribute]
    public class TestWS : IWebSocketBinaryQueue, IWebSocketMsgPacker, IWebSocketTextQueue
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
        public static async Task<HttpPostResult<TodoItemDTO>> RPCCreateTodo( int test)
        {

            return await HTTPTool.RpcHTTPPost<TodoItemDTO>(typeof(TestController), nameof(TestController.Test), new Dictionary<string, object> {
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
        TodoDbContext db;

        public TestController(TodoDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<IResult> Test( [FromHeader] int test)
        {
            TodoItemDTO todoItemDTO = new TodoItemDTO();
            var get = db.Get();
            todoItemDTO.Name=get.Name;


            await Task.Delay(100);
            return TypedResults.Ok(todoItemDTO);
        }
    }
}
