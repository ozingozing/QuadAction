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
        Application.targetFrameRate = 144;
        if (Instance == null) BossRock.Instance = this;
        angularPower = 0;
        scaleValue = 0;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    private void Update()
    {
        if (Boss.Instance.isDead)
        {
            BossDeadCheck();
        }
    }
    public void BossDeadCheck()
    {

        Debug.Log("º¸½ºÁ×À½");
        Destroy(this.gameObject);

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
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rb.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            isShoot = false;
            yield return null;
        }
        
    }
}
