using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObj : MonoBehaviour
{
    public dd masterUnit = null;
    public bool isMove = false;
    public float speed = 10.0f;
    public int lifeTime = 1000;

    public GameObject ball = null;
    public GameObject effect = null;
    // Start is called before the first frame update
    void Start()
    {
        masterUnit = GetComponent<dd>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 10.0f);
            //StartCoroutine(reloa1d());
            if (lifeTime <= 0)
            {
                //�ش� �Ѿ��� �������.
                Destroy(gameObject);
                return;
            }
            lifeTime--;
        }
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("�浹");

        //�Ѿ˰� �浹�� ������Ʈ�� ���̾ ���ؼ� Player���� üũ
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
            dd unit = collision.gameObject.GetComponent<dd>();

            //�ش� ���ֿ� ������� ����.
            unit.SetDamage(5);           

        }
        //�Ѿ˰� �浹�� ������Ʈ�� ���̾ ���ؼ� Unit���� üũ
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            Destroy(gameObject);
            dd unit = collision.gameObject.GetComponent<dd>();

            //�ش� ���ֿ� ������� ����.
            unit.enemySetDamage(5);
            
        }
    }

    public void MoveStart(dd unit)
    {
        //�� �Ѿ��� ������ ��� ���� �� ������ ���� �� ������ ����
        masterUnit = unit;
        //�Ѿ��� �������� ����(ȸ����)�� ������ �ٶ󺸴� �������� ����.
        transform.rotation = masterUnit.transform.rotation;

        //�̰��� true�� �Ǹ� update�Լ����� �̵��� ����.
        isMove = true;
        
    }


}
