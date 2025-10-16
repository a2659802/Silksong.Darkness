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

[HarmonyPatch(typeof(PlayMakerFSM), nameof(PlayMakerFSM.Awake))]
public static class VignetteFSM_AwakePatch
{
    public static void Postfix(PlayMakerFSM __instance)
    {
        // 只针对 vignetteFSM
        if (__instance.FsmName != "Darkness Control") return;

        // 找到 Dark 2 状态
        FsmState dark2 = null!;
        foreach (var state in __instance.FsmStates)
        {
            if (state.Name == "Dark 2")
            {
                dark2 = state;
                break;
            }
        }

        if (dark2 == null)
        {
            Debug.LogError("[Darkness] Could not find 'Dark 2' state.");
            return;
        }

        // 修改需要固定跳转的状态
        foreach (var state in __instance.FsmStates)
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

        // // 强制 FSM 当前状态为 Dark 2
        // __instance.SetState("Dark 2");
        // HeroController.instance.vignette.enabled = true;

        Debug.Log("[Darkness] Transition fixed and vignette forced Dark 2.");
    }
}