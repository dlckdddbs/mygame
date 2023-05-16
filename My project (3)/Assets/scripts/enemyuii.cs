using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dotomchi
{
    public class enemyuii : MonoBehaviour
    {
        public Camera mainCamera = null;
        public  List<dd> targetUnit = new List<dd>();
        public Image hpBar = null;
        //public Text hpValue = null;

        public int height = 0;

        public bool isFollow = true;
        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (isFollow == false)
                return;
            for (int i = 0; i < targetUnit.Count; i++)
            {
                if (mainCamera && targetUnit[i])
                {
                    Vector3 pos = mainCamera.WorldToScreenPoint(targetUnit[i].transform.position) + new Vector3(0, height, 0);
                    transform.position = pos;// new Vector3(pos.x, pos.y, -10);

                    if (hpBar)
                        hpBar.fillAmount = (float)targetUnit[i].enemyHp / (float)targetUnit[i].enemymaxhp;

                    //if (hpValue)
                    //    hpValue.text = string.Format("{0} / {1}", targetUnit.currentHP, targetUnit.maxHP);
                }
            }
        }
    }
}
