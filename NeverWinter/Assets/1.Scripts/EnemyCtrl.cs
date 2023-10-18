using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.tvOS;

// 메모장

public class EnemyCtrl : MonoBehaviour
{
    private WayContainer container;
    private int idx;
    public Renderer render = null;
    public Transform[] movePoints;

    // 체력  #미완성#
    
    
    public float Enemy_HP;
    public float Max_Hp;
    // 이동속도
    public float Enemy_move_Speed;
    
    //능력 #미완성#
    public string Enemy_Spell;

    // 현상금
    public int Reward;
    
    //적 사망여부 
    public bool isEnemyDie = false;
    private bool isEnd = false;
    private Coroutine damageCoroutine = null;

    private Transform target;
    //public NavMeshAgent agent;

    private Animator animator;
    //private readonly int hashRun = Animator.StringToHash("");
    
    
    
    void Start()
    {
        animator = GetComponent<Animator>();
        container = GameObject.Find("WayContainer").GetComponent<WayContainer>();
        Enemy_HP = Max_Hp;
    }

    private void Update()
    {
        MoveWay();
    }

    // 적이 데미지 받았을 때 쓰는 함수
    public void TakeDamage(float damage)
    {
        if (isEnemyDie)
            return;
        Enemy_HP -= damage;
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamageEvent());
        
        if (Enemy_HP <= 0)
        {
            EnemyDie();
        }
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Castle"))
            takeCastle();
        Debug.Log("콜라이더 반응중");
    }

    public void takeCastle()
    {
        GameManager.instance.Lives--;
        Destroy(gameObject);
    }
    

    public void Attack()
    {
        GameManager.instance.Lives -= 10;
    }

    // 적이 죽었을 때 쓰는 함수  # 미완성 #
    // 애니메이션 추가 예정
    private void EnemyDie()
    {
        isEnemyDie = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        

        
        //agent.enabled = false;

        
        GetComponent<Collider>().enabled = false;

        Cost.Coin += Reward;
        Destroy(gameObject, 1.0f);

    }

    IEnumerator DamageEvent()
    {
        if (render)
            render.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);
        if (render)
            render.material.color = Color.white;
    }

    void MoveWay()
    {
        if (!isEnd)
        {
            Transform tr = container.WayPoints[idx];
            transform.LookAt(tr);

            transform.position = Vector3.MoveTowards(transform.position, tr.position, Enemy_move_Speed * Time.deltaTime);

            if ((transform.position - tr.position).sqrMagnitude < 0.05f)
            {
                idx++;
                if (idx >= container.WayPoints.Length)
                {
                    isEnd = true;
                }
            }
        }
    }
}
