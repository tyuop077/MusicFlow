using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MusicFlow.Context;

namespace MusicFlow.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthManager authManager;
        public AuthController(AuthManager authManager) => this.authManager = authManager;

        [HttpPost("login")]
        public HttpResponseMessage Authenticate([FromBody]AuthCredentials credentials)
        {
            string token = authManager.Login(credentials.username, credentials.password);
            HttpResponseMessage resp = new HttpResponseMessage();
            if (token is null)
            {
                resp.StatusCode = HttpStatusCode.Unauthorized;
            }
            else
            {
                CookieHeaderValue cookie = new CookieHeaderValue("token", token);
                //resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                resp.StatusCode = HttpStatusCode.OK;
            }
            return resp;
        }
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
    }
    public class AuthCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
