using UnityEngine;
using UnityEngine.UIElements;

public static class UIExtensions
{
    public static void Display(this VisualElement element, bool display)
    {
        element.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
