using System;
using System.Collections.Generic;
using ApiModels = Imagegram.Api.Models.Api;

namespace Imagegram.Api.Models.Api
{
    public class Post
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public ApiModels.Account Creator { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<ApiModels.Comment> Comments { get; set; } = new List<ApiModels.Comment>(0);
    }
}