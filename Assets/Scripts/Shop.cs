using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static Shop Instance;

    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText;
    public string[] talkData;

    private void Awake()
    {
        if (Instance == null)
        {
            Shop.Instance = this.GetComponent<Shop>();
        }
    }

    

    public void Enter()
    {
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        Player.instance.isShop = false;
        Player.instance.isInteraction = false;
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        if(price > Player.instance.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        Player.instance.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                        + Vector3.forward * Random.Range(-3, 3);

        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemObj[index].transform.rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
