using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalk : MonoBehaviour
{
    // Скрипт рандомизирует анимации диалога у НПС, чтобы это выглядело более естественно
    public string[] triggersName;

    private Animator anim;
    private bool OnOff;

    private void Start()
    {
        anim = GetComponent<Animator>();
        OnOff = true;
    }

    private void Update()
    {
        if (OnOff)
        {
            OnOff = false;
            StartCoroutine(TalkAnimation());
        }
        
    }

    IEnumerator TalkAnimation()
    {
        anim.SetTrigger(triggersName[Random.Range(0, triggersName.Length)]);
        yield return new WaitForSeconds(Random.Range(10, 15));
        OnOff = true;
    }
}
