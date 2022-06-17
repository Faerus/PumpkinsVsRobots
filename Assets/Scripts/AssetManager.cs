using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : InstanceMonoBehaviour<AssetManager>
{
    [field: Header("Sprites")]
    [field: SerializeField]
    public Sprite TestSprite { get; set; }
}
