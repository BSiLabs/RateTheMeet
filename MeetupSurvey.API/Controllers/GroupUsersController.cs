using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupUsersController : ControllerBase
    {
        private readonly MeetupSurveyContext _context;

        public GroupUsersController(MeetupSurveyContext context)
        {
            _context = context;
        }

        // GET: api/GroupUsers
        [HttpGet]
        public IEnumerable<GroupUser> GetGroupUser()
        {
            return _context.GroupUsers;
        }

        // GET: api/GroupUsers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var groupUser = await _context.GroupUsers.FindAsync(id);

            if (groupUser == null)
            {
                return NotFound();
            }

            return Ok(groupUser);
        }

        // PUT: api/GroupUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroupUser([FromRoute] string id, [FromBody] GroupUser groupUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != groupUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(groupUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupUserExists(id))
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

        // POST: api/GroupUsers
        [HttpPost]
        public async Task<IActionResult> PostGroupUser([FromBody] GroupUser groupUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.GroupUsers.Add(groupUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroupUser", new { id = groupUser.Id }, groupUser);
        }

        // DELETE: api/GroupUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var groupUser = await _context.GroupUsers.FindAsync(id);
            if (groupUser == null)
            {
                return NotFound();
            }

            _context.GroupUsers.Remove(groupUser);
            await _context.SaveChangesAsync();

            return Ok(groupUser);
        }

        private bool GroupUserExists(string id)
        {
            return _context.GroupUsers.Any(e => e.Id == id);
        }
    }
}