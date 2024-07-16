using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyLittleStore
{
    public class CustomButton : Button
    {
        protected override void DoStateTransition(SelectionState State, bool Immediate)
        {
            var targetColor =
                State == SelectionState.Disabled ? colors.disabledColor :
                State == SelectionState.Highlighted ? colors.highlightedColor :
                State == SelectionState.Normal ? colors.normalColor :
                State == SelectionState.Pressed ? colors.pressedColor :
                State == SelectionState.Selected ? colors.selectedColor : Color.white;

            foreach (var image in GetComponentsInChildren<Image>())
                image.CrossFadeColor(targetColor, Immediate ? 0 : colors.fadeDuration, true, true);

        }
    }

}