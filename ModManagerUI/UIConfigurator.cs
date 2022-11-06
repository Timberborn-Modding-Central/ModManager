using Bindito.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TimberApi.ConfiguratorSystem;
using Timberborn.ModsSystemUI;

namespace ModManager
{
    [Configurator(TimberApi.SceneSystem.SceneEntrypoint.MainMenu)]
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<ModsBox>().AsSingleton();
            containerDefinition.Bind<IModService>().To<ModService>().AsSingleton();
        }
    }
}
