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
    class NewActorAssets : MonoBehaviour
    {
        public static void init()
        {
            loadAssets();
        }

        private static void loadAssets()
        {
            ActorAsset battleBoat = AssetManager.actor_library.clone("battle_boat", "_boat");
            battleBoat.get_override_sprite = AssetManager.actor_library.get("_boat").get_override_sprite;
            battleBoat.base_stats[S.speed] = 120f;
            battleBoat.base_stats[S.health] = 1000f;
            battleBoat.base_stats[S.armor] = 50f;
            battleBoat.base_stats[S.damage] = 80f;
            battleBoat.base_stats[S.attack_speed] = 10f;
            battleBoat.drawBoatMark = true;
            battleBoat.drawBoatMark_big = true;
            battleBoat.skipFightLogic = false;
            battleBoat.inspect_stats = true;
		    battleBoat.inspect_children = true;
            battleBoat.inspect_experience = true;
            battleBoat.inspect_kills = true;
            battleBoat.use_items = true;
            battleBoat.take_items = false;
            battleBoat.defaultAttack = "shotgun";
            // battleBoat.defaultWeapons = new List<string>{ "shotgun" };
            battleBoat.job = "boat_battle_job_dej";
            battleBoat.tech = "boats_transport";
            battleBoat.cost = new ConstructionCost(5, 0, 2, 20);
            battleBoat.actorSize = ActorSize.S17_Dragon;
            battleBoat.get_override_sprite = delegate(Actor pActor)
            {
                Boat component = pActor.GetComponent<Boat>();
                // AnimationDataBoat animationDataBoat = ActorAnimationLoader.loadAnimationBoat("battle_boat_human");
                AnimationDataBoat animationDataBoat = getAnimationDataBoat(component);
                ActorAnimation actorAnimation = animationDataBoat.normal;
                if (!pActor.data.alive)
                {
                    actorAnimation = animationDataBoat.broken;
                }
                else if (pActor.zPosition.y != 0f || pActor.isInMagnet())
                {
                    actorAnimation = animationDataBoat.normal;
                }
                else
                {
                    animationDataBoat.dict.TryGetValue(component.last_movement_angle, out actorAnimation);
                    if (actorAnimation == null)
                    {
                        int closestAngle = Toolbox.getClosestAngle(component.last_movement_angle, animationDataBoat);
                        animationDataBoat.dict.TryGetValue(closestAngle, out actorAnimation);
                    }
                }
                if (actorAnimation == null)
                {
                    actorAnimation = animationDataBoat.normal;
                }
                Sprite sprite2 = actorAnimation.frames[0];
                if (actorAnimation.frames.Length != 0)
                {
                    sprite2 = AnimationHelper.getSpriteFromList(0, actorAnimation.frames, pActor.asset.animation_swim_speed);
                }
                AnimationFrameData animationFrameData = null;
                Sprite sprite3 = sprite2;
                Kingdom kingdom2 = pActor.kingdom;
                Sprite spriteUnit = UnitSpriteConstructor.getSpriteUnit(animationFrameData, sprite3, pActor, (kingdom2 != null) ? kingdom2.kingdomColor : null, pActor.race, pActor.data.skin_set, pActor.data.skin, pActor.asset.texture_atlas);
                if (spriteUnit == null)
                {
                    return null;
                }
                return spriteUnit;
            };
        }

        public static AnimationDataBoat getAnimationDataBoat(Boat boat)
        {
            string text = "boat_fishing";
            if (boat.actor.kingdom != null)
            {
                string text2;
                if (boat.actor.kingdom.race == null)
                {
                    text2 = "human";
                }
                else
                {
                    text2 = boat.actor.kingdom.race.id;
                    if (boat.actor.kingdom.isCiv())
                    {
                        text2 = boat.actor.kingdom.race.id;
                    }
                    else
                    {
                        text2 = "human";
                    }
                }
                string id = boat.actor.asset.id;
                if (id != null)
                {
                    if (id == "boat_transport")
                    {
                        text = "boat_transport_" + text2;
                    }
                    else if (id == "boat_trading")
                    {
                        text = "boat_trading_" + text2;
                    }
                    else if (id == "battle_boat")
                    {
                        text = "battle_boat_" + text2;
                    }
                    else
                    {
                        text = "boat_fishing";
                    }
                }
            }
            return ActorAnimationLoader.loadAnimationBoat(text);
        }
    }
}