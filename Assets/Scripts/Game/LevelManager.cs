using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private int levelIndex = 1;
    [SerializeField] private float parTimeSeconds = 120f;
    [SerializeField] private int pointsPerCollectible = 100;

    private readonly HashSet<PlayerMovement> playersAtExit = new HashSet<PlayerMovement>();
    private readonly List<PlayerMovement> activePlayers = new List<PlayerMovement>();

    private int registeredCollectibles;
    private int collectedCount;
    private float elapsedTime;
    private bool isRunning;
    private bool isCompleted;
    private bool isPaused;

    public int LevelIndex => levelIndex;
    public float ElapsedTime => elapsedTime;
    public int CollectedCount => collectedCount;
    public int RegisteredCollectibles => registeredCollectibles;
    public bool IsCompleted => isCompleted;
    public bool IsPaused => isPaused;

    public event Action<LevelResults> LevelCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CachePlayers();
        registeredCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        BeginLevel();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!isRunning || isCompleted || isPaused)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
    }

    public void BeginLevel()
    {
        elapsedTime = 0f;
        collectedCount = 0;
        playersAtExit.Clear();
        isRunning = true;
        isCompleted = false;
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void RegisterCollectible()
    {
        registeredCollectibles++;
    }

    public void CollectPickup()
    {
        if (isCompleted)
        {
            return;
        }

        collectedCount++;
    }

    public void RegisterPlayerAtExit(PlayerMovement player)
    {
        if (isCompleted || player == null)
        {
            return;
        }

        playersAtExit.Add(player);
        if (playersAtExit.Count >= activePlayers.Count && activePlayers.Count > 0)
        {
            CompleteLevel();
        }
    }

    public void UnregisterPlayerAtExit(PlayerMovement player)
    {
        playersAtExit.Remove(player);
    }

    public void SetPaused(bool paused)
    {
        if (isCompleted)
        {
            return;
        }

        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }

    public LevelResults BuildResults()
    {
        int stars = CalculateStars();
        int score = collectedCount * pointsPerCollectible;
        return new LevelResults(levelIndex, elapsedTime, collectedCount, registeredCollectibles, stars, score);
    }

    private void CompleteLevel()
    {
        if (isCompleted)
        {
            return;
        }

        isCompleted = true;
        isRunning = false;
        isPaused = true;
        Time.timeScale = 0f;

        LevelResults results = BuildResults();
        SaveManager.RecordLevelCompletion(
            results.LevelIndex,
            results.ElapsedTime,
            results.Stars,
            results.Score);

        LevelCompleted?.Invoke(results);
    }

    private int CalculateStars()
    {
        bool allCollectibles = registeredCollectibles == 0 || collectedCount >= registeredCollectibles;
        bool fastRun = elapsedTime <= parTimeSeconds;

        if (fastRun && allCollectibles)
        {
            return 3;
        }

        if (fastRun || collectedCount > 0)
        {
            return 2;
        }

        return 1;
    }

    private void CachePlayers()
    {
        activePlayers.Clear();
        activePlayers.AddRange(FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None));
    }
}

public readonly struct LevelResults
{
    public LevelResults(int levelIndex, float elapsedTime, int collectedCount, int totalCollectibles, int stars, int score)
    {
        LevelIndex = levelIndex;
        ElapsedTime = elapsedTime;
        CollectedCount = collectedCount;
        TotalCollectibles = totalCollectibles;
        Stars = stars;
        Score = score;
    }

    public int LevelIndex { get; }
    public float ElapsedTime { get; }
    public int CollectedCount { get; }
    public int TotalCollectibles { get; }
    public int Stars { get; }
    public int Score { get; }
}
