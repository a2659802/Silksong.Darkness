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
        FsmState dark22 = null!;
        foreach (var state in __instance.FsmStates)
        {
            if (state.Name == "Dark 2")
            {
                dark2 = state;
            }
            else if (state.Name == "Dark 2 2")
            {
                dark22 = state;
            }
        }

        if (dark2 == null || dark22 == null)
        {
            Debug.LogError("[Darkness] Could not find 'Dark 2' and 'Dark 2 2' states.");
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
                Debug.Log("[Darkness] Transition fixed and vignette forced Dark 2.");
            }
            else if (state.Name == "Scene Reset")
            {
                foreach (var trans in state.Transitions)
                {
                    trans.ToState = "Dark 2 2";
                    trans.ToFsmState = dark22;
                }
                Debug.Log("[Darkness] Transition fixed and vignette forced Dark 2 2.");
            }
        }        
    }
}