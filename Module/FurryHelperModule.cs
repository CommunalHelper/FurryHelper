using System;

namespace Celeste.Mod.FurryHelper {
    public class FurryHelperModule : EverestModule {

        public static bool CommunalHelperLoaded = false;

        public static FurryHelperModule Instance { get; private set; }

        public FurryHelperModule() {
            Instance = this;
        }

        public override void Load() { }

        public override void Unload() { }

        public override void LoadContent(bool firstLoad) {
            base.LoadContent(firstLoad);
            if (Everest.Loader.DependencyLoaded(new EverestModuleMetadata() { Name = "CommunalHelper", Version = new Version(1, 13, 2) })) {
                CommunalHelperLoaded = true;
            }
        }
    }
}