using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;


    private void Awake()
    {
        instance = this;
    }

    public List<Item> shopItems;
}
