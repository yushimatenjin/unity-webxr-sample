using UnityEngine;
using WebXR;

public class SimpleHitTest : MonoBehaviour
{
    public GameObject objectToPlace;
    private WebXRManager webXRManager;

    void Start()
    {
        webXRManager = WebXRManager.Instance;
    }

    void Update()
    {
        // タッチまたはクリックで配置
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(objectToPlace, hit.point, Quaternion.identity);
            }
        }
    }
}
