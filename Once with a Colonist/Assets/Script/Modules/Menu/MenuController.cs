using DG.Tweening;
using TendedTarsier;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MenuController : MonoBehaviour
{
    private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

    [SerializeField]
    private Image _background;

    [SerializeField]
    private Button _continueButton;

    [SerializeField]
    private Button _newGameButton;

    [SerializeField]
    private Button _exitButton;

    private PlayerProfile _playerProfile;
    private TilemapProfile _tilemapProfile;
    private InventoryProfile _inventoryProfile;
    private MenuConfig _menuConfig;
    private GeneralConfig _generalConfig;

    [Inject]
    private void Construct(PlayerProfile playerProfile,
        TilemapProfile tilemapProfile,
        InventoryProfile inventoryProfile,
        MenuConfig menuConfig,
        GeneralConfig generalConfig)
    {
        _playerProfile = playerProfile;
        _tilemapProfile = tilemapProfile;
        _inventoryProfile = inventoryProfile;
        _menuConfig = menuConfig;
        _generalConfig = generalConfig;
    }

    private void Start()
    {
        InitBackground();
        InitButtons();
    }

    private void InitBackground()
    {
        _background.color = Color.black;
        _continueButton.targetGraphic.color = Color.clear;
        _newGameButton.targetGraphic.color = Color.clear;
        _exitButton.targetGraphic.color = Color.clear;

        var sequence = DOTween.Sequence();
        sequence.Append(_background.DOColor(_menuConfig.BackgroundFadeOutColor, _menuConfig.BackgroundFadeOutDuration));
        sequence.Join(_continueButton.targetGraphic.DOColor(Color.white, _menuConfig.BackgroundFadeOutDuration));
        sequence.Join(_newGameButton.targetGraphic.DOColor(Color.white, _menuConfig.BackgroundFadeOutDuration));
        sequence.Join(_exitButton.targetGraphic.DOColor(Color.white, _menuConfig.BackgroundFadeOutDuration));
        sequence.SetEase(_menuConfig.BackgroundFadeOutCurve);

        _compositeDisposable.Add(Disposable.Create(() => sequence.Kill()));
    }

    private void InitButtons()
    {
        _continueButton.interactable = _playerProfile.StartDate != null;
        _continueButton.OnClickAsObservable().Subscribe(OnContinueButtonClick).AddTo(_compositeDisposable);
        _newGameButton.OnClickAsObservable().Subscribe(OnNewGameButtonClick).AddTo(_compositeDisposable);
        _exitButton.OnClickAsObservable().Subscribe(OnExitButtonClick).AddTo(_compositeDisposable);
    }

    private void OnContinueButtonClick(Unit _)
    {
        SceneManager.LoadScene(_generalConfig.GameplayScene);
    }

    private void OnNewGameButtonClick(Unit _)
    {
        _playerProfile.Clear();
        _tilemapProfile.Clear();
        _inventoryProfile.Clear();
        SceneManager.LoadScene(_generalConfig.GameplayScene);
    }

    private void OnExitButtonClick(Unit _)
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        _compositeDisposable.Dispose();
    }
}