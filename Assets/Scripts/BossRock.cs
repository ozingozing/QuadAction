using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossRock : Bullet
{
    public static BossRock Instance;
    Rigidbody rb;
    public float angularPower;
    float scaleValue = 0.1f;
    bool isShoot;

    private void Awake()
    {
        Application.targetFrameRate = 144;
        if (Instance == null) BossRock.Instance = this;
        
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    private void OnDisable()
    {
        angularPower = 0;
        scaleValue = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (Boss.Instance.isDead)
        {
            BossDeadCheck();
        }

        angularPower += 0.08f;
        rb.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
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
            scaleValue += 0.0055f;
            transform.localScale = Vector3.one * scaleValue;
            rb.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            isShoot = false;
            yield return null;
        }
        
    }
}
