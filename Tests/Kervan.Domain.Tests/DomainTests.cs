using System;
using Xunit;
using Kervan.Domain;

namespace Kervan.Domain.Tests
{
    public class DomainTests
    {
        [Fact]
        public void Inventory_ShouldRespectMaxCapacity()
        {
            // Arrange (Hazırlık)
            var inventory = new Inventory(maxCapacityKg: 100f);
            var heavyItem = new Item("iron_ore", "Demir Külçe", ItemCategory.Metal, basePrice: 10, weightKg: 30f);

            // Act (Eylem)
            bool firstAdd = inventory.TryAddItem(heavyItem, 3); // 3 * 30 = 90 kg (Sığmalı)
            bool secondAdd = inventory.TryAddItem(heavyItem, 1); // 90 + 30 = 120 kg (Kapasite 100, sığmamalı)

            // Assert (Doğrulama)
            Assert.True(firstAdd);
            Assert.False(secondAdd);
            Assert.Equal(90f, inventory.CurrentWeightKg);
            Assert.Equal(3, inventory.GetQuantity("iron_ore"));
        }

        [Fact]
        public void Caravan_AddingMount_ShouldIncreaseCarryingCapacity()
        {
            // Arrange: 100 Akçe, 50 kg erzak
            var caravan = new Caravan(initialAkce: 100, initialFoodKg: 50f);
            float initialCapacity = caravan.CalculateTotalCapacity(); // Başlangıç: 1 tüccar (50kg) + 1 katır (120kg) = 170kg

            // Act: 1 Deve ekle (+200kg)
            caravan.AddMount(MountType.Camel, 1);
            float newCapacity = caravan.Cargo.MaxCapacityKg;

            // Assert
            Assert.Equal(170f, initialCapacity);
            Assert.Equal(370f, newCapacity);
        }

        [Fact]
        public void Caravan_AdvanceOneDay_ShouldConsumeFoodAndPayWages()
        {
            // Arrange: 50 Akçe, 20 kg erzak, 1 muhafız tutulmuş
            var caravan = new Caravan(initialAkce: 50, initialFoodKg: 20f);
            caravan.HireGuard(1); // 1 muhafız = 3 Akçe/gün

            float dailyFoodExpected = caravan.DailyFoodConsumption;
            int initialGold = caravan.Akce;

            // Act: 1 gün ilerle
            caravan.AdvanceOneDay();

            // Assert
            Assert.Equal(initialGold - 3, caravan.Akce); // 3 Akçe ulufe kesildi
            Assert.Equal(20f - dailyFoodExpected, caravan.FoodSupplyKg);
            Assert.Equal(100, caravan.Morale);
        }

        [Fact]
        public void FullTradeRoute_BursaToKonya_ShouldGenerateProfit()
        {
            // 1. Şehirleri Oluştur (Bursa ve Konya)
            var bursa = new City("bursa", "Bursa", "Osmanlı Beyliği", coordX: 100f, coordY: 200f);
            var konya = new City("konya", "Konya", "Karamanoğulları", coordX: 350f, coordY: 150f);

            // 2. Ticaret Malı: Bursa İpeği (Taban fiyat 20 Akçe, 2 kg)
            var bursaIpegi = new Item("silk", "Bursa İpeği", ItemCategory.Textile, basePrice: 20, weightKg: 2f);

            // 3. Pazarları Ayarla:
            // Bursa ipeğin başkenti: Bolluk var, çarpan 0.6 (%40 indirimli)
            bursa.LocalMarket.AddOrUpdateGood(bursaIpegi, stock: 50, multiplier: 0.6f);
            // Konya bozkırda: İpek nadir ve kıymetli, çarpan 1.8 (%80 primli)
            konya.LocalMarket.AddOrUpdateGood(bursaIpegi, stock: 5, multiplier: 1.8f);

            // 4. Kervanı Hazırla: 200 Akçe, 30 kg erzak, 1 muhafız
            var caravan = new Caravan(initialAkce: 200, initialFoodKg: 30f);
            caravan.HireGuard(1);

            int startAkce = caravan.Akce;

            // 5. Bursa'dan 10 adet İpek satın al
            int buyPricePerUnit = bursa.LocalMarket.GetBuyPrice(bursaIpegi.Id);
            bool buySuccess = bursa.LocalMarket.TryBuy(caravan, bursaIpegi.Id, quantity: 10);

            Assert.True(buySuccess, "Bursa'dan ipek alımı başarılı olmalı.");
            Assert.Equal(10, caravan.Cargo.GetQuantity(bursaIpegi.Id));
            Assert.Equal(startAkce - (buyPricePerUnit * 10), caravan.Akce);

            // 6. Yolda 3 günlük seyahat simülasyonu (Erzak tüketimi ve muhafız maaşı)
            for (int day = 0; day < 3; day++)
            {
                caravan.AdvanceOneDay();
            }

            // 7. Konya'ya varış ve 10 adet İpeğin tamamını Konya pazarına satma
            int sellPricePerUnit = konya.LocalMarket.GetSellPrice(bursaIpegi.Id);
            bool sellSuccess = konya.LocalMarket.TrySell(caravan, bursaIpegi.Id, quantity: 10);

            Assert.True(sellSuccess, "Konya'da ipek satışı başarılı olmalı.");
            Assert.Equal(0, caravan.Cargo.GetQuantity(bursaIpegi.Id));

            // Net kâr kontrolü:
            // Satış birim fiyatı alış birim fiyatından belirgin şekilde yüksek olmalı
            Assert.True(sellPricePerUnit > buyPricePerUnit, $"Satış fiyatı ({sellPricePerUnit}) alış fiyatından ({buyPricePerUnit}) yüksek olmalı.");
            // Yol masraflarına (muhafız maaşları) rağmen kervanın son parası başlangıç parasından fazla olmalı
            Assert.True(caravan.Akce > startAkce, $"Tüccar kâr etmiş olmalı. Başlangıç: {startAkce}, Bitiş: {caravan.Akce}");
        }

        [Fact]
        public void MapGraph_ShouldProvideRouteChoicesAndCalculateTravelDays()
        {
            // 1. Grafi Oluştur
            var graph = new MapGraph();

            // 2. Bursa -> İznik arasında 2 alternatif rota tanımla
            // Rota A: Güvenli Taş Yol (90 km, %5 tehlike)
            var highway = new MapRoute(
                routeId: "bursa_iznik_highway",
                routeName: "Bursa-İznik Kervan Yolu",
                fromCityId: "bursa",
                toCityId: "iznik",
                terrain: TerrainType.PavedRoad,
                distanceKm: 90f,
                dangerFactor: 0.05f
            );

            // Rota B: Kestirme Dağ Patikası (50 km, %40 haydut tehlikesi)
            var mountainPass = new MapRoute(
                routeId: "bursa_iznik_mountain",
                routeName: "Katırlı Dağ Patikası",
                fromCityId: "bursa",
                toCityId: "iznik",
                terrain: TerrainType.MountainPass,
                distanceKm: 50f,
                dangerFactor: 0.40f
            );

            graph.AddRoute(highway, isBidirectional: true);
            graph.AddRoute(mountainPass, isBidirectional: true);

            // 3. Kervanı Hazırla
            var caravan = new Caravan(initialAkce: 100, initialFoodKg: 50f);

            // 4. Testler
            var routesFromBursa = graph.GetAvailableRoutesFromCity("bursa");
            Assert.Equal(2, routesFromBursa.Count);

            // Dönüş rotalarının da çift yönlü oluştuğunu doğrula
            var routesFromIznik = graph.GetAvailableRoutesFromCity("iznik");
            Assert.Equal(2, routesFromIznik.Count);

            // Gün hesabı doğrulaması: Kestirme dağ yolu daha kısa sürmeli ama tehlikesi %40 olmalı
            int highwayDays = highway.CalculateEstimatedDays(caravan);
            int mountainDays = mountainPass.CalculateEstimatedDays(caravan);

            Assert.True(mountainDays <= highwayDays, "Dağ kestirmesi gün olarak daha kısa veya eşit olmalı.");
            Assert.True(mountainPass.DangerFactor > highway.DangerFactor, "Dağ yolunun tehlikesi taş yoldan yüksek olmalı.");
        }
    }
}
