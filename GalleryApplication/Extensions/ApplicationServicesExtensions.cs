﻿using CloudinaryDotNet;
using GalleryApplication.Data;
using GalleryApplication.Helpers;
using GalleryApplication.Interfaces;
using GalleryApplication.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GalleryApplication.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            string connection = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));
            
            return services;
        }
    }
}