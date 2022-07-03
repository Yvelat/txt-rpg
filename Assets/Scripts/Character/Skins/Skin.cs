using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Crea nuova Skin")]
public class Skin : ScriptableObject
{
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite rightSprite;
    [SerializeField] List<Sprite> sprites;

    public Sprite FrontSprite => frontSprite;
    public Sprite RightSprite => rightSprite;
    public List<Sprite> Sprites => sprites;
}
