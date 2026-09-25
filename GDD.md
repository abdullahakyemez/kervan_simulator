# KERVAN: 14. YÜZYIL ANADOLU TÜCCARI
## Game Design Document (Oyun Tasarım Belgesi)

---

### 1. Vizyon ve Konsept
* **Zaman Dilimi:** 14. Yüzyıl (1300'ler) - Beylikler Dönemi ve Osmanlı Beyliği'nin Söğüt/Bursa hattındaki yükselişi.
* **Mekân:** Anadolu (Söğüt, İznik, Bursa, Kütahya, Konya, Trabzon, Sinop, Kastamonu vb.).
* **Karakter:** Osmanlı mensubu bir Türk tüccar. Hem ticaret zekası hem de gerektiğinde kılıç kullanabilen savaşçı kökenli bir alp/gazi karakteri.
* **Ana Tür:** 2D Ticaret Simülasyonu, Rota & Kervan Yönetimi, Taktiksel Karşılaşmalar ve Hikayeli Rol Yapma (RPG).
* **Görsel Stil:** 2D Flat / Modern Vektörel Selçuklu-Osmanlı Minyatür & Parşömen estetiği (Pentiment, Bad North, Reigns referansı).

---

### 2. Temel Mekanikler

#### A. Kervan ve Envanter Yönetimi
* **Binek ve Taşıma:** Deve, katır, at ve arabalar. Her birinin taşıma kapasitesi, hız ve yem maliyeti farklıdır.
* **Mürettebat:**
  * Tüccar (Oyuncu - Savaş ve ticaret meziyetleri geliştirilebilir).
  * İşçiler/Kervancılar (Yükleme, boşaltma, kervan bakım hızı).
  * Muhafızlar (Haydut ve vahşi hayvan baskınlarında savunma gücü, günlük ulufe/maaş maliyeti).
* **Erzak ve İhtiyaçlar:** Günlük un, kurutulmuş et, su tüketimi. Erzak biterse kervan morali düşer, kaçışlar başlar.

#### B. Ekonomi ve Ticaret Sistemi
* **Döneme Uygun Mallar:** İpek, Baharat, Şam Çeliği, Tahıl, Deri, Yün, Çömlek, Zeytinyağı, Tuz, Ahşap.
* **Arz-Talep Formülü:** 
  * Şehirlerin üretim alanları vardır (Örn: Bursa - İpek, İznik - Çini/Seramik, Konya - Tahıl).
  * Uzaklık, kıtlık, savaş durumu ve mevsimler fiyatları dinamik etkiler.
* **Bozulma ve Ağırlık:** Erzak ve bazı hassas gıdalar zamanla bozulabilir, kırılabilir veya nemden zarar görebilir.

#### C. Harita, Rota ve Seyahat Mekaniği
* **Düğüm Tabanlı Harita (Node-Based Map):** Şehirlerarası yollar düğümlerle bağlıdır.
* **Rota Tercihleri:**
  * *Güvenli / Taş Döşeli Yol:* Uzun, masraflı (geçiş vergisi), düşük haydut riski.
  * *Dağ / Vadi Patikası:* Kısa, hızlı, fakat kaza, vahşi hayvan ve pusu riski yüksek.
* **Hava Şartları:** Kar fırtınası, çamur/aşırı yağmur, kavurucu sıcak. Rota süresini ve kervanın sağlığını etkiler.

#### D. Olaylar ve Karşılaşmalar (Encounters)
* **Tehditler:** Celali/aşiret haydutları, dağ kurtları, tefeciler, bozuk köprüler, salgın hastalıklar.
* **Çözüm Seçenekleri:**
  * Savaş (Muhafız gücüne ve tüccarın savaş yeteneğine bağlı zar/hesap).
  * Rüşvet / Mal Paylaşımı (Malların %20'sini verip canı kurtarmak).
  * İkna / Tehdit (Karizma ve şöhret puanına bağlı).
  * Kaçış (Kervanın hızına ve yük durumuna bağlı).

#### E. Tarihi Olaylar ve İstihbarat (Casusluk)
* **Dönemin Olayları:** Veba salgını, Bizans tekfurları ile çatışmalar, beylikler arası gerilimler, Ahilik teşkilatı ilişkileri.
* **İstihbarat Mekaniği:**
  * Osmanlı Beyliği adına şehirlerde bilgi toplama (Beyliklerin askeri durumu, halkın nabzı).
  * Gizli ferman ve mektupları sınır boylarına kaçırma görevleri.
  * Bu görevlerden kazanılan "Nüfuz / İtibar" ile askeri imtiyazlar ve vergi muafiyetleri elde etme.

---

### 3. Kullanıcı Deneyimi ve Arayüz (UI)
* Parşömen dokulu, altın yaldız ve çini mavisi detaylar.
* Sade, karmaşık olmayan dokunmatik dostu mobil butonlar.
* Kervanın durumunu anlık gösteren üst gösterge: Altın (Akçe), Erzak, Ağırlık/Kapasite, Gün ve Moral.
