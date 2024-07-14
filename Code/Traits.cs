using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod {
  class Traits : MonoBehaviour {
    public static void init() {
      return;
    }

    public static void initCRPGTraits() {
      Localization.AddOrSet("trait_group_talentRanks", "Talent Ranks");
      AssetManager.trait_groups.add(new ActorTraitGroupAsset {
        id = "talentRanks",
        name = "trait_group_talentRanks",
        color = Toolbox.makeColor("#FF8F44", -1f)
      });
      foreach (KeyValuePair<string, CRPGTrait> kv in Main.savedSettings.crpgTraits) {
        createCRPGTrait(kv.Key);
      }

      Localization.AddOrSet("trait_group_attributes", "Attributes");
      AssetManager.trait_groups.add(new ActorTraitGroupAsset {
        id = "attributeGroup",
        name = "trait_group_attributes",
        color = Toolbox.makeColor("#FF8F44", -1f)
      });
      foreach (KeyValuePair<string, AttributeTrait> kv in Main.savedSettings.crpgAttributes) {
        createAttributeTrait(kv.Key, kv.Value);
      }
    }

    private static void createCRPGTrait(string id) {
      Localization.AddOrSet($"trait_{id}", id);
      Localization.AddOrSet($"trait_{id}_info", id);
      string oppositeTraits = "";
      foreach (string key in Main.savedSettings.crpgTraits.Keys) {
        if (key == id) {
          continue;
        }

        oppositeTraits += "," + key;
      }

      ActorTrait crpgTrait = new ActorTrait {
        id = id,
        path_icon = $"ui/Icons/icon{id}_t",
        birth = Main.savedSettings.crpgTraits[id].birthRate,
        inherit = -1f,
        group_id = "talentRanks",
        type = TraitType.Positive,
        needs_to_be_explored = false,
        can_be_given = true,
        opposite = oppositeTraits,
        special_effect_interval = 1f,
        action_special_effect = new WorldAction(CustomizableRPG.updateEXP),
        action_attack_target = new AttackAction(CustomizableRPG.updateAttackEXP),
        action_get_hit = new GetHitAction(CustomizableRPG.updateHurtEXP)
      };
      AssetManager.traits.add(crpgTrait);
    }

    public static void deleteCRPGTraits() {
      foreach (KeyValuePair<string, CRPGTrait> kv in Main.savedSettings.crpgTraits) {
        if (!AssetManager.traits.dict.ContainsKey(kv.Key)) {
          continue;
        }

        AssetManager.traits.get(kv.Key).birth = 0f;
      }
    }

    private static void createAttributeTrait(string id, AttributeTrait aTrait) {
      Localization.AddOrSet($"trait_{id}", id);
      Localization.AddOrSet($"trait_{id}_info", id);
      AttackAction attributeAction = null;
      string generalID = id.Remove(id.Length - 1);
      switch (generalID) {
        case "FireAttribute":
          attributeAction = new AttackAction(CustomizableRPG.fireAttributeAction);
          AssetManager.status.get("burning").opposite_traits.Add(id);
          break;
        case "WaterAttribute":
          attributeAction = new AttackAction(CustomizableRPG.waterAttributeAction);
          AssetManager.status.get("slowness").opposite_traits.Add(id);
          break;
        case "EarthAttribute":
          attributeAction = new AttackAction(CustomizableRPG.earthAttributeAction);
          break;
        case "LightningAttribute":
          attributeAction = new AttackAction(CustomizableRPG.lightningAttributeAction);
          break;
      }

      string iconID = id;
      if (int.Parse(id[id.Length - 1].ToString()) > 6) {
        iconID = generalID;
      }

      AssetManager.traits.add(new ActorTrait {
        id = id,
        path_icon = $"ui/Icons/icon{iconID}_t",
        birth = -1f,
        inherit = -1f,
        group_id = "attributeGroup",
        type = TraitType.Positive,
        needs_to_be_explored = false,
        can_be_given = true,
        special_effect_interval = 10f,
        action_attack_target = attributeAction,
        action_special_effect = new WorldAction(CustomizableRPG.updateAttribute)
      });
    }
  }
}