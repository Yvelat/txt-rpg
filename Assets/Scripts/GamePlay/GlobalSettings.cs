using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    [SerializeField] TMP_ColorGradient commonColor;
    [SerializeField] TMP_ColorGradient uncommonColor;
    [SerializeField] TMP_ColorGradient rareColor;
    [SerializeField] TMP_ColorGradient ultraRareColor;
    [SerializeField] TMP_ColorGradient epicColor;
    [SerializeField] TMP_ColorGradient legendaryColor;

    public Color HighlightedColor => highlightedColor;
    public TMP_ColorGradient CommonColor => commonColor;
    public TMP_ColorGradient UncommonColor => uncommonColor;
    public TMP_ColorGradient RareColor => rareColor;
    public TMP_ColorGradient UltraRareColor => ultraRareColor;
    public TMP_ColorGradient EpicColor => epicColor;
    public TMP_ColorGradient LegendaryColor => legendaryColor;


    public static GlobalSettings i { get; private set; }

    private void Awake()
    {
        i = this;
    }
}
