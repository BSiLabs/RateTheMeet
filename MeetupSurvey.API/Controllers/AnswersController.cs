using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;
using MeetupSurvey.DTO;
using MeetupSurvey.API.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MeetupSurvey.API.ConfigOptions;

namespace MeetupSurvey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : AuthedControllerBase
    {
        public AnswersController(IOptions<AppSettings> options, MeetupSurveyContext context, IMeetupClient meetupClient, IMemoryCache cache) : base(options, context, meetupClient, cache)
        {
        }

        // GET: api/Answers
        [HttpGet]
        public IEnumerable<Answer> GetAnswers()
        {
            return _context.Answers;
        }

        // GET: api/Answers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswer([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var answer = await _context.Answers.FindAsync(id);

            if (answer == null)
            {
                return NotFound();
            }

            return Ok(answer);
        }

        // PUT: api/Answers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnswer([FromRoute] string id, [FromBody] Answer answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != answer.Id)
            {
                return BadRequest();
            }

            _context.Entry(answer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists(id))
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

        // POST: api/Answers
        [HttpPost]
        public async Task<IActionResult> PostAnswer([FromBody] Answer answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnswer", new { id = answer.Id }, answer);
        }

        // POST: api/Answers
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAnswers([FromBody] List<AnswerDTO> answers)
        {
            var user = await this.GetUser();
            if (user.Value == null)
                return user.Result;

            //Check if user has already answered this survey
            var questionIds = _context.Questions.Where(x => answers.Select(y => y.QuestionId).Contains(x.Id)).Select(x => x.Id).ToList();
            var existingAnswers = _context.Answers.Where(x => x.UserAccountId == user.Value.Id && questionIds.Contains(x.QuestionId));

            if (!ModelState.IsValid || existingAnswers.Any())
            {
                return BadRequest(ModelState);
            }

            foreach(var answerDTO in answers)
            {
                Answer answerModel = null;
                if (answerDTO.Id != null)
                    answerModel = await _context.Answers.FindAsync(answerDTO.Id);
                
                if(answerModel == null)
                {
                    answerModel = new Answer()
                    {
                        QuestionId = answerDTO.QuestionId,
                        Rating = answerDTO.Rating,
                        Comment = answerDTO.Comment,
                        UserAccountId = user.Value.Id
                    };
                    _context.Answers.Add(answerModel);
                    answerDTO.Id = answerModel.Id;
                }
                else
                {
                    _context.Attach(answerModel);
                    answerModel.Rating = answerDTO.Rating;
                    answerModel.Comment = answerDTO.Comment;
                }
            }
            
            await _context.SaveChangesAsync();

            return Ok(answers);
        }

        // DELETE: api/Answers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return Ok(answer);
        }

        private bool AnswerExists(string id)
        {
            return _context.Answers.Any(e => e.Id == id);
        }
    }
}