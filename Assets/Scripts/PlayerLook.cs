using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public static PlayerLook instance;

    public Transform playerCamera;
    public Vector2 sensitivities;
    private Vector2 XYRotation;

    public Camera[] arrCam;
    int CamCount = 3;
    public int nowCamNum = 0;

    public Transform target;

    private void Awake()
    {
        if(PlayerLook.instance == null) { PlayerLook.instance = this; }
        arrCam[0].enabled = true;
        arrCam[1].enabled = false;
        arrCam[2].enabled = false;
    }
 
    public void CamSelect()
    {
        if (Input.GetButtonUp("CamChange"))
        {
            ++nowCamNum;
            if (nowCamNum >= CamCount) nowCamNum = 0;

            switch(nowCamNum)
            {
                case 0:
                    arrCam[nowCamNum].enabled = true;
                    playerCamera = arrCam[nowCamNum].transform;
                    arrCam[CamCount - 1].enabled = false;
                    break;
                case 1:
                    arrCam[nowCamNum].enabled = true;
                    playerCamera = arrCam[nowCamNum].transform;
                    arrCam[nowCamNum - 1].enabled = false;
                    break;
                case 2:
                    arrCam[nowCamNum].enabled = true;
                    playerCamera = arrCam[nowCamNum].transform;
                    arrCam[nowCamNum - 1].enabled = false;
                    break;
            }
        }
    }

    public void Look()
    {
        Vector2 mouseInput = new Vector2
        {
            x = Input.GetAxis("Mouse X"),
            y = Input.GetAxis("Mouse Y")
        };

        XYRotation.x -= mouseInput.y * sensitivities.y;
        XYRotation.y += mouseInput.x * sensitivities.x;

        XYRotation.x = Mathf.Clamp(XYRotation.x, -90f, 90f);

        if(nowCamNum == 0 && !Player.instance.isInteraction)
        {
            playerCamera.localEulerAngles = new Vector3(XYRotation.x, 0f, 0f);
            transform.eulerAngles = new Vector3(0f, XYRotation.y, 0f);
        }
        if(nowCamNum == 1 && !Player.instance.isInteraction)
        {
            if (Player.instance.fireDown)
            {
                Ray ray = arrCam[1].ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    Vector3 nextVec = hit.point - transform.position;
                    nextVec.y = 0;
                    Player.instance.transform.LookAt(transform.position + nextVec);
                }
            }
        }

    }
    


}
