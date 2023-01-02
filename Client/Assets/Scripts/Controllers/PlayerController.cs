using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine _coSkill;
    bool _isRanged = false;
    protected override void init()
    {
        base.init();
    }

    protected override void updateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
        base.updateController();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += Vector3.up * Time.deltaTime * _speed;
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += Vector3.down * Time.deltaTime * _speed;
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += Vector3.right * Time.deltaTime * _speed;
            Dir = MoveDir.Right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += Vector3.left * Time.deltaTime * _speed;
            Dir = MoveDir.Left;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    protected override void UpdateIdle()
    {
        if(Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            //_coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    IEnumerator CoStartPunch()
    {
        //피격 판정
        GameObject obj = Managers.Object.Find(GetFrontCellPos());
        if (obj != null)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc != null)
                cc.onDamaged();
        }
            
        //딜레이
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
        _isRanged = false;
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
        _isRanged = true;
    }

    protected override void UpdateAnimation()
    {
        if (State == CreatureState.Idle)
        {
            if (_lastDir == MoveDir.Up)
            {
                _animator.Play("IDLE_BACK");
                _sprite.flipX = false;
            }
            else if (_lastDir == MoveDir.Down)
            {
                _animator.Play("IDLE_FRONT");
                _sprite.flipX = false;
            }
            else if (_lastDir == MoveDir.Left)
            {
                _animator.Play("IDLE_SIDE");
                _sprite.flipX = true;
            }
            else
            {
                _animator.Play("IDLE_SIDE");
                _sprite.flipX = false;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_SIDE");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_SIDE");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play(_isRanged ? "ATTACK_WEAPON_BACK"  : "ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_isRanged ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_isRanged ? "ATTACK_WEAPON_SIDE" : "ATTACK_SIDE");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play(_isRanged ? "ATTACK_WEAPON_SIDE" : "ATTACK_SIDE");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }
}
