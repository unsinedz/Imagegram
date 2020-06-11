using System;
using Dapper.Contrib.Extensions;

namespace Imagegram.Api.Models.Entity
{
    [Table("Posts")]
    public class Post
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public Guid CreatorId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}