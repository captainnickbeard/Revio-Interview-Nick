using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Globalization;

namespace Revio_Interview___Nick_S.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
      //  static string url = "http://api.wunderground.com/api/1cff1d23f9e63f4e/conditions/q/";

        public ActionResult Index(String city="", String state="")
        {
            ViewBag.Title = "something";
            //make call to wunderground
            //send date to model
            //update Viewbag
          
            if (state!="" && city !="")
                getWeatherAsync(city, state);
            return View();
        }
        private void getWeatherAsync(String city, String state)
        {

            string url = "http://api.wunderground.com/api/1cff1d23f9e63f4e/conditions/q/" + state.Replace(" ", "_") + "/" + city + ".json";
            HttpWebRequest wc = (HttpWebRequest)WebRequest.Create(url);
            wc.Method = "GET";
            HttpWebResponse wb = (HttpWebResponse)wc.GetResponse();

            if (wb.StatusCode == HttpStatusCode.NotFound)
                ViewBag.error = city + ", " + state + " cannot be found.";
            else if (wb.StatusCode >= HttpStatusCode.InternalServerError)
                ViewBag.error = "Oops. There was an error! It was Wundergrounds fault.";
            else if (wb.StatusCode >= HttpStatusCode.BadRequest && wb.StatusCode != HttpStatusCode.NotFound)
            {
                ViewBag.error = "It appears there was some form of error ¯\\_(ツ)_/¯";
            }
            else
            {
                try
                {
                    string responseText;
                    using (var reader = new System.IO.StreamReader(wb.GetResponseStream(), ASCIIEncoding.ASCII))
                    {
                        responseText = reader.ReadToEnd();
                    }

                    JObject test = JObject.Parse(responseText);
                    TextInfo textinfo = new CultureInfo("en-US", false).TextInfo;
                    ViewBag.City = textinfo.ToTitleCase(city);
                    ViewBag.State = state;
                    ViewBag.date = test.SelectToken("current_observation.observation_time").ToString();
                    ViewBag.windspeed = test.SelectToken("current_observation.wind_string").ToString();
                    ViewBag.condition = test.SelectToken("current_observation.weather").ToString();
                    ViewBag.icon = test.SelectToken("current_observation.icon_url").ToString();
                }catch(Exception e)
                {
                    ViewBag.error = "Cannot find weather information for " + city + ", " + state + " ¯\\_(ツ)_/¯";
                }
            }

    
        }

    }
}