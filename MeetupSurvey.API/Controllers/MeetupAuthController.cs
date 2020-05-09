using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetupSurvey.API.ConfigOptions;
using MeetupSurvey.API.Models;
using MeetupSurvey.API.Services;
using MeetupSurvey.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetupAuthController : AuthedControllerBase
    {
        private readonly IMeetupAuth meetupAuth;

        public MeetupAuthController(IOptions<AppSettings> options, IMeetupAuth meetupAuth, MeetupSurveyContext context, IMeetupClient meetupClient, IMemoryCache cache) : base(options, context, meetupClient, cache)
        {
            this.meetupAuth = meetupAuth;
        }
        

        // POST: api/MeetupAuth
        [HttpPost("Authorize")]
        public async Task<IActionResult> Post([FromBody] MeetupAccessArgs args)
        {
            args.client_secret = Constants.MeetupClientSecret;
            var token = await meetupAuth.Access(args.client_id, args.client_secret, args.grant_type, args.redirect_uri, args.code);

            var user = await meetupClient.GetUser(Helpers.FormatToken(token.access_token));

            var appUser = this._context.UserAccounts.FirstOrDefault(x => x.MeetupUserId == user.id);
            if(appUser == null)
            {
                try
                {
                    appUser = new UserAccount() { Email = user.email, MeetupUserId = user.id, Name = user.name };
                    this._context.UserAccounts.Add(appUser);
                    await this._context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    //Race condition, user already exists, get the user
                    appUser = this._context.UserAccounts.FirstOrDefault(x => x.MeetupUserId == user.id);
                }
            }

            var authUser = new AuthUser()
            {
                Id = appUser.Id,
                Name = user.name,
                Email = user.email,
                Photo = user.photo?.photo_link,
                MeetupUserId = user.id,
                access_token = token.access_token,
                refresh_token = token.refresh_token,
                token_type = token.token_type,
                Expiry = DateTime.UtcNow.AddSeconds(token.expires_in)
            };
            
            return Ok(authUser);
        }

        // POST: api/MeetupAuth
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> Post([FromBody] MeetupRefreshArgs args)
        {
            args.client_secret = Constants.MeetupClientSecret;
            var token = await meetupAuth.Access(args.client_id, args.client_secret, args.grant_type, args.refresh_token);

            var response = new AuthToken()
            {
                access_token = token.access_token,
                 refresh_token = token.refresh_token,
                  token_type = token.token_type,
                  expiry = DateTime.UtcNow.AddSeconds(token.expires_in)
            };

            return Ok(response);
        }
        
        
    }
}