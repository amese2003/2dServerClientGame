using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : UI_Base
{
    public List<UI_Invetory_Item> Items { get; set; } = new List<UI_Invetory_Item>();
    public override void Init()
    {
        Items.Clear();

        GameObject grid = transform.Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        for(int i = 0; i < 24; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Item", grid.transform);
            UI_Invetory_Item item = go.GetOrAddComponent<UI_Invetory_Item>();
            Items.Add(item);
        }
    }

    public void RefreshUI()
    {

    }
}
