using System;
using TendedTarsier;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class GameplayController : MonoBehaviour
{
    private readonly CompositeDisposable _compositeDisposable = new();

    [SerializeField]
    private Button _menuButton;

    private GeneralConfig _generalConfig;
    private GameplayProfile _gameplayProfile;

    [Inject]
    private void Construct(GeneralConfig generalConfig, GameplayProfile gameplayProfile)
    {
        _gameplayProfile = gameplayProfile;
        _generalConfig = generalConfig;
    }

    private void Start()
    {
        InitButtons();
    }

    public void OnGameplayStarted()
    {
        _gameplayProfile.StartDate ??= DateTime.UtcNow;
        _gameplayProfile.Save();
    }

    private void InitButtons()
    {
        _menuButton.OnClickAsObservable().Subscribe(OnMenuButtonClick).AddTo(_compositeDisposable);
    }

    private void OnMenuButtonClick(Unit _)
    {
        SceneManager.LoadScene(_generalConfig.MenuScene);
    }

    private void OnDestroy()
    {
        _compositeDisposable.Dispose();
    }
}