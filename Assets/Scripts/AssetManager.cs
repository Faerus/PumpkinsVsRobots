using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : InstanceMonoBehaviour<AssetManager>
{
    [field: Header("Prefabs")]
    [field: SerializeField]
    public GameObject Unit { get; set; }
}
