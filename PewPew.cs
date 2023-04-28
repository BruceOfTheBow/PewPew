using System;
using System.IO;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using System.Reflection;

using static PewPew.PluginConfig;

namespace PewPew {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class PewPew : BaseUnityPlugin {
    public const string PluginGuid = "bruce.comfy.valheim.pewpew";
    public const string PluginName = "PewPew";
    public const string PluginVersion = "1.0.0";

    Harmony _harmony;

    public static AudioClip PewPewAudioClip;
    private static string _pewPewAudioResource = "PewPew.Resources.PewPew.wav";

    public void Awake() {
      BindConfig(Config);

      CreateAudioClip();

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    // Supports .wav signed 16-bit uncompressed PCM files
    private static void CreateAudioClip() {
      Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_pewPewAudioResource);
      using (MemoryStream memoryStream = new MemoryStream()) {
        stream.CopyTo(memoryStream);
        byte[] data = memoryStream.GetBuffer();

        int headers = BitConverter.ToInt32(data, 16);
        UInt16 audioFormat = BitConverter.ToUInt16(data, 20);

        UInt16 channels = BitConverter.ToUInt16(data, 22);
        int sampleRate = BitConverter.ToInt32(data, 24);
        UInt16 bitDepth = BitConverter.ToUInt16(data, 34);

        int headerOffset = 16 + 4 + headers + 4;
        int dataSize = BitConverter.ToInt32(data, headerOffset);

        float[] floatData;
        floatData = ConvertByteArrayToAudioClip(data, headerOffset);

        PewPewAudioClip = AudioClip.Create("DumpsterBurning", data.Length, (int)channels, sampleRate, false);
        PewPewAudioClip.SetData(floatData, 0);
      }
    }

    private static float[] ConvertByteArrayToAudioClip(byte[] data, int headerOffset) {
      int wavSize = BitConverter.ToInt32(data, headerOffset);
      headerOffset += sizeof(int);
      int convertedSize = wavSize / sizeof(Int16);

      float[] floatData = new float[convertedSize];

      int bytesIn = 0;
      int i = 0;
      while (i < convertedSize) {
        bytesIn = i * sizeof(Int16) + headerOffset;
        floatData[i] = (float)BitConverter.ToInt16(data, bytesIn) / Int16.MaxValue;
        ++i;
      }

      return floatData;
    }
  }
}