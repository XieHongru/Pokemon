using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    Animator animator;

    Vector2 moveDirection;

    bool isMoving;
    bool isRuning;
    bool isPlayingMovement;

    //AnimatedTile grassCoverdTile;
    Tile grassCoverdTile;
    public GameObject grassAnimation;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        moveDirection = Vector2.zero;
        isMoving = false;
        isRuning = false;
        isPlayingMovement = false;

        //grassCoverdTile = Resources.Load<AnimatedTile>("Palette/Leaf_outside/Grass");
        grassCoverdTile = Resources.Load<Tile>("Palette/Emerald_outside/EmeraldExterior_5932");
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = false;
        isRuning = false;

        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection = Vector2.down;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection = Vector2.up;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection = Vector2.left;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection = Vector2.right;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.X) && isMoving)
        {
            isRuning = true;
        }

        if (moveDirection != Vector2.zero)
        {
            Move();
        }
    }

    private void Move()
    {
        if (isRuning)
        {
            animator.Play("Run");
        }
        else
        {
            animator.Play("Walk");
        }

        if (!isPlayingMovement)
        {
            StartCoroutine(MoveAnimation());
        }
    }

    bool CanMove()
    {
        Vector3 nextTile = transform.position + (Vector3)moveDirection;
        Tilemap collider = GameObject.Find("Collider").GetComponent<Tilemap>();

        if(collider.HasTile(collider.WorldToCell(nextTile)))
        {
            return false;
        }
        return true;
    }

    IEnumerator MoveAnimation()
    {
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);

        Tilemap grass = GameObject.Find("Grass_Up").GetComponent<Tilemap>();
        Tilemap grass_coverd = GameObject.Find("Grass_Down").GetComponent<Tilemap>();

        bool flag = false;
        Vector3 prePos = (Vector3)transform.position;
        if (grass.HasTile(grass.WorldToCell((Vector3)transform.position)))
        {
            flag = true;
        }

        if (CanMove())
        {
            float speed = 3f;

            if (isRuning)
                speed = 6f;

            isPlayingMovement = true;
            Vector2 targetPos = transform.position + (Vector3)moveDirection * 1f;

            while (Vector2.Distance(transform.position, targetPos) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                //if(Vector2.Distance(transform.position, targetPos) )
                yield return null;
            }

            if (grass.HasTile(grass.WorldToCell((Vector3)targetPos)))
            {
                Instantiate(grassAnimation, targetPos - new Vector2(0, 0.3f), Quaternion.identity);
                grass_coverd.SetTile(grass.WorldToCell((Vector3)targetPos), grassCoverdTile);
            }

            while (Vector2.Distance(transform.position, targetPos) > 0.02f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                //if(Vector2.Distance(transform.position, targetPos) )
                yield return null;
            }

            transform.position = targetPos;
            isPlayingMovement = false;
            moveDirection = Vector2.zero;

            if (flag)
            {
                grass_coverd.SetTile(grass.WorldToCell(prePos), null);
            }
        }

        if (!isMoving)
        {
            animator.Play("Wait");
        }
    }
}
