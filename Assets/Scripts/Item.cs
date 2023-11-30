using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items")]
public class Item : ScriptableObject
{
    public string abilityName;
    public int abilityLevel;
    public Sprite abilitySprite;
    public GameObject ability;
}
