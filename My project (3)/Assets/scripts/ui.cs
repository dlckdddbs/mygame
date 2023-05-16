using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dotomchi
{
    public class ui : MonoBehaviour
    {
        public Camera mainCamera = null;
        public dd targetUnit = null;
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

            if (mainCamera && targetUnit)
            {
                Vector3 pos = mainCamera.WorldToScreenPoint(targetUnit.transform.position) + new Vector3(0, height, 0);
                transform.position = pos;// new Vector3(pos.x, pos.y, -10);

                if (hpBar)
                    hpBar.fillAmount = (float)targetUnit.currentHP / (float)targetUnit.maxHP;
            }
        }
    }
}
