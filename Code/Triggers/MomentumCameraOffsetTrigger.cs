using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.FurryHelper {
    [CustomEntity("FurryHelper/momentumCameraOffsetTrigger")]
    public class MomentumCameraOffsetTrigger : Trigger {
        private readonly MomentumModes momentumMode;
        private readonly float momentumFrom;
        private readonly float momentumTo;
        private readonly bool onlyOnce;
        private readonly bool xOnly;
        private readonly bool yOnly;
        private Vector2 offsetFrom;
        private Vector2 offsetTo;
        private Vector2 prevPos;
        private bool skipFrame = false;

        public MomentumCameraOffsetTrigger(EntityData data, Vector2 offset) 
            : base(data, offset) {
            // parse the trigger attributes. Multiplying X dimensions by 48 and Y ones by 32 replicates the vanilla offset trigger behavior.
            offsetFrom = new Vector2(data.Float("offsetXFrom") * 48f, data.Float("offsetYFrom") * 32f);
            offsetTo = new Vector2(data.Float("offsetXTo") * 48f, data.Float("offsetYTo") * 32f);
            momentumFrom = data.Float("momentumFrom");
            momentumTo = data.Float("momentumTo");
            momentumMode = data.Enum<MomentumModes>("momentumMode");
            onlyOnce = data.Bool("onlyOnce");
            xOnly = data.Bool("xOnly");
            yOnly = data.Bool("yOnly");
            prevPos = new Vector2(0f, 0f);
        }

        public enum MomentumModes {
            HorizontalMomentum,
            VerticalMomentum
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
            if (Engine.FreezeTimer > 0) {
                skipFrame = true;
                return;
            }

            if (skipFrame) {
                skipFrame = false;
                prevPos = player.Position;
                return;
            }

            if (!yOnly) {
                SceneAs<Level>().CameraOffset.X = MathHelper.Lerp(offsetFrom.X, offsetTo.X, GetMomentumLerp(player));
            }

            if (!xOnly) {
                SceneAs<Level>().CameraOffset.Y = MathHelper.Lerp(offsetFrom.Y, offsetTo.Y, GetMomentumLerp(player));
            }

            prevPos = player.Position;
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);

            if (onlyOnce) {
                RemoveSelf();
            }
        }
        protected float GetMomentumLerp(Player player) {
            Vector2 Speed = player.Position - prevPos;
            Speed *= Engine.FPS;
            return momentumMode switch {
                MomentumModes.HorizontalMomentum => Calc.ClampedMap(Speed.X, momentumFrom, momentumTo),
                MomentumModes.VerticalMomentum => Calc.ClampedMap(Speed.Y, momentumFrom, momentumTo),
                _ => 1f,
            };
        }
    }
}
