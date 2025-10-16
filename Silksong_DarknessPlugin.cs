using BepInEx;

namespace Silksong.Darkness;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.silksong_darkness")]
public partial class Silksong_DarknessPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Put your initialization logic here
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
}