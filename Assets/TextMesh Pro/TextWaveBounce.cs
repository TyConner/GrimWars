using System.Collections;
using TMPro;
using UnityEngine;

public class TextWaveBounce : MonoBehaviour
{
    [SerializeField] TMP_Text[] texts;

    [SerializeField] float amplitude = 10f;
    [SerializeField] float frequency = 2f;
    [SerializeField] float speed = 2f;

    [SerializeField] float timePerText = 1.5f;
    [SerializeField] bool loop = true;

    [SerializeField] bool manualMode = false;
    [SerializeField] int manualIndex = 0;

    int currentIndex = 0;
    Coroutine sequenceRoutine;

    Vector3[][][] cachedOriginalVertices;

    void Awake()
    {
        CacheAllOriginalVertices();
    }

    void Start()
    {
        if (texts == null || texts.Length == 0)
            return;

        if (manualMode)
        {
            manualIndex = Mathf.Clamp(manualIndex, 0, texts.Length - 1);
            RestoreAllTexts();
            return;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, texts.Length - 1);
        sequenceRoutine = StartCoroutine(RunSequence());
    }

    void Update()
    {
        if (!manualMode)
            return;

        if (texts == null || texts.Length == 0)
            return;

        if (manualIndex < 0 || manualIndex >= texts.Length)
            return;

        TMP_Text text = texts[manualIndex];
        if (text == null)
            return;

        if (cachedOriginalVertices == null ||
            manualIndex >= cachedOriginalVertices.Length ||
            cachedOriginalVertices[manualIndex] == null)
            return;

        AnimateManual(text, manualIndex);
    }

    void CacheAllOriginalVertices()
    {
        if (texts == null)
        {
            cachedOriginalVertices = null;
            return;
        }

        cachedOriginalVertices = new Vector3[texts.Length][][];

        for (int textIndex = 0; textIndex < texts.Length; textIndex++)
        {
            TMP_Text text = texts[textIndex];
            if (text == null)
                continue;

            text.ForceMeshUpdate();
            TMP_TextInfo textInfo = text.textInfo;

            cachedOriginalVertices[textIndex] = new Vector3[textInfo.meshInfo.Length][];
            for (int meshIndex = 0; meshIndex < textInfo.meshInfo.Length; meshIndex++)
            {
                cachedOriginalVertices[textIndex][meshIndex] = (Vector3[])textInfo.meshInfo[meshIndex].vertices.Clone();
            }
        }
    }

    IEnumerator RunSequence()
    {
        if (texts == null || texts.Length == 0)
            yield break;

        while (true)
        {
            if (currentIndex < 0 || currentIndex >= texts.Length)
                yield break;

            TMP_Text text = texts[currentIndex];

            if (text != null &&
                cachedOriginalVertices != null &&
                currentIndex < cachedOriginalVertices.Length &&
                cachedOriginalVertices[currentIndex] != null)
            {
                yield return AnimateSingle(text, currentIndex);
            }

            currentIndex++;

            if (currentIndex >= texts.Length)
            {
                if (loop)
                    currentIndex = 0;
                else
                    yield break;
            }
        }
    }

    IEnumerator AnimateSingle(TMP_Text text, int textIndex)
    {
        if (text == null)
            yield break;

        if (cachedOriginalVertices == null ||
            textIndex < 0 ||
            textIndex >= cachedOriginalVertices.Length ||
            cachedOriginalVertices[textIndex] == null)
            yield break;

        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (i >= cachedOriginalVertices[textIndex].Length || cachedOriginalVertices[textIndex][i] == null)
                yield break;

            originalVertices[i] = (Vector3[])cachedOriginalVertices[textIndex][i].Clone();
        }

        float timer = 0f;

        while (timer < timePerText)
        {
            text.ForceMeshUpdate();
            textInfo = text.textInfo;

            float normalizedTime = timer / timePerText;
            float envelope = Mathf.Sin(normalizedTime * Mathf.PI);
            float waveTime = Time.time * speed;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                if (materialIndex < 0 || materialIndex >= originalVertices.Length)
                    continue;

                Vector3[] sourceVertices = originalVertices[materialIndex];
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                if (vertexIndex + 3 >= sourceVertices.Length || vertexIndex + 3 >= destinationVertices.Length)
                    continue;

                float offsetY = Mathf.Sin(waveTime + i * frequency) * amplitude * envelope;
                Vector3 offset = new Vector3(0f, offsetY, 0f);

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Restore(text, originalVertices);
    }

    void AnimateManual(TMP_Text text, int textIndex)
    {
        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        Vector3[][] originalVertices = cachedOriginalVertices[textIndex];
        float waveTime = Time.time * speed;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            if (materialIndex < 0 || materialIndex >= originalVertices.Length)
                continue;

            Vector3[] sourceVertices = originalVertices[materialIndex];
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            if (sourceVertices == null || destinationVertices == null)
                continue;

            if (vertexIndex + 3 >= sourceVertices.Length || vertexIndex + 3 >= destinationVertices.Length)
                continue;

            float offsetY = Mathf.Sin(waveTime + i * frequency) * amplitude;
            Vector3 offset = new Vector3(0f, offsetY, 0f);

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    public void SetManualMode(bool enabled)
    {
        manualMode = enabled;

        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        RestoreAllTexts();

        if (!manualMode && texts != null && texts.Length > 0)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, texts.Length - 1);
            sequenceRoutine = StartCoroutine(RunSequence());
        }
    }

    public void SetActiveIndex(int index)
    {
        if (texts == null || texts.Length == 0)
            return;

        if (index < 0 || index >= texts.Length)
            return;

        if (manualIndex >= 0 &&
            manualIndex < texts.Length &&
            texts[manualIndex] != null &&
            cachedOriginalVertices != null &&
            manualIndex < cachedOriginalVertices.Length &&
            cachedOriginalVertices[manualIndex] != null)
        {
            Restore(texts[manualIndex], cachedOriginalVertices[manualIndex]);
        }

        manualIndex = index;
    }

    void RestoreAllTexts()
    {
        if (texts == null || cachedOriginalVertices == null)
            return;

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null)
                continue;

            if (i >= cachedOriginalVertices.Length || cachedOriginalVertices[i] == null)
                continue;

            Restore(texts[i], cachedOriginalVertices[i]);
        }
    }

    void Restore(TMP_Text text, Vector3[][] originalVertices)
    {
        if (text == null || originalVertices == null)
            return;

        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (i >= originalVertices.Length || originalVertices[i] == null)
                continue;

            Vector3[] dest = textInfo.meshInfo[i].vertices;
            Vector3[] src = originalVertices[i];

            int count = Mathf.Min(dest.Length, src.Length);
            for (int j = 0; j < count; j++)
            {
                dest[j] = src[j];
            }

            textInfo.meshInfo[i].mesh.vertices = dest;
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}