using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jambav.Utilities
{
    public class CameraObjRotation : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject mainCamera;
        public float speed = 5;
        void Start()
        {
#if UNITY_EDITOR && !UNITY_EDITOR_WIN
            mainCamera.gameObject.SetActive(false);
#else
         gameObject.SetActive(false);
#endif

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                transform.eulerAngles += speed * new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
    }
}