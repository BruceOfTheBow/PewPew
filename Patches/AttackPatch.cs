using HarmonyLib;

using UnityEngine;

using static PewPew.PewPew;
using static PewPew.PluginConfig;

namespace PewPew.Patches {
  [HarmonyPatch(typeof(Attack))]
  public class AttackPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Attack.FireProjectileBurst))]
    public static void FireProjectileBurstPrefix(ref Attack __instance) {
      if (!IsModEnabled.Value) {
        return;
      }

      if (!__instance.m_character.gameObject.TryGetComponent(out AudioSource audioSource)) {
        audioSource = __instance.m_character.gameObject.AddComponent<AudioSource>();
      }

      audioSource.PlayOneShot(PewPewAudioClip);
    }
  }
}
