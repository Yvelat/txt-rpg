using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Crea nuova Skin")]
public class Skin : ScriptableObject
{
    [SerializeField] int id;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite rightSprite;

    public int ID => id;
    public Sprite FrontSprite => frontSprite;
    public Sprite RightSprite => rightSprite;
}
