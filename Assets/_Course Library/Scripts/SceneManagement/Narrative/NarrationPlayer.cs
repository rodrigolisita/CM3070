using UnityEngine;
using UnityEngine.Localization;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets; // <<< 1. ADD THIS NAMESPACE

/// <summary>
/// Handles playback of localized audio clips on demand.
/// Receives commands from StoryPlayer.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class NarrationPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private AsyncOperationHandle<AudioClip> currentLoadOperation;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Loads and plays a localized audio clip. Waits for playback completion.
    /// </summary>
    public IEnumerator PlayNarration(LocalizedAsset<AudioClip> localizedClipRef)
    {
        // Unload previous clip if one exists
        if (currentLoadOperation.IsValid())
        {
            // 2. CORRECTION: Release the previous operation handle, not the new asset reference.
            Addressables.Release(currentLoadOperation);
        }

        // Load new clip asynchronously
        currentLoadOperation = localizedClipRef.LoadAssetAsync();
        yield return currentLoadOperation;

        if (currentLoadOperation.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip clipToPlay = currentLoadOperation.Result;
            if (clipToPlay != null)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();

                // Wait for the audio clip to finish playing
                yield return new WaitForSeconds(clipToPlay.length);
            }
        }
        else
        {
            Debug.LogError($"Failed to load localized audio asset.");
        }
    }

    /// <summary>
    /// Stops any currently playing narration audio.
    /// </summary>
    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}