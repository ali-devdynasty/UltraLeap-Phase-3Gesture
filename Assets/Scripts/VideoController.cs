using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;

    private void Awake()
    {
        if (videoPlayer == null || rawImage == null)
        {
            Debug.LogError("VideoPlayer or RawImage is not assigned.");
            return;
        }

        // Ensure the VideoPlayer is not set to play on awake
        videoPlayer.playOnAwake = false;
    }

    public void PlayVideo()
    {
        if (videoPlayer == null || rawImage == null)
        {
            Debug.LogError("VideoPlayer or RawImage is not assigned.");
            return;
        }

        // Ensure the VideoPlayer is prepared
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        // Set the texture of the RawImage to the VideoPlayer's RenderTexture
        rawImage.texture = videoPlayer.targetTexture;
        videoPlayer.Play();
    }
}


