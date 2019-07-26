using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlugsController : ControllerBase
    {
        private readonly string plugNotConnected = "plug not connected";
        private readonly string valueNotRecognized = "value not recognized";

        // GET: api/Plugs/5
        [HttpGet("{mac}", Name = "GetPlug")]
        public ActionResult<Plug> Get(string mac)
        {
            return NotFound();
            Newtonsoft.Json.JsonConvert.SerializeObject(DatabaseManager.GetInstance().GetPlug(mac));
        }

        [HttpPost("{mac}/status/", Name = "PostPlug")]
        public string ChangeOn(string mac, string property,[FromQuery] string op)
        {
        }
        // POST: api/Plugs
        [HttpPost("{mac}/status/{value}", Name = "PostPlug")]
        public string Post(string mac, string property, string value)
        {
            Plug plug = DatabaseManager.GetInstance().GetPlug(mac);
            if (plug == null) return "no such plug";

            try
            {
                if (property.ToLower().Equals("ison"))
                {
                    if (value.ToLower().Equals("true"))
                    {
                        try
                        {
                            plug.TurnOn();
                            return "turned on";
                        }
                        catch (PlugNotConnectedException)
                        {
                            return plugNotConnected;
                        }
                    }
                    else if (value.ToLower().Equals("false"))
                    {
                        try
                        {
                            plug.TurnOff();
                            return "turned off";
                        }
                        catch (PlugNotConnectedException)
                        {
                            return plugNotConnected;
                        }
                    }
                }
                else if (property.ToLower().Equals("approved"))
                {
                    if (value.ToLower().Equals("true"))
                    {
                        plug.Approved = true;
                    }
                    else if (value.ToLower().Equals("false"))
                    {
                        // DatabaseManager.GetInstance().Context.Plugs.Remove(plug); prod - only
                        plug.Approved = false; // debug only
                    }
                    else return valueNotRecognized;

                    return "ok";
                }
                else if (property.ToLower().Equals("priority"))
                {
                    if (value.ToLower().Equals("essential")) plug.Priority = Plug.Priorities.ESSENTIAL;
                    else if (value.ToLower().Equals("nonessential")) plug.Priority = Plug.Priorities.NONESSENTIAL;
                    else if (value.ToLower().Equals("irrelevant")) plug.Priority = Plug.Priorities.IRRELEVANT;
                    else return valueNotRecognized;

                    return "ok";
                }
                else if (property.ToLower().Equals("nickname"))
                {
                    plug.Nickname = value;
                    return "ok";
                }
                return "property not recognized";
            }
            finally
            {
                DatabaseManager.GetInstance().Context.SaveChanges();
            }
        }
    }
}
