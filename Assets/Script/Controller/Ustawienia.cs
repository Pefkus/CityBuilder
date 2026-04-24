using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class Ustawienia : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    [Header("Audio")]
    public AudioMixer mixer;

    private bool isPaused = false;

    void Start()
    {
        // Ukryj panel na starcie
        settingsPanel.SetActive(false);

        // Za³aduj zapisane ustawienia
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        fullscreenToggle.isOn = Screen.fullScreen;

        // Dodaj listenery do UI
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);
    }

    public void SetVolume(float value)
    {
        // Zamiana wartoœci suwaka (0-1) na decybele (-80 do 0)
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        mixer.SetFloat("MasterVol", dB);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        // Zapisujemy bool jako int (1 lub 0)
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void ExitGame()
    {
        Debug.Log("Zamykanie gry...");
        Application.Quit();
    }
}
