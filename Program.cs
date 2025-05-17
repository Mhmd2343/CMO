using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using DeliveryAppSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();

    // Ensure required categories exist
    if (!db.ProductCategories.Any())
    {
        db.ProductCategories.AddRange(
            new ProductCategory { Name = "Drinks" },
            new ProductCategory { Name = "Desserts" }
        );
        db.SaveChanges();
    }

    // Ensure CMO Market exists
    var shop = db.Shops.FirstOrDefault(s => s.Name == "CMO Market");
    if (shop == null)
    {
        shop = new Shop { Name = "CMO Market", Location = "Main Street" };
        db.Shops.Add(shop);
        db.SaveChanges();
    }

    // ? Merge "Soft Drinks" into "Drinks" and delete "Soft Drinks"
    var softDrinks = db.ProductCategories.FirstOrDefault(c => c.Name == "Soft Drinks");
    var drinks = db.ProductCategories.FirstOrDefault(c => c.Name == "Drinks");

    if (softDrinks != null && drinks != null)
    {
        var softDrinkProducts = db.Products
            .Where(p => p.ProductCategoryId == softDrinks.ProductCategoryId)
            .ToList();

        foreach (var product in softDrinkProducts)
        {
            product.ProductCategoryId = drinks.ProductCategoryId;
        }

        db.ProductCategories.Remove(softDrinks);
        db.SaveChanges();
    }

    // Now seed updated product list
    SeedGroceryProducts(db);
}

void SeedGroceryProducts(ApplicationDbContext context)
{
    var drinksCategory = context.ProductCategories.FirstOrDefault(c => c.Name == "Drinks");
    var dessertCategory = context.ProductCategories.FirstOrDefault(c => c.Name == "Desserts");
    var shop = context.Shops.FirstOrDefault(s => s.Name == "CMO Market");

    if (drinksCategory == null || dessertCategory == null || shop == null) return;

    // Add other categories if missing
    ProductCategory EnsureCategory(string name)
    {
        var cat = context.ProductCategories.FirstOrDefault(c => c.Name == name);
        if (cat == null)
        {
            cat = new ProductCategory { Name = name };
            context.ProductCategories.Add(cat);
            context.SaveChanges();
        }
        return cat;
    }

    var freshCategory = EnsureCategory("Fresh");
    var snacksCategory = EnsureCategory("Snacks");
    var dairyCategory = EnsureCategory("Dairy");

    void AddProductIfNotExists(string name, string description, decimal price, int categoryId)
    {
        if (!context.Products.Any(p => p.Name == name))
        {
            context.Products.Add(new Product
            {
                Name = name,
                Description = description,
                Price = price,
                ProductCategoryId = categoryId,
                ShopId = shop.ShopId
            });
        }
    }

    // Drinks
    AddProductIfNotExists("Pepsi", "Regular", 3, drinksCategory.ProductCategoryId);
    AddProductIfNotExists("7UP", "Regular", 3, drinksCategory.ProductCategoryId);
    AddProductIfNotExists("Miranda", "Regular", 3, drinksCategory.ProductCategoryId);
    AddProductIfNotExists("Sparkling Water", "Taybe", 2.5m, drinksCategory.ProductCategoryId);
    AddProductIfNotExists("Beer", "Almaza", 5, drinksCategory.ProductCategoryId);
    AddProductIfNotExists("Orange Juice", "Freshly squeezed juice", 3.00m, drinksCategory.ProductCategoryId);

    // Desserts
    AddProductIfNotExists("Ice Cream", "Strawberry, Chocolate, Vanilla, Pistachio, Lemon, Orange", 10, dessertCategory.ProductCategoryId);
    AddProductIfNotExists("Milk Chocolate", "Smooth and creamy", 5.00m, dessertCategory.ProductCategoryId);
    AddProductIfNotExists("Dark Chocolate", "Rich and intense", 6.00m, dessertCategory.ProductCategoryId);

    // Fresh
    AddProductIfNotExists("Apple", "Fresh red apples", 1.50m, freshCategory.ProductCategoryId);
    AddProductIfNotExists("Banana", "Ripe yellow bananas", 1.20m, freshCategory.ProductCategoryId);
    AddProductIfNotExists("Carrot", "Organic carrots", 1.00m, freshCategory.ProductCategoryId);
    AddProductIfNotExists("Tomato", "Fresh tomatoes", 1.80m, freshCategory.ProductCategoryId);

    // Snacks
    AddProductIfNotExists("Potato Chips", "Crispy and salty", 2.50m, snacksCategory.ProductCategoryId);
    AddProductIfNotExists("Mixed Nuts", "Salted and roasted", 4.50m, snacksCategory.ProductCategoryId);
    AddProductIfNotExists("Granola Bar", "Healthy and crunchy", 3.50m, snacksCategory.ProductCategoryId);

    // Dairy
    AddProductIfNotExists("Cheddar Cheese", "Aged cheddar cheese block", 4.00m, dairyCategory.ProductCategoryId);
    AddProductIfNotExists("Milk", "Fresh whole milk", 2.00m, dairyCategory.ProductCategoryId);
    AddProductIfNotExists("Yogurt", "Creamy natural yogurt", 3.00m, dairyCategory.ProductCategoryId);
    AddProductIfNotExists("Butter", "Salted butter", 3.50m, dairyCategory.ProductCategoryId);

    context.SaveChanges();
}

void SeedDrivers(ApplicationDbContext context)
{
    if (context.Drivers.Any())
        return;

    // Ensure linked users exist
    var user2 = context.Users.FirstOrDefault(u => u.UserId == 2);
    var user3 = context.Users.FirstOrDefault(u => u.UserId == 3);
    var user4 = context.Users.FirstOrDefault(u => u.UserId == 4);

    if (user2 == null || user3 == null || user4 == null)
        return; // Skip if users don't exist

    var drivers = new List<Driver>
    {
        new Driver
        {
            UserId = user2.UserId,
            VehicleType = "Car",
            PlateNumber = "ABC123",
            PricingType = "PerKM",
            City = "Beirut",
            WorkingHours = "8 AM - 6 PM",
            PricePerKm = 2.5m,
            FixedDeliveryPrice = 0,
            AverageRating = 4.6m
        },
        new Driver
        {
            UserId = user3.UserId,
            VehicleType = "Bike",
            PlateNumber = "XYZ789",
            PricingType = "PerDelivery",
            City = "Tripoli",
            WorkingHours = "10 AM - 8 PM",
            PricePerKm = 0,
            FixedDeliveryPrice = 5,
            AverageRating = 4.8m
        },
        new Driver
        {
            UserId = user4.UserId,
            VehicleType = "Scooter",
            PlateNumber = "SCO123",
            PricingType = "PerKM",
            City = "Beirut",
            WorkingHours = "12 PM - 10 PM",
            PricePerKm = 1.8m,
            FixedDeliveryPrice = 0,
            AverageRating = 4.2m
        }
    };

    context.Drivers.AddRange(drivers);
    context.SaveChanges();
}

app.Run();


