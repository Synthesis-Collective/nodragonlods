using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Octodiff.Diagnostics;

namespace NoDragLODs
{
    public static class MyExtensions
    {
        public static bool ContainsInsensitive(this string str, string rhs)
        {
            return str.Contains(rhs, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class Program
    {
        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                new UserPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "NoDragonLODs.esp",
                        TargetRelease = GameRelease.SkyrimSE
                    }
                }
                );
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach(var npc in state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>())
            {
                if (npc.PlayerSkills?.FarAwayModelDistance == 0) continue;

                if (!npc.Race.TryResolve(state.LinkCache, out var race)) continue;

                if (race.SkeletalModel?.Male?.File.ContainsInsensitive("actors\\dragon\\") == false) continue;

                var modifiedNpc = state.PatchMod.Npcs.GetOrAddAsOverride(npc);

                if (modifiedNpc.PlayerSkills == null)
                {
                    modifiedNpc.PlayerSkills = new PlayerSkills();
                }
                modifiedNpc.PlayerSkills.FarAwayModelDistance = 0;
            }
        }
    }
}
