using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Menus")]
    [SerializeField] private GameObject musicPanelRoot;
    [SerializeField] private GameObject mainPanel;

    [Header("UI Audio")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip buttonClickSound;

    // Paramètres Audio Mixer
    private const string MASTER_PARAMETER = "MasterVolume";
    private const string MUSIC_PARAMETER = "MusicVolume";
    private const string SFX_PARAMETER = "SFXVolume";

    // PlayerPrefs
    private const string MASTER_PREF = "MasterVolume";
    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";

    // Valeur par défaut = 50 %
    private const float DEFAULT_VOLUME = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        PlayerPrefs.DeleteKey(MASTER_PREF); PlayerPrefs.DeleteKey(MUSIC_PREF); PlayerPrefs.DeleteKey(SFX_PREF);

        ChargerReglages();
        InitialiserSliders();

        // Abonnement des sliders
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SetMasterVolume);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    // =========================================================
    // VOLUME
    // =========================================================

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp01(value);

        audioMixer.SetFloat(
            MASTER_PARAMETER,
            ConvertToDecibels(value)
        );

        PlayerPrefs.SetFloat(MASTER_PREF, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);

        audioMixer.SetFloat(
            MUSIC_PARAMETER,
            ConvertToDecibels(value)
        );

        PlayerPrefs.SetFloat(MUSIC_PREF, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp01(value);

        audioMixer.SetFloat(
            SFX_PARAMETER,
            ConvertToDecibels(value)
        );

        PlayerPrefs.SetFloat(SFX_PREF, value);
        PlayerPrefs.Save();
    }

    // =========================================================
    // CHARGEMENT
    // =========================================================

    private void ChargerReglages()
    {
        float masterVolume = PlayerPrefs.GetFloat(MASTER_PREF, DEFAULT_VOLUME);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME);

        audioMixer.SetFloat(
            MASTER_PARAMETER,
            ConvertToDecibels(masterVolume)
        );

        audioMixer.SetFloat(
            MUSIC_PARAMETER,
            ConvertToDecibels(musicVolume)
        );

        audioMixer.SetFloat(
            SFX_PARAMETER,
            ConvertToDecibels(sfxVolume)
        );
    }

    // =========================================================
    // INITIALISATION DES SLIDERS
    // =========================================================

    private void InitialiserSliders()
    {
        float masterVolume = PlayerPrefs.GetFloat(
            MASTER_PREF,
            DEFAULT_VOLUME
        );

        float musicVolume = PlayerPrefs.GetFloat(
            MUSIC_PREF,
            DEFAULT_VOLUME
        );

        float sfxVolume = PlayerPrefs.GetFloat(
            SFX_PREF,
            DEFAULT_VOLUME
        );

        if (masterSlider != null)
            masterSlider.SetValueWithoutNotify(masterVolume);

        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(musicVolume);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxVolume);
    }

    // =========================================================
    // UI AUDIO
    // =========================================================

    public void PlayButtonClick()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    // =========================================================
    // MENU
    // =========================================================

    public void Retour()
    {
        if (musicPanelRoot != null)
            musicPanelRoot.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }

    // =========================================================
    // UTILITAIRE
    // =========================================================

    private float ConvertToDecibels(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }
}