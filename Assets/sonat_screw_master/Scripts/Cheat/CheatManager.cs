using DG.Tweening;
using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : SingletonBase<CheatManager>
{
    public InputField levelInput;
    public InputField coinInput;
    public InputField levelChestInput;

    public GameObject cheatPanel;

    private int count = 0;

    [SerializeField] private GameObject panelPassword;
    [SerializeField] private InputField passwordInput;

    [SerializeField] private GameObject cheatShow;

    [SerializeField] private Toggle toggleTurn;

    [SerializeField] private Button changePlatform;

    public TMP_Text textBG;

    public Image bg;
    public Sprite defaultBG;

    public InputField massInput;
    public InputField angularInput;
    public InputField gravityInput;
    public InputField scaleScrewInput;
    public InputField opacityInput;

    public TMP_Text logicFirst;

    private int indexBG = 0;

    public enum EPlatForm
    {
        Android,
        IOS
    }

    public EPlatForm curPlatform;

    public PlayerData playerData => DataManager.Instance.playerData;

    private void Awake()
    {
        toggleTurn.onValueChanged.AddListener((state) =>
        {
            Turn(state);
        });

        //#if UNITY_EDITOR
        cheatPanel.SetActive(true);
        //#endif

        massInput.text = "1";
        angularInput.text = "0.4";
        gravityInput.text = "4";

        logicFirst.text = "Off";

        scaleScrewInput.text = "1.3";
        opacityInput.text = "0.6";
    }

    public void DebugTracking()
    {
    }

    public void OnClick()
    {
        Debug.Log("1");

        if (cheatPanel.activeSelf)
        {
            cheatPanel.SetActive(false);

            return;
        }

        if (count == 0)
        {
            DOVirtual.DelayedCall(3f, () =>
            {
                count = 0;
            });
        }

        count++;

        if (count == 6)
        {
            panelPassword.SetActive(true);
        }
    }

    public void ConfirmButton()
    {
        if (passwordInput.text == "sonat@123")
        {
            cheatPanel.SetActive(true);
        }

        panelPassword.SetActive(false);

    }

    public void ClosePanel()
    {
        cheatPanel.SetActive(false);
    }

    public void Turn(bool isOn)
    {
        cheatShow.SetActive(isOn);
    }

    public void Win()
    {
        GameplayManager.Instance.Test_Win();
    }

    public void Lose()
    {
        GameplayManager.Instance.Lose();
    }

    public void Reload()
    {
        GameplayManager.Instance.StartLevel();
    }

    public void Next_Level()
    {
        playerData.saveLevelData.currentLevel++;
        playerData.saveLevelData.playCount = 0;
        playerData.preLevel = playerData.nextLevel;

        GameplayManager.Instance.StartLevel();
    }

    public void Back_Level()
    {
        playerData.saveLevelData.currentLevel--;
        playerData.saveLevelData.playCount = 0;
        playerData.preLevel = playerData.nextLevel;

        GameplayManager.Instance.StartLevel();
    }

    public void Set_Level()
    {
        int level;

        if (int.TryParse(levelInput.text, out level))
        {
            playerData.saveLevelData.currentLevel = level;
            playerData.saveLevelData.playCount = 0;
            playerData.preLevel = playerData.nextLevel;

            GameplayManager.Instance.StartLevel();
        }
    }

    public void Set_Level(string levelStr)
    {
        int level;

        if (int.TryParse(levelStr, out level))
        {
            playerData.saveLevelData.currentLevel = level;
            playerData.saveLevelData.playCount = 0;
            playerData.preLevel = playerData.nextLevel;

            GameplayManager.Instance.StartLevel();
        }
    }

    public void Set_Coin()
    {
        int coin;

        if (int.TryParse(coinInput.text, out coin))
        {
            playerData.coins = coin;

            DataManager.Instance.Save();
        }
    }

    public void SetLevelChest()
    {
        LevelChestManager.SetCheat(int.Parse(levelChestInput.text));
    }

    public void UnlockAllSkin()
    {
        foreach (var boxData in BoxController.Instance.BoxData.boxes)
        {
            SkinSystem.UnlockSkin(SkinSystem.ESkin.Box, boxData.id);
        }
    }

    public void ChangePlatform()
    {
        // Mở hộp thoại chọn file (hỗ trợ nhiều nền tảng)
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg") // Lọc file ảnh PNG, JPG
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Sprite", "", extensions, false);

        // Kiểm tra xem người dùng có chọn file hay không
        if (paths.Length > 0)
        {
            string path = paths[0]; // Lấy đường dẫn file

            // Đọc dữ liệu từ file
            byte[] fileData = File.ReadAllBytes(path);

            // Tạo texture từ dữ liệu file
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); // Load ảnh vào texture

            // Tạo sprite từ texture
            Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            // Gán sprite vào SpriteRenderer để hiển thị
            bg.sprite = newSprite;
        }
    }

    public void ResetBackground()
    {
        bg.sprite = defaultBG;
    }

    private IEnumerator capture;

    public void CaptureScreenshot()
    {
        capture = IECapture();
        StartCoroutine(capture);
    }

    public void StopCapture()
    {
        if (capture != null)
        {
            StopCoroutine(capture);
            capture = null;
            Turn(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopCapture();
        }
    }

    private IEnumerator IECapture()
    {
        // Đường dẫn lưu ảnh
        string path = Path.Combine(Application.dataPath, "Screenshots");

        // Tạo thư mục nếu chưa có
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Đặt tên file với timestamp

        // Sử dụng ScreenCapture để chụp ảnh

        Turn(false);
        yield return new WaitForEndOfFrame();

        string folderPath = "Levels";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(folderPath);

        List<string> name = new();

        foreach (string file in files)
        {
            if (Path.GetExtension(file).Equals(".meta", System.StringComparison.OrdinalIgnoreCase))
                continue;

            string fileNameWithoutExtensions = Path.GetFileNameWithoutExtension(file);

            while (Path.GetExtension(fileNameWithoutExtensions).Length > 0)
            {
                fileNameWithoutExtensions = Path.GetFileNameWithoutExtension(fileNameWithoutExtensions);
            }

            // Add the cleaned file name to the list
            name.Add(fileNameWithoutExtensions);
        }


        //foreach (var asset in files)
        //{
        //    if (Path.GetExtension(asset.name).Equals(".json", System.StringComparison.OrdinalIgnoreCase) || Path.GetExtension(asset.name).Equals(".txt", System.StringComparison.OrdinalIgnoreCase))
        //        name.Add(asset.name);
        //}

        var sortedNumbers = name.OrderBy(n => int.Parse(n)).ToArray();

        for (int i = 0; i < sortedNumbers.Length; i++)
        {
            Set_Level(sortedNumbers[i]);
            yield return new WaitForSeconds(0.3f);
            string screenshotFilename = Path.Combine(path, $"{DataManager.Instance.playerData.saveLevelData.currentLevel}.png");
            ScreenCapture.CaptureScreenshot(screenshotFilename, 1); // scale 1 là độ phân giải gốc
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        StopCapture();
    }

    public (float, float, float) GetPhysicConfig()
    {
        return (float.Parse(massInput.text), float.Parse(angularInput.text), float.Parse(gravityInput.text));
    }

    public float GetScaleScrew()
    {
        return float.Parse(scaleScrewInput.text);
    }

    public float GetOpacity()
    {
        return float.Parse(opacityInput.text);
    }

    public float GetScalePercent()
    {
        return float.Parse(scaleScrewInput.text) / 1.3f;
    }

    public void ResetPhysicConfig()
    {
        massInput.text = "1";
        angularInput.text = "0.4";
        gravityInput.text = "4";
    }

    public void SetLogicFirst()
    {
        if (logicFirst.text == "On")
        {
            logicFirst.text = "Off";
        }
        else
        {
            logicFirst.text = "On";
        }
    }

    public bool IsLogicFirst()
    {
        return logicFirst.text == "On";
    }
}
