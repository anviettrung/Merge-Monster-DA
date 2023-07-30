using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Screenshot
{
    static Screenshot()
    {
        EditorApplication.delayCall += () => {
            Resolution = Resolution;
            IsTransparentBackground = IsTransparentBackground;
            IsCropTransparent = IsCropTransparent;
        };
    }

    #region MenuItems

    /// <summary>
    /// Take a screenshot of the current view (Scene or Game)
    /// Shortcut : Shift + P
    /// </summary>
    [MenuItem("📷/Take Screenshot #p", priority = 100)]
    public static void TakeScreenshot()
    {
        string filePath = EditorUtility.SaveFilePanel("Save Screenshot", "*", "screenshot.png", "png");
        if (string.IsNullOrEmpty(filePath))
            return;

        int width = 1000;
        int height = 1000;

        switch (Resolution) {
            case 1:
                width = 1280;
                height = 720;
                break;
            case 2:
                width = 720;
                height = 1280;
                break;
            case 3:
                width = 1920;
                height = 1080;
                break;
            case 4:
                width = 1080;
                height = 1920;
                break;
            case 5:
                width = 3840;
                height = 2160;
                break;
            case 6:
                width = 7680;
                height = 4320;
                break;
        }

        Texture2D texture;
        if (IsTransparentBackground) {
            texture = CaptureTransparentScreenshot(GetCamera(), width, height);
        } else {
            texture = CaptureScreenshot(GetCamera(), width, height);
        }

        if (IsCropTransparent)
            CropTransparent(texture);

        texture.SaveToDisk(filePath);

        Object.DestroyImmediate(texture);
    }

    /// <summary>
    /// If set to true, the background is replace with a transparent background (handles semi transparency)
    /// </summary>
    [MenuItem("📷/Transparent Background", priority = 300)]
    private static void ToggleTransparentBackground()
    {
        IsTransparentBackground = !IsTransparentBackground;
        if (!IsTransparentBackground)
            IsCropTransparent = false;
    }

    /// <summary>
    /// IF set to true, crops the image to remove all unused space
    /// </summary>
    [MenuItem("📷/Crop Transparent", priority = 301)]
    private static void ToggleCropTransparent()
    {
        IsCropTransparent = !IsCropTransparent;
    }

    [MenuItem("📷/Crop Transparent", true)]
    private static bool CanCrop()
    {
        return IsTransparentBackground;
    }

    [MenuItem("📷/Landscape/720p", priority = 400)]
    private static void Set720pLS() => Resolution = 1;

    [MenuItem("📷/Portrait/720p", priority = 401)]
    private static void Set720pPT() => Resolution = 2;

    [MenuItem("📷/Landscape/1080p", priority = 402)]
    private static void Set1080pLS() => Resolution = 3;

    [MenuItem("📷/Portrait/1080p", priority = 403)]
    private static void Set1080pPT() => Resolution = 4;

    [MenuItem("📷/Landscape/2160p", priority = 404)]
    private static void Set4K() => Resolution = 5;

    [MenuItem("📷/Landscape/4320p", priority = 405)]
    private static void Set8K() => Resolution = 6;

    #endregion

    private static bool IsCropTransparent {
        get { return EditorPrefs.GetBool("SCREENSHOT_CROPTRANSPARENT", true); }
        set {
            EditorPrefs.SetBool("SCREENSHOT_CROPTRANSPARENT", value);
            Menu.SetChecked("📷/Crop Transparent", value);
        }
    }

    private static bool IsTransparentBackground {
        get { return EditorPrefs.GetBool("SCREENSHOT_ISTRANSPARENTBACKGROUND", true); }
        set {
            EditorPrefs.SetBool("SCREENSHOT_ISTRANSPARENTBACKGROUND", value);
            Menu.SetChecked("📷/Transparent Background", value);
        }
    }

    private static int Resolution {
        get { return EditorPrefs.GetInt("SCREENSHOT_RESOLUTION", 2); }
        set {
            EditorPrefs.SetInt("SCREENSHOT_RESOLUTION", value);
            Menu.SetChecked("📷/Landscape/720p", value == 1);
            Menu.SetChecked("📷/Portrait/720p", value == 2);
            Menu.SetChecked("📷/Landscape/1080p", value == 3);
            Menu.SetChecked("📷/Portrait/1080p", value == 4);
            Menu.SetChecked("📷/Landscape/2160p", value == 5);
            Menu.SetChecked("📷/Landscape/4320p", value == 6);
        }
    }

    private static Camera GetCamera()
    {
        if (Camera.current)
            return Camera.current;
        return Object.FindObjectOfType<Camera>();
    }

    public static Texture2D CaptureTransparentScreenshot(Camera camera, int width, int height)
    {
        // This is slower, but seems more reliable.
        RenderTexture cameraTargetTexture = camera.targetTexture;
        CameraClearFlags cameraClearFlags = camera.clearFlags;
        RenderTexture renderTextureActive = RenderTexture.active;

        var shotWhiteBg = new Texture2D(width, height, TextureFormat.ARGB32, false);
        var shotBlackBg = new Texture2D(width, height, TextureFormat.ARGB32, false);
        var shotTrnspBg = new Texture2D(width, height, TextureFormat.ARGB32, false);

        // Must use 24-bit depth buffer to be able to fill background.
        var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grabArea = new Rect(0, 0, width, height);

        RenderTexture.active = renderTexture;
        camera.targetTexture = renderTexture;
        camera.clearFlags = CameraClearFlags.SolidColor;

        camera.backgroundColor = Color.black;
        camera.Render();
        shotBlackBg.ReadPixels(grabArea, 0, 0);
        shotBlackBg.Apply();

        camera.backgroundColor = Color.white;
        camera.Render();
        shotWhiteBg.ReadPixels(grabArea, 0, 0);
        shotWhiteBg.Apply();

        Color[] whitePixels = shotWhiteBg.GetPixels();
        Color[] blackPixels = shotBlackBg.GetPixels();

        // Create Alpha from the difference between black and white camera renders
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float alpha = whitePixels[x + y * width].r - blackPixels[x + y * width].r;
                alpha = 1.0f - alpha;
                Color color;
                if (alpha == 0) {
                    color = Color.clear;
                } else {
                    color = blackPixels[x + y * width] / alpha;
                }
                color.a = alpha;
                blackPixels[x + y * width] = color;
            }
        }

        shotTrnspBg.SetPixels(blackPixels);

        // Clear temporary textures
        Object.DestroyImmediate(shotBlackBg);
        Object.DestroyImmediate(shotWhiteBg);

        // Revert properties back
        camera.clearFlags = cameraClearFlags;
        camera.targetTexture = cameraTargetTexture;
        RenderTexture.active = renderTextureActive;
        RenderTexture.ReleaseTemporary(renderTexture);

        return shotTrnspBg;
    }

    public static Texture2D CaptureScreenshot(Camera camera, int width, int height)
    {
        // This is slower, but seems more reliable.
        RenderTexture cameraTargetTexture = camera.targetTexture;
        CameraClearFlags cameraClearFlags = camera.clearFlags;
        RenderTexture renderTextureActive = RenderTexture.active;

        var shotWidthBg = new Texture2D(width, height, TextureFormat.ARGB32, false);

        // Must use 24-bit depth buffer to be able to fill background.
        var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grabArea = new Rect(0, 0, width, height);

        RenderTexture.active = renderTexture;
        camera.targetTexture = renderTexture;
        camera.clearFlags = CameraClearFlags.SolidColor;

        //camera.backgroundColor = Color.white;
        camera.Render();
        shotWidthBg.ReadPixels(grabArea, 0, 0);
        shotWidthBg.Apply();

        // Revert properties back
        camera.clearFlags = cameraClearFlags;
        camera.targetTexture = cameraTargetTexture;
        RenderTexture.active = renderTextureActive;
        RenderTexture.ReleaseTemporary(renderTexture);

        return shotWidthBg;
    }

    const float CROP_ALPHA_THRESHOLD = 0.01f;

    public static void CropTransparent(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();

        int BottomToTop()
        {
            for (int y = 0; y < texture.height; y++)
                for (int x = 0; x < texture.width; x++)
                    if (pixels[x + y * texture.width].a > CROP_ALPHA_THRESHOLD)
                        return y;
            return 0;
        }

        int TopToBottom()
        {
            for (int y = texture.height - 1; y > 0; y--)
                for (int x = 0; x < texture.width; x++)
                    if (pixels[x + y * texture.width].a > CROP_ALPHA_THRESHOLD)
                        return y;
            return texture.height;
        }

        int LeftToRight()
        {
            for (int x = 0; x < texture.width; x++)
                for (int y = 0; y < texture.height; y++)
                    if (pixels[x + y * texture.width].a > CROP_ALPHA_THRESHOLD)
                        return x;
            return 0;
        }

        int RightToLeft()
        {
            for (int x = texture.width - 1; x >= 0; x--)
                for (int y = 0; y < texture.height; y++)
                    if (pixels[x + y * texture.width].a > CROP_ALPHA_THRESHOLD)
                        return x;
            return texture.width;
        }

        int cx = LeftToRight();
        int cy = BottomToTop();
        int cwidth = RightToLeft() - cx;
        int cheight = TopToBottom() - cy;

        Color[] croppedPixels = texture.GetPixels(cx, cy, cwidth, cheight);
        Debug.Log($"Cropped from {texture.width}x{texture.height} to {cwidth}x{cheight}");

        texture.Resize(cwidth, cheight);

        texture.SetPixels(croppedPixels);
    }

    public static void SaveToDisk(this Texture2D texture, string filePath)
    {
        byte[] bytes = ImageConversion.EncodeToPNG(texture);
        File.WriteAllBytes(filePath, bytes);
    }
}