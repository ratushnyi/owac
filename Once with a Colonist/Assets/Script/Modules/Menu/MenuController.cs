using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using Unity.Netcode;
using UnityEngine.EventSystems;
using InventoryProfile = TendedTarsier.Script.Modules.General.Profiles.Inventory.InventoryProfile;
using MapProfile = TendedTarsier.Script.Modules.General.Profiles.Map.MapProfile;
using StatsProfile = TendedTarsier.Script.Modules.General.Profiles.Stats.StatsProfile;

namespace TendedTarsier.Script.Modules.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private Image _background;

        [SerializeField]
        private Button _continueButton;

        [SerializeField]
        private Button _newGameButton;

        [SerializeField]
        private Button _joinGameButton;

        [SerializeField]
        private Button _exitButton;

        private PlayerProfile _playerProfile;
        private StatsProfile _statsProfile;
        private MapProfile _mapProfile;
        private InventoryProfile _inventoryProfile;
        private MenuConfig _menuConfig;
        private GeneralConfig _generalConfig;
        private EventSystem _eventSystem;
        private NetworkManager _networkManager;

        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        private void Construct(
            PlayerProfile playerProfile,
            StatsProfile statsProfile,
            MapProfile mapProfile,
            InventoryProfile inventoryProfile,
            MenuConfig menuConfig,
            GeneralConfig generalConfig,
            EventSystem eventSystem,
            NetworkManager networkManager)
        {
            _playerProfile = playerProfile;
            _statsProfile = statsProfile;
            _mapProfile = mapProfile;
            _inventoryProfile = inventoryProfile;
            _menuConfig = menuConfig;
            _generalConfig = generalConfig;
            _eventSystem = eventSystem;
            _networkManager = networkManager;
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
            _joinGameButton.targetGraphic.color = Color.clear;
            _exitButton.targetGraphic.color = Color.clear;

            var sequence = DOTween.Sequence();
            sequence.Append(_background.DOColor(_menuConfig.BackgroundFadeOutColor, _menuConfig.BackgroundFadeOutDuration));
            sequence.Join(_continueButton.targetGraphic.DOColor(Color.white, _menuConfig.BackgroundFadeOutDuration));
            sequence.Join(_newGameButton.targetGraphic.DOColor(Color.white, _menuConfig.BackgroundFadeOutDuration));
            sequence.Join(_joinGameButton.targetGraphic.DOColor(Color.white, _menuConfig.BackgroundFadeOutDuration));
            sequence.Join(_exitButton.targetGraphic.DOColor(Color.white, _menuConfig.BackgroundFadeOutDuration));
            sequence.SetEase(_menuConfig.BackgroundFadeOutCurve);

            _compositeDisposable.Add(Disposable.Create(() => sequence.Kill()));
        }

        private void InitButtons()
        {
            _continueButton.interactable = _playerProfile.FirstStartDate != null;
            _continueButton.OnClickAsObservable().Subscribe(OnContinueButtonClick).AddTo(_compositeDisposable);
            _newGameButton.OnClickAsObservable().Subscribe(OnNewGameButtonClick).AddTo(_compositeDisposable);
            _joinGameButton.OnClickAsObservable().Subscribe(OnJoinGameButtonClick).AddTo(_compositeDisposable);
            _exitButton.OnClickAsObservable().Subscribe(OnExitButtonClick).AddTo(_compositeDisposable);
            _eventSystem.SetSelectedGameObject(_continueButton.interactable ? _continueButton.gameObject : _newGameButton.gameObject);
        }

        private void OnContinueButtonClick(Unit _)
        {
            _networkManager.StartHost();
            _networkManager.SceneManager.LoadScene(_generalConfig.GameplayScene, LoadSceneMode.Single);
        }

        private void OnNewGameButtonClick(Unit _)
        {
            _playerProfile.Clear();
            _statsProfile.Clear();
            _mapProfile.Clear();
            _inventoryProfile.Clear();
            _networkManager.StartHost();
            _networkManager.SceneManager.LoadScene(_generalConfig.GameplayScene, LoadSceneMode.Single);
        }

        private void OnJoinGameButtonClick(Unit _)
        {
            _networkManager.StartClient();
            _networkManager.SceneManager.LoadScene(_generalConfig.GameplayScene, LoadSceneMode.Single);
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
}