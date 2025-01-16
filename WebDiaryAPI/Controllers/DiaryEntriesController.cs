using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebDiaryAPI.Data;
using WebDiaryAPI.Models;

namespace WebDiaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaryEntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiaryEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaryEntry>>> GetDiaryEntries()
        {
            return await _context.DiaryEntries.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryEntry>> GetDiaryEntry(int id)
        {
            var diaryEntry = await _context.DiaryEntries.FindAsync(id);

            if (diaryEntry == null)
            {
                return NotFound();
            }
            return diaryEntry;
        }

        [HttpPost]
        public async Task<ActionResult<DiaryEntry>> PostDiaryEntry(DiaryEntry diaryEntry)
        {
            diaryEntry.Id = 0;
            _context.DiaryEntries.Add(diaryEntry);
            await _context.SaveChangesAsync();
            //Location
            var resourceUrl = Url.Action(nameof(DiaryEntry), new { id = diaryEntry.Id });
            return Created(resourceUrl, diaryEntry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiaryEntry(int id, [FromBody] DiaryEntry diaryEntry)
        {
            //The id from Url and id from body doesnt match
            if (id != diaryEntry.Id)
            {
                return BadRequest();
            }
            _context.Entry(diaryEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryEntryExists(id))
                {
                    return BadRequest();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool DiaryEntryExists(int id)
        {
            return _context.DiaryEntries.Any(e => e.Id == id);
        }

    }
}
