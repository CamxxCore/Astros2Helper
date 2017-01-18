using System;
using GTA;

namespace Astros2.ScriptBase
{
    public class ScriptMain : Script
    {
        private AstrosVehicle mainVehicle;

        private readonly Ped player = Game.Player.Character;

        public ScriptMain()
        {
            PreInit();
            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (mainVehicle == null) return;

            if (e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                mainVehicle.FireMissile();
            }
        }

        private void PreInit()
        {
            Scripts.RequestPTFXAsset("scr_agencyheist");
            Scripts.RequestPTFXAsset("des_train_crash");
            mainVehicle = null;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (mainVehicle != null && !player.IsInVehicle(mainVehicle.BaseRef))
            {
 
                mainVehicle = null;
            }

            if (player.IsInVehicle() && player.CurrentVehicle.Model.Hash == AstrosVehicle.NameHash)
            {
                if (mainVehicle == null || mainVehicle.BaseRef.Handle != player.CurrentVehicle.Handle)
                {
                    mainVehicle = AstrosVehicle.FromVehicle(player.CurrentVehicle);
                }

                else
                {
                    mainVehicle.OnUpdate();
                }
            }
        }

        protected override void Dispose(bool A_0)
        {
            mainVehicle.ClearExtensions();

            base.Dispose(A_0);
        }
    }   
}
