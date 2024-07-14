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

namespace CollectionMod
{
    class NewProjectiles : MonoBehaviour
    {
        private static List<BaseSimObject> enemyObjectsList = new List<BaseSimObject>();

        public static void init()
        {
            loadProjectiles();
        }

        private static void loadProjectiles()
        {
            AssetManager.projectiles.add(new ProjectileAsset
            {
                id = "FireAttributeProjectile1",
                speed = 10f,
                texture = "fireAttributeSpell",
                trailEffect_enabled = false,
                texture_shadow = "shadow_ball",
                terraformOption = "FireAttributeTerraform1",
                endEffect = string.Empty,
                terraformRange = 1,
                draw_light_area = true,
                draw_light_size = 0.1f,
                parabolic = true,
                sound_launch = "event:/SFX/WEAPONS/WeaponFireballStart",
                // sound_impact = "event:/SFX/WEAPONS/WeaponFireballLand",
                startScale = 0.1f,
                targetScale = 0.1f,
                impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
                {
                    return fireAttributeImpact(pSelf, pTarget, pTile, 0.15f, 0, 0, 2);
                })
            });
            string fTerraform = "FireAttributeTerraform";
            ProjectileAsset f2 = AssetManager.projectiles.clone("FireAttributeProjectile2", "FireAttributeProjectile1");
            f2.texture = "fireball";
            f2.sound_impact = "event:/SFX/WEAPONS/WeaponFireballLand";
            f2.startScale = 0.06f;
            f2.targetScale = 0.06f;
            f2.terraformRange = 3;
            f2.terraformOption = $"{fTerraform}2";
            f2.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return fireAttributeImpact(pSelf, pTarget, pTile, 0.15f, 1, 0, 4);
            });
            ProjectileAsset f3 = AssetManager.projectiles.clone("FireAttributeProjectile3", "FireAttributeProjectile2");
            f3.terraformRange = 6;
            f3.terraformOption = $"{fTerraform}3";
            f3.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return fireAttributeImpact(pSelf, pTarget, pTile, 0.2f, 3, 3, 7);
            });
            ProjectileAsset f4 = AssetManager.projectiles.clone("FireAttributeProjectile4", "FireAttributeProjectile2");
            f4.startScale = 0.09f;
            f4.targetScale = 0.09f;
            f4.terraformRange = 12;
            f4.terraformOption = $"{fTerraform}4";
            f4.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return fireAttributeImpact(pSelf, pTarget, pTile, 0.4f, 4, 10, 13);
            });
            ProjectileAsset f5 = AssetManager.projectiles.clone("FireAttributeProjectile5", "FireAttributeProjectile4");
            f5.terraformRange = 20;
            f5.terraformOption = $"{fTerraform}5";
            f5.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return fireAttributeImpact(pSelf, pTarget, pTile, 0.8f, 5, 16, 21);
            });
            ProjectileAsset f6 = AssetManager.projectiles.clone("FireAttributeProjectile6", "FireAttributeProjectile2");
            f6.startScale = 0.2f;
            f6.targetScale = 0.2f;
            f6.terraformRange = 30;
            f6.terraformOption = $"{fTerraform}6";
            f6.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return fireAttributeImpact(pSelf, pTarget, pTile, 1f, 6, 20, 31);
            });

            AssetManager.projectiles.add(new ProjectileAsset
            {
                id = "EarthAttributeProjectile1",
                speed = 10f,
                texture = "earthAttributeSpell",
                trailEffect_enabled = false,
                texture_shadow = "shadow_ball",
                terraformOption = "EarthAttributeTerraform1",
                endEffect = string.Empty,
                terraformRange = 1,
                draw_light_area = true,
                draw_light_size = 0.1f,
                parabolic = true,
                sound_launch = "event:/SFX/WEAPONS/WeaponFireballStart",
                // sound_impact = "event:/SFX/WEAPONS/WeaponFireballLand",
                impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
                {
                    return earthAttributeImpact(pSelf, pTarget, pTile, 0.08f, 2, false);
                }),
                startScale = 0.1f,
                targetScale = 0.1f
            });
            string eTerraform = "EarthAttributeTerraform";
            ProjectileAsset e2 = AssetManager.projectiles.clone("EarthAttributeProjectile2", "EarthAttributeProjectile1");
            e2.terraformRange = 3;
            e2.terraformOption = $"{eTerraform}2";
            e2.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return earthAttributeImpact(pSelf, pTarget, pTile, 0.09f, 4);
            });
            ProjectileAsset e3 = AssetManager.projectiles.clone("EarthAttributeProjectile3", "EarthAttributeProjectile1");
            e3.terraformRange = 6;
            e3.terraformOption = $"{eTerraform}3";
            e3.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return earthAttributeImpact(pSelf, pTarget, pTile, 0.3f, 7);
            });
            ProjectileAsset e4 = AssetManager.projectiles.clone("EarthAttributeProjectile4", "EarthAttributeProjectile1");
            e4.terraformRange = 12;
            e4.terraformOption = $"{eTerraform}4";
            e4.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return earthAttributeImpact(pSelf, pTarget, pTile, 0.6f, 13);
            });
            ProjectileAsset e5 = AssetManager.projectiles.clone("EarthAttributeProjectile5", "EarthAttributeProjectile1");
            e5.terraformRange = 20;
            e5.terraformOption = $"{eTerraform}5";
            e5.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return earthAttributeImpact(pSelf, pTarget, pTile, 1f, 21);
            });
            ProjectileAsset e6 = AssetManager.projectiles.clone("EarthAttributeProjectile6", "EarthAttributeProjectile1");
            e6.terraformRange = 30;
            e6.terraformOption = $"{eTerraform}6";
            e6.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return earthAttributeImpact(pSelf, pTarget, pTile, 1.8f, 31);
            });

            AssetManager.projectiles.add(new ProjectileAsset
            {
                id = "LightningAttributeProjectile1",
                speed = 10f,
                texture = "lightningAttributeSpell",
                trailEffect_enabled = false,
                texture_shadow = "shadow_ball",
                terraformOption = "LightningAttributeTerraform1",
                endEffect = string.Empty,
                terraformRange = 1,
                draw_light_area = true,
                draw_light_size = 0.1f,
                parabolic = true,
                sound_launch = "event:/SFX/WEAPONS/WeaponFireballStart",
                // sound_impact = "event:/SFX/WEAPONS/WeaponFireballLand",
                startScale = 0.1f,
                targetScale = 0.1f,
                impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
                {
                    return lightningAttributeImpact(pSelf, pTarget, pTile, 0.05f, 1, 0, 2);
                })
            });
            string lTerraform = "LightningAttributeTerraform";
            ProjectileAsset l2 = AssetManager.projectiles.clone("LightningAttributeProjectile2", "LightningAttributeProjectile1");
            l2.terraformRange = 3;
            l2.terraformOption = $"{lTerraform}2";
            l2.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return lightningAttributeImpact(pSelf, pTarget, pTile, 0.08f, 1, 0, 4);
            });
            ProjectileAsset l3 = AssetManager.projectiles.clone("LightningAttributeProjectile3", "LightningAttributeProjectile1");
            l3.terraformRange = 6;
            l3.terraformOption = $"{lTerraform}3";
            l3.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return lightningAttributeImpact(pSelf, pTarget, pTile, 0.1f, 3, 6, 7);
            });
            ProjectileAsset l4 = AssetManager.projectiles.clone("LightningAttributeProjectile4", "LightningAttributeProjectile1");
            l4.terraformRange = 12;
            l4.terraformOption = $"{lTerraform}4";
            l4.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return lightningAttributeImpact(pSelf, pTarget, pTile, 0.15f, 3, 8, 13);
            });
            ProjectileAsset l5 = AssetManager.projectiles.clone("LightningAttributeProjectile5", "LightningAttributeProjectile1");
            l5.terraformRange = 20;
            l5.terraformOption = $"{lTerraform}5";
            l5.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return lightningAttributeImpact(pSelf, pTarget, pTile, 0.2f, 4, 15, 21);
            });
            ProjectileAsset l6 = AssetManager.projectiles.clone("LightningAttributeProjectile6", "LightningAttributeProjectile1");
            l6.terraformRange = 30;
            l6.terraformOption = $"{lTerraform}6";
            l6.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return lightningAttributeImpact(pSelf, pTarget, pTile, 0.3f, 6, 20, 31);
            });

            AssetManager.projectiles.add(new ProjectileAsset
            {
                id = "WaterAttributeProjectile1",
                speed = 10f,
                texture = "waterAttributeSpell",
                trailEffect_enabled = false,
                texture_shadow = "shadow_ball",
                terraformOption = "WaterAttributeTerraform1",
                endEffect = string.Empty,
                terraformRange = 1,
                draw_light_area = true,
                draw_light_size = 0.1f,
                parabolic = true,
                sound_launch = "event:/SFX/WEAPONS/WeaponFireballStart",
                // sound_impact = "event:/SFX/WEAPONS/WeaponFireballLand",
                startScale = 0.1f,
                targetScale = 0.1f,
                impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
                {
                    return waterAttributeImpact(pSelf, pTarget, pTile, 6, 0.03f, false);
                })
            });
            string wTerraform = "WaterAttributeTerraform";
            ProjectileAsset w2 = AssetManager.projectiles.clone("WaterAttributeProjectile2", "WaterAttributeProjectile1");
            w2.terraformRange = 3;
            w2.terraformOption = $"{wTerraform}2";
            w2.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return waterAttributeImpact(pSelf, pTarget, pTile, 8, 0.05f);
            });
            ProjectileAsset w3 = AssetManager.projectiles.clone("WaterAttributeProjectile3", "WaterAttributeProjectile1");
            w3.terraformRange = 6;
            w3.terraformOption = $"{wTerraform}3";
            w3.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return waterAttributeImpact(pSelf, pTarget, pTile, 11, 0.08f);
            });
            ProjectileAsset w4 = AssetManager.projectiles.clone("WaterAttributeProjectile4", "WaterAttributeProjectile1");
            w4.terraformRange = 12;
            w4.terraformOption = $"{wTerraform}4";
            w4.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return waterAttributeImpact(pSelf, pTarget, pTile, 17, 0.3f);
            });
            ProjectileAsset w5 = AssetManager.projectiles.clone("WaterAttributeProjectile5", "WaterAttributeProjectile1");
            w5.terraformRange = 20;
            w5.terraformOption = $"{wTerraform}5";
            w5.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return waterAttributeImpact(pSelf, pTarget, pTile, 25, 0.5f);
            });
            ProjectileAsset w6 = AssetManager.projectiles.clone("WaterAttributeProjectile6", "WaterAttributeProjectile1");
            w6.terraformRange = 30;
            w6.terraformOption = $"{wTerraform}6";
            w6.impact_actions = new AttackAction(delegate(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
            {
                return waterAttributeImpact(pSelf, pTarget, pTile, 40,  1f);
            });
        }

        private static bool lightningAttributeImpact(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile, float effectSize, 
        int effectCount, int posVal, int impactRange)
        {
            // BaseEffect baseEffect = EffectsLibrary.spawnAtTile("fx_lightning", pTile, effectSize);
            for (int i = 0; i < effectCount; i++)
            {
                Vector2 newPos = new Vector2(0, 0);
                switch (i)
                {
                    case 0:
                        newPos = new Vector2(posVal, 0);
                        break;
                    case 1:
                        newPos = new Vector2(-posVal, 0);
                        break;
                    case 2:
                        newPos = new Vector2(0, posVal);
                        break;
                    case 3:
                        newPos = new Vector2(0, -posVal);
                        break;
                    case 4:
                        newPos = new Vector2(-posVal/2, -posVal/2);
                        break;
                    case 5:
                        newPos = new Vector2(posVal/2, posVal/2);
                        break;
                }
                WorldTile newTile = World.world.GetTile((int)(pTile.x + newPos.x), (int)(pTile.y + newPos.y));
                if (newTile != null)
                {
                    EffectsLibrary.spawnAtTile("fx_lightning", newTile, effectSize);
                }
            }
            // if (baseEffect == null)
            // {
            //     return true;
            // }
            // baseEffect.sprRenderer.flipX = Toolbox.randomBool();
            damageEnemyObjectsInChunks(pSelf, pTile, impactRange);
            return true;
        }

        private static bool waterAttributeImpact(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile, int impactRange, float effectSize, bool showEffect = true)
        {
            if (pTile == null)
            {
                return false;
            }
            if (showEffect)
            {
                EffectsLibrary.spawnAtTile("fx_waterAttribute_dej", pTile, effectSize);
            }
            if (impactRange > 20)
            {
                getObjectsInManyChunks(pTile, impactRange, MapObjectType.Actor);
            }
            else
            {
                World.world.getObjectsInChunks(pTile, impactRange, MapObjectType.Actor);
            }
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            if (temp_objs.Contains(pSelf))
            {
                temp_objs.Remove(pSelf);
            }
            for(int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = temp_objs[i].a;
                ActionLibrary.addSlowEffectOnTarget(pSelf, pActor, pTile);
                pActor.getHit(pSelf.a.stats[S.damage]/2f, true, AttackType.Weapon, pSelf, true, false);
            }
            return true;
        }

        private static bool earthAttributeImpact(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile, float effectSize, int impactRange, bool showEffect = true)
        {
            if (pTile == null)
            {
                return true;
            }
            if (showEffect)
            {
                EffectsLibrary.spawnAtTile("fx_earthAttribute_dej", pTile, effectSize);
            }
            damageEnemyObjectsInChunks(pSelf, pTile, impactRange);
            return true;
        }

        private static bool fireAttributeImpact(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile, float effectSize, 
        int effectCount, int posVal, int impactRange)
        {
            if (pTile == null)
            {
                return true;
            }
            // BaseEffect baseEffect = EffectsLibrary.spawnAtTile("fx_fireball_explosion", pTile, effectSize);
            for (int i = 0; i < effectCount; i++)
            {
                Vector2 newPos = new Vector2(0, 0);
                switch (i)
                {
                    case 0:
                        newPos = new Vector2(posVal, 0);
                        break;
                    case 1:
                        newPos = new Vector2(-posVal, 0);
                        break;
                    case 2:
                        newPos = new Vector2(0, posVal);
                        break;
                    case 3:
                        newPos = new Vector2(0, -posVal);
                        break;
                    case 4:
                        newPos = new Vector2(-posVal/2, -posVal/2);
                        break;
                    case 5:
                        newPos = new Vector2(posVal/2, posVal/2);
                        break;
                }
                WorldTile newTile = World.world.GetTile((int)(pTile.x + newPos.x), (int)(pTile.y + newPos.y));
                if (newTile != null)
                {
                    EffectsLibrary.spawnAtTile("fx_fireball_explosion", newTile, effectSize);
                }
            }
            damageEnemyObjectsInChunks(pSelf, pTile, impactRange);
            return true;
        }

        private static void getObjectsInManyChunks(WorldTile pTile, int pRadius = 3, MapObjectType pObjectType = MapObjectType.All)
        {
            World.world.temp_map_objects.Clear();
            World.world.checkChunk(pTile.chunk, pTile, pRadius, pObjectType);
            for (int i = 0; i < pTile.chunk.neighbours.Count; i++)
            {
                MapChunk mapChunk = pTile.chunk.neighbours[i];
                World.world.checkChunk(mapChunk, pTile, pRadius, pObjectType);
                for (int j = 0; j < mapChunk.neighbours.Count; j++)
                {
                    MapChunk mapChunk2 = mapChunk.neighbours[j];
                    World.world.checkChunk(mapChunk2, pTile, pRadius, pObjectType);
                }
            }
        }

        private static void damageEnemyObjectsInChunks(BaseSimObject pSelf, WorldTile pTile, int pRadius = 3)
        {
            if (enemyObjectsList.Count > 0)
            {
                return;
            }
            checkEnemyChunk(pSelf, pTile.chunk, pTile, pRadius);
            for (int i = 0; i < pTile.chunk.neighbours.Count; i++)
            {
                MapChunk mapChunk = pTile.chunk.neighbours[i];
                checkEnemyChunk(pSelf, mapChunk, pTile, pRadius);
                if (pRadius < 20)
                {
                    continue;
                }
                for (int j = 0; j < mapChunk.neighbours.Count; j++)
                {
                    MapChunk mapChunk2 = mapChunk.neighbours[j];
                    checkEnemyChunk(pSelf, mapChunk2, pTile, pRadius);
                }
            }
            // enemyObjectsList.Clear();
            return;
        }

        private static void checkEnemyChunk(BaseSimObject pSelf, MapChunk pChunk, WorldTile pTile, int pRadius)
        {
            for (int i = 0; i < pChunk.k_list_objects.Count; i++)
            {
                Kingdom kingdom = pChunk.k_list_objects[i];
                List<BaseSimObject> list = pChunk.k_dict_objects[kingdom];
                for (int j = 0; j < list.Count; j++)
                {
                    BaseSimObject baseSimObject = list[j];
                    if (!(baseSimObject == null) && baseSimObject.base_data.alive && !baseSimObject.object_destroyed && baseSimObject != pSelf)
                    {
                        if (!baseSimObject.isActor())
                        {
                            continue;
                        }
                        if (pRadius == 0 || Toolbox.DistTile(baseSimObject.currentTile, pTile) <= (float)pRadius)
                        {
                            // enemyObjectsList.Add(baseSimObject);
                            float dmg = pSelf.a.stats[S.damage]/2f;
                            baseSimObject.a.getHit(dmg, true, AttackType.Weapon, pSelf, false, false);
                        }
                    }
                }
            }
        }
    }
}