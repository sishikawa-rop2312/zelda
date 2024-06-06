using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName; // アイテム名
    public Sprite icon; // アイテム画像

    public abstract void Use(PlayerController player);
}
