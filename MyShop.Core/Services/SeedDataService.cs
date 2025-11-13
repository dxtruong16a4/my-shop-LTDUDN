using MyShop.Core.Entities;
using MyShop.Core.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace MyShop.Core.Services
{
    public interface ISeedDataService
    {
        Task SeedAsync(AppDbContext context);
    }

    public class SeedDataService : ISeedDataService
    {
        private readonly ILogger<SeedDataService> _logger;

        public SeedDataService(ILogger<SeedDataService> logger)
        {
            _logger = logger;
        }

        public async Task SeedAsync(AppDbContext context)
        {
            try
            {
                // Debug: Check what's in DB before clearing
                var countBefore = context.Categories.Count();
                _logger.LogInformation($"DEBUG: Categories before clear: {countBefore}");

                // Always clear existing data and reseed to ensure fresh state
                _logger.LogInformation("Clearing all existing data...");
                var allProducts = context.Products.ToList();
                _logger.LogInformation($"DEBUG: Found {allProducts.Count} products to delete");
                if (allProducts.Count > 0)
                {
                    context.Products.RemoveRange(allProducts);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("✓ Products deleted");
                }

                var allGroups = context.Groups.ToList();
                _logger.LogInformation($"DEBUG: Found {allGroups.Count} groups to delete");
                if (allGroups.Count > 0)
                {
                    context.Groups.RemoveRange(allGroups);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("✓ Groups deleted");
                }

                var allCategories = context.Categories.ToList();
                _logger.LogInformation($"DEBUG: Found {allCategories.Count} categories to delete");
                if (allCategories.Count > 0)
                {
                    context.Categories.RemoveRange(allCategories);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("✓ Categories deleted");
                }

                // Debug: Check what's in DB after clearing
                var countAfter = context.Categories.Count();
                _logger.LogInformation($"DEBUG: Categories after clear: {countAfter}");
                _logger.LogInformation("✓ Cleared all existing data");

                // Seed Categories
                {
                    _logger.LogInformation("Seeding categories...");
                    var categories = GetMockCategories();
                    _logger.LogInformation($"DEBUG: Created {categories.Count} mock categories");
                    context.Categories.AddRange(categories);
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"✓ Added {categories.Count} categories");
                }

                // Seed Groups
                {
                    _logger.LogInformation("Seeding groups...");
                    // Force clear the change tracker to ensure we get fresh data from DB
                    context.ChangeTracker.Clear();
                    var categoriesFromDb = await context.Categories.AsNoTracking().ToListAsync();
                    _logger.LogInformation($"DEBUG: Found {categoriesFromDb.Count} categories from DB after seeding");
                    foreach(var cat in categoriesFromDb)
                    {
                        _logger.LogInformation($"DEBUG: Category - Id: {cat.Id}, Name: {cat.Name}");
                    }
                    var groups = GetMockGroups(categoriesFromDb);
                    context.Groups.AddRange(groups);
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"✓ Added {groups.Count} groups");
                }

                // Seed Products
                {
                    _logger.LogInformation("Seeding products...");
                    // Force clear the change tracker to ensure we get fresh data from DB
                    context.ChangeTracker.Clear();
                    var groupsFromDb = await context.Groups.AsNoTracking().ToListAsync();
                    var products = GetMockProducts(groupsFromDb);
                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"✓ Added {products.Count} products");
                }

                _logger.LogInformation("✓ Seed data completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"✗ Error seeding database: {ex.Message}");
                throw;
            }
        }

        private List<Category> GetMockCategories()
        {
            return new List<Category>
            {
                // Root categories
                new Category
                {
                    Name = "Electronics",
                    OrderIndex = 1,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Fashion",
                    OrderIndex = 2,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Home & Garden",
                    OrderIndex = 3,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Sports & Outdoors",
                    OrderIndex = 4,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Beauty & Personal Care",
                    OrderIndex = 5,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Books & Media",
                    OrderIndex = 6,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Toys & Games",
                    OrderIndex = 7,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Food & Beverage",
                    OrderIndex = 8,
                    ParentId = null,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        private List<Group> GetMockGroups(List<Category> categories)
        {
            return new List<Group>
            {
                // Electronics groups
                new Group
                {
                    Name = "Smartphones",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Electronics")?.Id ?? 0,
                    Description = "Latest smartphones and mobile devices"
                },
                new Group
                {
                    Name = "Laptops & Computers",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Electronics")?.Id ?? 0,
                    Description = "Desktop and laptop computers for personal and professional use"
                },
                new Group
                {
                    Name = "Accessories",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Electronics")?.Id ?? 0,
                    Description = "Phone cases, chargers, cables, and other tech accessories"
                },

                // Fashion groups
                new Group
                {
                    Name = "Men's Clothing",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Fashion")?.Id ?? 0,
                    Description = "Shirts, pants, jackets, and more for men"
                },
                new Group
                {
                    Name = "Women's Clothing",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Fashion")?.Id ?? 0,
                    Description = "Dresses, tops, skirts, and more for women"
                },
                new Group
                {
                    Name = "Shoes",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Fashion")?.Id ?? 0,
                    Description = "Athletic shoes, casual shoes, formal shoes"
                },

                // Home & Garden groups
                new Group
                {
                    Name = "Kitchen Appliances",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Home & Garden")?.Id ?? 0,
                    Description = "Microwaves, blenders, coffee makers, and kitchen gadgets"
                },
                new Group
                {
                    Name = "Bedding & Bath",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Home & Garden")?.Id ?? 0,
                    Description = "Sheets, pillows, towels, and bathroom essentials"
                },
                new Group
                {
                    Name = "Furniture",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Home & Garden")?.Id ?? 0,
                    Description = "Sofas, tables, chairs, and home furniture"
                },

                // Sports & Outdoors groups
                new Group
                {
                    Name = "Fitness Equipment",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sports & Outdoors")?.Id ?? 0,
                    Description = "Dumbbells, yoga mats, treadmills, and gym equipment"
                },
                new Group
                {
                    Name = "Outdoor Gear",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Sports & Outdoors")?.Id ?? 0,
                    Description = "Camping tents, sleeping bags, backpacks"
                },

                // Beauty & Personal Care groups
                new Group
                {
                    Name = "Skincare",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Beauty & Personal Care")?.Id ?? 0,
                    Description = "Face creams, serums, cleansers, and skincare products"
                },
                new Group
                {
                    Name = "Hair Care",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Beauty & Personal Care")?.Id ?? 0,
                    Description = "Shampoos, conditioners, and hair styling products"
                },

                // Books & Media groups
                new Group
                {
                    Name = "Fiction Books",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Books & Media")?.Id ?? 0,
                    Description = "Novels, short stories, and fiction literature"
                },
                new Group
                {
                    Name = "Non-Fiction Books",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Books & Media")?.Id ?? 0,
                    Description = "Educational, self-help, and informative books"
                },

                // Toys & Games groups
                new Group
                {
                    Name = "Board Games",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Toys & Games")?.Id ?? 0,
                    Description = "Strategy games, party games, and board game classics"
                },
                new Group
                {
                    Name = "Video Games",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Toys & Games")?.Id ?? 0,
                    Description = "Console games, PC games, and video game accessories"
                },

                // Food & Beverage groups
                new Group
                {
                    Name = "Beverages",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Food & Beverage")?.Id ?? 0,
                    Description = "Coffee, tea, soft drinks, and juices"
                },
                new Group
                {
                    Name = "Snacks",
                    CategoryId = categories.FirstOrDefault(c => c.Name == "Food & Beverage")?.Id ?? 0,
                    Description = "Chips, cookies, nuts, and snack items"
                }
            };
        }

        private int GetGroupIdByName(List<Group> groups, string name)
        {
            return groups.FirstOrDefault(g => g.Name == name)?.Id ?? 0;
        }

        private List<Product> GetMockProducts(List<Group> groups)
        {
            var smartphones = GetGroupIdByName(groups, "Smartphones");
            var laptops = GetGroupIdByName(groups, "Laptops & Computers");
            var accessories = GetGroupIdByName(groups, "Accessories");
            var mensClothing = GetGroupIdByName(groups, "Men's Clothing");
            var womensClothing = GetGroupIdByName(groups, "Women's Clothing");
            var shoes = GetGroupIdByName(groups, "Shoes");
            var kitchenAppliances = GetGroupIdByName(groups, "Kitchen Appliances");
            var beddingBath = GetGroupIdByName(groups, "Bedding & Bath");
            var furniture = GetGroupIdByName(groups, "Furniture");
            var fitnessEquipment = GetGroupIdByName(groups, "Fitness Equipment");
            var outdoorGear = GetGroupIdByName(groups, "Outdoor Gear");
            var skincare = GetGroupIdByName(groups, "Skincare");
            var hairCare = GetGroupIdByName(groups, "Hair Care");
            var fictionBooks = GetGroupIdByName(groups, "Fiction Books");
            var nonFictionBooks = GetGroupIdByName(groups, "Non-Fiction Books");
            var boardGames = GetGroupIdByName(groups, "Board Games");
            var videoGames = GetGroupIdByName(groups, "Video Games");
            var beverages = GetGroupIdByName(groups, "Beverages");
            var snacks = GetGroupIdByName(groups, "Snacks");

            return new List<Product>
            {
                // Smartphones
                new Product
                {
                    Name = "iPhone 15 Pro",
                    Description = "Latest Apple flagship with advanced camera system and A17 Pro chip. Features a stunning 6.1-inch Super Retina XDR display, all-day battery life, and professional-grade video recording.",
                    Price = 999.99m,
                    Stock = 45,
                    GroupId = smartphones,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Samsung Galaxy S24",
                    Description = "Powerful Android flagship with 200MP camera, AI-enhanced features, and 120Hz AMOLED display. Perfect for productivity and content creation.",
                    Price = 899.99m,
                    Stock = 52,
                    GroupId = smartphones,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Google Pixel 8",
                    Description = "Google's flagship phone with exceptional computational photography, AI features, and pure Android experience. 6.2-inch OLED display.",
                    Price = 799.99m,
                    Stock = 38,
                    GroupId = groups.First(g => g.Name == "Smartphones").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Laptops
                new Product
                {
                    Name = "MacBook Pro 16\" M3 Max",
                    Description = "Powerful laptop for professionals with 16-core GPU, 16GB unified memory, and stunning Retina display. Perfect for video editing, 3D rendering, and development.",
                    Price = 2499.99m,
                    Stock = 18,
                    GroupId = groups.First(g => g.Name == "Laptops & Computers").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Dell XPS 15",
                    Description = "Premium Windows laptop with RTX 4060 GPU, Intel Core i7, 16GB RAM. 15.6-inch 4K OLED display. Ideal for creative professionals.",
                    Price = 1999.99m,
                    Stock = 25,
                    GroupId = groups.First(g => g.Name == "Laptops & Computers").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Lenovo ThinkPad X1 Carbon",
                    Description = "Ultraportable business laptop with Intel vPro processor, 14-hour battery life, and premium build quality. Perfect for business travelers.",
                    Price = 1699.99m,
                    Stock = 31,
                    GroupId = groups.First(g => g.Name == "Laptops & Computers").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Accessories
                new Product
                {
                    Name = "Apple AirPods Pro",
                    Description = "Premium wireless earbuds with active noise cancellation, transparency mode, and spatial audio. Seamless integration with Apple devices.",
                    Price = 249.99m,
                    Stock = 78,
                    GroupId = groups.First(g => g.Name == "Accessories").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Anker Fast Charging Cable",
                    Description = "Durable USB-C charging cable with 60W power delivery. Works with all USB-C devices. 6-foot length for convenience.",
                    Price = 12.99m,
                    Stock = 156,
                    GroupId = groups.First(g => g.Name == "Accessories").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Spigen iPhone Case",
                    Description = "Military-grade protective case with premium TPU material. Available in multiple colors. Drop protection up to 12 feet.",
                    Price = 19.99m,
                    Stock = 234,
                    GroupId = groups.First(g => g.Name == "Accessories").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Men's Clothing
                new Product
                {
                    Name = "Premium Cotton T-Shirt",
                    Description = "Comfortable 100% cotton t-shirt with breathable fabric. Available in multiple colors and sizes. Perfect for casual wear.",
                    Price = 24.99m,
                    Stock = 145,
                    GroupId = groups.First(g => g.Name == "Men's Clothing").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Classic Blue Jeans",
                    Description = "Durable denim jeans with comfortable fit. Features reinforced stitching and fade-resistant fabric. Available in various sizes.",
                    Price = 59.99m,
                    Stock = 89,
                    GroupId = groups.First(g => g.Name == "Men's Clothing").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Women's Clothing
                new Product
                {
                    Name = "Casual Sundress",
                    Description = "Light and breezy sundress perfect for summer. Made from 100% cotton blend with vibrant prints. Available in multiple sizes.",
                    Price = 49.99m,
                    Stock = 67,
                    GroupId = groups.First(g => g.Name == "Women's Clothing").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Elegant Blazer",
                    Description = "Professional blazer in classic black. Perfect for business meetings and formal occasions. Premium fabric with tailored fit.",
                    Price = 129.99m,
                    Stock = 34,
                    GroupId = groups.First(g => g.Name == "Women's Clothing").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Shoes
                new Product
                {
                    Name = "Nike Running Shoes",
                    Description = "High-performance running shoes with advanced cushioning technology. Lightweight and breathable. Available in all sizes.",
                    Price = 119.99m,
                    Stock = 56,
                    GroupId = groups.First(g => g.Name == "Shoes").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Casual Leather Sneakers",
                    Description = "Premium leather sneakers with comfortable insoles. Versatile style for everyday wear. Easy to clean and maintain.",
                    Price = 89.99m,
                    Stock = 72,
                    GroupId = groups.First(g => g.Name == "Shoes").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Kitchen Appliances
                new Product
                {
                    Name = "Digital Coffee Maker",
                    Description = "Programmable coffee maker with digital timer and automatic shut-off. Makes up to 12 cups of coffee. Stainless steel carafe.",
                    Price = 49.99m,
                    Stock = 41,
                    GroupId = groups.First(g => g.Name == "Kitchen Appliances").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Blender Pro 3000",
                    Description = "Powerful blender with 1200W motor. Features multiple speed settings and pulse function. Perfect for smoothies and soups.",
                    Price = 129.99m,
                    Stock = 28,
                    GroupId = groups.First(g => g.Name == "Kitchen Appliances").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Microwave Oven",
                    Description = "Compact microwave with 1000W power. Features automatic reheat and defrost functions. Perfect for apartments and dorms.",
                    Price = 99.99m,
                    Stock = 35,
                    GroupId = groups.First(g => g.Name == "Kitchen Appliances").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Bedding & Bath
                new Product
                {
                    Name = "Egyptian Cotton Sheets Set",
                    Description = "Luxury 1000 thread count Egyptian cotton sheets. Soft, breathable, and durable. Includes 2 pillowcases and fitted sheet.",
                    Price = 79.99m,
                    Stock = 52,
                    GroupId = groups.First(g => g.Name == "Bedding & Bath").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Plush Memory Foam Pillow",
                    Description = "Comfortable memory foam pillow with cooling gel. Helps prevent neck pain. Hypoallergenic and machine washable.",
                    Price = 39.99m,
                    Stock = 68,
                    GroupId = groups.First(g => g.Name == "Bedding & Bath").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Furniture
                new Product
                {
                    Name = "Modern Sectional Sofa",
                    Description = "Spacious sectional sofa with comfortable cushions. Perfect for family gatherings. Available in gray and black.",
                    Price = 899.99m,
                    Stock = 12,
                    GroupId = groups.First(g => g.Name == "Furniture").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Dining Table with Chairs",
                    Description = "Elegant dining set with solid wood table and 4 upholstered chairs. Perfect for dining rooms and apartments.",
                    Price = 599.99m,
                    Stock = 8,
                    GroupId = groups.First(g => g.Name == "Furniture").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Fitness Equipment
                new Product
                {
                    Name = "Adjustable Dumbbell Set",
                    Description = "Space-saving adjustable dumbbells from 5-25 lbs. Perfect for home gym. Includes storage rack.",
                    Price = 199.99m,
                    Stock = 24,
                    GroupId = groups.First(g => g.Name == "Fitness Equipment").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Yoga Mat",
                    Description = "Non-slip yoga mat with cushioning for comfort. Eco-friendly TPE material. Easy to clean and transport.",
                    Price = 29.99m,
                    Stock = 87,
                    GroupId = groups.First(g => g.Name == "Fitness Equipment").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Outdoor Gear
                new Product
                {
                    Name = "Camping Tent 2-Person",
                    Description = "Waterproof camping tent with double-layer design. Easy setup and tear-down. Includes stakes and carrying bag.",
                    Price = 149.99m,
                    Stock = 31,
                    GroupId = groups.First(g => g.Name == "Outdoor Gear").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Hiking Backpack 50L",
                    Description = "Large capacity backpack perfect for multi-day hikes. Features adjustable straps and multiple compartments. Waterproof cover included.",
                    Price = 129.99m,
                    Stock = 43,
                    GroupId = groups.First(g => g.Name == "Outdoor Gear").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Skincare
                new Product
                {
                    Name = "Anti-Aging Face Cream",
                    Description = "Premium face cream with retinol and vitamin C. Reduces wrinkles and fine lines. Suitable for all skin types.",
                    Price = 49.99m,
                    Stock = 64,
                    GroupId = groups.First(g => g.Name == "Skincare").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Gentle Facial Cleanser",
                    Description = "Mild pH-balanced cleanser for sensitive skin. Removes makeup and impurities without drying. 200ml bottle.",
                    Price = 14.99m,
                    Stock = 98,
                    GroupId = groups.First(g => g.Name == "Skincare").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Hair Care
                new Product
                {
                    Name = "Moisturizing Shampoo",
                    Description = "Sulfate-free shampoo for dry and damaged hair. Infused with argan oil and keratin. 500ml bottle.",
                    Price = 12.99m,
                    Stock = 112,
                    GroupId = groups.First(g => g.Name == "Hair Care").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Hair Straightener Brush",
                    Description = "Ionic hair straightener brush with 30 heat settings. Fast heating and automatic temperature control. Professional results at home.",
                    Price = 34.99m,
                    Stock = 47,
                    GroupId = groups.First(g => g.Name == "Hair Care").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Fiction Books
                new Product
                {
                    Name = "The Midnight Library",
                    Description = "A contemporary fantasy novel about infinite possibilities and second chances. Perfect for readers looking for inspiration and escapism.",
                    Price = 16.99m,
                    Stock = 74,
                    GroupId = groups.First(g => g.Name == "Fiction Books").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Atomic Habits",
                    Description = "Practical guide to building good habits and breaking bad ones. Transform your life through small daily actions.",
                    Price = 14.99m,
                    Stock = 156,
                    GroupId = groups.First(g => g.Name == "Fiction Books").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Non-Fiction Books
                new Product
                {
                    Name = "Sapiens",
                    Description = "A sweeping history of humankind from the Stone Age to modern times. Fascinating insights into civilization and society.",
                    Price = 18.99m,
                    Stock = 92,
                    GroupId = groups.First(g => g.Name == "Non-Fiction Books").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "The Power of Now",
                    Description = "A guide to spiritual enlightenment and living in the present moment. Transform your life with practical wisdom.",
                    Price = 15.99m,
                    Stock = 68,
                    GroupId = groups.First(g => g.Name == "Non-Fiction Books").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Board Games
                new Product
                {
                    Name = "Catan",
                    Description = "Classic strategy board game where players build settlements and cities. Perfect for family game night. 3-4 players, ages 10+.",
                    Price = 44.99m,
                    Stock = 36,
                    GroupId = groups.First(g => g.Name == "Board Games").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Ticket to Ride",
                    Description = "Railway-themed strategy game for 2-5 players. Collect cards and claim train routes across the board. Great for all ages.",
                    Price = 39.99m,
                    Stock = 41,
                    GroupId = groups.First(g => g.Name == "Board Games").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Video Games
                new Product
                {
                    Name = "Elden Ring",
                    Description = "Epic action RPG with open-world exploration. Challenging gameplay and stunning visuals. Available for PS5, Xbox Series X, and PC.",
                    Price = 59.99m,
                    Stock = 28,
                    GroupId = groups.First(g => g.Name == "Video Games").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "The Legend of Zelda: Tears of the Kingdom",
                    Description = "Latest Nintendo Switch exclusive adventure. Explore vast landscapes and solve puzzles. Perfect for adventure lovers.",
                    Price = 69.99m,
                    Stock = 22,
                    GroupId = groups.First(g => g.Name == "Video Games").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Beverages
                new Product
                {
                    Name = "Premium Arabica Coffee Beans",
                    Description = "Freshly roasted arabica coffee beans from Ethiopia. Rich flavor with notes of chocolate and berry. 1kg bag.",
                    Price = 24.99m,
                    Stock = 53,
                    GroupId = groups.First(g => g.Name == "Beverages").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Green Tea Collection",
                    Description = "Assorted organic green teas including jasmine, sencha, and matcha. Premium quality leaves. 50 tea bags.",
                    Price = 19.99m,
                    Stock = 67,
                    GroupId = groups.First(g => g.Name == "Beverages").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },

                // Snacks
                new Product
                {
                    Name = "Mixed Nuts Variety Pack",
                    Description = "Delicious mix of almonds, cashews, and walnuts. Healthy snack packed with nutrients. 500g bag.",
                    Price = 14.99m,
                    Stock = 89,
                    GroupId = groups.First(g => g.Name == "Snacks").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Organic Protein Bars",
                    Description = "High-protein snack bars made with natural ingredients. Perfect for pre/post workout. Box of 12 bars.",
                    Price = 22.99m,
                    Stock = 71,
                    GroupId = groups.First(g => g.Name == "Snacks").Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}


