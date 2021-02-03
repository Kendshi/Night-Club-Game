using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    //Данный скрипт вешается на отдельный префаб с коллайдером треугольной формы
    private GuardController guard;              //основной скрипт родительского объекта охранника
    private PlayerPosition playerPosition;      //Основной скрипт игрока


    void Start()
    {
        guard = GetComponentInParent<GuardController>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPosition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!playerPosition.PlayerOnSafeZone)       //если Игрок не находится в безопасной зоне, то обнаруживаем игрока
            {
                guard.PlayerDetected = true;
            }
            
        }
    }
}
