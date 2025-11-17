using CognitoMetric.Data;
using CognitoMetric.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CognitoMetric.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ApiController(AppDbContext db) { _db = db; }

        [HttpPost("record")]
        [Authorize] // require auth tokens / cookie
        public async Task<IActionResult> Record([FromBody] UsageRecordDto dto)
        {
            if (dto == null) return BadRequest();

            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var record = new UsageRecord
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AppName = dto.AppName,
                StartAt = dto.StartAt.ToUniversalTime(),
                DurationSeconds = dto.DurationSeconds,
                AttentionScore = dto.AttentionScore,
                ContentType = dto.ContentType
            };

            _db.UsageRecords.Add(record);
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpGet("me/usage")]
        [Authorize]
        public async Task<IActionResult> MyUsage()
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var usage = await _db.UsageRecords
                                 .Where(u => u.UserId == userId)
                                 .OrderByDescending(u => u.StartAt)
                                 .ToListAsync();

            return Ok(usage);
        }
    }

    public class UsageRecordDto
    {
        public string AppName { get; set; }
        public DateTime StartAt { get; set; }
        public int DurationSeconds { get; set; }
        public int AttentionScore { get; set; }
        public string ContentType { get; set; }
    }
} 