using System;
using AutoMapper;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using GalleryApplication.ViewModels;

namespace GalleryApplication.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        private readonly IUnitOfWork _unitOfWork;

        public AutoMapperProfiles(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AutoMapperProfiles()
        {
            CreateMap<RegisterViewModel, AppUser>()
                .ForMember(dest => dest.DateOfBirth,
                    opt =>
                        opt.MapFrom(src => Convert.ToDateTime(src.DateOfBirth)))
                .ForMember(dest => dest.Country,
                    opt =>
                        opt.MapFrom(src =>
                            _unitOfWork.CountryRepository.GetCountryByNameAsync(src.Country))); 
        }
    }
}