using System;
using Dapper.Contrib.Extensions;

namespace Imagegram.Api.Models.Entity
{
    [Table("Posts")]
    public class Post : ICursoredModel
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public Guid CreatorId { get; set; }

        public DateTime CreatedAt { get; set; }

        [Write(false)]
        public long ItemCursor { get; set; }
    }
}