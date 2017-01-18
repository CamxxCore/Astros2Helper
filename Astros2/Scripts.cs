using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Native;
using GTA.Math;
using GTA;

namespace Astros2
{
    public static class Scripts
    {
        public static void RequestScriptAudioBank(string name)
        {
            while (!Function.Call<bool>(Hash.REQUEST_SCRIPT_AUDIO_BANK, name, 0))
                Script.Wait(0);
        }

        public static void RequestPTFXAsset(string name)
        {
            if (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, name))
            {
                Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, name);

                while (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, name))
                    Script.Wait(0);
            }
        }

        public static uint GetPedVehicleWeapon(Ped ped)
        {
            var outArg = new OutputArgument();
            Function.Call(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, ped.Handle, outArg);
            return (uint)outArg.GetResult<int>();
        }


        public static void CreateBombExplosion(Vector3 position, bool water)
        {
            Function.Call(Hash._SET_PTFX_ASSET_NEXT_CALL, "scr_agencyheist");

            Function.Call(Hash.ADD_EXPLOSION, position.X, position.Y, position.Z, (int)ExplosionType.Valkyrie, 10.0f, true, true, 1.4f);
            //   World.AddExplosion(ent.Position, ExplosionType.Train, 1.0f, 1.0f);
            GameplayCamera.Shake(CameraShake.LargeExplosion, 0.09f);
       /*     Script.Wait(20);
            World.AddExplosion(ent.Position + ent.Position.LeftVector(Vector3.WorldUp) * 3, (ExplosionType)17, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.Position.RightVector(Vector3.WorldUp) * 3, (ExplosionType)26, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.ForwardVector * 3, (ExplosionType)17, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position - ent.ForwardVector * 3, (ExplosionType)26, 30f, 1.5f);*/

            if (water)
                Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_ojdg4_water_exp", position.X, position.Y, position.Z, 0.0, 0.0, 0.0, 3.0, 0, 0, 0);

            else Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_fbi_exp_building", position.X, position.Y, position.Z, 0.0, 0.0, 0.0, 1f, 0, 0, 0);
        }
    }
}
