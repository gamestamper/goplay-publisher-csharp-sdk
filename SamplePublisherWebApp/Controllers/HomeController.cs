using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay;
using GoPlay.Serialization;

namespace SamplePublisherWebApp.Controllers
{
    public class HomeController : Controller
    {
        private string key = "pbgspub";
        private string secret = "a8sdfjweuy3456";
        public ActionResult Index()
        {
            GraphRequest.Testing = true;
            dynamic SDK = new PublisherSDK(key, secret);
            ViewBag.Message = SDK.Pub.PlayersClub.Get().Data.ToString();
            ViewBag.Token = SDK.Token;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string email, string birthday, string accountId, string add, string delete)
        {
            bool success = false;
            if (!String.IsNullOrEmpty(add))
            {
                SDK.Pub.PlayersClub.Post(GetPlayerParams(email, birthday, accountId));
                success = true; 
            }
            else if (!String.IsNullOrEmpty(delete))
            {
                success = SDK.Pub.PlayersClub.Delete(accountId).Data.ToString() == "true";
            }

            if (success) {
                return RedirectToAction("Index");
            }
            return View();
        }

        public JsonDictionary GetPlayerParams(string email, string birthday, string accountId)
        {
            JsonDictionary players = new JsonDictionary("{\"players\":[{\"email\":\"" + email + "\",\"birthday\":\"" + birthday + "\",\"accountId\":\"" + accountId + "\",}]}");
            return players;
        }

        public dynamic SDK
        {
            get
            {
                return new PublisherSDK(key, secret);
            }
        }
    }
}
