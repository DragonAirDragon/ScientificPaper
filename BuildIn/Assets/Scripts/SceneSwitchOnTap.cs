using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitchOnTap : MonoBehaviour
{
	[Header("Input (New Input System)")]
	[Tooltip("Опционально: перетащите сюда действие типа Button (Tap/Click) из вашего Input Actions.")]
	public InputActionReference tapActionReference;

	private InputAction _tapAction;

	[Tooltip("Задержка перед загрузкой сцены, сек.")]
	public float delaySeconds = 0f;


	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void OnEnable()
	{
		if (tapActionReference != null && tapActionReference.action != null)
		{
			_tapAction = tapActionReference.action;
			if (!_tapAction.enabled) _tapAction.Enable();
            _tapAction.performed += OnTapPerformed;
		}

	}

	private void OnDisable()
	{
		if (_tapAction != null)
		{
			_tapAction.performed -= OnTapPerformed;
			if (tapActionReference == null) _tapAction.Dispose();
		}
	}

	private void OnTapPerformed(InputAction.CallbackContext _)
	{
		if (delaySeconds <= 0f)
		{
			LoadTargetScene();
		}
		else
		{
			Invoke(nameof(LoadTargetScene), delaySeconds);
		}
	}

	private void LoadTargetScene()
	{
		int total = SceneManager.sceneCountInBuildSettings;
		if (total == 0)
		{
			Debug.LogWarning("Build Settings пусты. Добавьте сцены в File > Build Settings.");
			return;
		}

		int current = SceneManager.GetActiveScene().buildIndex;
		int next = (current + 1) % total;
		SceneManager.LoadScene(next, LoadSceneMode.Single);
	}
}