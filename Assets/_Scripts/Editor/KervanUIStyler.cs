using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Kervan.Editor
{
    /// <summary>
    /// Unity Editöründe tek bir tıklama ile sahnedeki tüm UI elemanlarını
    /// profesyonel YATAY (Landscape - 16:9 / 1920x1080) mobil/tablet formatında
    /// 14. yüzyıl parşömen/deri estetiğinde mükemmel hizalayan ve renklendiren sihirbaz.
    /// </summary>
    public static class KervanUIStyler
    {
        // 14. Yüzyıl Osmanlı / Selçuklu Tarihi Renk Paleti
        private static readonly Color ColorDarkWalnut = new Color(0.14f, 0.10f, 0.08f, 0.98f);    // Koyu ceviz / ahşap zemin
        private static readonly Color ColorCardLeather = new Color(0.19f, 0.14f, 0.11f, 0.95f);   // Deri / kart zemini
        private static readonly Color ColorParchment = new Color(0.95f, 0.91f, 0.83f, 1.0f);     // Parşömen beyazı / fildişi
        private static readonly Color ColorGold = new Color(0.96f, 0.78f, 0.28f, 1.0f);          // Osmanlı sikkesi altın sarısı
        private static readonly Color ColorSoftGreen = new Color(0.35f, 0.75f, 0.40f, 1.0f);     // Erzak yeşili
        private static readonly Color ColorBuyButton = new Color(0.20f, 0.45f, 0.25f, 1.0f);     // Alış butonu yeşili
        private static readonly Color ColorSellButton = new Color(0.48f, 0.22f, 0.20f, 1.0f);    // Satış butonu kiremit/kırmızı
        private static readonly Color ColorNavButton = new Color(0.28f, 0.20f, 0.15f, 1.0f);     // Menü butonu kahvesi

        [MenuItem("Kervan/Arayüzü Otomatik Tasarla & Hizala (Landscape 16:9)", false, 1)]
        public static void PolishAllUI()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("Uyarı", "Sahnede Canvas bulunamadı!", "Tamam");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject, "Polish Kervan Landscape UI");

            // 1. Kamerayı Koyu Tarihi Tona Çevir
            var cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.11f, 0.08f, 0.06f, 1.0f);
            }

            // 2. Canvas Scaler Kontrolü (1920x1080 YATAY Mobil / 16:9)
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }

            // 3. Üst Barı Düzenle (Yatay 1920 px genişliğinde)
            PolishTopStatusBar(canvas);

            // 4. Alt Barı Düzenle
            PolishBottomNavBar(canvas);

            // 5. Kervan Panelini Düzenle (Sol İstatistik - Sağ Butonlar)
            PolishCaravanPanel(canvas);

            // 6. Pazar Panelini ve Tek Satır Yatay Kartları Düzenle
            PolishMarketPanel(canvas);

            // 7. Harita Panelini Düzenle (Sol Rota Seçimi - Sağ Bilgi & İlerleme)
            PolishMapPanel(canvas);

            // Sahneyi kaydedilebilir olarak işaretle
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "Yatay Format Hazır!",
                "Kervan arayüzü başarıyla YATAY (16:9 / 1920x1080) formata uyarlandı!\n\nLütfen Unity Game penceresinde çözünürlüğü '16:9' veya '1920x1080' olarak seçtiğinizden emin olun.",
                "Harika!"
            );
        }

        private static void PolishTopStatusBar(Canvas canvas)
        {
            var topBar = canvas.transform.Find("TopStatusBar");
            if (topBar == null) return;

            var img = topBar.GetComponent<Image>();
            if (img != null) img.color = ColorDarkWalnut;

            var rect = topBar.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 1);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(0, 95); // Yatayda 95 px yükseklik
            }

            var group = topBar.GetComponent<HorizontalLayoutGroup>();
            if (group != null)
            {
                group.padding = new RectOffset(50, 50, 12, 12);
                group.spacing = 35;
                group.childAlignment = TextAnchor.MiddleCenter;
                group.childControlWidth = true;
                group.childControlHeight = true;
                group.childForceExpandWidth = true;
                group.childForceExpandHeight = true;
            }

            // Renkleri boya
            SetTextColor(topBar, "GoldText", ColorGold, 23, true, TextAlignmentOptions.Center);
            SetTextColor(topBar, "FoodText", ColorSoftGreen, 21, true, TextAlignmentOptions.Center);
            SetTextColor(topBar, "WeightText", ColorParchment, 21, false, TextAlignmentOptions.Center);
            SetTextColor(topBar, "LocationText", ColorGold, 22, true, TextAlignmentOptions.Center);
        }

        private static void PolishBottomNavBar(Canvas canvas)
        {
            var navBar = canvas.transform.Find("BottomNavBar");
            if (navBar == null) return;

            var img = navBar.GetComponent<Image>();
            if (img != null) img.color = ColorDarkWalnut;

            var rect = navBar.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 0);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(0, 95); // Yatayda 95 px yükseklik
            }

            var group = navBar.GetComponent<HorizontalLayoutGroup>();
            if (group != null)
            {
                group.padding = new RectOffset(200, 200, 10, 10);
                group.spacing = 30;
                group.childAlignment = TextAnchor.MiddleCenter;
                group.childControlWidth = true;
                group.childControlHeight = true;
                group.childForceExpandWidth = true;
                group.childForceExpandHeight = true;
            }

            // Butonları güzelleştir
            foreach (Transform child in navBar)
            {
                var btnImg = child.GetComponent<Image>();
                if (btnImg != null) btnImg.color = ColorNavButton;

                var txt = child.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    var tRect = txt.rectTransform;
                    tRect.anchorMin = Vector2.zero;
                    tRect.anchorMax = Vector2.one;
                    tRect.offsetMin = Vector2.zero;
                    tRect.offsetMax = Vector2.zero;
                    txt.color = ColorParchment;
                    txt.fontSize = 22;
                    txt.fontStyle = FontStyles.Bold;
                    txt.alignment = TextAlignmentOptions.Center;
                }
            }
        }

        private static void PolishCaravanPanel(Canvas canvas)
        {
            var panel = canvas.transform.Find("Panel_Kervan");
            if (panel == null) return;

            var img = panel.GetComponent<Image>();
            if (img != null) img.color = ColorDarkWalnut;

            var rect = panel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = new Vector2(40, 105);
                rect.offsetMax = new Vector2(-40, -105);
            }

            // Sol Sütun: Kervan İstatistikleri
            PositionElement(panel, "Txt_Stats", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-440, 0), new Vector2(880, 500));
            var statsText = FindChildRecursive(panel, "Txt_Stats")?.GetComponent<TextMeshProUGUI>();
            if (statsText != null)
            {
                statsText.color = ColorParchment;
                statsText.fontSize = 23;
                statsText.lineSpacing = 16;
                statsText.alignment = TextAlignmentOptions.TopLeft;
            }

            // Sağ Sütun: Aksiyon Butonları
            PositionElement(panel, "Btn_HireGuard", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(450, 100), new Vector2(550, 80));
            SetButtonColor(panel, "Btn_HireGuard", ColorBuyButton);

            PositionElement(panel, "Btn_BuyCamel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(450, 0), new Vector2(550, 80));
            SetButtonColor(panel, "Btn_BuyCamel", ColorNavButton);

            PositionElement(panel, "Btn_BuyFood", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(450, -100), new Vector2(550, 80));
            SetButtonColor(panel, "Btn_BuyFood", ColorSoftGreen * 0.7f);
        }

        private static void PolishMarketPanel(Canvas canvas)
        {
            var panel = canvas.transform.Find("Panel_Pazar");
            if (panel == null) return;

            var img = panel.GetComponent<Image>();
            if (img != null) img.color = ColorDarkWalnut;

            var rect = panel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = new Vector2(40, 105);
                rect.offsetMax = new Vector2(-40, -105);
            }

            // Başlıklar
            PositionElement(panel, "Txt_BazaarTitle", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -18), new Vector2(1800, 40));
            var title = FindChildRecursive(panel, "Txt_BazaarTitle")?.GetComponent<TextMeshProUGUI>();
            if (title != null)
            {
                title.color = ColorGold;
                title.fontSize = 28;
                title.fontStyle = FontStyles.Bold;
                title.alignment = TextAlignmentOptions.Center;
            }

            PositionElement(panel, "Txt_BazaarSubtitle", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -52), new Vector2(1800, 28));
            var subtitle = FindChildRecursive(panel, "Txt_BazaarSubtitle")?.GetComponent<TextMeshProUGUI>();
            if (subtitle != null)
            {
                subtitle.color = new Color(0.85f, 0.78f, 0.68f);
                subtitle.fontSize = 17;
                subtitle.fontStyle = FontStyles.Italic;
                subtitle.alignment = TextAlignmentOptions.Center;
            }

            // Scroll View & Content
            var scroll = panel.Find("Scroll_Goods");
            if (scroll != null)
            {
                var sRect = scroll.GetComponent<RectTransform>();
                if (sRect != null)
                {
                    sRect.anchorMin = Vector2.zero;
                    sRect.anchorMax = Vector2.one;
                    sRect.offsetMin = new Vector2(20, 15);
                    sRect.offsetMax = new Vector2(-20, -85);
                }
            }

            var content = panel.Find("Scroll_Goods/Viewport/Content");
            if (content != null)
            {
                var vGroup = content.GetComponent<VerticalLayoutGroup>();
                if (vGroup != null)
                {
                    vGroup.padding = new RectOffset(10, 10, 10, 10);
                    vGroup.spacing = 10;
                    vGroup.childControlWidth = true;
                    vGroup.childControlHeight = false;
                    vGroup.childForceExpandWidth = true;
                    vGroup.childForceExpandHeight = false;
                }

                // Satır Şablonunu Düzenle (ItemEntry_Template)
                var template = content.Find("ItemEntry_Template");
                if (template != null)
                {
                    PolishItemEntryTemplate(template);
                }
            }
        }

        private static void PolishItemEntryTemplate(Transform template)
        {
            var templateRect = template.GetComponent<RectTransform>();
            if (templateRect != null)
            {
                templateRect.sizeDelta = new Vector2(templateRect.sizeDelta.x, 85); // Yatayda 85 px satır yüksekliği
            }

            var tImg = template.GetComponent<Image>();
            if (tImg != null) tImg.color = ColorCardLeather;

            // Yatayda Tek Satır Hizalaması (1780 px net genişlik için sol taraftan sağa doğru düzen)
            PositionElement(template, "Txt_Name", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(30, 14), new Vector2(310, 30));
            SetTextColor(template, "Txt_Name", ColorGold, 21, true, TextAlignmentOptions.Left);

            PositionElement(template, "Txt_Category", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(30, -16), new Vector2(310, 24));
            SetTextColor(template, "Txt_Category", new Color(0.82f, 0.76f, 0.68f), 15, false, TextAlignmentOptions.Left);

            PositionElement(template, "Txt_BuyPrice", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(360, 0), new Vector2(190, 32));
            SetTextColor(template, "Txt_BuyPrice", ColorSoftGreen, 19, true, TextAlignmentOptions.Left);

            PositionElement(template, "Txt_SellPrice", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(570, 0), new Vector2(190, 32));
            SetTextColor(template, "Txt_SellPrice", ColorGold * 0.9f, 19, true, TextAlignmentOptions.Left);

            PositionElement(template, "Txt_MarketStock", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(780, 0), new Vector2(190, 32));
            SetTextColor(template, "Txt_MarketStock", ColorParchment, 17, false, TextAlignmentOptions.Left);

            PositionElement(template, "Txt_CaravanAmount", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(990, 0), new Vector2(190, 32));
            SetTextColor(template, "Txt_CaravanAmount", ColorGold, 17, true, TextAlignmentOptions.Left);

            // Butonlar Sağ Kenara Yaslanır (Hiçbiri üst üste binmez)
            PositionElement(template, "Btn_BuyOne", new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-395, 0), new Vector2(110, 52));
            SetButtonColor(template, "Btn_BuyOne", ColorBuyButton);

            PositionElement(template, "Btn_BuyFive", new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-275, 0), new Vector2(110, 52));
            SetButtonColor(template, "Btn_BuyFive", ColorBuyButton * 0.88f);

            PositionElement(template, "Btn_SellOne", new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-155, 0), new Vector2(110, 52));
            SetButtonColor(template, "Btn_SellOne", ColorSellButton);

            PositionElement(template, "Btn_SellAll", new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-30, 0), new Vector2(115, 52));
            SetButtonColor(template, "Btn_SellAll", ColorSellButton * 0.85f);
        }

        private static void PolishMapPanel(Canvas canvas)
        {
            var panel = canvas.transform.Find("Panel_Harita");
            if (panel == null) return;

            var img = panel.GetComponent<Image>();
            if (img != null) img.color = ColorDarkWalnut;

            var rect = panel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = new Vector2(40, 105);
                rect.offsetMax = new Vector2(-40, -105);
            }

            // 1. Rota Seçim Alt Paneli (SubPanel_RouteSelection) - Sol ve Sağ Sütun Olarak Düzenlenir
            var routeSub = FindChildRecursive(panel, "SubPanel_RouteSelection");
            if (routeSub != null)
            {
                StretchFull(routeSub);

                // SOL SÜTUN: Rota Seçenekleri
                PositionElement(routeSub, "Txt_CurrentLocation", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-450, 200), new Vector2(880, 50));
                SetTextColor(routeSub, "Txt_CurrentLocation", ColorGold, 25, true, TextAlignmentOptions.Center);

                PositionElement(routeSub, "Btn_RouteHighway", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-450, 110), new Vector2(880, 75));
                SetButtonColor(routeSub, "Btn_RouteHighway", ColorBuyButton);

                PositionElement(routeSub, "Btn_RouteMountain", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-450, 15), new Vector2(880, 75));
                SetButtonColor(routeSub, "Btn_RouteMountain", ColorSellButton);

                PositionElement(routeSub, "Btn_StartJourney", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-450, -110), new Vector2(880, 85));
                SetButtonColor(routeSub, "Btn_StartJourney", ColorGold * 0.85f);

                // SAĞ SÜTUN: Rota Detayları
                PositionElement(routeSub, "Txt_RouteTitle", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(450, 170), new Vector2(880, 45));
                SetTextColor(routeSub, "Txt_RouteTitle", ColorGold, 24, true, TextAlignmentOptions.Center);

                PositionElement(routeSub, "Txt_RouteDetails", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(450, -10), new Vector2(880, 260));
                var detailsTxt = FindChildRecursive(routeSub, "Txt_RouteDetails")?.GetComponent<TextMeshProUGUI>();
                if (detailsTxt != null)
                {
                    detailsTxt.color = ColorParchment;
                    detailsTxt.fontSize = 21;
                    detailsTxt.lineSpacing = 14;
                    detailsTxt.alignment = TextAlignmentOptions.TopLeft;
                }
            }

            // 2. Seyahat İlerleme Alt Paneli (SubPanel_TravelProgress)
            var travelSub = FindChildRecursive(panel, "SubPanel_TravelProgress");
            if (travelSub != null)
            {
                StretchFull(travelSub);

                PositionElement(travelSub, "Txt_TravelStatus", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(1200, 60));
                SetTextColor(travelSub, "Txt_TravelStatus", ColorGold, 28, true, TextAlignmentOptions.Center);

                PositionElement(travelSub, "Slider_Progress", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(1000, 40));

                PositionElement(travelSub, "Btn_AdvanceDay", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -110), new Vector2(500, 80));
                SetButtonColor(travelSub, "Btn_AdvanceDay", ColorSoftGreen * 0.75f);
            }
        }

        private static void StretchFull(Transform target)
        {
            var rect = target.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }
        }

        private static Transform? FindChildRecursive(Transform parent, string childName)
        {
            if (parent.name == childName) return parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                var result = FindChildRecursive(parent.GetChild(i), childName);
                if (result != null) return result;
            }

            return null;
        }

        private static void PositionElement(Transform parent, string childName, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, Vector2? customPivot = null)
        {
            var child = FindChildRecursive(parent, childName);
            if (child == null) return;

            var rect = child.GetComponent<RectTransform>();
            if (rect == null) return;

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;

            // Eğer özel pivot verilmediyse, anchor'a göre ideal pivotu otomatik belirle:
            // Sol kenara yaslıysa pivot.x = 0, sağ kenara yaslıysa pivot.x = 1, ortalıysa 0.5
            float pivotX = anchorMin.x == 0 && anchorMax.x == 0 ? 0f : (anchorMin.x == 1 && anchorMax.x == 1 ? 1f : 0.5f);
            float pivotY = anchorMin.y == 0 && anchorMax.y == 0 ? 0f : (anchorMin.y == 1 && anchorMax.y == 1 ? 1f : 0.5f);

            rect.pivot = customPivot ?? new Vector2(pivotX, pivotY);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
        }

        private static void SetTextColor(Transform parent, string childName, Color color, float size, bool isBold, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var child = FindChildRecursive(parent, childName);
            if (child == null) return;

            var txt = child.GetComponent<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.color = color;
                txt.fontSize = size;
                txt.alignment = alignment;
                if (isBold) txt.fontStyle |= FontStyles.Bold;
            }
        }

        private static void SetButtonColor(Transform parent, string buttonName, Color color, int fontSize = 21)
        {
            var btn = FindChildRecursive(parent, buttonName);
            if (btn == null) return;

            var img = btn.GetComponent<Image>();
            if (img != null) img.color = color;

            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                var txtRect = txt.rectTransform;
                txtRect.anchorMin = Vector2.zero;
                txtRect.anchorMax = Vector2.one;
                txtRect.offsetMin = new Vector2(6, 4);
                txtRect.offsetMax = new Vector2(-6, -4);
                txt.color = ColorParchment;
                txt.fontSize = fontSize;
                txt.fontStyle |= FontStyles.Bold;
                txt.alignment = TextAlignmentOptions.Center;
                txt.enableWordWrapping = true;
                txt.enableAutoSizing = true;
                txt.fontSizeMin = 13;
                txt.fontSizeMax = fontSize;
            }
        }
    }
}
