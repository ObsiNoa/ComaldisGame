using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class AudioZoneCrossfade : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource outsideSource;   // звук ДО зоны
    public AudioSource insideSource;    // звук ВНУТРИ зоны

    [Header("Fade Settings")]
    public float fadeTime = 2.0f;

    [Header("Options")]
    public bool playInsideOnEnter = true;
    public bool playOutsideOnExit = true;

    private Coroutine _fadeRoutine;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (insideSource)
        {
            insideSource.volume = 0f;
            if (!insideSource.isPlaying && playInsideOnEnter)
                insideSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(Fade(outsideSource, insideSource));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(Fade(insideSource, outsideSource));
    }

    private IEnumerator Fade(AudioSource from, AudioSource to)
    {
        if (to && !to.isPlaying) to.Play();

        float t = 0f;
        float fromStart = from ? from.volume : 0f;
        float toStart = to ? to.volume : 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            float k = t / fadeTime;

            if (from) from.volume = Mathf.Lerp(fromStart, 0f, k);
            if (to) to.volume = Mathf.Lerp(toStart, 1f, k);

            yield return null;
        }

        if (from)
        {
            from.volume = 0f;
            from.Stop();
        }

        if (to) to.volume = 1f;
    }
}
