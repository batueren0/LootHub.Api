using LootHub.Application.Common;
using LootHub.Domain.Entities;
using LootHub.Domain.ValueObjects;
using LootHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LootHub.WebApi.Seeding;

/// <summary>
/// Development ortamında test verisi ekler (kategori, marka, özellik, ürün).
/// Var olan kayıtlara dokunmaz: kategori/marka slug'a, özellik isme, ürün slug'a göre kontrol edilir.
/// Fiyatlar ve stoklar gerçek değil, test amaçlıdır.
/// </summary>
public static class DevDataSeeder
{
    private sealed record SeedProduct(
        string Name,
        string Description,
        decimal Price,
        int Stock,
        string Category,
        string Brand,
        (string Property, string Value)[] Properties);

    private static readonly string[] CategoryNames = ["Klavye", "Mouse", "Kulaklık", "Mousepad", "Monitör"];
    private static readonly string[] BrandNames = ["Logitech", "Razer", "SteelSeries", "HyperX", "Corsair", "ASUS ROG"];
    private static readonly string[] PropertyNames = ["Bağlantı Tipi", "Switch Tipi", "Renk", "DPI", "Yenileme Hızı", "Panel Tipi"];

    private static readonly SeedProduct[] Products =
    [
        // Klavye
        new("Logitech G Pro X TKL", "Turnuva odaklı, kompakt TKL kablosuz mekanik oyuncu klavyesi.", 5499.90m, 18, "Klavye", "Logitech",
            [("Bağlantı Tipi", "Kablosuz"), ("Switch Tipi", "Brown"), ("Renk", "Siyah")]),
        new("Razer BlackWidow V4", "RGB aydınlatmalı, makro tuşlu tam boy mekanik oyuncu klavyesi.", 6299.00m, 10, "Klavye", "Razer",
            [("Bağlantı Tipi", "Kablolu"), ("Switch Tipi", "Green"), ("Renk", "Siyah")]),
        new("SteelSeries Apex Pro TKL", "Ayarlanabilir tetikleme mesafeli manyetik switch'li TKL klavye.", 7999.00m, 0, "Klavye", "SteelSeries",
            [("Bağlantı Tipi", "Kablolu"), ("Switch Tipi", "OmniPoint"), ("Renk", "Siyah")]),
        new("HyperX Alloy Origins Core", "Alüminyum gövdeli, sağlam ve uygun fiyatlı TKL mekanik klavye.", 2799.00m, 35, "Klavye", "HyperX",
            [("Bağlantı Tipi", "Kablolu"), ("Switch Tipi", "Red"), ("Renk", "Siyah")]),
        new("Corsair K70 RGB Pro", "PBT tuş takımlı, yüksek tarama hızına sahip tam boy mekanik klavye.", 4499.00m, 12, "Klavye", "Corsair",
            [("Bağlantı Tipi", "Kablolu"), ("Switch Tipi", "Red"), ("Renk", "Siyah")]),

        // Mouse
        new("Razer DeathAdder V3 Pro", "Ergonomik tasarımlı, ultra hafif kablosuz e-spor mouse'u.", 4699.00m, 22, "Mouse", "Razer",
            [("Bağlantı Tipi", "Kablosuz"), ("DPI", "30000"), ("Renk", "Beyaz")]),
        new("Logitech G502 X Plus", "Ayarlanabilir ağırlık ve bol programlanabilir tuşlu kablosuz mouse.", 4299.00m, 15, "Mouse", "Logitech",
            [("Bağlantı Tipi", "Kablosuz"), ("DPI", "25600"), ("Renk", "Siyah")]),
        new("SteelSeries Rival 3", "Giriş seviyesi için hafif ve dayanıklı kablolu oyuncu mouse'u.", 899.90m, 60, "Mouse", "SteelSeries",
            [("Bağlantı Tipi", "Kablolu"), ("DPI", "8500"), ("Renk", "Siyah")]),
        new("HyperX Pulsefire Haste 2", "Delikli gövde tasarımıyla çok hafif kablolu oyuncu mouse'u.", 1499.00m, 0, "Mouse", "HyperX",
            [("Bağlantı Tipi", "Kablolu"), ("DPI", "26000"), ("Renk", "Beyaz")]),

        // Kulaklık
        new("HyperX Cloud III", "Rahat yastıklı, net mikrofonlu kablolu oyuncu kulaklığı.", 2999.00m, 40, "Kulaklık", "HyperX",
            [("Bağlantı Tipi", "Kablolu"), ("Renk", "Siyah")]),
        new("SteelSeries Arctis Nova 7", "Çift bağlantılı, uzun pil ömürlü kablosuz oyuncu kulaklığı.", 5299.00m, 8, "Kulaklık", "SteelSeries",
            [("Bağlantı Tipi", "Kablosuz"), ("Renk", "Siyah")]),
        new("Razer BlackShark V2 X", "Hafif yapılı, uygun fiyatlı kablolu e-spor kulaklığı.", 1299.00m, 50, "Kulaklık", "Razer",
            [("Bağlantı Tipi", "Kablolu"), ("Renk", "Siyah")]),
        new("Logitech G Pro X 2 Lightspeed", "Grafen sürücülü, profesyonel seviye kablosuz oyuncu kulaklığı.", 7499.00m, 5, "Kulaklık", "Logitech",
            [("Bağlantı Tipi", "Kablosuz"), ("Renk", "Beyaz")]),

        // Mousepad
        new("SteelSeries QcK Heavy XXL", "Kalın kauçuk tabanlı, geniş kumaş oyuncu mousepad'i.", 1199.00m, 30, "Mousepad", "SteelSeries",
            [("Renk", "Siyah")]),
        new("Razer Gigantus V2 XXL", "Kontrol odaklı yüzeye sahip, masa boyu oyuncu mousepad'i.", 899.00m, 45, "Mousepad", "Razer",
            [("Renk", "Siyah")]),

        // Monitör
        new("ASUS ROG Swift PG27AQDM", "27 inç QHD OLED panelli, 240 Hz yenileme hızına sahip oyuncu monitörü.", 32999.00m, 4, "Monitör", "ASUS ROG",
            [("Yenileme Hızı", "240 Hz"), ("Panel Tipi", "OLED")]),
        new("Corsair Xeneon 27QHD240", "27 inç QHD OLED, 240 Hz ve 0.03 ms tepki süreli oyuncu monitörü.", 27999.00m, 3, "Monitör", "Corsair",
            [("Yenileme Hızı", "240 Hz"), ("Panel Tipi", "OLED")]),
    ];

    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 1) Kategoriler (slug'a göre kontrol)
        var existingCategorySlugs = await db.Categories.Select(c => c.Slug).ToListAsync(ct);
        foreach (var name in CategoryNames.Where(n => !existingCategorySlugs.Contains(n.ToKebabCase())))
            db.Categories.Add(new Category { Name = name, Slug = name.ToKebabCase() });

        // 2) Markalar (slug'a göre kontrol)
        var existingBrandSlugs = await db.Brands.Select(b => b.Slug).ToListAsync(ct);
        foreach (var name in BrandNames.Where(n => !existingBrandSlugs.Contains(n.ToKebabCase())))
            db.Brands.Add(new Brand { Name = name, Slug = name.ToKebabCase() });

        // 3) Özellikler (isme göre kontrol)
        var existingPropertyNames = await db.Properties.Select(p => p.Name).ToListAsync(ct);
        foreach (var name in PropertyNames.Where(n => !existingPropertyNames.Contains(n)))
            db.Properties.Add(new Property { Name = name });

        await db.SaveChangesAsync(ct);

        // 4) Ürünler: önce id'leri bulmak için sözlükler
        var categories = await db.Categories.ToDictionaryAsync(c => c.Slug, c => c.Id, ct);
        var brands = await db.Brands.ToDictionaryAsync(b => b.Slug, b => b.Id, ct);
        var properties = await db.Properties.ToDictionaryAsync(p => p.Name, p => p.Id, ct);
        var existingProductSlugs = await db.Products.Select(p => p.Slug).ToListAsync(ct);

        var now = TimeProvider.System.GetUtcNow();
        var index = 0;

        foreach (var seed in Products)
        {
            index++;
            var slug = seed.Name.ToKebabCase();
            if (existingProductSlugs.Contains(slug))
                continue;

            // "En yeni" sıralamasını test edebilmek için her ürüne farklı bir oluşturulma zamanı
            var createdAt = now.AddDays(-index);

            db.Products.Add(new Product
            {
                Name = seed.Name,
                Slug = slug,
                Description = seed.Description,
                UnitPrice = Money.TRY(seed.Price),
                Stock = seed.Stock,
                CategoryId = categories[seed.Category.ToKebabCase()],
                BrandId = brands[seed.Brand.ToKebabCase()],
                CreatedAtUtc = createdAt,
                LastModifiedAtUtc = createdAt,
                ProductProperties = seed.Properties.Select(p => new ProductProperty
                {
                    PropertyId = properties[p.Property],
                    Value = p.Value
                }).ToList()
            });
        }

        await db.SaveChangesAsync(ct);
    }
}
