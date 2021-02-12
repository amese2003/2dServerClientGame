using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public int TemplateId { get; private set; }
        public Monster()
        {
            ObjectType = GameObjectType.Monster;           

        }

        public void Init(int templateId)
        {
            TemplateId = templateId;

            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(templateId, out monsterData);
            Stat.MergeFrom(monsterData.stat);
            Stat.Hp = monsterData.stat.MaxHp;
            State = CreatureState.Idle;
        }

        // FSM (Finite State Machine)
        IJob _job;
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;

                case CreatureState.Moving:
                    UpdateMoving();
                    break;

                case CreatureState.Skill:
                    UpdateSkill();
                    break;

                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }

            // 5프레임 (0.2초마다 한번씩 update)
            if (Room != null)
                _job = Room.PushAfter(200, Update);
        }

        Player _target;
        int _searchCellDist = 10;
        long _nextSearchTick = 0;
        int _chaseCellDist = 20;

        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;

            _nextSearchTick = Environment.TickCount64 + 1000;

            Player target = Room.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - CellPos;
                return dir.cellDistFromZero < _searchCellDist;
            });

            if (target == null)
                return;


            _target = target;
            State = CreatureState.Moving;            
        }

        int _skillRange = 1;
        long _nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if(_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();
                return;
            }

            Vector2Int dir = _target.CellPos - CellPos;
            int dist = dir.cellDistFromZero;
            if (dist == 0 || dist > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();
                return;
            }


            List<Vector2Int> path = Room.Map.FindPath(CellPos, _target.CellPos, checkObject: false);
            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadCastMove();
                return;
            }

            // 스킬로 넘어갈지 체크
            if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);

            BroadCastMove();
        }

        void BroadCastMove()
        {
            // 다른 플레이어에게 알려주기
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }


        long _coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // 유효한 타겟인지
                if (_target ==null || _target.Room != Room || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadCastMove();
                    return;
                }


                // 스킬이 아직 사용 가능?
                Vector2Int dir = (_target.CellPos - CellPos);
                int dist = dir.cellDistFromZero;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
                if (!canUseSkill)
                {
                    State = CreatureState.Moving;
                    BroadCastMove();
                    return;
                }


                // 타게팅 방향 주시
                MoveDir lookDir = GetDirFromVec(dir);
                if(Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadCastMove();
                }

                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(1, out skillData);

                // 데미지 판정
                _target.OnDamaged(this, skillData.damage + TotalAttack);

                // 스킬 사용 브로드캐스팅
                S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
                skillPacket.ObjectId = Id;
                skillPacket.Info.SkillId = skillData.id;
                Room.Broadcast(skillPacket);


                // 스킬 쿨
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
                return;

            _coolTick = 0;
        }

        protected virtual void UpdateDead()
        {

        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            GameObject owenr = attacker.GetOwner();
            if (owenr.ObjectType == GameObjectType.Player)
            {
                RewardData rewardData = GetRandomReward();
                if(rewardData != null)
                {
                    Player player = (Player)owenr;
                    DbTransaction.RewardPlayer(player, rewardData, Room);
                }
            }

            if(_job != null)
            {
                _job.Cancel = true;
                _job = null;
            }
        }

        RewardData GetRandomReward()
        {
            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);


            int rand = new Random().Next(0, 101);

            // rand = 0 - 100 => 42?
            // 10 10 10 10 10
            // 10 20 30 40 50
            int sum = 0;
            foreach(RewardData rewardData in monsterData.rewards)
            {
                sum += rewardData.probability;
                if(rand <= sum)
                {
                    return rewardData;
                }
            }

            return null;
        }
    }
}
