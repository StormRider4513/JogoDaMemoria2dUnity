using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{

    [Header("Main Elements")]
    private bool isCheckingMatch = false;

    public AudioManager audioManager;
    public GameObject cardPrefab;
    [SerializeField] private Transform gridParent;
    private RectTransform gridRect;

    private GridLayoutGroup gridLayout;

    public GameObject panelSplash;
    public GameObject panelMenu;
    public GameObject panelGame;
    public GameObject panelWin;
    public GameObject panelRank;
    public Text winTimeText;
    public Text timerText;
    public Text winErrorsText;
    public Text errorsText;

    [SerializeField] private ConfettiUIController confettiUI;

    private int errors = 0;
    private float timer = 0f;
    private int currentRankSize = 4;


    [Header("Rank Buttons")]
    [SerializeField] private Button btn4x4;
    [SerializeField] private Button btn6x6;
    [SerializeField] private Button btn8x8;

    [Header("Rank Button Scale")]
    [SerializeField] private Vector3 normalScale = Vector3.one;
    [SerializeField] private Vector3 selectedScale = new Vector3(1.12f, 1.12f, 1f);


    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup mainCanvasGroup;


    [Header("Cards")]
    public List<Sprite> cardImages;
    private List<Card> revealedCards = new List<Card>();

    private int totalPairs;
    private int matchedPairs;
    private const int MaxScores = 10;

    [System.Serializable]
    public class RankLineUI
    {
        public TMP_Text pos;
        public TMP_Text tempo;
        public TMP_Text data;
        public TMP_Text erros;
    }

    [SerializeField] private RankLineUI[] rankLines;
    private int currentSize;

    //ads
    private int matchCounter = 0;

    [Header("Settings")]
    public GameObject panelSettings;


    void Start()
    {
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.Save();
        // Debug.Log("PlayerPrefs limpo no Start");

        panelSplash.SetActive(true);
        panelMenu.SetActive(false);
        audioManager = FindFirstObjectByType<AudioManager>();
        audioManager.PlayMusicForMode(0);
        panelGame.SetActive(false);
        panelWin.SetActive(false);
        panelRank.SetActive(false);
        panelSettings.SetActive(false);

    }

    void Update()
    {
        if (panelGame.activeSelf)
        {
            timer += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);

            timerText.text = "Tempo: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    public void StartGame(int size)
    {
        audioManager.PlayClick();
        currentSize = size;
        audioManager.PlayMusicForMode(size);
        timer = 0f;
        errors = 0;
        errorsText.text = "Erros: 0";
        timerText.text = "Tempo: 00:00";

        panelMenu.SetActive(false);
        panelGame.SetActive(true);
        panelWin.SetActive(false);
        panelRank.SetActive(false);

        gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        gridRect = gridParent.GetComponent<RectTransform>();

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = size;

        GenerateCards(size);
        AdjustmentGrid(size);
    }

    public void EnterMainMenu()
    {
        panelSplash.SetActive(false);
        panelMenu.SetActive(true);
    }

    private void GenerateCards(int size)
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        int totalCards = size * size;
        totalPairs = totalCards / 2;
        matchedPairs = 0;

        List<int> ids = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // Embaralhar IDs
        for (int i = 0; i < ids.Count; i++)
        {
            int temp = ids[i];
            int randomIndex = Random.Range(i, ids.Count);
            ids[i] = ids[randomIndex];
            ids[randomIndex] = temp;
        }

        foreach (int id in ids)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridParent);
            Card card = cardObj.GetComponent<Card>();
            card.id = id;

            card.front.GetComponentInChildren<Image>().sprite = cardImages[id];
        }
    }

    public bool CanClickCard()
    {
        return revealedCards.Count < 2 && !isCheckingMatch;
    }

    public void CardRevealed(Card card)
    {
        revealedCards.Add(card);
        audioManager.PlayFlip();

        if (revealedCards.Count == 2)
        {
            isCheckingMatch = true;
            Invoke(nameof(CheckMatch), 1.5f);
        }
    }

    private void CheckMatch()
    {
        if (revealedCards.Count < 2)
        {
            isCheckingMatch = false;
            return;
        }

        if (revealedCards[0].id == revealedCards[1].id)
        {
            matchedPairs++;
            audioManager.PlayCorrect();
            revealedCards[0].SetMatched();
            revealedCards[1].SetMatched();

            if (matchedPairs == totalPairs)
            {
                revealedCards.Clear();
                isCheckingMatch = false;

                panelGame.SetActive(false);
                panelWin.SetActive(true);
                confettiUI.Play();

                winTimeText.text = timerText.text;
                winErrorsText.text = "Erros: " + errors;

                SaveTime(timer);
                matchCounter++;

                if (matchCounter >= 3)
                {
                    AdsManager.Instance.ShowInterstitial();
                    matchCounter = 0;
                }
            }

            revealedCards.Clear();
            isCheckingMatch = false;
        }
        else
        {
            errors++;
            audioManager.PlayWrong();
            errorsText.text = "Erros: " + errors;
            revealedCards[0].Shake();
            revealedCards[1].Shake();

            StartCoroutine(FlipBackAfterDelay(0.3f));
        }
    }

    private IEnumerator FlipBackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        revealedCards[0].FlipToBack();
        audioManager.PlayUnflip();

        revealedCards[1].FlipToBack();
        audioManager.PlayUnflip();

        revealedCards.Clear();
        isCheckingMatch = false;
    }

    public void RestartGame()
    {
        audioManager.PlayClick();

        CancelInvoke(nameof(CheckMatch));
        revealedCards.Clear();
        isCheckingMatch = false;

        panelWin.SetActive(false);
        panelRank.SetActive(false);
        panelSettings.SetActive(false);

        RestoreMainInteraction();

        StartGame(currentSize);
    }

    public void BackToMenu()
    {
        audioManager.PlayClick();
        panelMenu.SetActive(true);

        panelGame.SetActive(false);
        panelWin.SetActive(false);
        revealedCards.Clear();

        AudioManager.Instance.StopMusic();
        audioManager.PlayMusicForMode(0);

        timer = 0f;
        errors = 0;
        timerText.text = "Tempo: 0.0s";
        errorsText.text = "Erros: 0";
    }

    private string GetRankKey(int size)
    {
        return "Rank_" + size + "x" + size;
    }

    private void SaveTime(float time)
    {
        ScoreList scoreList = LoadRank(currentSize);

        ScoreEntry entry = new ScoreEntry(time, errors);
        scoreList.scores.Add(entry);

        scoreList.scores.Sort((a, b) =>
        {
            int timeCompare = a.time.CompareTo(b.time);
            if (timeCompare != 0)
                return timeCompare;

            int errorCompare = a.errors.CompareTo(b.errors);
            if (errorCompare != 0)
                return errorCompare;

            return string.Compare(a.dateTime, b.dateTime);
        });

        if (scoreList.scores.Count > MaxScores)
            scoreList.scores.RemoveRange(MaxScores, scoreList.scores.Count - MaxScores);

        string json = JsonUtility.ToJson(scoreList);
        PlayerPrefs.SetString(GetRankKey(currentSize), json);
        PlayerPrefs.Save();
    }

    private ScoreList LoadRank(int size)
    {
        string json = PlayerPrefs.GetString(GetRankKey(size), "");

        if (string.IsNullOrEmpty(json))
            return new ScoreList();

        return JsonUtility.FromJson<ScoreList>(json);
    }

    private void HighlightRankMode(int size)
    {
        ResetRankButton(btn4x4);
        ResetRankButton(btn6x6);
        ResetRankButton(btn8x8);

        Button selected = null;

        switch (size)
        {
            case 4: selected = btn4x4; break;
            case 6: selected = btn6x6; break;
            case 8: selected = btn8x8; break;
        }

        if (selected != null)
            SelectRankButton(selected);
    }

    private void ResetRankButton(Button button)
    {
        if (button == null) return;

        button.transform.DOKill(false);
        button.transform.localScale = normalScale;
    }

    private void SelectRankButton(Button button)
    {
        if (button == null) return;

        button.transform.DOKill(false);
        button.transform.localScale = normalScale;

        button.transform
            .DOScale(selectedScale, 0.15f)
            .SetEase(Ease.OutBack);
    }

    public void UpdateRank(int size)
    {
        HighlightRankMode(size);

        ScoreList scoreList = LoadRank(size);

        for (int i = 0; i < rankLines.Length; i++)
        {
            if (i < scoreList.scores.Count)
            {
                ScoreEntry s = scoreList.scores[i];

                int minutes = Mathf.FloorToInt(s.time / 60f);
                int seconds = Mathf.FloorToInt(s.time % 60f);

                rankLines[i].pos.text = (i + 1) + "º";
                rankLines[i].tempo.text = $"{minutes:00}:{seconds:00}";
                rankLines[i].data.text = s.dateTime;
                rankLines[i].erros.text = s.errors.ToString();
            }
            else
            {
                rankLines[i].pos.text = i + 1 + "º";
                rankLines[i].tempo.text = "--:--";
                rankLines[i].data.text = "--/--/----";
                rankLines[i].erros.text = "--";
            }
        }
    }

    public void ChangeRankMode(int size)
    {
        currentRankSize = size;
        UpdateRank(size);
    }



    public void OpenRank()
    {
        audioManager.PlayClick();

        SetMainInteractable(false); // BLOQUEIA MENU

        panelRank.SetActive(true);
        ChangeRankMode(currentRankSize);

        CanvasGroup canvasGroup = panelRank.GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.alpha = 0;
        panelRank.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1, 0.25f));
        seq.Join(panelRank.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
    }

    public void CloseRank()
    {
        audioManager.PlayClick();

        CanvasGroup canvasGroup = panelRank.GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(0, 0.25f));
        seq.Join(panelRank.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));
        seq.OnComplete(() =>
        {
            panelRank.SetActive(false);
            panelRank.transform.localScale = Vector3.one;

            SetMainInteractable(true); // DESBLOQUEIA MENU
            HighlightRankMode(currentRankSize);
        });
    }


    public void OpenSettings()
    {
        audioManager.PlayClick();

        SetMainInteractable(false); // BLOQUEIA MENU

        panelSettings.SetActive(true);
        CanvasGroup canvasGroup = panelSettings.GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.alpha = 0;
        panelSettings.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1, 0.25f));
        seq.Join(panelSettings.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
    }


    public void CloseSettings()
    {
        audioManager.PlayClick();

        CanvasGroup canvasGroup = panelSettings.GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(0, 0.25f));
        seq.Join(panelSettings.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));
        seq.OnComplete(() =>
        {
            panelSettings.SetActive(false);
            panelSettings.transform.localScale = Vector3.one;

            SetMainInteractable(true); // DESBLOQUEIA MENU
        });
    }




    private void AdjustmentGrid(int gridSize)
    {
        float width = gridRect.rect.width;
        float height = gridRect.rect.height;

        float spacing = Mathf.Min(width, height) * 0.005f;
        gridLayout.spacing = new Vector2(spacing, spacing);

        float padding = Mathf.Min(width, height) * 0.02f;
        gridLayout.padding.left = gridLayout.padding.right = gridLayout.padding.top = gridLayout.padding.bottom = Mathf.RoundToInt(padding);

        int columns = gridSize;
        int rows = Mathf.CeilToInt((float)(gridSize * gridSize) / columns);

        float cellWidth = (width - gridLayout.padding.left - gridLayout.padding.right - (spacing * (columns - 1))) / columns;
        float cellHeight = (height - gridLayout.padding.top - gridLayout.padding.bottom - (spacing * (rows - 1))) / rows;

        float cellSize = Mathf.Min(cellWidth, cellHeight);
        gridLayout.cellSize = new Vector2(cellSize, cellSize);

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridRect);
    }

    private void SetMainInteractable(bool value)
    {
        if (mainCanvasGroup == null) return;

        mainCanvasGroup.interactable = value;
        mainCanvasGroup.blocksRaycasts = value;
    }

    private void RestoreMainInteraction()
    {
        if (mainCanvasGroup == null) return;

        mainCanvasGroup.interactable = true;
        mainCanvasGroup.blocksRaycasts = true;
    }


}

[System.Serializable]
public class ScoreEntry
{
    public float time;
    public int errors;
    public string dateTime;

    public ScoreEntry(float time, int errors)
    {
        this.time = time;
        this.errors = errors;
        this.dateTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }
}

[System.Serializable]
public class ScoreList
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

