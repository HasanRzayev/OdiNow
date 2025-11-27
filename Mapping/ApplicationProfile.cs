using AutoMapper;
using OdiNow.Contracts.Responses.Auth;
using OdiNow.Contracts.Responses.Catalog;
using OdiNow.Contracts.Responses.Profile;
using OdiNow.Contracts.Responses.Orders;
using OdiNow.Contracts.Responses.Offers;
using OdiNow.Models;

namespace OdiNow.Mapping;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IsPhoneVerified, opt => opt.MapFrom(src => src.PhoneVerifications.Any(pv => pv.VerifiedAt != null)));

        CreateMap<Category, CategoryResponse>();
        CreateMap<RestaurantAttribute, RestaurantAttributeResponse>();
        CreateMap<Restaurant, RestaurantDetailResponse>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
            .ForMember(dest => dest.MenuItems, opt => opt.Ignore())
            .ForMember(dest => dest.ActiveOffers, opt => opt.Ignore());
        CreateMap<MenuItem, MenuItemResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<UserAddress, UserAddressResponse>();
        CreateMap<Favorite, FavoriteResponse>();
        CreateMap<SearchHistory, SearchHistoryResponse>();
        CreateMap<CouponView, CouponViewResponse>();
        CreateMap<User, ProfileResponse>()
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.FavoritesCount, opt => opt.Ignore());

        CreateMap<Order, OrderResponse>();
        CreateMap<OrderItem, OrderItemResponse>();
        CreateMap<Payment, PaymentResponse>();
        CreateMap<Offer, OfferDetailResponse>()
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : null))
            .ForMember(dest => dest.MenuItemTitle, opt => opt.MapFrom(src => src.MenuItem != null ? src.MenuItem.Title : null));
    }
}

