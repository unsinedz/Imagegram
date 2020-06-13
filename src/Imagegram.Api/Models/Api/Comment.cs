using System;
using ApiModels = Imagegram.Api.Models.Api;

namespace Imagegram.Api.Models.Api
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public ApiModels.Account Creator { get; set; }

        public DateTime CreatedAt { get; set; }

        public long Cursor { get; set; }
    }
}