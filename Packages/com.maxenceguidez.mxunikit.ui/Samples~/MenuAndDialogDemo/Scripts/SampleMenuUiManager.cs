using MxUnikit.Log;
using UnityEngine.UIElements;

namespace MxUnikit.UI.Samples
{
    public class SampleMenuUiManager : MxMenuUiManager<SampleMenuType>
    {
        public SampleHomeMenu HomeMenu { get; private set; }
        public SampleSettingsMenu SettingsMenu { get; private set; }
        public SamplePauseMenu PauseMenu { get; private set; }

        private Button _pauseButton;

        protected override void OnUiReady()
        {
            HomeMenu = new SampleHomeMenu();
            SettingsMenu = new SampleSettingsMenu();
            PauseMenu = new SamplePauseMenu();

            Root.Add(HomeMenu);
            Root.Add(SettingsMenu);
            Root.Add(PauseMenu);

            RegisterMenu(SampleMenuType.Home, HomeMenu);
            RegisterMenu(SampleMenuType.Settings, SettingsMenu);
            RegisterMenu(SampleMenuType.Pause, PauseMenu);

            _pauseButton = new Button(OpenPauseMenu) { text = "Pause" };
            _pauseButton.AddToClassList("mx-demo-pause-button");
            _pauseButton.AddToClassList(MxStyles.HiddenClassName);
            Root.Add(_pauseButton);

            HomeMenu.OnClickSettings += () => ShowMenu(SampleMenuType.Settings);
            SettingsMenu.OnClickBack += OnClickBack;
            PauseMenu.OnClickResume += ResumeGameplay;
            PauseMenu.OnClickQuitToMenu += QuitToHome;

            ShowMenu(SampleMenuType.Home, isRoot: true);
        }

        // Simulates entering gameplay: menus hide and the floating pause button appears.
        public void StartGameplay()
        {
            HideAllMenus();
            _pauseButton.RemoveFromClassList(MxStyles.HiddenClassName);
        }

        private void OpenPauseMenu()
        {
            _pauseButton.AddToClassList(MxStyles.HiddenClassName);
            ShowMenu(SampleMenuType.Pause, isRoot: true);
        }

        private void ResumeGameplay()
        {
            HideAllMenus();
            _pauseButton.RemoveFromClassList(MxStyles.HiddenClassName);
        }

        private void QuitToHome()
        {
            _pauseButton.AddToClassList(MxStyles.HiddenClassName);
            ShowMenu(SampleMenuType.Home, isRoot: true);
        }

        protected override void OnMenuShown(SampleMenuType key, MxMenu menu)
        {
            MxLog.L($"Menu '{key}' shown.");
        }

        protected override void OnAllMenusHidden()
        {
            MxLog.L("All menus hidden.");
        }
    }
}
