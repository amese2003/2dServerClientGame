using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public Grid _grid;
    public float _speed = 5.0f;

    Vector3Int _cellPos = Vector3Int.zero;
    MoveDir _dir = MoveDir.None;
    bool _isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    void Update()
    {
        GetDirInput();
        UpdateIsMoving();
        UpdatePosition();
    }

    private void UpdateIsMoving()
    {
        if (!_isMoving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _cellPos += Vector3Int.up;
                    _isMoving = true;
                    break;
                case MoveDir.Down:
                    _cellPos += Vector3Int.down;
                    _isMoving = true;
                    break;

                case MoveDir.Left:
                    _cellPos += Vector3Int.left;
                    _isMoving = true;
                    break;
                case MoveDir.Right:
                    _cellPos += Vector3Int.right;
                    _isMoving = true;
                    break;
            }
        }
    }

    private void UpdatePosition()
    {
        if (!_isMoving)
            return;

        Vector3 destPos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부
        float dist = moveDir.magnitude;
        if(dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            _isMoving = true;
        }
    }

    private void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _dir = MoveDir.Up;
        }

        else if (Input.GetKey(KeyCode.S))
        {
            _dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _dir = MoveDir.Right;
        }
        else
        {
            _dir = MoveDir.None;
        }
    }
}
