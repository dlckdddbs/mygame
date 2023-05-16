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
                //해당 총알은 사라진다.
                Destroy(gameObject);
                return;
            }
            lifeTime--;
        }
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("충돌");

        //총알과 충돌한 오브젝트의 레이어를 구해서 Player인지 체크
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
            dd unit = collision.gameObject.GetComponent<dd>();

            //해당 유닛에 대미지를 입힘.
            unit.SetDamage(5);           

        }
        //총알과 충돌한 오브젝트의 레이어를 구해서 Unit인지 체크
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            Destroy(gameObject);
            dd unit = collision.gameObject.GetComponent<dd>();

            //해당 유닛에 대미지를 입힘.
            unit.enemySetDamage(5);
            
        }
    }

    public void MoveStart(dd unit)
    {
        //이 총알의 주인을 등록 내가 쏜 것인지 적이 쏜 것인지 구별
        masterUnit = unit;
        //총알이 나가려는 방향(회전값)을 주인이 바라보는 방향으로 설정.
        transform.rotation = masterUnit.transform.rotation;

        //이값이 true가 되면 update함수에서 이동을 시작.
        isMove = true;
        
    }


}
