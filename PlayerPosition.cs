using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class PlayerPosition : MonoBehaviour
{
    public static PlayerPosition player; //синглтон для быстрого доступа к этому скрипту

    public bool PlayerOnSafeZone; // если true, охранники не замечают/теряют интерес к игроку
    public int DrunkState; // степень опьянения героя от 0 до 3 в зависимости от выпитых бутылок
    public CinemachineVirtualCamera vcam; //основная игровая камера синемашины

    private Rigidbody rb;
    private UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter character;
    [SerializeField] private GameObject[] bottels; // бутылки на сцене(которые можно выпить)
    [SerializeField] private BoxCollider[] drunkScene; //коллайдеры тригерных объектов запускающие кат-сцены распития бутылок
    [SerializeField] private GameObject LoseText; // текст проигрыша, если игрока поймали
    [SerializeField] private GameObject FirstBottelText; // текст первой стадии опьянения
    [SerializeField] private GameObject SecondBottelText;
    [SerializeField] private GameObject ThirdBottelText;
    [SerializeField] private GameObject GoodEndText;// текст хорошей концовки
    [SerializeField] private GameObject BadEndText;//текст плохой концовки
    [SerializeField] private PlayableDirector BadEndCutScene; // таймлиния запускающаяся перед плохой концовкой
    //предохранители корутины от двойного срабатывания (по умолчанию стоят в False)
    private bool firstCoroutOn;
    private bool secondCoroutOn;
    private bool thirdCoroutOn;

    private void Awake()
    {
        player = this;// инициализация синглтона
        rb = GetComponent<Rigidbody>();
        firstCoroutOn = false;
        secondCoroutOn = false;
        thirdCoroutOn = false;
        character = GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SafeZone") || other.gameObject.CompareTag("DanceFloor"))
        {
            PlayerOnSafeZone = true;//охранники не трогают игрока если он находится в безопасной зоне либо на танцполе
        }

        if (other.gameObject.CompareTag("Guard"))
        { //встреча с охранником приведет к поражению
            StartCoroutine(LoseGame());// запускаем текст пораженния
            DrunkState = 0; // обнуляем счетчик выпитого
            //предохранители возвращаем в исходное положение
            firstCoroutOn = false;
            secondCoroutOn = false;
            thirdCoroutOn = false;

            for (int i = 0; i < bottels.Length; i++)
                bottels[i].SetActive(true);//возвращаем на место бутылки, после проигрыша

            for (int i = 0; i < drunkScene.Length; i++)
                drunkScene[i].enabled = true; //тригеры сцен тоже возвращаем
        }

        if (other.gameObject.CompareTag("GoodEnd"))
        {//показываем текст с хорошей концовкой если дошли до нужного тригера и выпили хотя бы 1 бутылку
            if (DrunkState > 0)
                StartCoroutine(End(GoodEndText, 12f));
        }

        if (other.gameObject.CompareTag("BadEnd"))
        {//елси все таки выпили все бутылки и пришли к тригеру, то текст плохой концовки
            if (DrunkState == 3)
                StartCoroutine(FinalCutScene());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SafeZone") || other.gameObject.CompareTag("DanceFloor"))
            PlayerOnSafeZone = false;
    }
    private void FixedUpdate()
    {//в зависимости от степени опьянения будет показыватся соответсвующий текст и камера будет дрожать все сильнее
        if (DrunkState == 1)
        {
            if (!firstCoroutOn)
            {
                firstCoroutOn = true;
            StartCoroutine(DrinkBottel(FirstBottelText));
            }
            
            DrunkShake(4f, new Vector3(-10, 0, 5)); //параметры "пьяной камеры"
        }
        else if (DrunkState == 2)
        {
            if (!secondCoroutOn)
            {
                secondCoroutOn = true;
                StartCoroutine(DrinkBottel(SecondBottelText));
            }

            DrunkShake(10f, new Vector3(-20, 5, 5));
        }
        else if (DrunkState == 3)
        {
            if (!thirdCoroutOn)
            {
                thirdCoroutOn = true;
                StartCoroutine(DrinkBottel(ThirdBottelText));
            }

            DrunkShake(15f, new Vector3(-20, 5, 10));
        }
        else if (DrunkState == 0)
        {//если игрок проиграл то параметры "пьяной камеры" обнуляются
            DrunkShake(0f, Vector3.zero); 
        }
    }
    /// <summary>
    /// метод создания эфффекта "пьяной камеры"
    /// </summary>
    /// <param name="intensit"></param>
    /// <param name="vector"></param>
    private void DrunkShake(float intensit, Vector3 vector)
    {//для начала получаем нужный компонент
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensit;
        cinemachineBasicMultiChannelPerlin.m_PivotOffset = vector;
    }

    /// <summary>
    /// Метод переключения безопасности игрока нужен только для таймлинии
    /// </summary>
    /// <param name="OnOff"></param>
    public void SafeZoneOnOff(bool OnOff)
    {
        PlayerOnSafeZone = OnOff;
    }

    /// <summary>
    /// метод нужен только для таймлинии начальной кат-сцены
    /// </summary>
    public void EndCutScene()
    {
        rb.isKinematic = false;//даем игроку двигатся, так как по умолчанию параметр включен в true
        transform.position = new Vector3(14, 0, -16); //перемещаем в начало игровой зоны
    }

    /// <summary>
    /// метод срабатывает при проигрыше
    /// </summary>
    /// <returns></returns>
    IEnumerator LoseGame()
    {
        LoseText.SetActive(true);
        Time.timeScale = 0.5f;
        rb.isKinematic = true;// не даем игроку двигатся пока он смотрит текст
        yield return new WaitForSeconds(1f);
        transform.position = new Vector3(27, 0, -12);//возвращаем игрока в начало уровня
        character.m_GroundCheckDistance = 1f; // приземляем его через контроллер персонажа, а то бывают баги
        character.m_IsGrounded = true;
        LoseText.SetActive(false);
        rb.isKinematic = false;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Метод запускает на экран текст по время кат-сцены выпивания бутылки
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IEnumerator DrinkBottel(GameObject text)
    {
        text.SetActive(true);
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(3f);
        Time.timeScale = 1f;
        text.SetActive(false);
    }

    /// <summary>
    /// Запускает текст в конце игры, после чего загружает сцену меню
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator End(GameObject text, float time)
    {
        rb.isKinematic = true;
        text.SetActive(true);
        DrunkState = 0;
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// запуск финальной таймлинии с плохой концовкой
    /// </summary>
    /// <returns></returns>
    IEnumerator FinalCutScene()
    {
        BadEndCutScene.Play();
        yield return new WaitForSeconds(2.58f);
        StartCoroutine(End(BadEndText, 20f));
    }
}
