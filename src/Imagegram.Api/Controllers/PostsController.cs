using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Imagegram.Api.Extensions;
using Imagegram.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiModels = Imagegram.Api.Models.Api;
using EntityModels = Imagegram.Api.Models.Entity;

namespace Imagegram.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IPostService postService;
        private readonly IImageService imageService;

        public PostsController(
            IMapper mapper,
            IPostService postService,
            IImageService imageService)
        {
            this.mapper = mapper;
            this.postService = postService;
            this.imageService = imageService;
        }

        [HttpPost]
        public async Task<ApiModels.Post> PostAsync([Required, FromForm] ApiModels.PostInput postInput)
        {
            var postToCreate = mapper.Map<EntityModels.Post>(postInput);
            postToCreate.CreatorId = User.GetId();

            var imageDescriptor = await imageService.GetImageDescriptorAsync(postInput.Image);
            var post = await postService.CreateAsync(postToCreate, imageDescriptor);
            return mapper.Map<ApiModels.Post>(post);
        }

        [HttpGet]
        public async Task<ICollection<ApiModels.Post>> GetAsync(int? limit, long? previousPostCursor)
        {
            var posts = await postService.GetLatestAsync(limit, previousPostCursor);
            return posts.Select(x => mapper.Map<ApiModels.Post>(x)).ToList();
        }
    }
}