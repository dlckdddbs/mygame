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
        //�������� ���� �ִϸ����Ϳ� �׺�Ž� ������Ʈ�� �ν���Ʈ�� ��ϵ� ������ �����´�.
        //�̷��� �����Ϳ��� ������� �ʾƵ� �ȴ�.
        agent = GetComponent<NavMeshAgent>();

        //���� ü���� �ִ�ü�¸�ŭ ���� ä���.
        //maxHP�� ������>�����鿡�� ���������ؼ� �뷱�� ������ ������.
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAIType)
        {
            float moveZ = Input.GetAxis("Vertical");
            float moveX = Input.GetAxis("Horizontal");

            //���� ���Ͱ��� ����
            Vector3 angle = new Vector3(moveX, 0, moveZ);
            if (angle != Vector3.zero && a != true)
            {
                Quaternion r = Quaternion.LookRotation(angle);
                //õõ�� Ÿ���� �ٶ󺸰� �ϱ� ���� RotateTowards���
                transform.rotation = Quaternion.RotateTowards(transform.rotation, r, 10f);
                //�ٶ󺸴� �������� �̵�
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
                
                ani.SetFloat("movespeed", 0.3f);
                //�̵��� ���������Ƿ� �ִϸ����� �Ķ���� ���� �����ؼ� ���� �ִϸ� �ϵ��� �Ѵ�.
            }
            else
            {
                ani.SetFloat("movespeed", 0.0f);
            }

            if (Input.GetMouseButtonDown(0)&&!ani.GetCurrentAnimatorStateInfo(0).IsName("burst"))//�ִϸ��̼� ����üũ
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

            dd targetUnit = null;   //ã������ Ÿ���� �� ���� ����.
                                    //���ӿ�����Ʈ �߿� player�� ������ ���̾ ���� ������Ʈ�� ã�� �Լ�
                                    //���������� ū ���� �׷��� �׾ȿ� �����ϴ�(�ݶ��̴��� �ִ�) ��ü ������Ʈ�� ���ؿ´�.
            Collider[] colliderList = Physics.OverlapSphere(transform.position, searchDist, LayerMask.GetMask("Player"));
            //�׷��� ã�� �ݶ��̴� ��ü���� ��ȯ��Ű�鼭
            for (int i = 0; i < colliderList.Length; i++)
            {
                //player��� ��ũ��Ʈ�� ���� ������Ʈ�� �ִ� �� Ȯ��
                dd searchTarget = colliderList[i].GetComponent<dd>();
                //player ��ũ��Ʈ�� �ְ� �ش� AiType�� false�̸�
                if (searchTarget && searchTarget.isAIType == false)
                {
                    //�̳��� �÷��̾��̱� ������ ���� Ÿ���� �˴ϴ�.
                    targetUnit = searchTarget;

                    //ã������ ���� ���̻� ��ȯ���� �� �ʿ� ������ ����.
                    break;
                }

            }
            //Ÿ���� ã�Ҵٸ�
            if (targetUnit != null)
            {
                //�̵��� ���߰�
                if (agent)
                    agent.isStopped = true;

                //�ش� Ÿ���� �������� �ٶ󺸰� ������ ȸ�� ��Ų��
                //���� �ٶ󺸴� ����� ���� ��ġ�� ���� �������� ����(�ٶ󺸴� ����� �տ� �ְ� �� ��ġ�� �����Ѵ�.)
                Vector3 viewPos = targetUnit.transform.position - transform.position;
                //� ������ �����ϸ� �� ������ �Ĵٺ��� �Ѵ�.
                Quaternion rot = Quaternion.LookRotation(viewPos, Vector3.up);
                //�ش� ȸ���� ��ŭ �� ���� ȸ�� ��Ŵ(�ΰ��� ���̰����� ȸ��).
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 20.0f);

                //������ �Ϸ��µ� �������̸� ������ �ϸ� �ȵ�.
                if (ani.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    //���ݾִ� ����
                    ani.Play("shoot");
                    BulletShoot();
                }


                //������ ������ Ÿ���� �־����� �� �ٽ� �̵��� �ؾ��ϴµ�.
                //������ agent.remainingDistance ���� 0���� ����� �ֱ� ���� ���� ��ġ�� �׺�Ž� ������Ʈ�� �������ش�.
                //�� ��ġ�� ������ �ʱ�ȭ
                if (agent)
                    agent.SetDestination(transform.position);
            }
            //remainingDistance ������ ���� ���� �Ÿ� ��ȯ
            else if (agent && agent.remainingDistance < 1.0f)//���� �׺�Ž�������Ʈ�� �������� ���� ������ ������
            {
                //���� ���������� ã�ƾ� �ϱ� ������ ������Ʈ�� ������¸� �����Ѵ�.
                if (agent)
                    agent.isStopped = false;

                //30�����ȿ� ������ ���� ��ġ�� �˷��ش�.
                Vector3 destPos = (Random.insideUnitSphere * 30.0f) + transform.position;
                //���̰��� 0����
                destPos.y = 0;

                //���ο� �������� �����Ѵ�.
                agent.SetDestination(destPos);
            }
            //������Ʈ �ӵ� ���ϴ°�
            if (agent)
                ani.SetFloat("movespeed", agent.velocity.magnitude / agent.speed);

        }

    }

    public void SetDamage(int dmgValue)
    {
        //�� ü���� ���
        currentHP -= dmgValue;
        if (currentHP <= 0)
        {
            Destroy(hp);
            Destroy(player);
            
        }
    }
    public void enemySetDamage(int dmgValue)
    {
        //�� ü���� ���
        enemyHp -= dmgValue;
        if (enemyHp <= 0)
        {
            Destroy(enemy);    
        }
    }

    IEnumerator HpAttack()
    {
        //�����̸鼭 ���� ��� �ִ� ����� ��ġ�Ƿ�
        //���� �� �� �ٸ� ��� ����
        yield return new WaitForSeconds(0.8f);
        ani.Play("idle");
        a = false;
    }

    public void BulletShoot()
    {
        //�Ѿ��� �����Ѵ�.(Resources ���� �ȿ� �ִ� Sphere ��ü�� �����´� bullet�� �����Ѵ�.)
        GameObject bullet = Instantiate(Resources.Load<GameObject>("object/Sphere"));
        if (bullet)
        {
            //�Ѿ��� ��ġ�� ���ֿ� �̸� ����� �ѱ� ��ġ�� �ű��.
            bullet.transform.position = shootPoint.transform.position;
            //�ش� �Ѿ˿��� BulletObj�� �ִ��� Ȯ���ϰ�
            BulletObj obj = bullet.GetComponent<BulletObj>();
            if (obj)
            {
                //�ش� �Ѿ� �߻� ���� �Լ��� �����Ѵ�.
                obj.MoveStart(this);
            }
        }
    }

    public IEnumerator BulletBustShoot()
    {
        for (int i = 0; i < 3; i++)
        {
            //�Ѿ� �߻�
            BulletShoot();
            //0.2�� �� ��� ������
            yield return new WaitForSeconds(0.1f);

        }
    }

}
