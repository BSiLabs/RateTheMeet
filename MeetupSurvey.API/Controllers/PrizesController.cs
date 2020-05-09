using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;
using Refit;
using MeetupSurvey.API.Services;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MeetupSurvey.API.ConfigOptions;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrizesController : AuthedControllerBase
    {
        public PrizesController(IOptions<AppSettings> options, MeetupSurveyContext context, IMeetupClient meetupClient, IMemoryCache cache) : base(options, context, meetupClient, cache)
        {

        }

        // GET: api/Prizes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prize>>> GetPrizes()
        {
            return await _context.Prizes.ToListAsync();
        }

        // GET: api/Prizes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prize>> GetPrize(string id)
        {
            var prize = await _context.Prizes.FindAsync(id);

            if (prize == null)
            {
                return NotFound();
            }

            return prize;
        }

        // PUT: api/Prizes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrize(string id, Prize prize)
        {
            if (id != prize.Id)
            {
                return BadRequest();
            }

            _context.Entry(prize).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrizeExists(id))
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

        // POST: api/Prizes
        [HttpPost("{id}/photo")]
        public async Task<ActionResult> UploadPhoto(string id, IFormFile file)
        {
            var user = await this.GetUser();



            var prize = await _context.Prizes.FindAsync(id);



            //Do stuff with stream
            string s = file.FileName;
            string container = user.Value.Id;
            await BlobService.CreateContainer(container);

            if(prize.Photo != null)
                await BlobService.DeleteBlob(container, prize.Photo.Substring(prize.Photo.LastIndexOf('/') + 1));

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var bytes = ms.ToArray();
                var blob = await BlobService.UploadBlob(container, bytes, file.FileName);

                _context.Attach(prize);
                prize.Photo = blob.Uri.AbsoluteUri;
                await _context.SaveChangesAsync();
                return Ok(prize.Photo);
            }
        }

        // POST: api/Prizes
        [HttpPost]
        public async Task<ActionResult<Prize>> PostPrize(Prize prize)
        {
            _context.Prizes.Add(prize);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrize", new { id = prize.Id }, prize);
        }

        // DELETE: api/Prizes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Prize>> DeletePrize(string id)
        {
            var prize = await _context.Prizes.FindAsync(id);
            if (prize == null)
            {
                return NotFound();
            }

            _context.Prizes.Remove(prize);
            await _context.SaveChangesAsync();

            return prize;
        }

        private bool PrizeExists(string id)
        {
            return _context.Prizes.Any(e => e.Id == id);
        }
    }
}
