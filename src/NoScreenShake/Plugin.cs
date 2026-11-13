using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;


namespace NoScreenShake;

[BepInPlugin(Id, Name, Version)]
public sealed class Plugin : BaseUnityPlugin
{
    private const string Id = "Rombusz.NoScreenShake";
    private const string Name = "NoScreenShake";
    private const string Version = "1.1.1";

    internal static ConfigEntry<bool> DisableShake = null!;

    private Harmony? _harmony;

    private void Awake()
    {
        DisableShake = Config.Bind(
            "General",
            "Disable Screen Shake",
            true,
            "Disable camera shake effects."
            );

        _harmony = new Harmony(Id);
        _harmony.PatchAll();
        Logger.LogMessage($"Plugin {Name} {Version} is loaded!");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}

[HarmonyPatch(typeof(GamefeelHandler), nameof(GamefeelHandler.GetRotation))]
internal static class GameplayShakePatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Vector3 __result)
    {
        if (!Plugin.DisableShake.Value)
            return true;

        __result = Vector3.zero;
        return false;
    }
}
