using AutoMapper;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Article;
using System;

namespace EasyUiBackend.Infrastructure.Mappings
{
    public class ArticleMapping : Profile
    {
        public ArticleMapping()
        {
            // Article -> ArticleResponse
            CreateMap<Article, ArticleResponse>();

            // Article -> ArticleSummaryResponse
            CreateMap<Article, ArticleSummaryResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName));

            // CreateArticleRequest -> Article
            CreateMap<CreateArticleRequest, Article>()
                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedAt.HasValue ? src.PublishedAt.Value : DateTime.UtcNow))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Description) ? src.Description : src.ShortDescription));

            // UpdateArticleRequest -> Article
            CreateMap<UpdateArticleRequest, Article>()
                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedAt.HasValue ? src.PublishedAt.Value : DateTime.UtcNow))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Description) ? src.Description : src.ShortDescription));
        }
    }
} 