using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Threading.Tasks;

namespace NoDragLODs
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "NoDragonLODs.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach(var npc in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                if (npc.PlayerSkills?.FarAwayModelDistance.EqualsWithin(0) ?? true) continue;

                if (!npc.Race.TryResolve(state.LinkCache, out var race)) continue;

                if (race.SkeletalModel?.Male?.File.ContainsInsensitive("actors\\dragon\\") == false) continue;

                var modifiedNpc = state.PatchMod.Npcs.GetOrAddAsOverride(npc);

                modifiedNpc.PlayerSkills ??= new PlayerSkills();
                modifiedNpc.PlayerSkills.FarAwayModelDistance = 0;
            }
        }
    }
}
