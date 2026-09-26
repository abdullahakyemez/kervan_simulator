namespace Kervan.Domain
{
    /// <summary>
    /// Kervan rotasının geçtiği coğrafi arazi türleri.
    /// Farklı arazilere göre kervan hızı, hayvanların yıpranması ve kaza riski değişir.
    /// </summary>
    public enum TerrainType
    {
        PavedRoad,    // Taş Döşeli Kervan Yolu: Güvenli, hayvanlar rahat ilerler, düşük kaza riski.
        MountainPass, // Dağ Geçidi: Dik ve yorucu, katırlar avantajlı, çığ/fırtına ve kaza riski yüksek.
        Valley,       // Vadi ve Nehir Boyu: Su ve otlak bol, sel ve vadi pususu riski mevcut.
        Steppe,       // Bozkır: Düz ama su kıt, develer avantajlı, sıcak/soğuk rüzgarlar etkili.
        Forest        // Sık Ormanlık: Görüş kısıtlı, haydut ve vahşi kurt pususu riski en yüksek.
    }
}
