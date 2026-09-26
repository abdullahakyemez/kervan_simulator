namespace Kervan.Domain
{
    /// <summary>
    /// 14. yüzyıl ticaret mallarının kategorileri.
    /// enum (numaralandırma), ilgili sabitleri tek bir çatı altında toplayan tür güvenli (type-safe) bir yapıdır.
    /// </summary>
    public enum ItemCategory
    {
        Food,         // Erzak & Gıda (Buğday, zeytinyağı, tuz, kurutulmuş et)
        Textile,      // Kumaş & Dokuma (Bursa ipeği, yün, keçe)
        Metal,        // Maden & Silah (Şam çeliği, demir külçe, bakır kap)
        Pottery,      // Seramik & Zanaat (İznik çinisi, çömlek)
        Luxury,       // Lüks & Baharat (Doğu baharatları, safran, kehribar)
        RawMaterial   // Hammadde (Kereste, deri, balmumu)
    }
}
