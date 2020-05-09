using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetupSurvey.API.Models;
using MeetupSurvey.API.Services;
using MeetupSurvey.DTO;
using MeetupSurvey.Services.PushNotifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        readonly IPushNotifications push;
        readonly MeetupSurveyContext context;
        public PushController(MeetupSurveyContext context, IPushNotifications push)
        {
            this.context = context;
            this.push = push;
        }


        [HttpPost("Registration")]
        public async Task<ActionResult> Registration([FromBody] PushRegisterArgs args)
        {
            var device = new Models.PushRegistration
            {
                Email = args.Email,
                Jwt = args.Jwt,
                InstallId = args.InstallId,
                IsAndroid = args.IsAndroid
            };

            if (args.IsUnRegister)
            {
                await Remove(device);
            }
            else
            {
                await Remove(device);
                await Add(device);
            }

            return this.Ok();
        }

        private async Task Remove(Models.PushRegistration device)
        {
            var found = context.PushRegistrations
                    .Where(x => x.Email.Equals(device.Email, StringComparison.InvariantCultureIgnoreCase) &&
                    x.InstallId.Equals(device.InstallId) &&
                    x.IsAndroid == device.IsAndroid).ToList();

            if (found.Any())
            {
                foreach (var match in found)
                {
                    context.PushRegistrations.Remove(match);
                }
                await context.SaveChangesAsync();
            }
        }

        private async Task Add(Models.PushRegistration device)
        {
            context.PushRegistrations.Add(device);
            await context.SaveChangesAsync();
        }


        //[HttpPost("sendall")]
        //public async Task<ActionResult> SendAll([FromBody] Notification notification)
        //{
        //    await this.push.SendAll(notification.Title, notification.Message);
        //    return this.Ok();
        //}


        //[HttpPost("send")]
        //public async Task<ActionResult> Send([FromBody] Notification notification)
        //{
        //    await this.push.Send(notification.Email, notification.Title, notification.Message, notification.CustomData);
        //    return this.Ok();
        //}
    }
}