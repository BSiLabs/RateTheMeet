using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;
using MeetupSurvey.API.Services;
using MeetupSurvey.DTO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MeetupSurvey.API.ConfigOptions;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountsController : AuthedControllerBase
    {
        public UserAccountsController(IOptions<AppSettings> options, MeetupSurveyContext context, IMeetupClient meetupClient, IMemoryCache cache) : base(options, context, meetupClient, cache)
        {
        }

        [HttpPost("ClearCache")]
        public async Task<IActionResult> ClearCache()
        {
            var appUser = await this.GetUser();
            if (appUser.Value == null)
                return appUser.Result;

            this.ClearUserCache();
            return Ok();
        }

        [HttpGet("RefreshProfile")]
        public async Task<IActionResult> RefreshProfile()
        {
            var appUser = await this.GetUser();
            if (appUser.Value == null)
                return appUser.Result;

            var user = await meetupClient.GetUser(Token);

            var authUser = new AuthUser()
            {
                Id = appUser.Value.Id,
                Name = user.name,
                Email = user.email,
                Photo = user.photo?.photo_link,
                MeetupUserId = user.id
            };

            return Ok(authUser);
        }

        // GET: api/UserAccounts
        [HttpGet]
        public IEnumerable<UserAccount> GetUserAccount()
        {
            return _context.UserAccounts;
        }

        // GET: api/UserAccounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAccount([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);

            if (userAccount == null)
            {
                return NotFound();
            }

            return Ok(userAccount);
        }

        // PUT: api/UserAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAccount([FromRoute] string id, [FromBody] UserAccount userAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(userAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserAccounts
        [HttpPost]
        public async Task<IActionResult> PostUserAccount([FromBody] UserAccount userAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserAccounts.Add(userAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAccount", new { id = userAccount.Id }, userAccount);
        }

        // DELETE: api/UserAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAccount([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }

            _context.UserAccounts.Remove(userAccount);
            await _context.SaveChangesAsync();

            return Ok(userAccount);
        }

        private bool UserAccountExists(string id)
        {
            return _context.UserAccounts.Any(e => e.Id == id);
        }
    }
}