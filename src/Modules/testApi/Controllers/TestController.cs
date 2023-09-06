using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController
    {

        public TestController()
        {
        }

        [HttpGet(Name = "testGetValue")]
        public string Get()
        {
            return "ok";
        }
    }
}