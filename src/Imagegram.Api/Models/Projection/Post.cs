using System;
using System.Collections.Generic;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Models.Projection
{
    public class Post
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public ProjectionModels.Account Creator { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<ProjectionModels.Comment> Comments { get; set; }

        public long VersionCursor { get; set; }
    }
}