using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;
using SiraUtil;

namespace CustomEnergyBar.Installer
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<CustomEnergyBarController>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
        }
    }
}
