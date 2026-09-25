# KERVAN MİMARİ VE C# KODLAMA STANDARTLARI
## Architecture & Conventions

---

### 1. Üç Katmanlı Mimari (Three-Layer Architecture)

```
[Presentation Layer (Unity UI / Monobehaviour)]
       │
       ▼ (Events / Actions)
[Application & Service Layer (Managers / Controllers)]
       │
       ▼ (Pure C# Data & Logic)
[Domain Layer (POCO - Plain Old C# Objects)]
```

* **Domain Katmanı:** Unity kütüphanelerini (`UnityEngine`) KESİNLİKLE kullanmaz. Saf C# ile yazılır. 
  * Avantajı: Çok hızlı test edilebilir, oyun motoru değişse bile mantık çöpe gitmez.
* **Service Katmanı:** Oyun döngüsünü ve durumunu (GameManager, TravelManager) yönetir.
* **Presentation Katmanı:** Kullanıcı arayüzünü çizer, sesleri oynatır, tıklamaları algılar ve Service katmanına haber verir.

---

### 2. C# Kodlama Standartları

* **İsimlendirme:**
  * Sınıflar, Metotlar, Enumlar, Public Değişkenler: `PascalCase` (Örn: `CalculatePrice()`, `CurrentGold`).
  * Private/Internal Alanlar: `_camelCase` (Örn: `_currentWeight`, `_inventoryList`).
  * Parametreler ve Yerel Değişkenler: `camelCase` (Örn: `targetCity`, `itemAmount`).
  * Arayüzler (Interfaces): `I` ile başlar (Örn: `IEncounterResolver`, `ITradeable`).

* **Encapsulation (Kapsülleme) Kuralı:**
  * Public değişken tanımlamak yerine Property kullanın:
    ```csharp
    // DOĞRU:
    public int Akce { get; private set; }
    
    // YANLIŞ:
    public int Akce;
    ```

* **Bağımlılıkları Azaltma (Decoupling):**
  * Sınıflar birbirini doğrudan aramak yerine C# `event` veya `System.Action` kullanır:
    ```csharp
    public static event Action<int> OnGoldChanged;
    ```

---

### 3. Klasör Standartları (Unity Projesi İçi)

```
Assets/
├── _Scripts/
│   ├── Domain/           # Saf C# sınıfları (Item, Caravan, Inventory, Formulas)
│   ├── Data/             # ScriptableObject sınıfları ve JSON modelleri
│   ├── Services/         # Oyun yöneticileri (TradeService, TravelService)
│   └── Presentation/     # MonoBehaviour UI, ses, efekt sınıfları
├── Art/
│   ├── Sprites/          # 2D görseller, ikonlar
│   ├── UI/               # Parşömen arayüz elemanları, fontlar
│   └── Textures/
├── Audio/
│   ├── Music/            # Arka plan müzikleri
│   └── SFX/              # Ses efektleri
└── Resources_Data/       # JSON / ScriptableObject veri varlıkları
```
