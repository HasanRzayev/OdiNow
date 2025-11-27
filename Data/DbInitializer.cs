using Microsoft.EntityFrameworkCore;
using OdiNow.Models;
using OdiNow.Security;
using System.Linq.Expressions;

namespace OdiNow.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        async Task AddMissingAsync<TEntity>(DbSet<TEntity> dbSet, IEnumerable<TEntity> seeds, Expression<Func<TEntity, Guid>> keySelector)
            where TEntity : class
        {
            var compiledKeySelector = keySelector.Compile();
            var existingKeys = (await dbSet.AsNoTracking().Select(keySelector).ToListAsync()).ToHashSet();
            var missingEntities = seeds
                .Where(entity => !existingKeys.Contains(compiledKeySelector(entity)))
                .ToList();

            if (missingEntities.Count > 0)
            {
                await dbSet.AddRangeAsync(missingEntities);
            }
        }

        // Seed Categories (15 categories)
        var categories = new List<Category>
        {
            new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Fast Food", Slug = "fast-food", Description = "Sürətli və dadlı yeməklər", DisplayOrder = 1, IsActive = true },
            new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Sağlam Seçimlər", Slug = "saglam-secimler", Description = "Sağlam və balanslı yeməklər", DisplayOrder = 2, IsActive = true },
            new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Asiya Yeməkləri", Slug = "asiya-yemekleri", Description = "Çin, Yapon və digər Asiya mətbəxləri", DisplayOrder = 3, IsActive = true },
            new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Milli Yeməklər", Slug = "milli-yemekler", Description = "Azərbaycan milli mətbəxi", DisplayOrder = 4, IsActive = true },
            new Category { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Dessert", Slug = "dessert", Description = "Şirniyyatlar və desertlər", DisplayOrder = 5, IsActive = true },
            new Category { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "İçkilər", Slug = "ickiler", Description = "Soyuq və isti içkilər", DisplayOrder = 6, IsActive = true },
            new Category { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Kabab", Slug = "kabab", Description = "Müxtəlif növ kabablar", DisplayOrder = 7, IsActive = true },
            new Category { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Sushi", Slug = "sushi", Description = "Yapon sushi və rolllar", DisplayOrder = 8, IsActive = true },
            new Category { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Name = "Salatlar", Slug = "salatlar", Description = "Təzə salatlar və qarışıqlar", DisplayOrder = 9, IsActive = true },
            new Category { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Sup", Slug = "sup", Description = "İsti və soyuq suplar", DisplayOrder = 10, IsActive = true },
            new Category { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Qəlyanaltı", Slug = "qelyanalti", Description = "Səhər yeməkləri", DisplayOrder = 11, IsActive = true },
            new Category { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Balıq", Slug = "balig", Description = "Dəniz məhsulları", DisplayOrder = 12, IsActive = true },
            new Category { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Name = "Vegetarian", Slug = "vegetarian", Description = "Bitki əsaslı yeməklər", DisplayOrder = 13, IsActive = true },
            new Category { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Name = "Türk Mətbəxi", Slug = "turk-mətbəxi", Description = "Türkiyə mətbəxi", DisplayOrder = 14, IsActive = true },
            new Category { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Name = "Qəlyanaltı", Slug = "qelyanalti-2", Description = "Səhər yeməkləri və qəlyanaltılar", DisplayOrder = 15, IsActive = true }
        };
        await AddMissingAsync(context.Categories, categories, c => c.Id);

        // Seed Restaurants (per area coverage)
        var restaurants = new List<Restaurant>
        {
            new Restaurant { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Chicky House", Description = "Dadlı toyuq və fast food məhsulları", PhoneNumber = "(055) 568 87 34", Email = "info@chickyhouse.az", AddressLine = "Nizami street", City = "Baku", District = "Nizami", PostalCode = "AZ1000", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(9), ClosingTime = TimeSpan.FromHours(21), AverageRating = 4.91m, TotalReviews = 532, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "KFC", Description = "Dünyaca məşhur toyuq restoranı", PhoneNumber = "(012) 123 45 67", Email = "info@kfc.az", AddressLine = "28 May street", City = "Baku", District = "Nasimi", PostalCode = "AZ1001", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(10), ClosingTime = TimeSpan.FromHours(22), AverageRating = 4.75m, TotalReviews = 890, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Pizza Palace", Description = "İtaliyan pizza və pasta", PhoneNumber = "(012) 234 56 78", Email = "info@pizzapalace.az", AddressLine = "Fountain Square", City = "Baku", District = "Sabail", PostalCode = "AZ1002", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(11), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.60m, TotalReviews = 345, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Name = "Sushi Master", Description = "Yapon sushi və rolllar", PhoneNumber = "(012) 345 67 89", Email = "info@sushimaster.az", AddressLine = "Neftchilar avenue", City = "Baku", District = "Yasamal", PostalCode = "AZ1003", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(12), ClosingTime = TimeSpan.FromHours(23.5), AverageRating = 4.85m, TotalReviews = 678, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Name = "Milli Restoran", Description = "Azərbaycan milli mətbəxi", PhoneNumber = "(012) 456 78 90", Email = "info@milli.az", AddressLine = "Bulvar", City = "Baku", District = "Sabail", PostalCode = "AZ1004", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(10), ClosingTime = TimeSpan.FromHours(22), AverageRating = 4.70m, TotalReviews = 456, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Name = "Burger King", Description = "Amerikan burger restoranı", PhoneNumber = "(012) 567 89 01", Email = "info@burgerking.az", AddressLine = "Port Baku", City = "Baku", District = "Sabail", PostalCode = "AZ1005", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(9), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.55m, TotalReviews = 789, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Name = "Kabab Evi", Description = "Ən yaxşı kabablar", PhoneNumber = "(012) 678 90 12", Email = "info@kababevi.az", AddressLine = "Torgovaya street", City = "Baku", District = "Nizami", PostalCode = "AZ1006", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(11), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.80m, TotalReviews = 623, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("22222222-2222-2222-2222-222222222223"), Name = "Papa Johns", Description = "Pizza və pasta", PhoneNumber = "(012) 789 01 23", Email = "info@papajohns.az", AddressLine = "Ganjlik", City = "Baku", District = "Nasimi", PostalCode = "AZ1007", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(10), ClosingTime = TimeSpan.FromHours(22), AverageRating = 4.65m, TotalReviews = 512, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Name = "Dondurma Evi", Description = "Dondurma və desertlər", PhoneNumber = "(012) 890 12 34", Email = "info@dondurma.az", AddressLine = "28 May", City = "Baku", District = "Nasimi", PostalCode = "AZ1008", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(10), ClosingTime = TimeSpan.FromHours(22), AverageRating = 4.90m, TotalReviews = 345, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("44444444-4444-4444-4444-444444444445"), Name = "Balıq Restoranı", Description = "Təzə dəniz məhsulları", PhoneNumber = "(012) 901 23 45", Email = "info@balig.az", AddressLine = "Bulvar", City = "Baku", District = "Sabail", PostalCode = "AZ1009", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(12), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.75m, TotalReviews = 234, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("55555555-5555-5555-5555-555555555556"), Name = "Vegetarian House", Description = "Bitki əsaslı yeməklər", PhoneNumber = "(012) 012 34 56", Email = "info@vegetarian.az", AddressLine = "Nizami", City = "Baku", District = "Nizami", PostalCode = "AZ1010", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(9), ClosingTime = TimeSpan.FromHours(21), AverageRating = 4.50m, TotalReviews = 189, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("66666666-6666-6666-6666-666666666667"), Name = "Türk Restoranı", Description = "Türkiyə mətbəxi", PhoneNumber = "(012) 123 45 67", Email = "info@turk.az", AddressLine = "Fountain Square", City = "Baku", District = "Sabail", PostalCode = "AZ1011", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(11), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.68m, TotalReviews = 567, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("77777777-7777-7777-7777-777777777778"), Name = "Çin Restoranı", Description = "Çin mətbəxi", PhoneNumber = "(012) 234 56 78", Email = "info@cin.az", AddressLine = "28 May", City = "Baku", District = "Nasimi", PostalCode = "AZ1012", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(11), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.55m, TotalReviews = 412, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("88888888-8888-8888-8888-888888888889"), Name = "Qəlyanaltı Evi", Description = "Səhər yeməkləri", PhoneNumber = "(012) 345 67 89", Email = "info@qelyanalti.az", AddressLine = "Nizami", City = "Baku", District = "Nizami", PostalCode = "AZ1013", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(7), ClosingTime = TimeSpan.FromHours(14), AverageRating = 4.72m, TotalReviews = 298, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("99999999-9999-9999-9999-99999999999a"), Name = "Sup Evi", Description = "Müxtəlif suplar", PhoneNumber = "(012) 456 78 90", Email = "info@sup.az", AddressLine = "Bulvar", City = "Baku", District = "Sabail", PostalCode = "AZ1014", Latitude = 40.4093, Longitude = 49.8671, OpeningTime = TimeSpan.FromHours(10), ClosingTime = TimeSpan.FromHours(22), AverageRating = 4.60m, TotalReviews = 156, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("a1a1a1a1-0001-0001-0001-111111111111"), Name = "Binəqədi Ocakbaşı", Description = "Binəqədi rayonunda kabab və manqal ixtisaslı restoran", PhoneNumber = "(012) 600 11 22", Email = "bineqedi@ocakbasi.az", AddressLine = "Binəqədi avenue 45", City = "Baku", District = "Binəqədi", PostalCode = "AZ1015", Latitude = 40.4381, Longitude = 49.8030, OpeningTime = TimeSpan.FromHours(10), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.66m, TotalReviews = 287, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("b2b2b2b2-0002-0002-0002-222222222222"), Name = "Suraxanı Balıqçı", Description = "Xəzərdən gündəlik təzə balıq və dəniz məhsulları", PhoneNumber = "(012) 610 22 33", Email = "info@suraxanibalıq.az", AddressLine = "Suraxanı seaside 12", City = "Baku", District = "Suraxanı", PostalCode = "AZ1016", Latitude = 40.4004, Longitude = 50.0083, OpeningTime = TimeSpan.FromHours(9), ClosingTime = TimeSpan.FromHours(22), AverageRating = 4.58m, TotalReviews = 198, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("c3c3c3c3-0003-0003-0003-333333333333"), Name = "Xətai Street Food", Description = "Xətai rayonunda modern fast food konsepti", PhoneNumber = "(012) 620 33 44", Email = "xetai@streetfood.az", AddressLine = "Babək prospekti 21", City = "Baku", District = "Xətai", PostalCode = "AZ1017", Latitude = 40.3865, Longitude = 49.8773, OpeningTime = TimeSpan.FromHours(8), ClosingTime = TimeSpan.FromHours(22.5), AverageRating = 4.62m, TotalReviews = 342, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("d4d4d4d4-0004-0004-0004-444444444444"), Name = "Nərimanov Bistro", Description = "Füzyon mətbəxi və sağlam seçimlər", PhoneNumber = "(012) 630 44 55", Email = "narimanov@bistro.az", AddressLine = "Nərimanov parkı 7", City = "Baku", District = "Nərimanov", PostalCode = "AZ1018", Latitude = 40.4116, Longitude = 49.8673, OpeningTime = TimeSpan.FromHours(8.5), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.78m, TotalReviews = 401, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("e5e5e5e5-0005-0005-0005-555555555555"), Name = "Sabunçu Lahmacun", Description = "Təndirdən isti lahmacun və pide çeşidləri", PhoneNumber = "(012) 640 55 66", Email = "sabuncu@lahmacun.az", AddressLine = "Sabunçu stansiyası 3", City = "Baku", District = "Sabunçu", PostalCode = "AZ1019", Latitude = 40.4441, Longitude = 49.9480, OpeningTime = TimeSpan.FromHours(7.5), ClosingTime = TimeSpan.FromHours(21.5), AverageRating = 4.48m, TotalReviews = 154, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("f6f6f6f6-0006-0006-0006-666666666666"), Name = "Qaradağ Çay Evi", Description = "Çay süfrəsi və milli təamlar", PhoneNumber = "(012) 650 66 77", Email = "qaradag@chayevi.az", AddressLine = "Salyan şosesi 8", City = "Baku", District = "Qaradağ", PostalCode = "AZ1020", Latitude = 40.3125, Longitude = 49.7046, OpeningTime = TimeSpan.FromHours(9), ClosingTime = TimeSpan.FromHours(21), AverageRating = 4.40m, TotalReviews = 132, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("12345678-90ab-cdef-1234-567890abcdef"), Name = "Sumqayıt Family House", Description = "Sumqayıtda ailəvi milli yeməklər", PhoneNumber = "(018) 550 77 88", Email = "info@sumqayitfamily.az", AddressLine = "Heydər Əliyev 102", City = "Sumqayit", District = "Sumqayit", PostalCode = "AZ5000", Latitude = 40.5853, Longitude = 49.6310, OpeningTime = TimeSpan.FromHours(9), ClosingTime = TimeSpan.FromHours(22), AverageRating = 4.69m, TotalReviews = 268, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("0fedcba9-8765-4321-0fed-cba987654321"), Name = "Gəncə Garden Kitchen", Description = "Gəncə mətbəxi və bağ konseptli restoran", PhoneNumber = "(022) 255 44 66", Email = "info@gencegarden.az", AddressLine = "Cavad xan küçəsi 14", City = "Gəncə", District = "Gəncə", PostalCode = "AZ2000", Latitude = 40.6828, Longitude = 46.3606, OpeningTime = TimeSpan.FromHours(10), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.73m, TotalReviews = 321, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new Restaurant { Id = Guid.Parse("abcdabcd-abcd-abcd-abcd-abcdabcd0001"), Name = "Downtown Bulbul Cafe", Description = "66 Bülbül prospektində specialty coffee və yüngül yeməklər", PhoneNumber = "(012) 777 12 34", Email = "bulbul@downtowncafe.az", AddressLine = "66 Bülbül Prospekti", City = "Baku", District = "Sabail", PostalCode = "AZ1005", Latitude = 40.38307, Longitude = 49.84098, OpeningTime = TimeSpan.FromHours(8), ClosingTime = TimeSpan.FromHours(23), AverageRating = 4.82m, TotalReviews = 256, IsDeleted = false, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }
        };
        await AddMissingAsync(context.Restaurants, restaurants, r => r.Id);

        // Seed Restaurant Attributes (multiple attributes per restaurant)
        var restaurantAttributes = new List<RestaurantAttribute>();
        var existingAttributeKeys = (await context.RestaurantAttributes
            .AsNoTracking()
            .Select(ra => new { ra.RestaurantId, ra.Key })
            .ToListAsync())
            .Select(ra => (ra.RestaurantId, ra.Key))
            .ToHashSet();

        foreach (var restaurant in restaurants)
        {
            void AddAttribute(string key, string value)
            {
                var signature = (restaurant.Id, key);
                if (existingAttributeKeys.Contains(signature))
                {
                    return;
                }

                restaurantAttributes.Add(new RestaurantAttribute
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurant.Id,
                    Key = key,
                    Value = value
                });

                existingAttributeKeys.Add(signature);
            }

            AddAttribute("Halal", "true");
            AddAttribute("Delivery", "true");

            if (restaurant.Name.Contains("Vegetarian"))
            {
                AddAttribute("Vegetarian", "true");
            }

            if (restaurant.Name.Contains("Pizza") || restaurant.Name.Contains("Palace"))
            {
                AddAttribute("Teras", "true");
            }

            AddAttribute("Family Friendly", "true");
        }
        if (restaurantAttributes.Count > 0)
        {
            await context.RestaurantAttributes.AddRangeAsync(restaurantAttributes);
        }

        // Seed Menu Items (15+ items)
        var menuItems = new List<MenuItem>
        {
            new MenuItem { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), RestaurantId = restaurants[0].Id, CategoryId = categories[0].Id, Title = "Zinger Fiesta Kombo", Description = "Toyuq, kartof, içki və sous ilə", BasePrice = 12.50m, IsAvailable = true, PreparationTimeMinutes = 35, ImageUrl = "https://example.com/zinger-fiesta.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), RestaurantId = restaurants[1].Id, CategoryId = categories[0].Id, Title = "KFC Bucket (3 nəfərlik)", Description = "Toyuq parçaları, kartof və içki", BasePrice = 25.99m, IsAvailable = true, PreparationTimeMinutes = 20, ImageUrl = "https://example.com/kfc-bucket.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), RestaurantId = restaurants[2].Id, CategoryId = categories[0].Id, Title = "Margherita Pizza", Description = "Pomidor, mozzarella və fesilən", BasePrice = 15.50m, IsAvailable = true, PreparationTimeMinutes = 25, ImageUrl = "https://example.com/margherita.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), RestaurantId = restaurants[2].Id, CategoryId = categories[0].Id, Title = "Tomato Cream Penne Pasta", Description = "Penne pasta, pomidor sousu və krem ilə", BasePrice = 18.00m, IsAvailable = true, PreparationTimeMinutes = 30, ImageUrl = "https://example.com/penne-pasta.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[3].Id, CategoryId = categories[7].Id, Title = "California Roll", Description = "Avokado, xiyar və krab ilə", BasePrice = 22.00m, IsAvailable = true, PreparationTimeMinutes = 15, ImageUrl = "https://example.com/california-roll.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[3].Id, CategoryId = categories[7].Id, Title = "Salmon Sushi Set", Description = "10 ədəd salmon sushi", BasePrice = 28.50m, IsAvailable = true, PreparationTimeMinutes = 20, ImageUrl = "https://example.com/salmon-sushi.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[4].Id, CategoryId = categories[3].Id, Title = "Plov", Description = "Ətli plov", BasePrice = 14.00m, IsAvailable = true, PreparationTimeMinutes = 40, ImageUrl = "https://example.com/plov.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[4].Id, CategoryId = categories[3].Id, Title = "Dolma", Description = "Yarpaq dolması", BasePrice = 16.50m, IsAvailable = true, PreparationTimeMinutes = 45, ImageUrl = "https://example.com/dolma.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[5].Id, CategoryId = categories[0].Id, Title = "Whopper Burger", Description = "Toyuq, pomidor, salat və sous", BasePrice = 11.99m, IsAvailable = true, PreparationTimeMinutes = 15, ImageUrl = "https://example.com/whopper.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[6].Id, CategoryId = categories[6].Id, Title = "Lüle Kabab", Description = "Ətli lüle kabab", BasePrice = 13.50m, IsAvailable = true, PreparationTimeMinutes = 25, ImageUrl = "https://example.com/lule-kabab.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[6].Id, CategoryId = categories[6].Id, Title = "Tika Kabab", Description = "Toyuq tika kabab", BasePrice = 12.00m, IsAvailable = true, PreparationTimeMinutes = 30, ImageUrl = "https://example.com/tika-kabab.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[7].Id, CategoryId = categories[0].Id, Title = "Pepperoni Pizza", Description = "Pepperoni və pendir", BasePrice = 17.50m, IsAvailable = true, PreparationTimeMinutes = 25, ImageUrl = "https://example.com/pepperoni.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[8].Id, CategoryId = categories[4].Id, Title = "Çokoladlı Dondurma", Description = "Çokoladlı dondurma", BasePrice = 5.50m, IsAvailable = true, PreparationTimeMinutes = 5, ImageUrl = "https://example.com/chocolate-icecream.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[9].Id, CategoryId = categories[11].Id, Title = "Qızardılmış Balıq", Description = "Təzə balıq qızardılmış", BasePrice = 24.00m, IsAvailable = true, PreparationTimeMinutes = 35, ImageUrl = "https://example.com/fried-fish.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[10].Id, CategoryId = categories[12].Id, Title = "Vegetarian Salat", Description = "Təzə tərəvəz salatı", BasePrice = 8.50m, IsAvailable = true, PreparationTimeMinutes = 10, ImageUrl = "https://example.com/vegetarian-salad.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[11].Id, CategoryId = categories[13].Id, Title = "Döner", Description = "Toyuq döner", BasePrice = 10.50m, IsAvailable = true, PreparationTimeMinutes = 20, ImageUrl = "https://example.com/doner.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[12].Id, CategoryId = categories[2].Id, Title = "Çin Nudulları", Description = "Toyuq ilə çin nudulları", BasePrice = 19.00m, IsAvailable = true, PreparationTimeMinutes = 25, ImageUrl = "https://example.com/chinese-noodles.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[13].Id, CategoryId = categories[10].Id, Title = "Qəlyanaltı Seti", Description = "Yumurta, pendir, bal, kərə yağı", BasePrice = 9.00m, IsAvailable = true, PreparationTimeMinutes = 15, ImageUrl = "https://example.com/breakfast-set.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[14].Id, CategoryId = categories[9].Id, Title = "Toyuq Supu", Description = "Toyuq ilə isti sup", BasePrice = 7.50m, IsAvailable = true, PreparationTimeMinutes = 20, ImageUrl = "https://example.com/chicken-soup.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[15].Id, CategoryId = categories[6].Id, Title = "Binəqədi Qarışıq Kabab", Description = "Lüle, tikə və quzu ətindən qarışıq set", BasePrice = 19.50m, IsAvailable = true, PreparationTimeMinutes = 30, ImageUrl = "https://example.com/bineqedi-kabab.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[16].Id, CategoryId = categories[11].Id, Title = "Xəzər Qızılbalığı", Description = "Kömürdə qızardılmış qızılbalıq və salat", BasePrice = 26.00m, IsAvailable = true, PreparationTimeMinutes = 35, ImageUrl = "https://example.com/suraxani-fish.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[17].Id, CategoryId = categories[0].Id, Title = "Xətai Smash Burger", Description = "İki qat smash burger, kartof və içki", BasePrice = 15.90m, IsAvailable = true, PreparationTimeMinutes = 18, ImageUrl = "https://example.com/xetai-burger.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[18].Id, CategoryId = categories[1].Id, Title = "Nərimanov Quinoa Bowl", Description = "Quinoa, avokado, edamame və sous", BasePrice = 13.75m, IsAvailable = true, PreparationTimeMinutes = 15, ImageUrl = "https://example.com/narimanov-bowl.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[19].Id, CategoryId = categories[3].Id, Title = "Sabunçu Lahmacun Seti", Description = "4 ədəd lahmacun və ayran", BasePrice = 11.00m, IsAvailable = true, PreparationTimeMinutes = 12, ImageUrl = "https://example.com/sabuncu-lahmacun.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[20].Id, CategoryId = categories[5].Id, Title = "Qaradağ Çay Süfrəsi", Description = "Samovar çayı, qutab və mürəbbə", BasePrice = 9.50m, IsAvailable = true, PreparationTimeMinutes = 10, ImageUrl = "https://example.com/qaradag-tea.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[21].Id, CategoryId = categories[3].Id, Title = "Sumqayıt Çoban Salatı", Description = "Təzə tərəvəzlər və pendir", BasePrice = 8.80m, IsAvailable = true, PreparationTimeMinutes = 10, ImageUrl = "https://example.com/sumqayit-salad.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[22].Id, CategoryId = categories[12].Id, Title = "Gəncə Göyərti Plovu", Description = "Vegetarian göyərti plovu və qatıq", BasePrice = 13.20m, IsAvailable = true, PreparationTimeMinutes = 28, ImageUrl = "https://example.com/gence-plov.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new MenuItem { Id = Guid.NewGuid(), RestaurantId = restaurants[23].Id, CategoryId = categories[1].Id, Title = "Bulvar Quinoa Sandwich", Description = "Quinoa, avokado və toyuqdan hazırlanmış sendviç", BasePrice = 12.40m, IsAvailable = true, PreparationTimeMinutes = 12, ImageUrl = "https://example.com/bulbul-sandwich.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }
        };
        await AddMissingAsync(context.MenuItems, menuItems, mi => mi.Id);

        // Seed Offers (15+ offers)
        var offers = new List<Offer>
        {
            new Offer { Id = Guid.NewGuid(), Title = "Zinger Fiesta Endirimi", Description = "Seçilmiş menyuda 20% endirim!", DiscountPercent = 20.00m, StartAt = DateTimeOffset.UtcNow.AddDays(-1), EndAt = DateTimeOffset.UtcNow.AddDays(7), RestaurantId = restaurants[0].Id, MenuItemId = menuItems[0].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "KFC Xüsusi Təklif", Description = "KFC Bucket üçün 25% endirim", DiscountPercent = 25.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(3), RestaurantId = restaurants[1].Id, MenuItemId = menuItems[1].Id, IsPersonalized = true, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Pizza Həftəsi", Description = "Bütün pizzalarda 30% endirim", DiscountPercent = 30.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(5), RestaurantId = restaurants[2].Id, MenuItemId = menuItems[2].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Sushi Endirimi", Description = "Sushi setlərdə 15% endirim", DiscountPercent = 15.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(10), RestaurantId = restaurants[3].Id, MenuItemId = menuItems[4].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Milli Yeməklər Təklifi", Description = "Plov və dolmada 10% endirim", DiscountPercent = 10.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(7), RestaurantId = restaurants[4].Id, MenuItemId = menuItems[6].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Burger Günü", Description = "Bütün burgerlərdə 20% endirim", DiscountPercent = 20.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(1), RestaurantId = restaurants[5].Id, MenuItemId = menuItems[8].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Kabab Xüsusi", Description = "Kabablarda 15% endirim", DiscountPercent = 15.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(6), RestaurantId = restaurants[6].Id, MenuItemId = menuItems[9].Id, IsPersonalized = true, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Papa Johns Təklifi", Description = "Pizzalarda 25% endirim", DiscountPercent = 25.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(4), RestaurantId = restaurants[7].Id, MenuItemId = menuItems[11].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Dondurma Endirimi", Description = "Bütün dondurmalarda 30% endirim", DiscountPercent = 30.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(3), RestaurantId = restaurants[8].Id, MenuItemId = menuItems[12].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Balıq Xüsusi", Description = "Balıq yeməklərində 20% endirim", DiscountPercent = 20.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(5), RestaurantId = restaurants[9].Id, MenuItemId = menuItems[13].Id, IsPersonalized = true, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Vegetarian Təklifi", Description = "Vegetarian yeməklərdə 10% endirim", DiscountPercent = 10.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(7), RestaurantId = restaurants[10].Id, MenuItemId = menuItems[14].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Türk Mətbəxi Endirimi", Description = "Dönerdə 15% endirim", DiscountPercent = 15.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(6), RestaurantId = restaurants[11].Id, MenuItemId = menuItems[15].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Çin Yeməkləri Təklifi", Description = "Çin nudullarında 20% endirim", DiscountPercent = 20.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(4), RestaurantId = restaurants[12].Id, MenuItemId = menuItems[16].Id, IsPersonalized = true, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Qəlyanaltı Xüsusi", Description = "Qəlyanaltı setində 25% endirim", DiscountPercent = 25.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(2), RestaurantId = restaurants[13].Id, MenuItemId = menuItems[17].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Sup Endirimi", Description = "Bütün suplarda 15% endirim", DiscountPercent = 15.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(8), RestaurantId = restaurants[14].Id, MenuItemId = menuItems[18].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Binəqədi Kabab Seti", Description = "Qarışıq kabablarda 18% endirim", DiscountPercent = 18.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(5), RestaurantId = restaurants[15].Id, MenuItemId = menuItems[19].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Suraxanı Balıq Günləri", Description = "Xəzər balıqlarında 22% endirim", DiscountPercent = 22.00m, StartAt = DateTimeOffset.UtcNow.AddDays(-2), EndAt = DateTimeOffset.UtcNow.AddDays(6), RestaurantId = restaurants[16].Id, MenuItemId = menuItems[20].Id, IsPersonalized = true, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Xətai Smash Kampaniyası", Description = "Smash burger menyusunda 15% endirim", DiscountPercent = 15.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(4), RestaurantId = restaurants[17].Id, MenuItemId = menuItems[21].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Nərimanov Sağlamlıq Həftəsi", Description = "Quinoa bowl sifarişlərinə 12% endirim", DiscountPercent = 12.00m, StartAt = DateTimeOffset.UtcNow.AddDays(-1), EndAt = DateTimeOffset.UtcNow.AddDays(3), RestaurantId = restaurants[18].Id, MenuItemId = menuItems[22].Id, IsPersonalized = true, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Sabunçu Lahmacun Endirimi", Description = "Lahmacun setlərində 20% endirim", DiscountPercent = 20.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(2), RestaurantId = restaurants[19].Id, MenuItemId = menuItems[23].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Qaradağ Çay Saatı", Description = "Çay süfrəsində 10% bonus", DiscountPercent = 10.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(10), RestaurantId = restaurants[20].Id, MenuItemId = menuItems[24].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Sumqayıt Ailə Menyusu", Description = "Ailə salatlarında 14% endirim", DiscountPercent = 14.00m, StartAt = DateTimeOffset.UtcNow.AddDays(-3), EndAt = DateTimeOffset.UtcNow.AddDays(5), RestaurantId = restaurants[21].Id, MenuItemId = menuItems[25].Id, IsPersonalized = true, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Gəncə Göyərti Plovu", Description = "Vegetarian plov sifarişlərinə 16% endirim", DiscountPercent = 16.00m, StartAt = DateTimeOffset.UtcNow, EndAt = DateTimeOffset.UtcNow.AddDays(7), RestaurantId = restaurants[22].Id, MenuItemId = menuItems[26].Id, IsPersonalized = false, IsActive = true },
            new Offer { Id = Guid.NewGuid(), Title = "Bulvar Coffee Break", Description = "Downtown Bulbul Cafe menyusunda 15% endirim", DiscountPercent = 15.00m, StartAt = DateTimeOffset.UtcNow.AddHours(-2), EndAt = DateTimeOffset.UtcNow.AddDays(3), RestaurantId = restaurants[23].Id, MenuItemId = menuItems[27].Id, IsPersonalized = false, IsActive = true }
        };
        await AddMissingAsync(context.Offers, offers, o => o.Id);

        if (!await context.Users.AnyAsync())
        {
            // Seed Users (15 users) - Changed from "Roza" to "Test"
            var users = new List<User>
            {
                new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), FirstName = "Test", LastName = "User", Email = "test@test.com", IsEmailVerified = true, PhoneNumber = "501234567", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile1.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Ali", LastName = "Mammadov", Email = "ali@test.com", IsEmailVerified = true, PhoneNumber = "501234568", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile2.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Leyla", LastName = "Aliyeva", Email = "leyla@test.com", IsEmailVerified = true, PhoneNumber = "501234569", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile3.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Ruslan", LastName = "Hasanov", Email = "ruslan@test.com", IsEmailVerified = true, PhoneNumber = "501234570", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile4.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Aysel", LastName = "Quliyeva", Email = "aysel@test.com", IsEmailVerified = true, PhoneNumber = "501234571", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile5.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Elvin", LastName = "Məmmədov", Email = "elvin@test.com", IsEmailVerified = true, PhoneNumber = "501234572", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile6.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Günel", LastName = "Rəhimova", Email = "gunel@test.com", IsEmailVerified = true, PhoneNumber = "501234573", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile7.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Orxan", LastName = "İbrahimov", Email = "orxan@test.com", IsEmailVerified = true, PhoneNumber = "501234574", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile8.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Nərgiz", LastName = "Vəliyeva", Email = "nergiz@test.com", IsEmailVerified = true, PhoneNumber = "501234575", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile9.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Tural", LastName = "Qasımov", Email = "tural@test.com", IsEmailVerified = true, PhoneNumber = "501234576", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile10.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Səbinə", LastName = "Məmmədova", Email = "sebine@test.com", IsEmailVerified = true, PhoneNumber = "501234577", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile11.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Rəşad", LastName = "Hüseynov", Email = "resad@test.com", IsEmailVerified = true, PhoneNumber = "501234578", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile12.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Gülnar", LastName = "Əliyeva", Email = "gulnar@test.com", IsEmailVerified = true, PhoneNumber = "501234579", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile13.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Vüsal", LastName = "Bayramov", Email = "vusal@test.com", IsEmailVerified = true, PhoneNumber = "501234580", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile14.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
                new User { Id = Guid.NewGuid(), FirstName = "Aygün", LastName = "Məlikova", Email = "aygun@test.com", IsEmailVerified = true, PhoneNumber = "501234581", PhoneCountryCode = "+994", PasswordEmb5 = passwordHasher.Hash("Test1234!"), IsDeleted = false, ProfilePhotoUrl = "https://example.com/profile15.jpg", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }
            };
            await context.Users.AddRangeAsync(users);

            // Seed User Addresses (at least one per user)
            var userAddresses = new List<UserAddress>();
            var districts = new[] { "Nizami", "Nasimi", "Sabail", "Yasamal", "Binəqədi", "Suraxanı", "Xətai", "Nəsimi" };
            var cities = new[] { "Baku", "Sumqayit", "Gəncə" };
            for (int i = 0; i < users.Count; i++)
            {
                userAddresses.Add(new UserAddress
                {
                    Id = Guid.NewGuid(),
                    UserId = users[i].Id,
                    Label = i == 0 ? "Ev" : (i % 2 == 0 ? "İş" : "Ev"),
                    Line1 = $"{districts[i % districts.Length]} street {100 + i}",
                    Line2 = i % 3 == 0 ? $"Apt {10 + i}" : null,
                    City = cities[i % cities.Length],
                    District = districts[i % districts.Length],
                    PostalCode = $"AZ{1000 + i}",
                    Latitude = 40.4093 + (i * 0.01),
                    Longitude = 49.8671 + (i * 0.01),
                    IsDefault = i == 0,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }
            await context.UserAddresses.AddRangeAsync(userAddresses);

            // Seed User Settings (one per user)
            var userSettings = new List<UserSetting>();
            for (int i = 0; i < users.Count; i++)
            {
                userSettings.Add(new UserSetting
                {
                    Id = Guid.NewGuid(),
                    UserId = users[i].Id,
                    LanguageCode = i % 2 == 0 ? "az" : "en",
                    DefaultAddressId = userAddresses[i].Id,
                    ReceivePushNotifications = true,
                    ReceiveEmailNotifications = i % 3 != 0,
                    MarketingOptIn = i % 4 == 0,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                });
            }
            await context.UserSettings.AddRangeAsync(userSettings);

            // Seed Favorites (15+ favorites)
            var favorites = new List<Favorite>();
            var random = new Random();
            for (int i = 0; i < 20; i++)
            {
                var user = users[random.Next(users.Count)];
                var favoriteType = i % 2 == 0 ? FavoriteType.Restaurant : FavoriteType.MenuItem;
                var targetId = favoriteType == FavoriteType.Restaurant
                    ? restaurants[random.Next(restaurants.Count)].Id
                    : menuItems[random.Next(menuItems.Count)].Id;

                favorites.Add(new Favorite
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    FavoriteType = favoriteType,
                    TargetId = targetId,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-random.Next(30))
                });
            }
            await context.Favorites.AddRangeAsync(favorites);

            // Seed Search History (15+ searches)
            var searchHistories = new List<SearchHistory>();
            var searchQueries = new[] { "KFC Zinger menu", "Pizza", "Sushi", "Kabab", "Plov", "Burger", "Dondurma", "Balıq", "Salat", "Sup", "Döner", "Çin yeməkləri", "Qəlyanaltı", "Vegetarian", "Dessert" };
            for (int i = 0; i < 20; i++)
            {
                searchHistories.Add(new SearchHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = users[random.Next(users.Count)].Id,
                    Query = searchQueries[random.Next(searchQueries.Length)],
                    CreatedAt = DateTimeOffset.UtcNow.AddHours(-random.Next(168)) // Last 7 days
                });
            }
            await context.SearchHistories.AddRangeAsync(searchHistories);

            // Seed Reviews (15+ reviews)
            var reviews = new List<Review>();
            var reviewComments = new[]
            {
                "Amazing! The room is good than the picture. Thanks for amazing experience!",
                "The service is on point, and I really like the facilities. Good job!",
                "Çox dadlı və sürətli xidmət. Tövsiyə edirəm!",
                "Yaxşı keyfiyyət, lakin qiymət bir az bahadır.",
                "Əla xidmət və dadlı yemək. Yenidən gələcəyəm!",
                "Çox gözəl mühit və xidmət. Təşəkkürlər!",
                "Yemək çox dadlı idi, amma gözləmə müddəti uzun oldu.",
                "Əla restoran! Çox məmnun qaldım.",
                "Yaxşı keyfiyyət və sürətli çatdırılma.",
                "Çox gözəl məkan və xidmət. Tövsiyə edirəm!",
                "Yemək çox dadlı idi, amma qiymət bir az yüksəkdir.",
                "Əla xidmət və keyfiyyət. Yenidən gələcəyəm!",
                "Çox gözəl mühit və dadlı yemək.",
                "Yaxşı restoran, amma bəzi yeməklər çox bahadır.",
                "Əla keyfiyyət və sürətli xidmət. Təşəkkürlər!"
            };
            for (int i = 0; i < 20; i++)
            {
                reviews.Add(new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = users[random.Next(users.Count)].Id,
                    RestaurantId = restaurants[random.Next(restaurants.Count)].Id,
                    Rating = random.Next(3, 6), // 3-5 stars
                    Comment = reviewComments[random.Next(reviewComments.Length)],
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-random.Next(60))
                });
            }
            await context.Reviews.AddRangeAsync(reviews);

            // Seed Coupon Views (15+ views)
            var couponViews = new List<CouponView>();
            for (int i = 0; i < 20; i++)
            {
                couponViews.Add(new CouponView
                {
                    Id = Guid.NewGuid(),
                    UserId = users[random.Next(users.Count)].Id,
                    OfferId = offers[random.Next(offers.Count)].Id,
                    ViewedAt = DateTimeOffset.UtcNow.AddHours(-random.Next(168))
                });
            }
            await context.CouponViews.AddRangeAsync(couponViews);
        }

        await context.SaveChangesAsync();
    }
}
