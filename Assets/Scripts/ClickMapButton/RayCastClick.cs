using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastClick : MonoBehaviour {

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                GameObject gameObj = hitInfo.collider.gameObject;
                if (gameObj.tag == "map")//当射线碰撞目标为boot类型的物品 ，执行拾取操作
                {
                    Debug.Log(XmlLoader.GetDescribeByName(gameObj.name));
                }
            }
        }
    }
}
