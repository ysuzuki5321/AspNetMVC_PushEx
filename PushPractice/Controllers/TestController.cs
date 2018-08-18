using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PushPractice.Controllers
{
    public class Status
    {
        
        public int Id { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public List<Status> list { get; set; }
    }
    public class TestController : CometController
    {
        public static int maxId = 0;
        public static List<Status> list = new List<Status>();
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            var s = new Status()
            {
                Id = ++maxId,
                list = list,
            };
            return View(s);
        }
        [HttpPost]
        public ActionResult Create(Status s)
        {
            lock (list)
            {
                list.Add(s);
            }
            Notify();
            s.list = list;
            return View(s);
        }
        
        public void ReceiveAsync(int since_id = 0)
        {
            if (since_id == 0)
            {
                return;
            }
            AsyncManager.OutstandingOperations.Increment();
            AddObserver(since_id, () =>
             {
                 AsyncManager.Parameters["since_id"] = since_id;
                 AsyncManager.OutstandingOperations.Decrement();
             });
        }

        public ActionResult ReceiveCompleted(int since_id = 0)
        {
            RemoveObserver(since_id);

            var statuses = list.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Text = p.Text
            }).ToArray();
            return Json(statuses);
        }
    }
}