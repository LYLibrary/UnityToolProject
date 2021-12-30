using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetInputFieldContentType : MonoBehaviour
{
    public InputField _input;

    public void SetInputPassword()
    {
        _input.contentType = InputField.ContentType.Custom;
        _input.lineType = InputField.LineType.SingleLine;
        _input.inputType = InputField.InputType.Password;
        _input.keyboardType = TouchScreenKeyboardType.Default;
        _input.characterValidation = InputField.CharacterValidation.Alphanumeric;

        _input.ForceLabelUpdate();
    }
    public void SetInputAlphanumeric()
    {
        _input.contentType = InputField.ContentType.Alphanumeric;
        _input.ForceLabelUpdate();
    }
    public void SetInputStandard()
    {
        _input.contentType = InputField.ContentType.Standard;
        _input.ForceLabelUpdate();
    }


}


