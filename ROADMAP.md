# KERVAN: 14. YÜZYIL ANADOLU TÜCCARI
## Canlı Oyun Geliştirme ve Üretim Yol Haritası (Master Production Roadmap)

Bu belge, oyunumuzun ilk C# satırından mağazaya (Google Play / App Store) çıkışına kadar olan tüm adımların canlı takip rehberidir. Tamamlanan her adım `[x]` olarak güncellenir.

---

### [x] BÖLÜM I: TEMEL ÇEKİRDEK & C# MİMARİSİ (TAMAMLANDI)
- [x] **Faz 0:** Unity 6 2D URP ortamı, kurumsal klasör yapısı ve Git deposu kurulumu.
- [x] **Faz 1:** Saf C# Çekirdek Mantığı (`Item`, `Inventory`, `Caravan`, `City`, `Market`, `TradeCalculator`).
  - *Doğrulama:* 8/8 xUnit birim testi sıfır hata ile tamamlandı.
- [x] **Faz 2:** Unity Veri Mimarisi (`ItemDataSO`, `CityDataSO`, `HistoricalEventDataSO`, Veritabanları).
- [x] **Faz 3.1:** Graf Tabanlı Rota Sistemi (`MapGraph`, `MapRoute`, `TerrainType`).
- [x] **Faz 3.2:** Üst Bilgi Barı & Navigasyon (`GameManager`, `TopStatusBarUI`, `ScreenNavigationManager`).
- [x] **Faz 3.3:** Pazar Mantığı (`MarketScreenUI`, `MarketItemEntryUI`).
- [x] **Faz 3.4:** Kervan Yönetimi (`CaravanScreenUI` - Muhafız kiralama, deve alma, erzak depolama).
- [x] **Faz 4:** Karşılaşma Durum Makinesi (`TravelStateMachine`, Strategy Pattern Savaş/Rüşvet/Kaçış/İkna, Osmanlı Casusluk Sistemi).
- [x] **Faz 5:** Dosya Kayıt Sistemi (`SaveManager`), Ses Sistemi (`AudioManager`), Mobil Çentik Uyumu (`SafeAreaHandler`).

---

### [ ] BÖLÜM II: GÖRSEL SAHNE & OYNANIŞ ENTEGRASYONU (ŞU AN BURADAYIZ)
*Bu bölümde yazdığımız tüm C# mekaniklerini ekranda tek tek oynanabilir hale getiriyoruz.*

#### [x] AŞAMA 1: Pazar (Bedesten) Ekranının Kurulması
- [x] 1.1 `Panel_Pazar` arayüzünün Canvas içine yerleştirilmesi (Kaydırılabilir ScrollView listesi).
- [x] 1.2 `MarketItemEntryUI` pazar satır prefab'ının oluşturulması (Alış/Satış butonları, pazar stoğu).
- [x] 1.3 Bursa Bedesteni'nden İpek, Buğday ve Şam Çeliği alıp-satma döngüsünün test edilmesi.

#### [x] AŞAMA 2: 2D Anadolu Haritası ve Rota Seyahati
- [x] 2.1 `Panel_Harita` ekranının kurulması (Bursa, İznik, Söğüt, Kütahya, Konya düğüm noktaları).
- [x] 2.2 Rota Seçim Kartı: "Güvenli Taş Yol" vs "Kestirme Dağ Patikası" (Mesafe ve tehlike kıyası).
- [x] 2.3 "Yola Çık" butonu ve seyahat ilerleme çubuğu (`TravelProgressBar` - Yolda geçen günlerin akışı).

#### [ ] AŞAMA 3: Tehlike & Karşılaşma Karar Ekranı (Pusu / Kurt Sürüsü)
- [ ] 3.1 `EncounterDialogUI` diyalog penceresinin Canvas'a eklenmesi.
- [ ] 3.2 Haydut pususu anında 4 strateji butonunun bağlanması: [Savaş], [Haraç Ver], [Hızla Kaç], [İkna Et].
- [ ] 3.3 Karşılaşma sonucu bildirimi (Ganimet kazanma / muhafız kaybı ve moral etkisi).

#### [ ] AŞAMA 4: Osmanlı İstihbarat & Ferman Görevleri Ekranı
- [ ] 4.1 `Panel_Istihbarat` ekranının kurulması (Tarihi mektup/ferman tasarımı).
- [ ] 4.2 Konya veya İznik'teki casusluk görevini kabul etme, bilgiyi toplama ve Bursa'ya teslim edip ödül alma.

---

### [ ] BÖLÜM III: SANAT, SES & ATMOSFER (CİLA / POLISH)
*Oyunu amatör görünümden kurtarıp 14. yüzyıl minyatür sanatına dönüştürme.*
- [ ] **Görsel Tasarım:**
  - Parşömen, eski kağıt ve deri dokulu UI arka planlarının uygulanması.
  - Ticaret malları için 2D flat/minyatür ikonların eklenmesi (İpek topu, çuval, kılıç, deve).
  - Tarihi Osmanlı/Selçuklu tarzı serif yazı tipinin (Font) TextMeshPro'ya aktarılması.
- [ ] **Ses ve Atmosfer:**
  - Ney, tambur ve bendir arka plan ambient döngülerinin `AudioManager`'a eklenmesi.
  - Sikke şıngırtısı, deve adımları ve kılıç çekme ses efektlerinin bağlanması.

---

### [ ] BÖLÜM IV: MOBİL DERLEME & YAYIN (RELEASE)
- [ ] **Kayıt Testi:** Oyundan çıkıp tekrar girildiğinde kervanın ve paranın kaldığı yerden devam etmesi.
- [ ] **Android APK Çıktısı:** Unity Build Settings üzerinden ilk çalıştırılabilir Android paketinin alınması.
- [ ] **Dokunmatik Test:** Telefona yükleyip tek elle akıcı oynanışın doğrulanması.
