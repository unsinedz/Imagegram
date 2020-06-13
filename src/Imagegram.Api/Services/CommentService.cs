using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Imagegram.Api.Exceptions;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IPostRepository postRepository;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository)
        {
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
        }

        public async Task<EntityModels.Comment> CreateAsync(EntityModels.Comment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));

            await ValidatePostIdAsync(comment.PostId);

            var createdId = await commentRepository.CreateAsync(comment);
            return await commentRepository.GetAsync(createdId);
        }

        public async Task<ICollection<EntityModels.Comment>> GetAllAsync(Guid postId, int? limit, long? previousCommentCursor)
        {
            await ValidatePostIdAsync(postId);

            return await commentRepository.GetByPostAsync(postId, limit, previousCommentCursor);
        }

        private async Task ValidatePostIdAsync(Guid postId)
        {
            var existingPost = await postRepository.GetAsync(postId);
            if (existingPost == null)
                throw new NotFoundException("Post was not found.");
        }
    }
}