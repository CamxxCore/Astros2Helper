using GTA;
using GTA.Math;
using GTA.Native;
using Astros2.Core;
using Astros2.Core.Events;
using Astros2.Core.Interfaces;

namespace Astros2.ScriptBase.Extensions
{
    public class ProjectileEntity : ScriptEntity<Prop>
    {
        public bool Active {  get { return updatingProjectile; } }

        private Vector3 target, startPosition;

        private float speed = 100.0f;

        private bool updatingProjectile = true;

        private Vector3 lastPosition;

        private LoopedPTFX trailFx = new LoopedPTFX("scr_exile1", "scr_ex1_heatseeker");

        protected ProjectileEntity(Prop entity) : base(entity)
        {
            PreInit();
            EnterWater += OnEnterWater;
        }

        private void PreInit()
        {
            startPosition = BaseRef.Position;

            Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, BaseRef.Handle, true);

            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, BaseRef.Handle, true);

            Function.Call(Hash.SET_ENTITY_LOD_DIST, BaseRef.Handle, 1000);

            Function.Call(Hash.SET_ENTITY_COLLISION, BaseRef.Handle, false, true);

            if (!trailFx.IsLoaded) trailFx.Load();

            trailFx.Start(BaseRef, 0.3f, new Vector3(0.0f, -0.2f, 0.0f), Vector3.Zero, null);

           trailFx.SetEvolution("LOD", 10000);
        }

        public static ProjectileEntity FromObject(Prop obj)
        {       
            return new ProjectileEntity(obj);
        }

        public void SetProjectileSpeed(float speed)
        {
            this.speed = speed;
        }

        public void SetProjectileTarget(Vector3 target)
        {
            this.target = target;
        }

        public override void OnUpdate()
        {
            if (updatingProjectile)
            {
                Vector3 position = BaseRef.Position;

                if (!Function.Call<bool>(Hash.HAS_COLLISION_LOADED_AROUND_ENTITY, BaseRef))
                {
                    Function.Call(Hash.REQUEST_COLLISION_AT_COORD, position.X, position.Y, position.Z);
                }

                if ((Function.Call<bool>(Hash.HAS_ENTITY_COLLIDED_WITH_ANYTHING, BaseRef.Handle) || BaseRef.HeightAboveGround < 1.0f) && BaseRef.Position.DistanceTo(startPosition) > 15.0f)
                {
                    Scripts.CreateBombExplosion(BaseRef.Position, BaseRef.IsInWater);

                    updatingProjectile = false;
                }

                else
                {
                    Vector3 vec = Vector3.Normalize(target - startPosition);

                    Vector3 direction = vec * speed;

                    BaseRef.ApplyForce(direction);

                    BaseRef.Velocity *= 1.0674f;

                    Function.Call(Hash.SET_ENTITY_MAX_SPEED, BaseRef, 72.0f);

                    float dist = startPosition.X - target.X;

                    float curveFactor = 0.023f * (position.X - startPosition.X) * (position.X - target.X) / (-0.25f * dist * dist);

                    BaseRef.Position = new Vector3(BaseRef.Position.X, BaseRef.Position.Y, BaseRef.Position.Z + curveFactor);

                    BaseRef.Rotation = Utils.DirectionToRotation(Vector3.Normalize(position - lastPosition));

                    lastPosition = position;
                }
            }

            base.OnUpdate();
        }

        private void OnEnterWater(IManagedEntity sender, EntityChangedEventArgs args)
        {
            Scripts.CreateBombExplosion(BaseRef.Position, BaseRef.IsInWater);

            updatingProjectile = false;
        }

        public override void Remove()
        {
            updatingProjectile = false;

            trailFx.Remove();

            base.Remove();
        }
    }
}
