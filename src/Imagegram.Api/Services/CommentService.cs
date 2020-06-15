using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Exceptions;
using Imagegram.Api.Repositories;
using EntityModels = Imagegram.Api.Models.Entity;
using ProjectionModels = Imagegram.Api.Models.Projection;

namespace Imagegram.Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IPostRepository postRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public CommentService(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }

        public async Task<ProjectionModels.Comment> CreateAsync(EntityModels.Comment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));

            await ValidatePostIdAsync(comment.PostId);

            var createdId = await commentRepository.CreateAsync(comment);
            return await commentRepository.GetAsync(createdId);
        }

        public async Task<ICollection<ProjectionModels.Comment>> GetByPostAsync(Guid postId, int? limit, long? previousCommentCursor)
        {
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