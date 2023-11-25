using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace TendedTarsier
{
    public class GameplayController : MonoBehaviour
    {
        private readonly CompositeDisposable _compositeDisposable = new ();

        [SerializeField]
        private Button _menuButton;

        private GeneralConfig _generalConfig;
        private PlayerProfile _playerProfile;

        [Inject]
        private void Construct(GeneralConfig generalConfig, PlayerProfile playerProfile)
        {
            _playerProfile = playerProfile;
            _generalConfig = generalConfig;
        }

        private void Start()
        {
            InitButtons();
        }

        public void OnGameplayStarted()
        {
            _playerProfile.StartDate ??= DateTime.UtcNow;
            _playerProfile.Save();
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
}