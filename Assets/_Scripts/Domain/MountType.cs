namespace Kervan.Domain
{
    /// <summary>
    /// Kervanda yük taşımak ve seyahat etmek için kullanılan binek ve yük hayvanları.
    /// </summary>
    public enum MountType
    {
        Camel, // Deve: Yüksek taşıma kapasitesi (200 kg), düşük su/yem ihtiyacı, orta hız.
        Mule,  // Katır: Orta kapasite (120 kg), dayanıklı dağ hayvanı, zorlu arazide hızlı.
        Horse  // At: Düşük yük kapasitesi (80 kg), yüksek hız, yüksek yem maliyeti, muhafızlar için ideal.
    }
}
