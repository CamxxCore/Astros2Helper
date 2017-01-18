using GTA;
using Astros2.Core;
using Astros2.ScriptBase.Extensions;

namespace Astros2.ScriptBase
{
    public class AstrosVehicle : ScriptEntity<Vehicle>
    {
        public const int NameHash = -494727742;

        private ProjectileManager projectileMgr;

        protected AstrosVehicle(Vehicle vehicle) : base(vehicle)
        {
            projectileMgr = new ProjectileManager();
            AddExtension(projectileMgr);
        }

        public void FireMissile()
        {
            projectileMgr.RunProjectiles();
        }

        public static AstrosVehicle FromVehicle(Vehicle vehicle)
        {
            return new AstrosVehicle(vehicle);
        }
    }
}
