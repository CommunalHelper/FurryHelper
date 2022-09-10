using MonoMod.ModInterop;
using System;

namespace Celeste.Mod.FurryHelper {
    public class FurryHelperModule : EverestModule {

        public static FurryHelperModule Instance { get; private set; }

        public FurryHelperModule() {
            Instance = this;
        }

        public override void Load() {
            typeof(CommunalHelperImports).ModInterop();
        }

        public override void Unload() { }
    }

    [ModImportName("CommunalHelper.DashStates")]
    public static class CommunalHelperImports {
        public static Func<int> StDreamTunnelDash;
    }
}