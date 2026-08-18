using System.Text;
using UnityEngine;

/// <summary>
/// Enter a name, then view it as draggable revolving 3D text. Drag text to move;
/// drag empty space to orbit the camera.
/// </summary>
public class Starter : MonoBehaviour
{
    enum AppPhase
    {
        EnterName,
        Viewing,
    }

    const string DefaultHint = "Enter your name";

    [SerializeField] float revolutionSpeed = 35f;
    [SerializeField] float orbitSensitivity = 0.25f;
    [SerializeField] float dragSensitivity = 0.004f;

    AppPhase phase = AppPhase.EnterName;
    string nameInput = string.Empty;
    TouchScreenKeyboard keyboard;

    Transform textRoot;
    TextMesh textMesh;
    MeshRenderer textRenderer;
    BoxCollider textCollider;
    Font englishFont;

    Camera orbitCamera;
    Vector3 textWorldPosition = Vector3.zero;

    float orbitYaw = 0f;
    float orbitPitch = 14f;
    float orbitDistance = 5f;

    bool draggingText;
    bool orbitingCamera;
    Vector2 lastPointerPosition;
    int activePointerId = -1;

    void Start()
    {
        SetupLighting();
        SetupCamera();
    }

    void Update()
    {
        if (phase == AppPhase.Viewing)
        {
            HandleViewingInput();
            RevolveText();
            UpdateCameraFromOrbit();
        }

        PollMobileKeyboard();
    }

    void OnGUI()
    {
        if (phase == AppPhase.EnterName)
        {
            DrawNameEntryUi();
            return;
        }

        DrawViewingHints();
    }

    void DrawNameEntryUi()
    {
        var panel = new Rect(Screen.width * 0.08f, Screen.height * 0.28f, Screen.width * 0.84f, Screen.height * 0.38f);
        GUI.Box(panel, GUIContent.none);

        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 34,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
        };
        GUI.Label(new Rect(panel.x, panel.y + 24f, panel.width, 44f), "Name Show", titleStyle);

        var hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.85f, 0.88f, 0.92f) },
        };
        GUI.Label(new Rect(panel.x, panel.y + 72f, panel.width, 32f), "English letters only (A–Z)", hintStyle);

        var fieldRect = new Rect(panel.x + 24f, panel.y + 120f, panel.width - 48f, 52f);
        GUI.SetNextControlName("NameField");
        nameInput = GUI.TextField(fieldRect, nameInput, 24);

        nameInput = FilterEnglishLetters(nameInput);

        if (GUI.Button(new Rect(panel.x + 24f, panel.y + 188f, panel.width - 48f, 56f), "Show in 3D"))
        {
            TryBeginViewing();
        }

        if (TouchScreenKeyboard.isSupported &&
            GUI.Button(new Rect(panel.x + 24f, panel.y + 252f, panel.width - 48f, 48f), "Open phone keyboard"))
        {
            OpenMobileKeyboard();
        }
    }

    void DrawViewingHints()
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            alignment = TextAnchor.LowerCenter,
            normal = { textColor = new Color(0.75f, 0.78f, 0.85f, 0.9f) },
        };
        GUI.Label(
            new Rect(0f, Screen.height - 72f, Screen.width, 56f),
            "Drag text to move  •  Drag elsewhere to rotate view",
            style);
    }

    void OpenMobileKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open(
            nameInput,
            TouchScreenKeyboardType.Default,
            false,
            false,
            false,
            false,
            DefaultHint);
    }

    void PollMobileKeyboard()
    {
        if (keyboard == null)
        {
            return;
        }

        nameInput = FilterEnglishLetters(keyboard.text);

        if (keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            keyboard = null;
            TryBeginViewing();
        }
    }

    void TryBeginViewing()
    {
        var cleaned = FilterEnglishLetters(nameInput).Trim();
        if (string.IsNullOrEmpty(cleaned))
        {
            return;
        }

        phase = AppPhase.Viewing;
        Create3DText(cleaned);
        textWorldPosition = Vector3.zero;
        textRoot.position = textWorldPosition;
        orbitYaw = 0f;
        orbitPitch = 14f;
        UpdateTextCollider();
        UpdateCameraFromOrbit();
    }

    static string FilterEnglishLetters(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (character == ' ')
            {
                builder.Append(' ');
                continue;
            }

            if (character >= 'A' && character <= 'Z')
            {
                builder.Append(character);
            }
            else if (character >= 'a' && character <= 'z')
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }

    void SetupLighting()
    {
        RenderSettings.ambientLight = new Color(0.22f, 0.25f, 0.3f);

        var lightGo = new GameObject("Key Light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.15f;
        light.color = new Color(1f, 0.96f, 0.9f);
        lightGo.transform.rotation = Quaternion.Euler(52f, -30f, 0f);

        var fillGo = new GameObject("Fill Light");
        var fill = fillGo.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.35f;
        fill.color = new Color(0.55f, 0.75f, 1f);
        fillGo.transform.rotation = Quaternion.Euler(10f, 140f, 0f);
    }

    void SetupCamera()
    {
        orbitCamera = Camera.main;
        if (orbitCamera == null)
        {
            return;
        }

        orbitCamera.backgroundColor = new Color(0.07f, 0.08f, 0.13f);
        orbitCamera.clearFlags = CameraClearFlags.SolidColor;
    }

    void Create3DText(string displayName)
    {
        if (textRoot == null)
        {
            var textGo = new GameObject("NameShow3DText");
            textRoot = textGo.transform;

            textMesh = textGo.AddComponent<TextMesh>();
            textRenderer = textGo.GetComponent<MeshRenderer>();
            textCollider = textGo.AddComponent<BoxCollider>();

            textRenderer.receiveShadows = false;
            textRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        englishFont = LoadEnglishFont();
        textMesh.font = englishFont;
        textMesh.text = displayName;
        textMesh.fontSize = 96;
        textMesh.characterSize = ScaleCharacterSize(displayName.Length);
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = new Color(0.35f, 0.88f, 1f);

        if (englishFont != null && englishFont.material != null)
        {
            textRenderer.sharedMaterial = englishFont.material;
            textRenderer.sharedMaterial.color = textMesh.color;
        }
    }

    static Font LoadEnglishFont()
    {
        var embedded = Resources.Load<Font>("Fonts/Arial");
        if (embedded != null)
        {
            return embedded;
        }

        var builtin = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (builtin != null)
        {
            return builtin;
        }

        return Font.CreateDynamicFontFromOSFont("Arial", 64);
    }

    static float ScaleCharacterSize(int length)
    {
        if (length <= 6)
        {
            return 0.12f;
        }

        if (length <= 10)
        {
            return 0.09f;
        }

        return 0.07f;
    }

    void UpdateTextCollider()
    {
        if (textCollider == null || textRenderer == null)
        {
            return;
        }

        var bounds = textRenderer.bounds;
        textCollider.center = textRoot.InverseTransformPoint(bounds.center);
        textCollider.size = bounds.size + Vector3.one * 0.15f;
    }

    void RevolveText()
    {
        if (textRoot == null)
        {
            return;
        }

        textRoot.Rotate(Vector3.up, revolutionSpeed * Time.deltaTime, Space.World);
    }

    void HandleViewingInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
            return;
        }

        HandleMouseInput();
    }

    void HandleTouchInput()
    {
        var touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                activePointerId = touch.fingerId;
                lastPointerPosition = touch.position;
                draggingText = IsPointerOnText(touch.position);
                orbitingCamera = !draggingText;
                break;

            case TouchPhase.Moved:
                if (touch.fingerId != activePointerId)
                {
                    return;
                }

                ApplyDrag(touch.position - lastPointerPosition);
                lastPointerPosition = touch.position;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                draggingText = false;
                orbitingCamera = false;
                activePointerId = -1;
                break;
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPointerPosition = Input.mousePosition;
            draggingText = IsPointerOnText(Input.mousePosition);
            orbitingCamera = !draggingText;
        }
        else if (Input.GetMouseButton(0))
        {
            ApplyDrag((Vector2)Input.mousePosition - lastPointerPosition);
            lastPointerPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            draggingText = false;
            orbitingCamera = false;
        }
    }

    void ApplyDrag(Vector2 screenDelta)
    {
        if (draggingText)
        {
            MoveTextByScreenDelta(screenDelta);
        }
        else if (orbitingCamera)
        {
            orbitYaw += screenDelta.x * orbitSensitivity;
            orbitPitch -= screenDelta.y * orbitSensitivity;
            orbitPitch = Mathf.Clamp(orbitPitch, -20f, 75f);
        }
    }

    bool IsPointerOnText(Vector2 screenPosition)
    {
        if (orbitCamera == null || textCollider == null)
        {
            return false;
        }

        var ray = orbitCamera.ScreenPointToRay(screenPosition);
        return textCollider.Raycast(ray, out _, 100f);
    }

    void MoveTextByScreenDelta(Vector2 screenDelta)
    {
        if (orbitCamera == null || textRoot == null)
        {
            return;
        }

        var scale = orbitDistance * dragSensitivity;
        var move = orbitCamera.transform.right * screenDelta.x + orbitCamera.transform.up * screenDelta.y;
        textWorldPosition += move * scale;
        textRoot.position = textWorldPosition;
        UpdateCameraFromOrbit();
    }

    void UpdateCameraFromOrbit()
    {
        if (orbitCamera == null)
        {
            return;
        }

        var rotation = Quaternion.Euler(orbitPitch, orbitYaw, 0f);
        var offset = rotation * new Vector3(0f, 0.35f, -orbitDistance);
        orbitCamera.transform.position = textWorldPosition + offset;
        orbitCamera.transform.LookAt(textWorldPosition + Vector3.up * 0.15f);
    }
}
