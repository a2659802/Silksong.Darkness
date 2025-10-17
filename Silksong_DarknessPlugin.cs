using BepInEx;
using HarmonyLib;
using HutongGames.PlayMaker;
using UnityEngine;
using HutongGames.PlayMaker.Actions;

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
        // 缩小光圈范围，使其更暗一些
        changeDarkAction(dark2);
        changeDarkAction(dark22);

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
    private static void changeDarkAction(FsmState dark)
    {

        // Idle Scale
        var idleScaleAction = dark.GetAction<SetVector3XYZ>(1);
        if (idleScaleAction == null)
        {
            Debug.LogError("[Darkness] Could not find SetVector3XYZ action in Dark state.");
            return;
        }
        idleScaleAction.x = 1.2f; // 原本的值是1.8f
        idleScaleAction.y = 1.6f; // 原本的值是1.8f
        idleScaleAction.z = 1.6f; // 原本的值是1.8f
        // Damage Scale
        var damageScaleAction = dark.GetAction<SetVector3XYZ>(2);
        if (damageScaleAction == null)
        {
            Debug.LogError("[Darkness] Could not find SetVector3XYZ action in Dark state.");
            return;
        }
        damageScaleAction.x = 1.1f; // 原本的值是1.6f
        damageScaleAction.y = 1.5f; // 原本的值是1.6f
        damageScaleAction.z = 1.5f; // 原本的值是1.6f
    }
    private static T?GetAction<T>(this FsmState state, int actionIndex) where T : FsmStateAction
    {
        if (actionIndex < 0 || actionIndex >= state.Actions.Length)
        {
            return null!;
        }

        return state.Actions[actionIndex] as T;
    }
}


[HarmonyPatch(typeof(GameManager), nameof(GameManager.EnterHero))]
public static class Darkness_EnterHeroPatch
{
    public static void Postfix()
    {
        // 查找所有名字包含Vignette Cutout的游戏对象并禁用它们
        var vignetteObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in vignetteObjects)
        {
            if (obj.name.Contains("Vignette Cutout"))
            {
                obj.SetActive(false);
                Debug.Log("[Darkness] Disabled Vignette Cutout object: " + obj.name);
            }
        }
    }
}