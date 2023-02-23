using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private GameObject selectedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                selectedObject = hit.collider.transform.parent.gameObject;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (selectedObject != null && (selectedObject == gameObject || col.gameObject == gameObject))
        {
            Destroy(col.gameObject);
            // �ٸ� ���ӿ�����Ʈ�� ������ ���ӿ�����Ʈ�� ������ �Ѱ��ִ� �ڵ�
        }
    }
}