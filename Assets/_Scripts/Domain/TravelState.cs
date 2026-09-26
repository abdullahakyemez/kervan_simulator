namespace Kervan.Domain
{
    /// <summary>
    /// Kervanın seyahat ve oyun akışı esnasındaki anlık durumları (FSM - Finite State Machine).
    /// </summary>
    public enum TravelState
    {
        InCity,      // Şehirde: Pazar, kervan yönetimi ve dinlenme
        OnTheRoad,   // Yolda: Günlük kilometre ilerlemesi ve tüketim
        InEncounter, // Karşılaşma anı: Haydut, fırtına veya kurt pususu çözümü bekleniyor
        Arrived      // Hedef şehre varıldı: Şehre giriş hazırlığı
    }

    /// <summary>
    /// Seyahat esnasında karşılaşılabilecek tarihi ve coğrafi tehlike/olay türleri.
    /// </summary>
    public enum EncounterType
    {
        BanditAmbush,        // Celali / Aşiret Haydutları Pususu (Savaş veya Rüşvet)
        WolfPack,            // Dağ Kurtları Saldırısı (Savaş veya Kaçış)
        StormWeather,        // Kar Fırtınası / Şiddetli Yağmur (Sığınma veya İlerleme)
        BrokenBridge,        // Yıkılmış Köprü / Sel (Tamir, Geçiş Vergisi veya Dağdan Dolaşma)
        MerchantInDistress   // Yolda Mahsur Kalmış Tüccar (Yardım etme / Mal Takası)
    }
}
