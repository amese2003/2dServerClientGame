using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUi { get; private set; }
    public UI_Inventory InvenUI { get; private set; }

    public override void Init()
    {
        base.Init();

        StatUi = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();

        StatUi.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
    }
}
