using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossRock : Bullet
{
    public static BossRock Instance;
    Rigidbody rb;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShoot;

    private void Awake()
    {
        if(Instance == null) BossRock.Instance = this;
        angularPower = 0;
        scaleValue = 0;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    public void BossDeadCheck()
    {
        if (Boss.Instance.isDead)
        {
            Debug.Log("º¸½ºÁ×À½");
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while(!isShoot)
        {
            angularPower += 0.05f;
            scaleValue += 0.002f;
            transform.localScale = Vector3.one * scaleValue;
            rb.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            isShoot = false;
            yield return null;
        }
        
    }
}
