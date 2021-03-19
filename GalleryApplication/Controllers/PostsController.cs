using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using GalleryApplication.Services;
using GalleryApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GalleryApplication.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public PostsController(IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _unitOfWork.PostRepository.GetPostsForUser(User.Identity.Name);
            
            return View(posts);
        }
        
        [HttpGet]
        public IActionResult AddPost()
        {
            AddPostViewModel model = new AddPostViewModel(); 
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddPost(string description, IFormFile file)
        {
            AddPostViewModel model = new AddPostViewModel();
            
            if (file == null)
            {
                model.Error = "Please select image";
                return View();
            }

            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);

            if (user == null) return RedirectToAction("NotFoundResponse", "Error");

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return View(new AddPostViewModel() {Error = "Something went wrong"});
            }

            var post = new Post()
            {
                Description = description,
                PhotoUrl = result.SecureUrl.AbsoluteUri,
                PhotoPublicId = result.PublicId,
                User = user
            };
            _unitOfWork.PostRepository.AddPost(post);

            if (await _unitOfWork.Complete())
            {
                return RedirectToAction("Detail", "Users", new {
                    username = User.Identity.Name
                });
            }
            
            return RedirectToAction("NotFoundResponse", "Error");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            
            _unitOfWork.PostRepository.DeletePost(post);

            await _unitOfWork.Complete();

            return RedirectToAction("Detail", "Users", new {username = User.Identity.Name});
        }

        [HttpGet("[controller]/Detail/{postId}")]
        public async Task<IActionResult> Detail(int postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            
            if (post == null) return RedirectToAction("NotFoundResponse", "Error");
            
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(int postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);

            if (post.Likes.Select(x => x.LikeSenderId).Contains(user.Id)) 
                return RedirectToAction("Detail", "Posts", new {postId});

            Like like = new Like()
            {
                Post = post,
                LikeSender = user,
            };
            
            _unitOfWork.LikeRepository.AddLike(like);

            await _unitOfWork.Complete();
            
            return RedirectToAction("Detail", "Posts", new {postId});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLike(int postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);
            

            var like = post.Likes.SingleOrDefault(x => x.LikeSenderId == user.Id);
            
            if(like == null) return RedirectToAction("Detail", "Posts", new {postId});
            
            _unitOfWork.LikeRepository.DeleteLike(like);

            await _unitOfWork.Complete();
            
            return RedirectToAction("Detail", "Posts", new {postId});
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string comment)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);

            var cmt = new Comment()
            {
                Content = comment,
                Post = post,
                Sender = user,
            };
            
            _unitOfWork.CommentRepository.AddComment(cmt);

            await _unitOfWork.Complete();
            
            return RedirectToAction("Detail", "Posts", new {postId});
        }

        [Authorize(Roles = "moderator, admin")]
        [HttpPost]
        public async Task<IActionResult> SavePost(int postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            
            post.HasComplaint = false;

            await _unitOfWork.Complete();
            
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost("/Posts/Detail/{postId}")]
        public async Task<IActionResult> ComplainPost(int postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);

            post.HasComplaint = true;

            await _unitOfWork.Complete();

            return RedirectToAction("Detail", "Posts", new {post.PostId});
        }
    }
}