using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public float _speed = 5.0f;

    //protected bool _isMoving = false;
    protected Animator _animator;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected MoveDir _dir = MoveDir.Down;
    protected MoveDir _lastDir = MoveDir.Down;
    protected CreatureState _state = CreatureState.Idle;
    protected SpriteRenderer _sprite;
    public virtual CreatureState State
    {
        get { return _state; }
        set {
            if(_state != value )
                _state = value;
            UpdateAnimation();
        }
    }
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;
            
            _dir = value;
            if (value != MoveDir.None)
                _lastDir = value;
            UpdateAnimation();
        }
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;
        switch(_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left ;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }
    protected virtual void UpdateAnimation()
    {
        if(State == CreatureState.Idle)
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
        else if(State == CreatureState.Moving)
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
        else if(State == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_SIDE");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_SIDE");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        updateController();
    }

    protected virtual void init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    protected virtual void updateController()
    {
        switch(State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdatMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdatMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }
    protected virtual void MoveToNextPos()
    {
        if(_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }
        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }
        State = CreatureState.Moving;

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
    }
    protected virtual void UpdateIdle()
    {

    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void onDamaged()
    {

    }
}
