using IssueCRUD.Data;
using IssueCRUD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IssueDbContext _context;

        public IssueController(IssueDbContext context)
        {
            _context = context;
        }

        //get all issues api/issue
        [HttpGet]
        public async Task<IEnumerable<Issue>> Get()
        {
            return await _context.Issues.ToListAsync();
        }

        //get issue by id in parameters
        [HttpGet("{id}")]
        //for swagger doc we use the ProducesResponseType
        [ProducesResponseType(typeof(Issue), StatusCodes.Status200OK)] //here we use typeoft cuz we are returing an issue
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            return issue == null ? NotFound() : Ok(issue); //not found genreate status code 404, ok generates 200
        }

        //create a new issue
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create( Issue issue )
        {
            await _context.Issues.AddAsync(issue);
            await _context.SaveChangesAsync();

            //helper that returns a response with a status code 201 and the location in the headers of the response to our created source (http://localhost:7061/api/issue/id
            return CreatedAtAction(nameof(GetById), new {id = issue.Id}, issue); //receives 3 params, action that returns a single issue, the id of the issue as anonymous obj and the issue itself
                        //ActionName,    routeValues(info necessary to generate correct URL),        createdResource
        }

        //update a new issue
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, Issue issue )
        {
            if (id != issue.Id) return BadRequest();
            if (issue == null) return BadRequest();
            var issueToUpdate = await _context.Issues.FindAsync(id);
            if (issueToUpdate == null) return NotFound();

            var local = _context.Set<Issue>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(id));

            // check if local is not null 
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(issue).State = EntityState.Modified; //allows to update register
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //delete an issue
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete (int id)
        {
            var issueToDelete = await _context.Issues.FindAsync(id);
            if (issueToDelete == null) return NotFound();

            _context.Issues.Remove(issueToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
