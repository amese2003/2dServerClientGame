using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coSkill;

    [SerializeField]
    bool _rangedSkill = false;

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;

        _rangedSkill = (Random.Range(0, 2) == 0 ? true : false);
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }


    public override void OnDamaged()
    {
        //Managers.Obj.Remove(id);
        //Managers.Resource.Destroy(gameObject);
    }


    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = Dir;
        ac.CellPos = CellPos;


        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        _coSkill = null;
    }

    IEnumerator CoStartPunch()
    {
        //피격 판정
        GameObject go = Managers.Obj.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
