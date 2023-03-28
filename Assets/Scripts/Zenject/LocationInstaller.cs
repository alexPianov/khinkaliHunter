using Cysharp.Threading.Tasks;
using Playstel;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Zenject
{
    public class LocationInstaller : MonoInstaller
    {
        public Player Player;
        public UiInfo Info;
        
        public override void InstallBindings()
        {
            BindFromInstance(this);
            BindFromInstance(Player);
            BindFromInstance(Info);
        }

        public void BindFromInstance<T>(T instance)
        {
            if (Container.HasBinding<T>())
            {
                Container.Unbind<T>();
            }

            Container.Bind<T>().FromInstance(instance).AsTransient();
        }
    }
}