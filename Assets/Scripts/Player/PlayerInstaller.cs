using Enemies;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private PlayerRoot _player;
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private InputActionAsset _actions;

        [Header("Input ids")]
        [SerializeField] private string _actionMapName = "Player";
        [SerializeField] private string _moveActionName = "Move";
        [SerializeField] private string _lookActionName = "Look";

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();
            Container.BindInstance(_actions).AsSingle();

            Container.Bind<IPlayerInput>()
                .To<PlayerInputComponent>()
                .AsSingle()
                .WithArguments(
                    _actions,
                    _actionMapName,
                    _moveActionName,
                    _lookActionName);

            Container.BindInterfacesAndSelfTo<PlayerRoot>()
                .FromInstance(_player)
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PlayerAttackComponent>()
                .AsSingle()
                .WithArguments(_player.transform)
                .NonLazy();
            
            Container.BindInterfacesTo<PlayerTargetLineView>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}