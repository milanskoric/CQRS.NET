using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace CQRS.NET.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Command(string name, [DynamicJson]  dynamic dataTransferObject)
        {
            ICommandMessage cmd = Mapper.Map<ICommandMessage>(name, dataTransferObject);
            
            Result r = CommandBus.Submit(cmd, GetCurrentContext());

            return ToActionResult(r);
 
        }

        private InvocationContext GetCurrentContext()
        {
           InvocationContext context = new InvocationContext();

            context.ParticipantId = User.Identity.GetUserId();

            return context;
        }
        private ActionResult ToActionResult(Result r)
        {
            if (r.Success)
                return Content(r.ObjectId);
            else
                return Content(r.Message);
        }
    }
}