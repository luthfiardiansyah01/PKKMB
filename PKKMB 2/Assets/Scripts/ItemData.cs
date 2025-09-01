// ItemData.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // This line makes it visible in the Inspector!
public class ItemData
{
    public int id;
    public string name;
    public string formal;
    public string description;
    public string buildingId; // Added buildingId field
}

[System.Serializable]
public class ItemCollection
{
    public List<ItemData> itemList;
}