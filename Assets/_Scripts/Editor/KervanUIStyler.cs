using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Kervan.Editor
{
    /// <summary>
    /// Unity Editöründe tek bir tıklama ile sahnedeki tüm UI elemanlarını
    /// profesyonel 14. yüzyıl parşömen/deri estetiğinde mükemmel hizalayan ve renklendiren sihirbaz.
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

        [MenuItem("Kervan/Arayüzü Otomatik Tasarla & Hizala (Polish UI)", false, 1)]
        public static void PolishAllUI()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("Uyarı", "Sahnede Canvas bulunamadı!", "Tamam");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject, "Polish Kervan UI");

            // 1. Kamerayı Koyu Tarihi Tona Çevir
            var cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.11f, 0.08f, 0.06f, 1.0f);
            }

            // 2. Canvas Scaler Kontrolü (1080x1920 Dikey Mobil)
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;
            }

            // 3. Üst Barı Düzenle
            PolishTopStatusBar(canvas);

            // 4. Alt Barı Düzenle
            PolishBottomNavBar(canvas);

            // 5. Kervan Panelini Düzenle
            PolishCaravanPanel(canvas);

            // 6. Pazar Panelini ve Satır Şablonunu Düzenle
            PolishMarketPanel(canvas);

            // 7. Harita Panelini Düzenle
            PolishMapPanel(canvas);

            // Sahneyi kaydedilebilir olarak işaretle
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "Tebrikler!",
                "Kervan arayüzü başarıyla profesyonel 14. yüzyıl tasarım standartlarına göre hizalandı ve renklendirildi!\n\nŞimdi Game penceresinde görünümü inceleyebilirsiniz.",
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
                rect.sizeDelta = new Vector2(0, 160);
            }

            var group = topBar.GetComponent<HorizontalLayoutGroup>();
            if (group != null)
            {
                group.padding = new RectOffset(25, 25, 20, 20);
                group.spacing = 15;
                group.childAlignment = TextAnchor.MiddleCenter;
                group.childControlWidth = true;
                group.childControlHeight = true;
                group.childForceExpandWidth = true;
                group.childForceExpandHeight = true;
            }

            // Renkleri boya
            SetTextColor(topBar, "GoldText", ColorGold, 26, true);
            SetTextColor(topBar, "FoodText", ColorSoftGreen, 24, true);
            SetTextColor(topBar, "WeightText", ColorParchment, 24, false);
            SetTextColor(topBar, "LocationText", ColorGold, 24, true);
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
                rect.sizeDelta = new Vector2(0, 150);
            }

            var group = navBar.GetComponent<HorizontalLayoutGroup>();
            if (group != null)
            {
                group.padding = new RectOffset(20, 20, 15, 15);
                group.spacing = 15;
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
                    txt.color = ColorParchment;
                    txt.fontSize = 24;
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
                rect.offsetMin = new Vector2(20, 155);
                rect.offsetMax = new Vector2(-20, -165);
            }

            // Metinler ve Konumlandırma
            PositionElement(panel, "Txt_Stats", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -30), new Vector2(960, 250));
            var statsText = FindChildRecursive(panel, "Txt_Stats")?.GetComponent<TextMeshProUGUI>();
            if (statsText != null)
            {
                statsText.color = ColorParchment;
                statsText.fontSize = 22;
                statsText.lineSpacing = 12;
                statsText.alignment = TextAlignmentOptions.TopLeft;
            }

            // Buton Konumları
            PositionElement(panel, "Btn_HireGuard", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(-240, -320), new Vector2(460, 85));
            SetButtonColor(panel, "Btn_HireGuard", ColorBuyButton);

            PositionElement(panel, "Btn_BuyCamel", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(240, -320), new Vector2(460, 85));
            SetButtonColor(panel, "Btn_BuyCamel", ColorNavButton);

            PositionElement(panel, "Btn_BuyFood", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -430), new Vector2(520, 85));
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
                rect.offsetMin = new Vector2(20, 155);
                rect.offsetMax = new Vector2(-20, -165);
            }

            // Başlıklar
            var title = panel.Find("Txt_BazaarTitle")?.GetComponent<TextMeshProUGUI>();
            if (title != null)
            {
                title.color = ColorGold;
                title.fontSize = 30;
                title.fontStyle = FontStyles.Bold;
                title.alignment = TextAlignmentOptions.Center;
            }

            var subtitle = panel.Find("Txt_BazaarSubtitle")?.GetComponent<TextMeshProUGUI>();
            if (subtitle != null)
            {
                subtitle.color = new Color(0.85f, 0.78f, 0.68f);
                subtitle.fontSize = 19;
                subtitle.fontStyle = FontStyles.Italic;
                subtitle.alignment = TextAlignmentOptions.Center;
            }

            // Scroll View & Content
            var content = panel.Find("Scroll_Goods/Viewport/Content");
            if (content != null)
            {
                var vGroup = content.GetComponent<VerticalLayoutGroup>();
                if (vGroup != null)
                {
                    vGroup.padding = new RectOffset(10, 10, 10, 10);
                    vGroup.spacing = 15;
                    vGroup.childControlWidth = true;
                    vGroup.childControlHeight = false; // Yüksekliği serbest bırak!
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
                templateRect.sizeDelta = new Vector2(templateRect.sizeDelta.x, 130);
            }

            var tImg = template.GetComponent<Image>();
            if (tImg != null) tImg.color = ColorCardLeather;

            // 1. SOL BİLGİ ALANI (İsim, Kategori, Fiyatlar, Stok)
            PositionElement(template, "Txt_Name", new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -22), new Vector2(400, 32));
            SetTextColor(template, "Txt_Name", ColorGold, 24, true);

            PositionElement(template, "Txt_Category", new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -50), new Vector2(400, 24));
            SetTextColor(template, "Txt_Category", new Color(0.82f, 0.76f, 0.68f), 17, false);

            PositionElement(template, "Txt_BuyPrice", new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -78), new Vector2(180, 24));
            SetTextColor(template, "Txt_BuyPrice", ColorSoftGreen, 19, true);

            PositionElement(template, "Txt_SellPrice", new Vector2(0, 1), new Vector2(0, 1), new Vector2(210, -78), new Vector2(180, 24));
            SetTextColor(template, "Txt_SellPrice", ColorGold * 0.9f, 19, true);

            PositionElement(template, "Txt_MarketStock", new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -104), new Vector2(180, 22));
            SetTextColor(template, "Txt_MarketStock", ColorParchment, 17, false);

            PositionElement(template, "Txt_CaravanAmount", new Vector2(0, 1), new Vector2(0, 1), new Vector2(210, -104), new Vector2(180, 22));
            SetTextColor(template, "Txt_CaravanAmount", ColorGold, 17, true);

            // 2. SAĞ AKSİYON BUTONLARI (Alış ve Satış)
            PositionElement(template, "Btn_BuyOne", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-280, -32), new Vector2(130, 48));
            SetButtonColor(template, "Btn_BuyOne", ColorBuyButton);

            PositionElement(template, "Btn_BuyFive", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-135, -32), new Vector2(130, 48));
            SetButtonColor(template, "Btn_BuyFive", ColorBuyButton * 0.88f);

            PositionElement(template, "Btn_SellOne", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-280, -90), new Vector2(130, 48));
            SetButtonColor(template, "Btn_SellOne", ColorSellButton);

            PositionElement(template, "Btn_SellAll", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-135, -90), new Vector2(130, 48));
            SetButtonColor(template, "Btn_SellAll", ColorSellButton * 0.85f);
        }

        private static void PositionElement(Transform parent, string childName, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            var child = parent.Find(childName);
            if (child == null) return;

            var rect = child.GetComponent<RectTransform>();
            if (rect == null) return;

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMin.x == 1 ? 0 : 0, 1);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
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
                rect.offsetMin = new Vector2(20, 155);
                rect.offsetMax = new Vector2(-20, -165);
            }

            // 1. Rota Seçim Alt Paneli (SubPanel_RouteSelection)
            var routeSub = FindChildRecursive(panel, "SubPanel_RouteSelection");
            if (routeSub != null)
            {
                StretchFull(routeSub);

                // Mevcut Şehir Başlığı
                PositionElement(routeSub, "Txt_CurrentLocation", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -35), new Vector2(960, 60));
                SetTextColor(routeSub, "Txt_CurrentLocation", ColorGold, 26, true, TextAlignmentOptions.Center);

                // Rota Seçim Butonları (Yan Yana)
                PositionElement(routeSub, "Btn_RouteHighway", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(-240, -130), new Vector2(460, 85));
                SetButtonColor(routeSub, "Btn_RouteHighway", ColorBuyButton);

                PositionElement(routeSub, "Btn_RouteMountain", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(240, -130), new Vector2(460, 85));
                SetButtonColor(routeSub, "Btn_RouteMountain", ColorSellButton);

                // Rota Başlık ve Detay Kartı
                PositionElement(routeSub, "Txt_RouteTitle", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -210), new Vector2(960, 50));
                SetTextColor(routeSub, "Txt_RouteTitle", ColorGold, 24, true, TextAlignmentOptions.Center);

                PositionElement(routeSub, "Txt_RouteDetails", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -370), new Vector2(920, 260));
                var detailsTxt = FindChildRecursive(routeSub, "Txt_RouteDetails")?.GetComponent<TextMeshProUGUI>();
                if (detailsTxt != null)
                {
                    detailsTxt.color = ColorParchment;
                    detailsTxt.fontSize = 21;
                    detailsTxt.lineSpacing = 14;
                    detailsTxt.alignment = TextAlignmentOptions.TopLeft;
                }

                // Yola Çık Butonu
                PositionElement(routeSub, "Btn_StartJourney", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -560), new Vector2(560, 95));
                SetButtonColor(routeSub, "Btn_StartJourney", ColorGold * 0.85f);
            }

            // 2. Seyahat İlerleme Alt Paneli (SubPanel_TravelProgress)
            var travelSub = FindChildRecursive(panel, "SubPanel_TravelProgress");
            if (travelSub != null)
            {
                StretchFull(travelSub);

                PositionElement(travelSub, "Txt_TravelStatus", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -160), new Vector2(960, 70));
                SetTextColor(travelSub, "Txt_TravelStatus", ColorGold, 28, true, TextAlignmentOptions.Center);

                PositionElement(travelSub, "Slider_Progress", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -260), new Vector2(850, 45));

                PositionElement(travelSub, "Btn_AdvanceDay", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -390), new Vector2(520, 95));
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

        private static void SetButtonColor(Transform parent, string buttonName, Color color)
        {
            var btn = FindChildRecursive(parent, buttonName);
            if (btn == null) return;

            var img = btn.GetComponent<Image>();
            if (img != null) img.color = color;

            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.color = ColorParchment;
                txt.fontSize = 24;
                txt.fontStyle |= FontStyles.Bold;
                txt.alignment = TextAlignmentOptions.Center;
            }
        }
    }
}
