using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Events;
using System;

public class MainGameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> scenes = new List<GameObject>();

    [SerializeField]
    private GameObject lobbyScene;

    [SerializeField]
    private Button aceSceneButton;
    [SerializeField]
    private Button wordsSceneButton;
    [SerializeField]
    private Button flameSceneButton;

    [SerializeField]
    private TextMeshProUGUI fpsText;

    [SerializeField]
    public Sprite defaultAvatar;
    [SerializeField]
    public string dataUrl = "https://private-624120-softgamesassignment.apiary-mock.com/v3/magicwords";

    private int currentSceneIndex = 0;
    private float deltaTime = 0.0f;

    private void Start()
    {
        Application.targetFrameRate = 30;

        aceSceneButton.onClick.AddListener(() => {
            StartScene(0);
        });

        wordsSceneButton.onClick.AddListener(() => {
            StartScene(1);
        });

        flameSceneButton.onClick.AddListener(() => {
            StartScene(2);
        });

        EventManager.Subscribe<EvGameSceneClosed>(OnGameSceneClosed);
        QueryDialogueData();
    }

    private void QueryDialogueData()
    {
        StartCoroutine(DialogueDataCache.Initialize(dataUrl, defaultAvatar));
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
    }

    private void OnGameSceneClosed(EvGameSceneClosed ev)
    {
        DisableScene(currentSceneIndex);
    }

    private void StartScene(int index)
    {
        currentSceneIndex = index;

        scenes[index].SetActive(true);
        lobbyScene.SetActive(false);
    }

    private void DisableScene(int index)
    {
        scenes[index].SetActive(false);
        lobbyScene.SetActive(true);
    }

    private void OnDestroy()
    {
        aceSceneButton.onClick.RemoveAllListeners();
        wordsSceneButton.onClick.RemoveAllListeners();
        flameSceneButton.onClick.RemoveAllListeners();
    }
}
