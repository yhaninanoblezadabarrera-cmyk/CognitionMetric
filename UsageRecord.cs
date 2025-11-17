using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CognitoMetric.Models
{
    public class UsageRecord
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; }          // Identity user Id (string)

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public string AppName { get; set; }         // e.g., "youtube.com", "instagram.com"

        public DateTime StartAt { get; set; }       // UTC

        public int DurationSeconds { get; set; }

        public int AttentionScore { get; set; }     // 0-100 optional

        public string ContentType { get; set; }     // Optional: "Education", "Entertainment"
    }
}