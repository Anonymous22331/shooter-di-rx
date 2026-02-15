using UnityEngine.SceneManagement;
using Zenject;

namespace Bootstrap
{
    public class GameBootstrapper : IInitializable
    {
        private readonly ZenjectSceneLoader _sceneLoader;
        private readonly string _firstScene;

        public GameBootstrapper(
            ZenjectSceneLoader sceneLoader,
            [Inject] string firstScene)
        {
            _sceneLoader = sceneLoader;
            _firstScene = firstScene;
        }

        public void Initialize()
        {
            _sceneLoader.LoadSceneAsync(_firstScene, LoadSceneMode.Single);
        }
    }
}