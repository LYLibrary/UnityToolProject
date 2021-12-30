using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCell : MonoBehaviour
{
    private Color color;
    public Color ColorValue
    {
        get { return color; }
        set
        {
            color = value;
            Pallet.color = color;
        }
    }
    public Image Pallet;
    public Image Background;
    public Image BackgroundTip;

    public Sprite CircleItemBgNormal;
    public Sprite CircleItemBgSelected;

    public void OnSelectChange(bool selected)
    {
        Background.sprite = selected ? CircleItemBgSelected : CircleItemBgNormal;
    }

    private void Update()
    {
        Background.transform.rotation = Quaternion.identity;
        BackgroundTip.transform.rotation = Quaternion.identity;
    }

}
