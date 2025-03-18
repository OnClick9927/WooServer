
using Microsoft.EntityFrameworkCore;
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
        public (int id, int sid, object msg, bool succeed) Decode(byte[] unpack)
        {
            return (1, 1, null, true);
        }

        public byte[] Encode(int id, int sid, object msg)
        {
            return null;
        }

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

        public void Refresh(WebSocketToken token, DateTime now)
        {
        }

        public void Remove(WebSocketToken token)
        {
        }

        [WebSocketMethodAttribute]
        public void Test(WebSocketToken token, int id, int sid, TestWS hha)
        {
            Console.WriteLine();
        }

        public byte[] Unpack()
        {
            return null;
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

    //[HttpServiceAttribute(ServerType.Game, "TODO")]
    //public class TestHttpService
    //{
    //    private TodoDbContext db;
    //    public TestHttpService(TestWS ws, TodoDbContext db)
    //    {
    //        this.db = db;
    //    }

    //    [HttpMethod(HttpMethodAttribute.MethodType.GET)]
    //    public async Task<IResult> GetAllTodos()
    //    {

    //        return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());

    //    }

    //    [HttpMethod(HttpMethodAttribute.MethodType.GET)]
    //    public async Task<IResult> GetCompleteTodos()
    //    {

    //        return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
    //    }

    //    [HttpMethod(HttpMethodAttribute.MethodType.GET)]
    //    public async Task<IResult> GetTodo(int id)
    //    {

    //        return await db.Todos.FindAsync(id)
    //        is Todo todo
    //            ? TypedResults.Ok(new TodoItemDTO(todo))
    //            : TypedResults.NotFound();
    //    }

    //    [HttpMethod(HttpMethodAttribute.MethodType.Post, "HH", true)]
    //    public async Task<IResult> CreateTodo([FromHeader] TodoItemDTO todoItemDTO, [FromHeader] int test)
    //    {
    //        var todoItem = new Todo
    //        {
    //            IsComplete = todoItemDTO.IsComplete,
    //            Name = todoItemDTO.Name
    //        };

    //        db.Todos.Add(todoItem);
    //        await db.SaveChangesAsync();

    //        todoItemDTO = new TodoItemDTO(todoItem);

    //        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
    //    }

    //    [HttpMethod(HttpMethodAttribute.MethodType.Post)]
    //    public async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO)
    //    {
    //        var todo = await db.Todos.FindAsync(id);

    //        if (todo is null) return TypedResults.NotFound();

    //        todo.Name = todoItemDTO.Name;
    //        todo.IsComplete = todoItemDTO.IsComplete;

    //        await db.SaveChangesAsync();

    //        return TypedResults.NoContent();
    //    }
    //    [HttpMethod(HttpMethodAttribute.MethodType.Delete)]
    //    public async Task<IResult> DeleteTodo(int id)
    //    {
    //        if (await db.Todos.FindAsync(id) is Todo todo)
    //        {
    //            db.Todos.Remove(todo);
    //            await db.SaveChangesAsync();
    //            return TypedResults.NoContent();
    //        }

    //        return TypedResults.NotFound();

    //    }





    //}


    [Route("api/[controller]")]
    [ApiController]
    [RpcControllerAttribute(ServerType.Game)]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public async Task<IResult> Test([FromHeader]TodoItemDTO todoItemDTO, [FromHeader]int test)
        {
            await Task.Delay(100);
            return TypedResults.Ok(todoItemDTO);
        }
    }
}
