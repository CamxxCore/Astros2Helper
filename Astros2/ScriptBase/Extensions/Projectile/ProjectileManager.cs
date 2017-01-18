using System;
using GTA;
using GTA.Math;
using GTA.Native;
using Astros2.Core;
using Astros2.Core.Interfaces;

namespace Astros2.ScriptBase.Extensions
{
    public class ProjectileManager : IScriptEntityExtension<Vehicle>
    {
        public int ID { get; } = (int) EExtensionID.ProjectileMgr;

        public const int MaxProjectiles = 250;

        public ScriptEntity<Vehicle> Entity { get; set; }

        private LoopedPTFX launcherSmokeFx = new LoopedPTFX("core", "exp_grd_rpg_spawn");

        private ProjectileEntity[] activeEntityPool = new ProjectileEntity[MaxProjectiles];

        private Vector2[] turretAxisExtents = new Vector2[]
        {
            new Vector2(1.1f, 1.1f), // top left
            new Vector2(0.5f, 1.1f), // top left inside
            new Vector2(-1.1f, 1.1f), // top right
            new Vector2(-0.45f, 1.1f), // top right inside
            new Vector2(1.1f, 0.35f), // bottom left
            new Vector2(0.5f, 0.35f), // bottom left inside
            new Vector2(-1.1f, 0.35f), // bottom right
            new Vector2(-0.45f, 0.35f), // bottom right inside
        };

        private int activeEntityCount = 0, projectileIndex = 0;

        public void RunProjectiles()
        {
            int boneIdx = Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, Entity.BaseRef.Handle, "turret_1barrel");

            var barrelCoord = Function.Call<Vector3>(Hash._GET_ENTITY_BONE_COORDS, Entity.BaseRef.Handle, boneIdx);

            boneIdx = Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, Entity.BaseRef.Handle, "turret_1base");

            var baseCoord = Function.Call<Vector3>(Hash._GET_ENTITY_BONE_COORDS, Entity.BaseRef.Handle, boneIdx);

            Vector3 turretDirection = Vector3.Normalize(barrelCoord - baseCoord);

            Vector3 horzAxis = Vector3.Cross(turretDirection, new Vector3(0.0f, 0.0f, 1.0f));

            Vector3 vertAxis = Vector3.Cross(horzAxis, turretDirection);

            Vector3 rotation = Utils.DirectionToRotation(turretDirection);

            projectileIndex++;
            projectileIndex %= turretAxisExtents.Length;

            //for (int i = 0; i < turretAxisExtents.Length; i++)
            // {
            Vector3 position = barrelCoord + (horzAxis * turretAxisExtents[projectileIndex].X) + (vertAxis * turretAxisExtents[projectileIndex].Y);

            ProjectileEntity p = CreateProjectile(position, rotation);

            p.SetProjectileTarget(position + turretDirection * 56.0f);
            // }

            if (!launcherSmokeFx.IsLoaded) launcherSmokeFx.Load();

            launcherSmokeFx.Start(baseCoord, 1.0f);
        }

        private ProjectileEntity CreateProjectile(Vector3 position, Vector3 rotation)
        {
            if (activeEntityCount >= activeEntityPool.Length) return null;

            for (int i = activeEntityPool.Length - 1; i > 0; i--)
                activeEntityPool[i] = activeEntityPool[i - 1];

            Model model = new Model(-1146260322);

            if (!model.IsLoaded)
            {
                model.Request(1000);
            }

            Prop prop = World.CreateProp(model, position, rotation, false, false);

            activeEntityPool[0] = ProjectileEntity.FromObject(prop);

            activeEntityCount = Math.Min(activeEntityCount + 1, activeEntityPool.Length);

            return activeEntityPool[0];
        }

        public void RemoveActiveEntity(int entityIndex)
        {
            if (entityIndex < 0 || entityIndex > activeEntityCount - 1)
                return;

            for (int i = entityIndex; i < activeEntityCount - 1; i++)
            {
                activeEntityPool[i] = activeEntityPool[i + 1];
            }

            activeEntityCount--;
        }

        public void Update()
        {
            for (int i = 0; i < activeEntityCount; i++)
            {
                if (!activeEntityPool[i].Active)
                {
                    activeEntityPool[i].Remove();

                    RemoveActiveEntity(i);
                }

                activeEntityPool[i].OnUpdate();
            }
        }

        public void Dispose()
        {
            launcherSmokeFx.Remove();
        }
    }
}
