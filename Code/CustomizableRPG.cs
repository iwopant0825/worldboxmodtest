using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod {
  class CustomizableRPG : MonoBehaviour {
    public static void init() { }

    public static void turnOnCRPG() {
      if (PowerButtons.GetToggleValue("crpg_toggle_dej")) {
        Traits.initCRPGTraits();
        Main.modifyBoolOption("CRPGOption", true);
        return;
      }

      Traits.deleteCRPGTraits();
      Main.modifyBoolOption("CRPGOption", false);
    }

    public static bool updateEXP(BaseSimObject pTarget, WorldTile pTile = null) {
      string talentID = getTalent(pTarget.a);
      if (string.IsNullOrEmpty(talentID)) {
        return false;
      }

      if (pTarget.a.data.custom_data_int == null) {
        pTarget.a.data.set("expGainTimer", 57);
        return false;
      }

      int curTime = 57;
      if (!pTarget.a.data.custom_data_int.TryGetValue("expGainTimer", out curTime)) {
        pTarget.a.data.set("expGainTimer", curTime);
        return false;
      }

      if (curTime - 1 < 0) {
        pTarget.a.data.set("expGainTimer", 57);
      } else {
        pTarget.a.data.set("expGainTimer", curTime - 1);
        return false;
      }

      pTarget.a.addExperience(Main.savedSettings.crpgTraits[talentID].expGain);
      return true;
    }

    public static bool updateAttackEXP(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      string talentID = getTalent(pSelf.a);
      pSelf.a.addExperience(Main.savedSettings.crpgTraits[talentID].expGainHit);
      if (pTarget.a != null && !pTarget.a.data.alive) {
        pSelf.a.addExperience(Main.savedSettings.crpgTraits[talentID].expGainKill);
      }

      return true;
    }

    public static bool updateHurtEXP(BaseSimObject pSelf, BaseSimObject pAttackedBy = null, WorldTile pTile = null) {
      string talentID = getTalent(pSelf.a);
      pSelf.a.addExperience(Main.savedSettings.crpgTraits[talentID].expGainHurt);
      if (pAttackedBy != null && pAttackedBy.a != null && Main.savedSettings.inputOptions["CRPG-LevelDMGSuppressionOption"].active) {
        int attackerLevel = Mathf.FloorToInt(pAttackedBy.a.data.level / 10);
        int selfLevel = Mathf.FloorToInt(pSelf.a.data.level / 10);
        if (attackerLevel < selfLevel &&
            ((Main.savedSettings.inputOptions["CRPG-InitialStartForLevelDMGSuppresionOption"].active && int.Parse(Main.savedSettings.inputOptions["CRPG-InitialStartForLevelDMGSuppresionOption"].value) <= pSelf.a.data.level) ||
             !Main.savedSettings.inputOptions["CRPG-InitialStartForLevelDMGSuppresionOption"].active)) {
          float pDamage = pAttackedBy.a.stats[S.damage];
          float num = 1f - pSelf.a.stats[S.armor] / 100f;
          float trueDMG = pDamage * num;
          float restoreValue = (trueDMG * (float.Parse(Main.savedSettings.inputOptions["CRPG-LevelDMGSuppressionOption"].value, CultureInfo.InvariantCulture) / 100f));
          if (trueDMG > 1) {
            pSelf.a.restoreHealth((int)restoreValue);
          }
        }
      }

      return true;
    }

    public static string getTalent(Actor pActor) {
      foreach (string key in Main.savedSettings.crpgTraits.Keys) {
        if (pActor.hasTrait(key)) {
          return key;
        }
      }

      return "";
    }

    public static string getLevelTitle(int level) {
      string title = "";
      int index = level.ToString().Length;
      foreach (char c in level.ToString()) {
        if (Main.savedSettings.inputOptions["CRPG-VisibleDigitTitlesOption"].active &&
            int.Parse(Main.savedSettings.inputOptions["CRPG-VisibleDigitTitlesOption"].value) == level.ToString().Length - index) {
          return title;
        }

        if (!Main.savedSettings.CRPGTitles.ContainsKey(index.ToString())) {
          index--;
          continue;
        }

        title += " " + Main.savedSettings.CRPGTitles[index.ToString()][c.ToString()];
        index--;
      }

      return title;
    }

    public static bool fireAttributeAction(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      string attributeSpell = "FireAttribute";
      foreach (string aTrait in Main.savedSettings.crpgAttributes.Keys) {
        if (!pSelf.a.hasTrait(aTrait) || !aTrait.Contains(attributeSpell)) {
          continue;
        }

        attributeSpell += "Spell" + aTrait[aTrait.Length - 1];
      }

      tryToCastAttribute(pSelf.a, ref pTarget, attributeSpell);
      return true;
    }

    public static bool waterAttributeAction(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      string attributeSpell = "WaterAttribute";
      foreach (string aTrait in Main.savedSettings.crpgAttributes.Keys) {
        if (!pSelf.a.hasTrait(aTrait) || !aTrait.Contains(attributeSpell)) {
          continue;
        }

        attributeSpell += "Spell" + aTrait[aTrait.Length - 1];
      }

      tryToCastAttribute(pSelf.a, ref pTarget, attributeSpell);
      return true;
    }

    public static bool earthAttributeAction(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      string attributeSpell = "EarthAttribute";
      foreach (string aTrait in Main.savedSettings.crpgAttributes.Keys) {
        if (!pSelf.a.hasTrait(aTrait) || !aTrait.Contains(attributeSpell)) {
          continue;
        }

        attributeSpell += "Spell" + aTrait[aTrait.Length - 1];
      }

      tryToCastAttribute(pSelf.a, ref pTarget, attributeSpell);
      return true;
    }

    public static bool lightningAttributeAction(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      string attributeSpell = "LightningAttribute";
      foreach (string aTrait in Main.savedSettings.crpgAttributes.Keys) {
        if (!pSelf.a.hasTrait(aTrait) || !aTrait.Contains(attributeSpell)) {
          continue;
        }

        attributeSpell += "Spell" + aTrait[aTrait.Length - 1];
      }

      tryToCastAttribute(pSelf.a, ref pTarget, attributeSpell);
      return true;
    }

    private static bool tryToCastAttribute(Actor pSelf, ref BaseSimObject pTarget, string spellID) {
      Spell spell = AssetManager.spells.get(spellID);
      if (!Toolbox.randomChance(spell.chance)) {
        return false;
      }

      if (spell.castTarget == CastTarget.Himself) {
        pTarget = pSelf;
      }

      if (spell.castEntity == CastEntity.BuildingsOnly) {
        if (pTarget.isActor()) {
          return false;
        }
      } else if (spell.castEntity == CastEntity.UnitsOnly && pTarget.isBuilding()) {
        return false;
      }

      float num = (float)pSelf.data.health / (float)pSelf.getMaxHealth();
      if (spell.healthPercent != 0f && spell.healthPercent <= num) {
        return false;
      }

      if (spell.min_distance != 0f && Toolbox.DistTile(pSelf.currentTile, pTarget.currentTile) < spell.min_distance) {
        return false;
      }

      bool flag = false;
      if (spell.action != null) {
        flag = spell.action.RunAnyTrue(pSelf, pTarget, pTarget.currentTile);
      }

      // if (flag)
      // {
      //     pSelf.doCastAnimation();
      // }
      return flag;
    }

    public static bool updateAttribute(BaseSimObject pTarget, WorldTile pTile = null) {
      int evolNum = 0;
      pTarget.a.data.get("evolNum", out evolNum, -1);
      if (evolNum == -1) {
        pTarget.a.data.set("evolNum", 0);
      } else if (evolNum >= int.Parse(Main.savedSettings.inputOptions["CRPG-AttributeEvolutionsLimitOption"].value) && Main.savedSettings.inputOptions["CRPG-AttributeEvolutionsLimitOption"].active) {
        return false;
      }

      string pAttribute = getAttribute(pTarget.a);
      int attributeNum = int.Parse(pAttribute[pAttribute.Length - 1].ToString());
      if (attributeNum >= 6) {
        return true;
      }

      attributeNum++;
      string newAttribute = $"{pAttribute.Remove(pAttribute.Length - 1)}{attributeNum}";
      bool flag = true;
      if (Main.savedSettings.crpgAttributes[newAttribute].damageReq > pTarget.a.stats[S.damage]) {
        flag = false;
      }

      if (Main.savedSettings.crpgAttributes[newAttribute].ageReq > pTarget.a.getAge()) {
        flag = false;
      }

      if (Main.savedSettings.crpgAttributes[newAttribute].levelReq > pTarget.a.data.level) {
        flag = false;
      }

      if (Main.savedSettings.crpgAttributes[newAttribute].killsReq > pTarget.a.data.kills) {
        flag = false;
      }

      if (flag) {
        evolNum++;
        pTarget.a.data.set("evolNum", evolNum);
        pTarget.a.addTrait(newAttribute, true);
        pTarget.a.removeTrait(pAttribute);
      }

      return true;
    }

    public static string getAttribute(Actor pActor) {
      foreach (string key in Main.savedSettings.crpgAttributes.Keys) {
        if (pActor.hasTrait(key)) {
          return key;
        }
      }

      return "";
    }
  }
}