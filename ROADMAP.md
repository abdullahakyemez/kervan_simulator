# KERVAN GELİŞTİRME YOL HARİTASI (ROADMAP)

Bu belge projenin canlı ilerleme kaydıdır. Tamamlanan her görev `[x]` olarak işaretlenir.

---

### [x] FAZ 0: Temel Ortam ve Proje Yapısı
- [x] Mimari ve oyun tasarım belgelerinin oluşturulması (`GDD.md`, `ROADMAP.md`, `ARCHITECTURE.md`).
- [x] Git deposunun başlatılması (`git init`) ve kurumsal Unity/C# `.gitignore` dosyasının eklenmesi.
- [x] Unity Hub ve Unity 6 (`6000.6.3f1`) kurulumunun doğrulanması.
- [x] Unity 2D (URP) proje şablonunun yapılandırılması ve kurumsal klasör yapısının (`_Scripts/Domain`, `_Scripts/Presentation`, `_Scripts/Data`, `Art`, `Audio`) oluşturulması.

---

### [x] FAZ 1: Saf C# Çekirdek Mantığı (Domain Layer)
*Unity'den bağımsız, konsolda ve test edilebilir saf C# sınıfları.*
- [x] **1.1 Ticaret Malı ve Envanter:** `Item`, `Inventory` (Ağırlık kontrolü, eşya ekleme/çıkarma, bozulma).
- [x] **1.2 Kervan Modeli:** `Caravan` (Binek hayvanları, muhafızlar, işçiler, günlük erzak tüketimi, hız formülü).
- [x] **1.3 Ekonomi ve Pazar:** `City`, `Market`, `TradeCalculator` (Arz-talep ve mesafe bazlı dinamik fiyat hesabı).
- [x] **1.4 Birim Testleri / Doğrulama:** xUnit test paketi ile tüm kervan ve ticaret döngüsünün doğrulanması (4/4 test başarılı).

---

### [x] FAZ 2: Veri Mimarisi ve İçerik Tasarımı (Data Layer)
*Oyun dünyasının zenginleştirilmesi ve ScriptableObject / JSON entegrasyonu.*
- [x] **2.1 Eşya Veri Tabanı:** 14. yy mallarının ScriptableObject tanımları (`ItemDataSO`, `ItemDatabaseSO`).
- [x] **2.2 Şehirler ve Harita Verisi:** Şehirlerin coğrafi ve ekonomik profilleri (`CityDataSO`, `CityDatabaseSO`).
- [x] **2.3 Tarihi Olay Verisi:** `HistoricalEventDataSO`, `EventDatabaseSO` (Veba, Beylik çatışmaları, kıtlık, istihbarat).

---

### [ ] FAZ 3: Harita, Rota Seçimi ve Kullanıcı Arayüzü (UI)
*Graf tabanlı seyahat ve modern 2D flat arayüz.*
- [ ] **3.1 Harita Graf Sistemi (Node-Graph):** Şehir düğümleri ve rota bağlantıları (Dağ Yolu vs Vadi Yolu).
- [ ] **3.2 UI Tema & Temel Ekranlar:** Parşömen tarzı UI, Üst Bilgi Barı (Akçe, Erzak, Kapasite).
- [ ] **3.3 Pazar Ekranı:** Alım-satım arayüzü, sepet mekanizması.
- [ ] **3.4 Kervan Yönetim Ekranı:** Muhafız kiralama, binek hayvanı satın alma, erzak depolama.

---

### [ ] FAZ 4: Olaylar, Karşılaşmalar ve İstihbarat
*Rastlantısal olaylar, taktiksel kararlar ve casusluk mekaniği.*
- [ ] **4.1 Karşılaşma Motoru (Event Engine):** Seyahat durum makinesi (State Machine), günlük risk zar hesapları.
- [ ] **4.2 Tehdit Çözüm Sistemi (Strategy Pattern):** Savaş, Rüşvet, İkna, Kaçış sonuç hesapları.
- [ ] **4.3 İstihbarat ve Görev Sistemi:** Osmanlı Beyliği'ne bilgi aktarma, gizli mektup taşıma görevleri.

---

### [ ] FAZ 5: İyileştirme (Game Feel), Ses, Kayıt ve Mobil Çıkış
- [ ] **5.1 Kayıt Sistemi (Save/Load):** `System.IO` ile JSON tabanlı kayıt/yükleme.
- [ ] **5.2 Ses & Müzik:** `AudioManager` (Ney, kopuz, pazar ambiyansı, sikke sesleri).
- [ ] **5.3 UI Animasyonları & Game Feel:** DOTween ile akıcı geçişler.
- [ ] **5.4 Mobil Optimizasyon:** Dokunmatik kontroller ve Android/iOS derlemesi.
