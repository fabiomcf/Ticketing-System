using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cloud_Tickets.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Web.Security;


namespace Cloud_Tickets.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            Computer computer = new Computer();
            computer.GetHostDetails();
            string username_ = User.Identity.Name;
            string _username = username_.Substring(username_.IndexOf("\\") + 1);
            ViewBag.Username = _username;
            
            ViewBag.Hostname = computer.Host;
            ViewBag.IP = computer.IP;
            ViewBag.Date = computer.Date;
            ViewBag.Location = computer.Location;
            ViewBag.Notifications = computer.Notifications;
            return View();
        }


        public bool GetAdmin(string username_)
        {
            Computer computer = new Computer();
            return computer.GetAdmin(username_);
        } 

        public string Save(string username, string hostname, string ipadrs, string loc, string date, string report, string descript)
        {
            Computer computer = new Computer();
            int result = computer.SaveToDB(username, hostname, ipadrs, loc, date, report, descript);
            computer.SendEmail(username, report, descript, loc);
            string RES=null;
            if (result == 1)
            {
                RES = "1";
            }
            else if (result == 0)
            {
                RES = computer.DB_RESULT;
            }
            return RES;
        }

        public JsonResult ThrowJSONError(Exception e)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            Response.TrySkipIisCustomErrors = true;
            //Log your exception
            return Json(new { Message = e.Message });
        }

        [HttpGet]
        public JsonResult TicketsAbertos(string username_)
        {
            Computer computer = new Computer();
            computer.GetHostDetails();
            List<Computer> list = new List<Computer>();
            foreach (Computer item in computer.GetOpenTickets(username_))
            {
                list.Add(item);
            }
            return Json(list,JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult TicketsResolvidos(string username_)
        {
            Computer computer = new Computer();
            computer.GetHostDetails();
            List<Computer> list = new List<Computer>();
            string user_ = User.Identity.Name;
            foreach (Computer item in computer.GetClosedTickets(username_))
            {
                list.Add(item);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetAdminMenu()
        {
            Computer computer = new Computer();
            return Json(computer.Admin());
        }
        public string ChangeStatus(int ID, string Username)
        {
            Computer computer = new Computer();
            return computer.ChangeStatus(ID,Username);
        }
        public string SendMessageToUser(int ID, string IN_CH, string Username, string Msg)
        {
            Computer computer = new Computer();
            computer.SendMessageToUser(ID, IN_CH, Username, Msg);
            return computer.ChangeStatus(ID, Username);
 
        }

    }
}
