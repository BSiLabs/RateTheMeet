using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;
using MeetupSurvey.API.Services;
using Microsoft.Extensions.Caching.Memory;
using MeetupSurvey.DTO;
using Microsoft.Extensions.Options;
using MeetupSurvey.API.ConfigOptions;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : AuthedControllerBase
    {
        public QuestionsController(IOptions<AppSettings> options, MeetupSurveyContext context, IMeetupClient meetupClient, IMemoryCache cache) : base(options, context, meetupClient, cache)
        {
        }


        // GET: api/Questions
        [HttpGet("previous")]
        public async Task<ActionResult<List<QuestionDTO>>> GetPreviousQuestions()
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            var questions = await _context.Questions.Join(_context.Surveys, x => x.SurveyId, y => y.Id, (x, y) => new { Question = x, Survey = y })
                                        .Where(x => x.Survey.UserAccountId == user.Value.Id)
                                        .Select(x => x.Question)
                                        .ToListAsync();

            var groupedQuestions = questions.GroupBy(x => x.Name).ToList();

            var orderedQuestions = groupedQuestions.OrderByDescending(x => x.Count()).ToList();


            List<QuestionDTO> returnList = new List<QuestionDTO>();
            foreach (var group in orderedQuestions)
            {
                returnList.Add(ToDTO(group.FirstOrDefault()));
            }

            return returnList;
        }

        // GET: api/Questions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        // PUT: api/Questions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion([FromRoute] string id, [FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != question.Id)
            {
                return BadRequest();
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
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

        // POST: api/Questions
        [HttpPost]
        public async Task<IActionResult> PostQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestion", new { id = question.Id }, question);
        }

        // DELETE: api/Questions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(question);
        }

        private bool QuestionExists(string id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}