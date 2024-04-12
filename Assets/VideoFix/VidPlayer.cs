using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class VidPlayer : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private string videoFileName;
    
    
    // Start is called before the first frame update
    void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        // Get the video player component attached to the game object
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();

        // dont try accessing variables if null
        if (videoPlayer == null)
        {
            Debug.Log("NO VIDEO PLAYER COMPONENT FOUND ON " + gameObject.name);
            return;
        }
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        Debug.Log(videoPath);
        videoPlayer.url = videoPath;
        videoPlayer.Play();
    }
}
