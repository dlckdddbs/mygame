using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class dd : MonoBehaviour
{
    private Animator ani;
    public bool a = true;
    public float speed = 10.0f;
    public GameObject shootPoint;
    public int currentHP, maxHP,enemyHp = 10;
    public int enemymaxhp = 10;
    private bool attack = true;

    public GameObject player;
    public GameObject enemy;
    public GameObject hp;


    public NavMeshAgent agent = null;

    public float searchDist = 10.0f;
    public bool isAIType = false;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        //시작하자 마다 애니메이터와 네비매쉬 에이전트를 인스펙트에 등록된 것으로 가져온다.
        //이러면 에디터에서 등록하지 않아도 된다.
        agent = GetComponent<NavMeshAgent>();

        //현재 체력을 최대체력만큼 값을 채운다.
        //maxHP는 에디터>프리펩에서 수정가능해서 밸런스 수정에 용이함.
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAIType)
        {
            float moveZ = Input.GetAxis("Vertical");
            float moveX = Input.GetAxis("Horizontal");

            //현재 백터값을 설정
            Vector3 angle = new Vector3(moveX, 0, moveZ);
            if (angle != Vector3.zero && a != true)
            {
                Quaternion r = Quaternion.LookRotation(angle);
                //천천히 타겟을 바라보게 하기 위해 RotateTowards사용
                transform.rotation = Quaternion.RotateTowards(transform.rotation, r, 10f);
                //바라보는 방향으로 이동
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
                
                ani.SetFloat("movespeed", 0.3f);
                //이동을 시작했으므로 애니메이터 파라미터 값을 전달해서 무브 애니를 하도록 한다.
            }
            else
            {
                ani.SetFloat("movespeed", 0.0f);
            }

            if (Input.GetMouseButtonDown(0)&&!ani.GetCurrentAnimatorStateInfo(0).IsName("burst"))//애니메이션 상태체크
            {              
                ani.Play("burst");
                //BulletShoot();
                a = true;
                StartCoroutine(BulletBustShoot());
                StartCoroutine(HpAttack());
            }
        }

        if (isAIType)
        {

            dd targetUnit = null;   //찾으려는 타겟의 빈 값을 생성.
                                    //게임오브젝트 중에 player라 지정된 레이어를 가진 오브젝트를 찾는 함수
                                    //물리적으로 큰 원을 그려서 그안에 존재하는(콜라이더가 있는) 전체 오브젝트를 구해온다.
            Collider[] colliderList = Physics.OverlapSphere(transform.position, searchDist, LayerMask.GetMask("Player"));
            //그렇게 찾은 콜라이더 객체들을 순환시키면서
            for (int i = 0; i < colliderList.Length; i++)
            {
                //player라는 스크립트를 가진 오브젝트가 있는 지 확인
                dd searchTarget = colliderList[i].GetComponent<dd>();
                //player 스크립트가 있고 해당 AiType이 false이면
                if (searchTarget && searchTarget.isAIType == false)
                {
                    //이놈은 플레이어이기 때문에 적의 타겟이 됩니다.
                    targetUnit = searchTarget;

                    //찾았으니 이제 더이상 순환문을 돌 필요 없으니 나감.
                    break;
                }

            }
            //타겟을 찾았다면
            if (targetUnit != null)
            {
                //이동을 멈추고
                if (agent)
                    agent.isStopped = true;

                //해당 타겟의 방향으로 바라보고 내몸을 회전 시킨다
                //적을 바라보는 방법은 적의 위치를 나의 포지션의 빼고(바라보는 대상이 앞에 있고 내 위치를 빼야한다.)
                Vector3 viewPos = targetUnit.transform.position - transform.position;
                //어떤 방향을 제시하면 그 방향을 쳐다보게 한다.
                Quaternion rot = Quaternion.LookRotation(viewPos, Vector3.up);
                //해당 회전값 만큼 내 몸을 회전 시킴(두개의 사이값으로 회전).
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 20.0f);

                //공격을 하려는데 공격중이면 공격을 하면 안됨.
                if (ani.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    //공격애니 실행
                    ani.Play("shoot");
                    BulletShoot();
                }


                //공격이 끝나고 타겟이 멀어졌을 때 다시 이동을 해야하는데.
                //강제로 agent.remainingDistance 값을 0으로 만들어 주기 위해 현재 위치를 네비매쉬 에이전트에 적용해준다.
                //내 위치로 목적지 초기화
                if (agent)
                    agent.SetDestination(transform.position);
            }
            //remainingDistance 목적지 까지 남은 거리 반환
            else if (agent && agent.remainingDistance < 1.0f)//적의 네비매쉬에이전트의 목적지에 거의 도착을 했을때
            {
                //다음 도착지점을 찾아야 하기 때문에 에이전트의 스톱상태를 해제한다.
                if (agent)
                    agent.isStopped = false;

                //30범위안에 적당한 랜덤 위치를 알려준다.
                Vector3 destPos = (Random.insideUnitSphere * 30.0f) + transform.position;
                //높이값은 0으로
                destPos.y = 0;

                //새로운 목적지를 세팅한다.
                agent.SetDestination(destPos);
            }
            //에이전트 속도 구하는거
            if (agent)
                ani.SetFloat("movespeed", agent.velocity.magnitude / agent.speed);

        }

    }

    public void SetDamage(int dmgValue)
    {
        //내 체력을 깎고
        currentHP -= dmgValue;
        if (currentHP <= 0)
        {
            Destroy(hp);
            Destroy(player);
            
        }
    }
    public void enemySetDamage(int dmgValue)
    {
        //내 체력을 깎고
        enemyHp -= dmgValue;
        if (enemyHp <= 0)
        {
            Destroy(enemy);    
        }
    }

    IEnumerator HpAttack()
    {
        //움직이면서 총을 쏘면 애니 모션이 겹치므로
        //총을 쏠 땐 다른 모션 금지
        yield return new WaitForSeconds(0.8f);
        ani.Play("idle");
        a = false;
    }

    public void BulletShoot()
    {
        //총알을 생성한다.(Resources 폴더 안에 있는 Sphere 객체를 가져온다 bullet에 저장한다.)
        GameObject bullet = Instantiate(Resources.Load<GameObject>("object/Sphere"));
        if (bullet)
        {
            //총알의 위치를 유닛에 미리 등록한 총구 위치로 옮긴다.
            bullet.transform.position = shootPoint.transform.position;
            //해당 총알에서 BulletObj이 있는지 확인하고
            BulletObj obj = bullet.GetComponent<BulletObj>();
            if (obj)
            {
                //해당 총알 발사 시작 함수를 실행한다.
                obj.MoveStart(this);
            }
        }
    }

    public IEnumerator BulletBustShoot()
    {
        for (int i = 0; i < 3; i++)
        {
            //총알 발사
            BulletShoot();
            //0.2초 간 잠시 딜레이
            yield return new WaitForSeconds(0.1f);

        }
    }

}
