using AutoMapper;
using EcommerceMVC.Areas.Admin.ViewModels;
using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace EcommerceMVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, User>();
            CreateMap<Order, OrderVM>();
            CreateMap<Product, ProductVM>();
            CreateMap<Category, CategoryVM>();
            CreateMap<Supplier, SupplierVM>();
        }
    }
}
