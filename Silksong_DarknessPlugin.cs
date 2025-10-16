using BepInEx;
using HarmonyLib;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Silksong.Darkness;

[BepInAutoPlugin(id: "io.github.silksong_darkness")]
public partial class Silksong_DarknessPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo($"[Darkness] Plugin {Name} ({Id}) has loaded!");
        new Harmony(Id).PatchAll();
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.EnterHero))]
public static class Darkness_EnterHeroPatch
{
    public static void Postfix()
    {
        var hc = HeroController.instance;
        if (hc == null || hc.vignetteFSM == null)
            return;

        hc.vignetteFSM.SetState("Dark 2");
        hc.vignette.enabled = true;

        Debug.Log("[Darkness_EnterHeroPatch] Forced vignette to Dark 2.");

        FsmState? dark2 = null;

        foreach (var state in hc.vignetteFSM.FsmStates)
        {
            if (state.Name == "Dark 2")
            {
                dark2 = state;
                break;
            }
        }
        if (dark2 == null)
        {
            Debug.LogError("[Darkness_EnterHeroPatch] Could not find 'Dark 2' state in vignette FSM.");
            return;
        }

        foreach (var state in hc.vignetteFSM.FsmStates)
        {
            if (state.Name == "Dark Lev Check")
            {
                foreach (var trans in state.Transitions)
                {
                    trans.ToState = "Dark 2";
                    trans.ToFsmState = dark2;
                }
            }
        }
        
         Debug.Log("[Darkness_EnterHeroPatch] change transition rule");
    }
}

