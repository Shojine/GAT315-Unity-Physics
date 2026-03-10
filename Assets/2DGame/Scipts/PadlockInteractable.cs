using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// 4-digit padlock controlled with arrow keys.
///   Left / Right  — move between digits
///   Up   / Down   — increment / decrement selected digit (wraps 0-9)
/// Auto-unlocks when the correct code is entered.
/// On solve: closes UI, enables RoomInteractable on this GameObject, disables itself.
///
/// UI hierarchy:
///   Canvas > PadlockPanel
///     DigitDisplays   — 4 x TMP_Text showing each digit
///     SelectorDisplay — TMP_Text (or Image) showing which digit is selected (optional)
///     CloseButton     — wire to CloseUI()
/// </summary>
public class PadlockInteractable : MonoBehaviour
{
    [Header("First Look")]
    [SerializeField] private string firstLookMessage = "Hmm, there's a lock here. Maybe there's a code somewhere...";

    [Header("Combination")]
    [SerializeField] private string correctCode = ""; // Leave blank for a random code, or set a fixed code

    [Header("UI")]
    [SerializeField] private GameObject padlockPanel;
    [SerializeField] private TMP_Text[] digitDisplays = new TMP_Text[4]; // One per digit
    [SerializeField] private TMP_Text selectorDisplay; // Optional: shows which digit is active e.g. "^"

    [Header("Colors")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("Audio")]
    [SerializeField] private AudioClip unlockSound;

    [Header("Lock Object")]
    [SerializeField] private SpriteRenderer lockSprite; // Lock sprite to hide when solved

    public static string RuntimeCode = null; // Shared in memory, resets each play session

    private int[] digits = new int[4];
    private int selectedIndex = 0;
    private bool isOpen = false;
    private bool isPlayerInRange = false;
    private bool isSolved = false;
    private bool hasShownFirstLook = false;
    private bool isWaitingToOpen = false;
    private AudioSource audioSource;
    private InputListener[] playerInputListeners;
    private DialogueManager dialogueManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogueManager = FindAnyObjectByType<DialogueManager>();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerInputListeners = player.GetComponents<InputListener>();

        // Resolve the code: fixed in Inspector > already generated this session > generate new random
        if (!string.IsNullOrEmpty(correctCode))
        {
            RuntimeCode = correctCode;
        }
        else if (RuntimeCode != null)
        {
            correctCode = RuntimeCode;
        }
        else
        {
            correctCode = Random.Range(0, 10000).ToString("D4");
            RuntimeCode = correctCode;
        }

        if (InventoryManager.Instance.HasKey("initialPadlock"))
            hasShownFirstLook = true;

        if (padlockPanel != null)
            padlockPanel.SetActive(false);

        UpdateDisplays();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Open UI when player presses E in range (blocked after solving)
        if (!isOpen && !isSolved && !isWaitingToOpen && isPlayerInRange && kb.eKey.wasPressedThisFrame)
        {
            OpenUI();
            return;
        }

        if (!isOpen) return;

        if (kb.leftArrowKey.wasPressedThisFrame)  MoveSelector(-1);
        if (kb.rightArrowKey.wasPressedThisFrame) MoveSelector(1);
        if (kb.upArrowKey.wasPressedThisFrame)    ChangeDigit(1);
        if (kb.downArrowKey.wasPressedThisFrame)  ChangeDigit(-1);
        if (kb.escapeKey.wasPressedThisFrame)     CloseUI();
    }

    // ── Interaction ──────────────────────────────────────────────

    public void OnInteract()
    {
        if (!enabled || !isPlayerInRange || isSolved) return;
        OpenUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseUI();
        }
    }

    // ── UI ───────────────────────────────────────────────────────

    public void OpenUI()
    {
        if (padlockPanel == null) return;

        if (!hasShownFirstLook && !string.IsNullOrEmpty(firstLookMessage) && !InventoryManager.Instance.HasKey("initialPadlock"))
        {
            hasShownFirstLook = true;
            isWaitingToOpen = true;
            InventoryManager.Instance.AddKey("initialPadlock");
            dialogueManager?.ShowDialogue(firstLookMessage);
            StartCoroutine(OpenAfterDialogue(firstLookMessage));
            return;
        }

        padlockPanel.SetActive(true);
        isOpen = true;
        selectedIndex = 0;
        UpdateDisplays();

        if (playerInputListeners != null)
            foreach (var l in playerInputListeners) l.enabled = false;
    }

    private System.Collections.IEnumerator OpenAfterDialogue(string message)
    {
        yield return new WaitForSeconds(dialogueManager.totalDisplayTime + 0.5f);

        OpenUI();
    }

    public void CloseUI()
    {
        if (padlockPanel == null) return;
        padlockPanel.SetActive(false);
        isOpen = false;

        if (playerInputListeners != null)
            foreach (var l in playerInputListeners) l.enabled = true;
    }

    // ── Digit Controls ───────────────────────────────────────────

    private void MoveSelector(int direction)
    {
        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, 3);
        UpdateDisplays();
    }

    private void ChangeDigit(int direction)
    {
        digits[selectedIndex] = (digits[selectedIndex] + direction + 10) % 10;
        UpdateDisplays();
        CheckCode();
    }

    private void UpdateDisplays()
    {
        for (int i = 0; i < digitDisplays.Length; i++)
        {
            if (digitDisplays[i] == null) continue;
            digitDisplays[i].text = digits[i].ToString();
            digitDisplays[i].color = (i == selectedIndex) ? selectedColor : defaultColor;
        }
    }

    // ── Auto Check ───────────────────────────────────────────────

    private void CheckCode()
    {
        string entered = $"{digits[0]}{digits[1]}{digits[2]}{digits[3]}";
        if (entered != correctCode) return;

        isSolved = true;
        CloseUI();

        if (lockSprite != null)
            lockSprite.enabled = false;

        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);

        var door = GetComponent<RoomInteractable>();
        if (door != null) door.enabled = true;

        this.enabled = false;
    }
}
