using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Linq;

namespace Celeste.Mod.FurryHelper {
    [CustomEntity("FurryHelper/GlitchWall")]
    public class GlitchWall : Solid {
        private static Vector2 screenSize = new(Celeste.GameWidth, Celeste.GameHeight);
        private static readonly char[] separators = { ',' };
        private readonly float[] TimeDelays;
        private readonly char TileType;
        private float timer = 0;
        private bool atEnd = false;
        private Vector2 StartPos;
        private Vector2 EndPos;
        private TileGrid StartTile;
        private TileGrid EndTile;
        private TileInterceptor StartInterceptor;
        private TileInterceptor EndInterceptor;

        private Vector2 LevelTopLeft {
            get {
                Rectangle bounds = SceneAs<Level>().Bounds;
                return new Vector2(bounds.Left, bounds.Top);
            }
        }
        private Vector2 LevelBotRight {
            get {
                Rectangle bounds = SceneAs<Level>().Bounds;
                return new Vector2(bounds.Right, bounds.Bottom);
            }
        }

        public GlitchWall(EntityData data, Vector2 offset) : base(offset, data.Width, data.Height, true) {
            TileType = data.Char("tiletype", 'm');

            StartPos = data.Position + offset;
            EndPos = data.Nodes[0] + offset;

            Position = data.Position + offset;
            Tag = Tags.PauseUpdate;

            float bps = data.Int("BPM", 120) / 60f;
            string[] delays = data.Attr("TimeDelays", "1")
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Trim())
                .ToArray();
            TimeDelays = new float[delays.Length];
            for (int i = 0; i < delays.Length; i++) {
                TimeDelays[i] = float.Parse(delays[i]) / bps;
            }
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            StartTile = getCurrentTilegrid();
            StartInterceptor = new TileInterceptor(StartTile, highPriority: true);
            Position = EndPos;
            EndTile = getCurrentTilegrid();
            EndInterceptor = new TileInterceptor(StartTile, highPriority: true);
            Position = StartPos;

            Add(StartTile);
            Add(StartInterceptor);
            Add(new LightOcclude());
            Add(new Coroutine(DoPattern()));
        }

        public TileGrid getCurrentTilegrid() {
            TileGrid tileGrid;
            Level level = SceneAs<Level>();
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            VirtualMap<char> solidsData = level.SolidsData;
            int x = (int)(X / 8f) - tileBounds.Left;
            int y = (int)(Y / 8f) - tileBounds.Top;
            int tilesX = (int)Width / 8;
            int tilesY = (int)Height / 8;
            tileGrid = GFX.FGAutotiler.GenerateOverlay(TileType, x, y, tilesX, tilesY, solidsData).TileGrid;
            Depth = -10501;
            return tileGrid;
        }

        public void SetStartGrid() {
            Remove(EndTile);
            Remove(EndInterceptor);
            Add(StartTile);
            Add(StartInterceptor);
        }

        public void SetEndGrid() {
            Remove(StartTile);
            Remove(StartInterceptor);
            Add(EndTile);
            Add(EndInterceptor);
        }

        public override void Update() {
            base.Update();
            if (!Scene.Paused) {
                timer -= Engine.DeltaTime;
            }
        }

        private IEnumerator DoPattern() {
            int counter = 0;
            float sCounter = 0;
            Player current;
            Level curLevel = SceneAs<Level>();
            Vector2 playerOffset = screenSize / 2;
            Vector2 playerOffsetEnd = playerOffset;
            Vector2 playerOffsetStart = playerOffset;
            while (true) {
                if (TimeDelays[counter] == 0) {
                    counter = (counter + 1) % TimeDelays.Length;
                    continue;
                }

                timer = TimeDelays[counter];
                while (timer > 0) {
                    yield return null;
                }

                sCounter += TimeDelays[counter];
                counter = (counter + 1) % TimeDelays.Length;
                if (TimeDelays[counter] == 0) {
                    continue;
                }

                current = GetPlayerRider();

                //teleport
                if (atEnd) {
                    if (current != null) {
                        playerOffsetEnd = current.Position - curLevel.Camera.Position;
                        current.Position += StartPos - Position;
                    }
                    //curLevel.Camera.Position += StartPos - Position;
                    Position = StartPos;
                    playerOffset = playerOffsetStart;
                    SetStartGrid();
                } else {
                    if (current != null) {
                        playerOffsetStart = current.Position - curLevel.Camera.Position;
                        current.Position += EndPos - Position;
                    }
                    //curLevel.Camera.Position += EndPos - Position;
                    Position = EndPos;
                    playerOffset = playerOffsetEnd;
                    SetEndGrid();
                }

                //move Camera
                if (current != null) {
                    Vector2 camPos = current.Position - playerOffset;
                    camPos = Vector2.Clamp(camPos, LevelTopLeft, LevelBotRight - screenSize);
                    curLevel.Camera.Position = camPos;
                }

                current = CollideFirst<Player>();
                if (current != null && current.Collider is Hitbox && !IsDreamDashing(current)) {
                    Vector2 direction = IntersectionDiff(Hitbox, current.Collider as Hitbox);
                    if (Math.Max(Math.Abs(direction.X), Math.Abs(direction.Y)) <= 8) {
                        current.Position += direction;
                    } else {
                        direction.Normalize();
                        current.Die(direction);
                    }
                }

                atEnd = !atEnd;
            }
        }
        private static bool IsDreamDashing(Player player) {
            bool inDreamState = player.StateMachine.State == Player.StDreamDash;
            bool inDreamTunnelState = player.StateMachine.State == (CommunalHelperImports.StDreamTunnelDash?.Invoke() ?? -1);
            return inDreamState || inDreamTunnelState;
        }

        public Vector2 IntersectionDiff(Hitbox hitbox1, Hitbox hitbox2) {
            if (!hitbox1.Collide(hitbox2)) {
                return Vector2.Zero;
            }

            float top = hitbox1.AbsoluteTop - hitbox2.AbsoluteBottom;
            float bot = hitbox1.AbsoluteBottom - hitbox2.AbsoluteTop;
            float left = hitbox1.AbsoluteLeft - hitbox2.AbsoluteRight;
            float right = hitbox1.AbsoluteRight - hitbox2.AbsoluteLeft;
            Vector2 diff = new(
                Math.Abs(left) > Math.Abs(right) ? right : left,
                Math.Abs(top) > Math.Abs(bot) ? bot : top
            );
            if (Math.Abs(diff.X) < Math.Abs(diff.Y)) {
                diff.Y = 0;
            } else {
                diff.X = 0;
            }

            return diff;
        }

        public Player GetPlayerSliding() {
            foreach (Player entity in Scene.Tracker.GetEntities<Player>()) {
                if (entity.Facing == Facings.Left && CollideCheck(entity, Position + Vector2.UnitX)) {
                    return entity;
                }

                if (entity.Facing == Facings.Right && CollideCheck(entity, Position - Vector2.UnitX)) {
                    return entity;
                }
            }

            return null;
        }
    }
}
