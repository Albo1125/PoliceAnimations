using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceAnimations
{
    public class PoliceAnimations : BaseScript
    {
        private const string drunkClipset = "MOVE_M@DRUNK@VERYDRUNK";
        private const string shieldmodel = "PROP_RIOT_SHIELD";
        public enum AnimationStates { None, Radio, Pointing};
        private AnimationStates currentState = AnimationStates.None;
        private Prop shield;
        private bool isDrunk = false;

        public PoliceAnimations()
        {
            EventHandlers["PoliceAnimations:Shield"] += new Action<dynamic>((dynamic) => {
                ToggleShield();
            });

            EventHandlers["PoliceAnimations:Drunk"] += new Action<dynamic>((dynamic) => {
                ToggleDrunk();
            });
            Main();
        }

        private async Task ToggleShield()
        {
            if (shield != null && shield.Exists())
            {
                shield.Delete();              
            }
            else
            {
                shield = await World.CreateProp(shieldmodel, LocalPlayer.Character.Position, true, false);
                shield.AttachTo(LocalPlayer.Character.Bones[Bone.SKEL_L_UpperArm], new Vector3(0.2f, 0.3f, 0f), rotation:new Vector3(150f, -60f, 0));
            }

        }

        private async Task ToggleDrunk()
        {
            if (!isDrunk)
            {
                CitizenFX.Core.Native.Function.Call(CitizenFX.Core.Native.Hash.REQUEST_ANIM_SET, drunkClipset);
                while (!CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash.HAS_ANIM_SET_LOADED, drunkClipset))
                {
                    await Delay(1);
                }
                CitizenFX.Core.Native.Function.Call(CitizenFX.Core.Native.Hash.SET_PED_MOVEMENT_CLIPSET, LocalPlayer.Character, drunkClipset, 1);
                isDrunk = true;
            }
            else
            {
                CitizenFX.Core.Native.Function.Call(CitizenFX.Core.Native.Hash.RESET_PED_MOVEMENT_CLIPSET, LocalPlayer.Character, 0);
                isDrunk = false;
            }

        }

        private bool LastInputWasController()
        {
            return !CitizenFX.Core.Native.Function.Call<bool>(CitizenFX.Core.Native.Hash._IS_INPUT_DISABLED, 2);
        }

        private async void Main()
        {

            while (true)
            {
                await Delay(0);
                if (Game.IsControlJustPressed(0, (Control)57) && !LastInputWasController()) //F10
                {
                    if (currentState == AnimationStates.Radio)
                    {
                        Game.PlayerPed.Task.ClearSecondary();
                        currentState = AnimationStates.None;
                    }
                    else
                    {
                        Game.PlayerPed.Task.PlayAnimation("random@arrests", "generic_radio_chatter", 1.5f, -1, (AnimationFlags)50);
                        currentState = AnimationStates.Radio;
                    }
                }
            }
        }
    }
}
