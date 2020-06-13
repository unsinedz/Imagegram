using System;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Models.Projection
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public ProjectionModels.Account Creator { get; set; }

        public DateTime CreatedAt { get; set; }

        public long VersionCursor { get; set; }
    }
}