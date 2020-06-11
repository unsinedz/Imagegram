using System;

namespace Imagegram.Api.Models.Entity
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid CreatorId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}