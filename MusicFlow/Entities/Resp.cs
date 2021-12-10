using Microsoft.AspNetCore.Mvc;

namespace MusicFlow.Entities
{
    public class Resp
    {
        public bool Success { get; set; } = false;
        public string Error { get; set; } = null;
        public static JsonResult SetSuccess() => new JsonResult(new Resp { Success = true });
        public static JsonResult SetError(string reason) => new JsonResult(new Resp { Error = reason });
    }
}
