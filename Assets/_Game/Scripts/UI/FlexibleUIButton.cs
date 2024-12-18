using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class FlexibleUIButton : FlexibleUI
{
    Image image;
    Button button;

    protected override void OnThemeUI()
    {
        base.OnThemeUI();

        image = GetComponent<Image>();
        button = GetComponent<Button>();

        button.transition = Selectable.Transition.SpriteSwap;
        button.targetGraphic = image;

        image.sprite = themeData.buttonSprite;
        button.spriteState = themeData.buttonSpriteState;

    }
}
