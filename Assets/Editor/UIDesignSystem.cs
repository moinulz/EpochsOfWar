#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

public static class UIDesignSystem
{
    public static class Colors
    {
        public static readonly Color PrimaryDark = new Color(0.08f, 0.12f, 0.18f, 1f);      
        public static readonly Color SecondaryDark = new Color(0.12f, 0.15f, 0.20f, 1f);    
        public static readonly Color AccentGold = new Color(0.85f, 0.65f, 0.13f, 1f);       
        public static readonly Color AccentGoldLight = new Color(1f, 0.85f, 0.4f, 1f);      
        public static readonly Color TextPrimary = new Color(0.95f, 0.95f, 0.95f, 1f);      
        public static readonly Color TextSecondary = new Color(0.8f, 0.8f, 0.8f, 1f);       
        public static readonly Color ButtonPrimary = new Color(0.15f, 0.25f, 0.35f, 1f);    
        public static readonly Color ButtonSecondary = new Color(0.25f, 0.20f, 0.15f, 1f);  
        public static readonly Color ButtonDanger = new Color(0.45f, 0.15f, 0.10f, 1f);     
        public static readonly Color ButtonSuccess = new Color(0.15f, 0.35f, 0.20f, 1f);    
    }

    public static class Typography
    {
        public const int TitleLarge = 48;      
        public const int TitleMedium = 36;     
        public const int TitleSmall = 28;      
        public const int BodyLarge = 20;       
        public const int BodyMedium = 16;      
        public const int BodySmall = 14;       
        public const int Caption = 12;         
    }

    public static class Spacing
    {
        public const float XXLarge = 48f;
        public const float XLarge = 32f;
        public const float Large = 24f;
        public const float Medium = 16f;
        public const float Small = 12f;
        public const float XSmall = 8f;
        public const float XXSmall = 4f;
    }

    public static class Sizes
    {
        public static readonly Vector2 ButtonLarge = new Vector2(320f, 64f);      
        public static readonly Vector2 ButtonMedium = new Vector2(240f, 56f);     
        public static readonly Vector2 ButtonSmall = new Vector2(120f, 48f);      
        public static readonly Vector2 TouchTarget = new Vector2(48f, 48f);       
    }

    public static void SetupResponsiveCanvas(CanvasScaler scaler)
    {
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f); 
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f; 
    }

    public static class ButtonFactory
    {
        public static Button CreatePrimaryButton(Transform parent, string name, string text)
        {
            var button = CreateBaseButton(parent, name, text, Colors.ButtonPrimary, Colors.AccentGold);
            var rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = Sizes.ButtonLarge;
            
            var textComp = button.GetComponentInChildren<Text>();
            textComp.fontSize = Typography.BodyLarge;
            textComp.fontStyle = FontStyle.Bold;
            
            return button;
        }

        public static Button CreateSecondaryButton(Transform parent, string name, string text)
        {
            var button = CreateBaseButton(parent, name, text, Colors.ButtonSecondary, Colors.AccentGoldLight);
            var rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = Sizes.ButtonMedium;
            
            var textComp = button.GetComponentInChildren<Text>();
            textComp.fontSize = Typography.BodyMedium;
            
            return button;
        }

        private static Button CreateBaseButton(Transform parent, string name, string text, Color bgColor, Color borderColor)
        {
            var buttonObj = new GameObject(name, typeof(Image), typeof(Button));
            buttonObj.transform.SetParent(parent, false);
            
            var image = buttonObj.GetComponent<Image>();
            image.color = bgColor;
            
            var border = new GameObject("Border", typeof(Image));
            border.transform.SetParent(buttonObj.transform, false);
            var borderImg = border.GetComponent<Image>();
            borderImg.color = borderColor;
            var borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            
            var innerBg = new GameObject("Background", typeof(Image));
            innerBg.transform.SetParent(border.transform, false);
            var innerImg = innerBg.GetComponent<Image>();
            innerImg.color = bgColor;
            var innerRect = innerBg.GetComponent<RectTransform>();
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(3f, 3f);
            innerRect.offsetMax = new Vector2(-3f, -3f);
            
            var textObj = new GameObject("Text", typeof(Text));
            textObj.transform.SetParent(buttonObj.transform, false);
            var textComp = textObj.GetComponent<Text>();
            textComp.text = text;
            textComp.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            textComp.color = Colors.TextPrimary;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.fontSize = Typography.BodyMedium;
            
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(Spacing.Medium, Spacing.Small);
            textRect.offsetMax = new Vector2(-Spacing.Medium, -Spacing.Small);
            
            return buttonObj.GetComponent<Button>();
        }

        public static Button CreateGameModeButton(Transform parent, string name, string title, string description, Color bgColor)
        {
            var container = new GameObject(name + "Container", typeof(RectTransform), typeof(LayoutElement));
            container.transform.SetParent(parent, false);
            var containerLayout = container.GetComponent<LayoutElement>();
            containerLayout.preferredHeight = 120f;
            containerLayout.preferredWidth = 500f;
            
            var button = new GameObject(name, typeof(Image), typeof(Button));
            button.transform.SetParent(container.transform, false);
            var buttonRect = button.GetComponent<RectTransform>();
            buttonRect.anchorMin = Vector2.zero;
            buttonRect.anchorMax = Vector2.one;
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            var image = button.GetComponent<Image>();
            image.color = bgColor;
            
            var border = new GameObject("Border", typeof(Image));
            border.transform.SetParent(button.transform, false);
            var borderImg = border.GetComponent<Image>();
            borderImg.color = Colors.AccentGold;
            var borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            
            var innerBg = new GameObject("Background", typeof(Image));
            innerBg.transform.SetParent(border.transform, false);
            var innerImg = innerBg.GetComponent<Image>();
            innerImg.color = bgColor;
            var innerRect = innerBg.GetComponent<RectTransform>();
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(4f, 4f);
            innerRect.offsetMax = new Vector2(-4f, -4f);
            
            var titleObj = new GameObject("Title", typeof(Text));
            titleObj.transform.SetParent(button.transform, false);
            var titleText = titleObj.GetComponent<Text>();
            titleText.text = title;
            titleText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            titleText.fontSize = Typography.TitleSmall;
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = Colors.AccentGoldLight;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.6f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(Spacing.Medium, 0f);
            titleRect.offsetMax = new Vector2(-Spacing.Medium, -Spacing.Small);
            
            var descObj = new GameObject("Description", typeof(Text));
            descObj.transform.SetParent(button.transform, false);
            var descText = descObj.GetComponent<Text>();
            descText.text = description;
            descText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            descText.fontSize = Typography.BodyMedium;
            descText.color = Colors.TextSecondary;
            descText.alignment = TextAnchor.MiddleCenter;
            
            var descRect = descObj.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0f, 0.2f);
            descRect.anchorMax = new Vector2(1f, 0.6f);
            descRect.offsetMin = new Vector2(Spacing.Medium, Spacing.Small);
            descRect.offsetMax = new Vector2(-Spacing.Medium, 0f);
            
            return button.GetComponent<Button>();
        }
    }

    public static Dropdown CreateStyledSettingsDropdown(Transform parent, string label, string[] options)
    {
        var container = new GameObject($"{label}Container", typeof(RectTransform), typeof(LayoutElement));
        container.transform.SetParent(parent, false);
        var containerLayout = container.GetComponent<LayoutElement>();
        containerLayout.preferredHeight = 80f;
        
        var labelObj = new GameObject("Label", typeof(Text));
        labelObj.transform.SetParent(container.transform, false);
        var labelText = labelObj.GetComponent<Text>();
        labelText.text = label;
        labelText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        labelText.fontSize = Typography.BodyLarge;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = Colors.AccentGoldLight;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        var labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0.6f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        var dd = new GameObject("Dropdown", typeof(Image), typeof(Dropdown));
        dd.transform.SetParent(container.transform, false);
        var dropdown = dd.GetComponent<Dropdown>();
        var ddImg = dd.GetComponent<Image>();
        ddImg.color = Colors.ButtonSecondary;
        
        var ddRect = dd.GetComponent<RectTransform>();
        ddRect.anchorMin = new Vector2(0f, 0f);
        ddRect.anchorMax = new Vector2(1f, 0.55f);
        ddRect.offsetMin = Vector2.zero;
        ddRect.offsetMax = Vector2.zero;
        
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(dd.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = Colors.AccentGold;
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        var innerBg = new GameObject("InnerBackground", typeof(Image));
        innerBg.transform.SetParent(border.transform, false);
        var innerImg = innerBg.GetComponent<Image>();
        innerImg.color = Colors.ButtonSecondary;
        var innerRect = innerBg.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(2f, 2f);
        innerRect.offsetMax = new Vector2(-2f, -2f);
        
        dropdown.options.Clear();
        foreach (var option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        dropdown.value = 2;
        
        return dropdown;
    }

    public static Slider CreateStyledSettingsSlider(Transform parent, string label, float min, float max, float value)
    {
        var container = new GameObject($"{label}Container", typeof(RectTransform), typeof(LayoutElement));
        container.transform.SetParent(parent, false);
        var containerLayout = container.GetComponent<LayoutElement>();
        containerLayout.preferredHeight = 80f;
        
        var labelObj = new GameObject("Label", typeof(Text));
        labelObj.transform.SetParent(container.transform, false);
        var labelText = labelObj.GetComponent<Text>();
        labelText.text = label;
        labelText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        labelText.fontSize = Typography.BodyLarge;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = Colors.AccentGoldLight;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        var labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0.6f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        var sl = new GameObject("Slider", typeof(Slider));
        sl.transform.SetParent(container.transform, false);
        var slider = sl.GetComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
        
        var slRect = sl.GetComponent<RectTransform>();
        slRect.anchorMin = new Vector2(0f, 0f);
        slRect.anchorMax = new Vector2(1f, 0.55f);
        slRect.offsetMin = Vector2.zero;
        slRect.offsetMax = Vector2.zero;
        
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(sl.transform, false);
        bg.GetComponent<Image>().color = Colors.ButtonSecondary;
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        var handle = new GameObject("Handle", typeof(Image));
        handle.transform.SetParent(sl.transform, false);
        handle.GetComponent<Image>().color = Colors.AccentGold;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);
        slider.targetGraphic = handle.GetComponent<Image>();
        
        return slider;
    }

    public static Toggle CreateStyledSettingsToggle(Transform parent, string label, bool value)
    {
        var container = new GameObject($"{label}Container", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        container.transform.SetParent(parent, false);
        var containerLayout = container.GetComponent<LayoutElement>();
        containerLayout.preferredHeight = 60f;
        
        var hlg = container.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = Spacing.Medium;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        
        var toggleObj = new GameObject("Toggle", typeof(Toggle), typeof(Image));
        toggleObj.transform.SetParent(container.transform, false);
        var toggle = toggleObj.GetComponent<Toggle>();
        var toggleImg = toggleObj.GetComponent<Image>();
        toggleImg.color = Colors.ButtonSecondary;
        toggle.isOn = value;
        
        var toggleLayout = toggleObj.AddComponent<LayoutElement>();
        toggleLayout.preferredWidth = 60f;
        
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(toggleObj.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = Colors.AccentGold;
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        var innerBg = new GameObject("InnerBackground", typeof(Image));
        innerBg.transform.SetParent(border.transform, false);
        var innerImg = innerBg.GetComponent<Image>();
        innerImg.color = Colors.ButtonSecondary;
        var innerRect = innerBg.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(2f, 2f);
        innerRect.offsetMax = new Vector2(-2f, -2f);
        
        var checkmark = new GameObject("Checkmark", typeof(Image));
        checkmark.transform.SetParent(toggleObj.transform, false);
        var checkImg = checkmark.GetComponent<Image>();
        checkImg.color = Colors.AccentGoldLight;
        var checkRect = checkmark.GetComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = new Vector2(10, 10);
        checkRect.offsetMax = new Vector2(-10, -10);
        toggle.graphic = checkImg;
        
        var labelObj = new GameObject("Label", typeof(Text));
        labelObj.transform.SetParent(container.transform, false);
        var labelText = labelObj.GetComponent<Text>();
        labelText.text = label;
        labelText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        labelText.fontSize = Typography.BodyLarge;
        labelText.color = Colors.TextPrimary;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        return toggle;
    }

    public static Dropdown CreateMobileDropdown(Transform parent, string[] options)
    {
        var dd = new GameObject("MobileDropdown", typeof(Image), typeof(Dropdown));
        dd.transform.SetParent(parent, false);
        var dropdown = dd.GetComponent<Dropdown>();
        var ddImg = dd.GetComponent<Image>();
        ddImg.color = Colors.ButtonSecondary;
        
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(dd.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = Colors.AccentGold;
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        var innerBg = new GameObject("InnerBackground", typeof(Image));
        innerBg.transform.SetParent(border.transform, false);
        var innerImg = innerBg.GetComponent<Image>();
        innerImg.color = Colors.ButtonSecondary;
        var innerRect = innerBg.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(2f, 2f);
        innerRect.offsetMax = new Vector2(-2f, -2f);
        
        dropdown.options.Clear();
        foreach (var option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        
        return dropdown;
    }

    public static GameObject CreateSection(Transform parent, string title)
    {
        var section = new GameObject($"Section_{title.Replace(" ", "")}", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(Image));
        section.transform.SetParent(parent, false);
        
        var sectionImg = section.GetComponent<Image>();
        sectionImg.color = new Color(0f, 0f, 0f, 0.3f);
        
        var sectionVlg = section.GetComponent<VerticalLayoutGroup>();
        sectionVlg.spacing = Spacing.Medium;
        sectionVlg.padding = new RectOffset(
            (int)Spacing.Large, 
            (int)Spacing.Large, 
            (int)Spacing.Medium, 
            (int)Spacing.Medium
        );
        sectionVlg.childControlWidth = true;
        sectionVlg.childControlHeight = false;
        sectionVlg.childForceExpandWidth = true;
        
        var sectionFitter = section.GetComponent<ContentSizeFitter>();
        sectionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        var titleObj = new GameObject("SectionTitle", typeof(Text));
        titleObj.transform.SetParent(section.transform, false);
        var titleText = titleObj.GetComponent<Text>();
        titleText.text = title;
        titleText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        titleText.fontSize = Typography.TitleSmall;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = Colors.AccentGoldLight;
        titleText.alignment = TextAnchor.MiddleLeft;
        
        var titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 40f;
        
        return section;
    }
}
#endif
