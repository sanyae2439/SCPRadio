using System.Collections.Generic;
using Smod2;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;
using MEC;
using Harmony;
using UnityEngine;
using Dissonance.Audio.Playback;

namespace SCPRadio
{
    [PluginDetails(
    author = "sanyae2439",
    name = "SCPRadio",
    description = "SCPs talk to humans through the radio",
    id = "sanyae2439.SCPRadio",
    version = "1.0",
    SmodMajor = 3,
    SmodMinor = 5,
    SmodRevision = 0
    )]
    public class SCPRadio : Plugin
    {
        internal static SCPRadio instance;
        public override void OnDisable()
        {
            this.Info("SCPRadio Disabled...");
        }

        public override void OnEnable()
        {
            instance = this;
            this.Info("SCPRadio Enabled!");
        }

        public override void Register()
        {
            HarmonyInstance.Create(Details.id).PatchAll();
            this.AddEventHandlers(new EventHandler(this));
        }
    }

    public class EventHandler : IEventHandlerCallCommand
    {
        private readonly SCPRadio plugin;
        public EventHandler(SCPRadio instance)
        {
            plugin = instance;
        }

        public void OnCallCommand(PlayerCallCommandEvent ev)
        {
            plugin.Debug($"[OnCallCommand] {ev.Player.Name} -> {ev.Command}");

            if(ev.ReturnMessage == "Command not found.")
            {
                if(ev.Command.ToUpper().StartsWith("SCPRADIO"))
                {
                    plugin.Debug("command called");

                    Radio radio = (ev.Player.GetGameObject() as GameObject).GetComponent<Radio>();

                    radio.NetworkisTransmitting = !radio.isTransmitting;

                    ev.ReturnMessage = $"OK. (changed to {radio.isTransmitting})";
                }
            }
        }
    }

    [HarmonyPatch(typeof(Radio), "CmdSyncTransmitionStatus")]
    class isTransmittingPatch
    {
        static bool Prefix(Radio __instance)
        {
            SCPRadio.instance.Debug($"[CmdSyncTransmitionStatus] isSCP:{__instance.voiceInfo.isSCP} isAliveHuman:{__instance.voiceInfo.isAliveHuman} isDead:{__instance.voiceInfo.IsDead()}");
            if(__instance.voiceInfo.isSCP)
            {
                return false;
            }
            return true;
        }
    }
}
