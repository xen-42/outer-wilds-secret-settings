using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace SecretSettingsMod;

[HarmonyPatch]
public class SecretSettingsMod : ModBehaviour
{
	private static SecretSettingsMod _instance;

	private const string PhysicsRate = nameof(PhysicsRate);
	private const string VSyncCount = nameof(VSyncCount);
	private const string DisableEssentialPrompts = nameof(DisableEssentialPrompts);
	private const string DisablePartygoerProxies = nameof(DisablePartygoerProxies);
	private const string EnableVisorBreathFog = nameof(EnableVisorBreathFog);

    private void Awake()
    {
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
	}

	public override void Configure(IModConfig config)
	{
		base.Configure(config);
		UpdateSettings();
	}

	public void UpdateSettings()
	{
		SecretSettings.s_settings[PhysicsRate] = GetSettingsValue<int>(PhysicsRate).ToString();
		SecretSettings.s_settings[DisablePartygoerProxies] = GetSettingsValue<bool>(DisablePartygoerProxies).ToString();
		SecretSettings.s_settings[EnableVisorBreathFog] = GetSettingsValue<bool>(EnableVisorBreathFog).ToString();

		// VSyncCount should default to the base game setting if of on/off if default
		var vSync = GetSettingsValue<string>(VSyncCount);
		if (vSync != "default")
		{
			SecretSettings.s_settings[VSyncCount] = vSync;
		}

		// Has to be 1 or 0 for some reason, but for simplicity I made it a bool in the options
		SecretSettings.s_settings[DisableEssentialPrompts] = (GetSettingsValue<bool>(DisableEssentialPrompts) ? 1 : 0).ToString(); 

		if (SceneManager.GetActiveScene().name != "TitleScreen")
		{
			ModHelper.Console.WriteLine("Warning: Secret setting changes might not take effect until you restart the current scene.", MessageType.Warning);
		}
	}

	private T GetSettingsValue<T>(string key) => ModHelper.Config.GetSettingsValue<T>(key);

	[HarmonyPostfix]
	[HarmonyPatch(typeof(SecretSettings), nameof(SecretSettings.TryLoadSettingsFromFile))]
	private static void SecretSettings_TryLoadSettingsFromFile() => _instance.UpdateSettings();
}
