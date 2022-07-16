using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class BuggyController: BaseApiController
    {
        private readonly DataContext context;

        public BuggyController(DataContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }



        [HttpGet("not-found")]
        public ActionResult<string> GetNotFound()
        {
            return NotFound();
        }



        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            return context.Users.Find(-1).ToString();
        }



        [HttpGet("bad-request")]
        public ActionResult<string> GetBadReq()
        {
            return BadRequest("this was bad request");
        }





    }
}

