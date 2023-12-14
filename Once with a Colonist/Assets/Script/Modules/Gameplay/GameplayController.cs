using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TendedTarsier
{
    public class GameplayController : MonoBehaviour
    {
        private readonly CompositeDisposable _compositeDisposable = new ();
        
        private GeneralConfig _generalConfig;
        private PlayerProfile _playerProfile;
        private PanelLoader<ToolBarController> _toolBarPanel;

        [Inject]
        private void Construct(GeneralConfig generalConfig, PlayerProfile playerProfile, PanelLoader<ToolBarController> toolBarPanel)
        {
            _toolBarPanel = toolBarPanel;
            _playerProfile = playerProfile;
            _generalConfig = generalConfig;
        }

        private void Start()
        {
            InitToolBar();
        }

        public void OnGameplayStarted()
        {
            _playerProfile.StartDate ??= DateTime.UtcNow;
            _playerProfile.Save();
        }

        private async void InitToolBar()
        {
            await _toolBarPanel.Show();
            _toolBarPanel.Instance.MenuButton.OnClickAsObservable().Subscribe(OnMenuButtonClick).AddTo(_compositeDisposable);
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