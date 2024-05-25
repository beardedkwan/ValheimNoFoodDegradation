using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValheimNoFoodDegradation
{
    public class PluginInfo
    {
        public const string Name = "ValheimNoFoodDegradation";
        public const string Guid = "beardedkwan" + Name;
        public const string Version = "1.0.0";
    }

    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    [BepInProcess("valheim.exe")]
    public class ValheimNoFoodDegradation : BaseUnityPlugin
    {
        // FOOD DEGREDATION
        [HarmonyPatch]
        public static class FoodDeg_Patch
        {
            private static FieldInfo field_Food_m_health = AccessTools.Field(typeof(Player.Food), "m_health");
            private static FieldInfo field_Food_m_stamina = AccessTools.Field(typeof(Player.Food), "m_stamina");
            private static FieldInfo field_Food_m_eitr = AccessTools.Field(typeof(Player.Food), "m_eitr");

            private static FieldInfo field_Food_m_item = AccessTools.Field(typeof(Player.Food), "m_item");
            private static FieldInfo field_ItemData_m_shared = AccessTools.Field(typeof(ItemDrop.ItemData), "m_shared");
            private static FieldInfo field_SharedData_m_food = AccessTools.Field(typeof(ItemDrop.ItemData.SharedData), "m_food");
            private static FieldInfo field_SharedData_m_foodStamina = AccessTools.Field(typeof(ItemDrop.ItemData.SharedData), "m_foodStamina");
            private static FieldInfo field_SharedData_m_foodEitr = AccessTools.Field(typeof(ItemDrop.ItemData.SharedData), "m_foodEitr");

            [HarmonyTranspiler]
            [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
            public static IEnumerable<CodeInstruction> Player_GetTotalFoodValue(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> il = instructions.ToList();

                for (int i = 0; i < il.Count; ++i)
                {
                    if (il[i].LoadsField(field_Food_m_health))
                    {
                        il[i].operand = field_Food_m_item;
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, field_ItemData_m_shared));
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, field_SharedData_m_food));
                    }
                    else if (il[i].LoadsField(field_Food_m_stamina))
                    {
                        il[i].operand = field_Food_m_item;
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, field_ItemData_m_shared));
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, field_SharedData_m_foodStamina));
                    }
                    else if (il[i].LoadsField(field_Food_m_eitr))
                    {
                        il[i].operand = field_Food_m_item;
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, field_ItemData_m_shared));
                        il.Insert(++i, new CodeInstruction(OpCodes.Ldfld, field_SharedData_m_foodEitr));
                    }
                }

                return il.AsEnumerable();
            }
        }
    }
}
