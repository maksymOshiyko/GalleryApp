using System.Threading.Tasks;
using AutoMapper;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace GalleryApplication.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public UnitOfWork(DataContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        
        public IUserRepository UserRepository => new UserRepository(_context, _mapper, _userManager);
        public ICountryRepository CountryRepository => new CountryRepository(_context);
        public IPostRepository PostRepository => new PostRepository(_context);
        public ILikeRepository LikeRepository => new LikeRepository(_context);
        public ICommentRepository CommentRepository => new CommentRepository(_context);
        
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}