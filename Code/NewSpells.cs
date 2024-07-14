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
  class NewSpells : MonoBehaviour {
    public static void init() {
      loadSpells();
    }

    private static void loadSpells() {
      createAttributeSpell();
    }

    private static void createAttributeSpell() {
      foreach (KeyValuePair<string, AttributeTrait> kv in Main.savedSettings.crpgAttributes) {
        string attributeNum = kv.Key[kv.Key.Length - 1].ToString();
        AssetManager.spells.add(new Spell {
          id = $"{kv.Key.Remove(kv.Key.Length - 1)}Spell{attributeNum}",
          chance = kv.Value.attackChance,
          castTarget = CastTarget.Enemy,
          healthPercent = 0f,
          action = (AttackAction)Delegate.CreateDelegate(typeof(AttackAction), typeof(NewSpells).GetMethod(kv.Value.attackAction)),
          min_distance = 0f
        });
      }
    }

    public static bool fireAction1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "FireAttributeProjectile1", pTile);
      return true;
    }

    public static bool fireAction2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "FireAttributeProjectile2", pTile);
      return true;
    }

    public static bool fireAction3(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "FireAttributeProjectile3", pTile);
      return true;
    }

    public static bool fireAction4(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "FireAttributeProjectile4", pTile);
      return true;
    }

    public static bool fireAction5(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "FireAttributeProjectile5", pTile);
      return true;
    }

    public static bool fireAction6(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "FireAttributeProjectile6", pTile);
      return true;
    }

    public static bool waterAction1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "WaterAttributeProjectile1", pTile);
      return true;
    }

    public static bool waterAction2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "WaterAttributeProjectile2", pTile);
      return true;
    }

    public static bool waterAction3(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "WaterAttributeProjectile3", pTile);
      return true;
    }

    public static bool waterAction4(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "WaterAttributeProjectile4", pTile);
      return true;
    }

    public static bool waterAction5(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "WaterAttributeProjectile5", pTile);
      return true;
    }

    public static bool waterAction6(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "WaterAttributeProjectile6", pTile);
      return true;
    }

    public static bool earthAction1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "EarthAttributeProjectile1", pTile);
      return true;
    }

    public static bool earthAction2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "EarthAttributeProjectile2", pTile);
      return true;
    }

    public static bool earthAction3(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "EarthAttributeProjectile3", pTile);
      return true;
    }

    public static bool earthAction4(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "EarthAttributeProjectile4", pTile);
      return true;
    }

    public static bool earthAction5(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "EarthAttributeProjectile5", pTile);
      return true;
    }

    public static bool earthAction6(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "EarthAttributeProjectile6", pTile);
      return true;
    }

    public static bool lightningAction1(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "LightningAttributeProjectile1", pTile);
      return true;
    }

    public static bool lightningAction2(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "LightningAttributeProjectile2", pTile);
      return true;
    }

    public static bool lightningAction3(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "LightningAttributeProjectile3", pTile);
      return true;
    }

    public static bool lightningAction4(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "LightningAttributeProjectile4", pTile);
      return true;
    }

    public static bool lightningAction5(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "LightningAttributeProjectile5", pTile);
      return true;
    }

    public static bool lightningAction6(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null) {
      doProjectileAction(pSelf, pTarget, "LightningAttributeProjectile6", pTile);
      return true;
    }


    private static void doProjectileAction(BaseSimObject pSelf, BaseSimObject pTarget, string projectileID, WorldTile pTile = null) {
      float num = pSelf.a.stats[S.size];
      float num2 = pTarget.stats[S.size];
      Vector3 vector = new Vector3(pTarget.currentPosition.x, pTarget.currentPosition.y);
      if (pTarget.isActor() && pTarget.a.is_moving && pTarget.isFlying()) {
        vector = Vector3.MoveTowards(vector, pTarget.a.nextStepPosition, num2 * 3f);
      }

      float num3 = Vector2.Distance(pSelf.currentPosition, pTarget.currentPosition) + pTarget.getZ();
      Vector3 newPoint = Toolbox.getNewPoint(pSelf.currentPosition.x, pSelf.currentPosition.y, vector.x, vector.y, num3 - num2, true);
      Vector3 vector2 = pSelf.currentPosition;
      vector2.y += 0.5f;
      float num4 = Toolbox.getAngle(vector2.x, vector2.y, vector.x, vector.y) * 57.29578f;

      newPoint.x = vector.x;
      newPoint.y = vector.y + 0.1f;
      newPoint.x += Toolbox.randomFloat(-(num2 + 1f), num2 + 1f);
      newPoint.y += Toolbox.randomFloat(-num2, num2);
      Vector3 newPoint2 = Toolbox.getNewPoint(pSelf.currentPosition.x, pSelf.currentPosition.y, vector.x, vector.y, num, true);
      newPoint2.y += 0.5f;
      float num6 = 0f;
      if (pTarget.isInAir()) {
        num6 = pTarget.getZ();
      }

      Projectile projectile2 = EffectsLibrary.spawnProjectile(projectileID, newPoint2, newPoint, num6);
      num4 = projectile2.getAngle();
      if (projectile2 != null) {
        projectile2.byWho = pSelf;
        projectile2.setStats(pSelf.stats);
        projectile2.targetObject = pTarget;
        projectile2.gameObject.AddComponent<isAttributeProjectile>();
      }

      if (!string.IsNullOrEmpty(pSelf.a.asset.fmod_attack)) {
        MusicBox.playSound(pSelf.a.asset.fmod_attack, (float)pSelf.currentTile.x, (float)pSelf.currentTile.y, false, false);
      }
    }
  }

  public class isAttributeProjectile : MonoBehaviour { }
}