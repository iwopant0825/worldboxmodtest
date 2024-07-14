using ReflectionUtility;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NCMS;
using NCMS.Utils;

namespace CollectionMod
{
    public class FilteredMagnet
    {
        private int magnetState;
        private WorldTile magnetLastPos;
        private bool _hasUnits;
        private List<Actor> units = new List<Actor>();
        internal HashSet<Actor> magnetUnits = new HashSet<Actor>();
        private float pickedup_mod = 1f;
        private const float pickedup_mod_min = 0.1f;
        private float angle;
        private float lastCount;

        internal void magnetAction(bool pFromUpdate, WorldTile pTile = null)
        {
            if (ScrollWindow.isWindowActive())
            {
                this.dropPickedUnits();
                return;
            }
            if (pFromUpdate && this.magnetState != 1 && this.magnetState != 3)
            {
                return;
            }
            if (pTile != null)
            {
                this.magnetLastPos = pTile;
            }
            this.updatePickedUnits();
            if (pTile != null)
            {
                World.world.flashEffects.flashPixel(pTile, 10, ColorType.White);
            }
            switch (this.magnetState)
            {
            case 0:
                if (Input.GetMouseButton(0))
                {
                    this.magnetState = 1;
                    return;
                }
                break;
            case 1:
                if (!pFromUpdate)
                {
                    this.pickupUnits(pTile);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    this.magnetState = 2;
                    this.dropPickedUnits();
                    return;
                }
                break;
            case 2:
                if (!pFromUpdate && Input.GetMouseButton(0))
                {
                    this.dropPickedUnits();
                    this.magnetState = 0;
                }
                break;
            default:
                return;
            }
        }

        public void dropPickedUnits()
        {
            if (this.units.Count == 0)
            {
                return;
            }
            Config.currentBrushData = Brush.get(Config.currentBrush);
            WorldTile worldTile = this.magnetLastPos;
            if (this.magnetLastPos == null)
            {
                World.world.getMouseTilePos();
            }
            for (int i = 0; i < this.units.Count; i++)
            {
                Actor actor = this.units[i];
                if (!(actor == null))
                {
                    actor.currentPosition = actor.transform.position;
                    Actor actor2 = actor;
                    actor2.currentPosition.y = actor2.currentPosition.y - actor.zPosition.y;
                    actor.is_in_magnet = false;
                    actor.dirty_current_tile = true;
                    actor.findCurrentTile(true);
                    actor.spawnOn(actor.currentTile, actor.zPosition.y);
                    actor.addForce(0f, 0f, 0.6f);
                    // TODO: not 100% sure about this tbh
                    for (int j = 0; j < actor.callbacks_magnet_drop.GetInvocationList().Length; j++)
                    {
                        actor.callbacks_magnet_drop.GetInvocationList()[j].DynamicInvoke(actor);
                    }
                }
            }
            this.units.Clear();
            this.magnetUnits.Clear();
            this._hasUnits = false;
        }

        private void updatePickedUnits()
        {
            if (this.magnetLastPos == null)
            {
                return;
            }
            if (this.units.Count == 0)
            {
                return;
            }
            if (this.pickedup_mod > 0.1f)
            {
                this.pickedup_mod -= World.world.deltaTime * 0.3f;
                if (this.pickedup_mod < 0.1f)
                {
                    this.pickedup_mod = 0.1f;
                }
            }
            float num = (float)this.units.Count;
            float num2 = 6f;
            if (num > 5f)
            {
                num2 = 5f;
            }
            if (num > 50f)
            {
                num2 = 4.5f;
            }
            if (num > 100f)
            {
                num2 = 4f;
            }
            float num3 = 6.2831855f / num2;
            float num4 = 1f / num * 50f * this.pickedup_mod;
            num4 = this.pickedup_mod;
            float num5 = Mathf.Max(1f / num * 3.5f, 0.025f);
            num5 *= this.pickedup_mod;
            if (this.lastCount != num)
            {
                this.lastCount = num;
            }
            this.angle += num3 * Time.deltaTime;
            int num6 = 0;
            while ((float)num6 < num)
            {
                Actor actor = this.units[num6];
                if (!(actor == null) && actor.data.alive)
                {
                    actor.currentPosition = actor.transform.position;
                    actor.findCurrentTile(true);
                    Vector3 posV = this.magnetLastPos.posV3;
                    posV.x += Mathf.Cos(this.angle + (float)num6) * (num4 + (float)num6 * num5);
                    posV.y += Mathf.Sin(this.angle + (float)num6) * (num4 + (float)num6 * num5);
                    actor.currentPosition = new Vector3(posV.x, posV.y - actor.zPosition.y);
                    actor.transform.localPosition = posV;
                    // TODO: also not 100% sure about this implementation
                    for (int i = 0; i < actor.callbacks_magnet_update.GetInvocationList().Length; i++)
                    {
                        actor.callbacks_magnet_update.GetInvocationList()[i].DynamicInvoke(actor);
                    }
                }
                num6++;
            }
        }

        private void pickupUnits(WorldTile pTile)
        {
            BrushData currentBrushData = Config.currentBrushData;
            bool tUnitsAdded = false;
            Action<Actor> pActionActor = null;
            for (int i = 0; i < currentBrushData.pos.Length; i++)
            {
                WorldTile tile = World.world.GetTile(currentBrushData.pos[i].x + pTile.x, currentBrushData.pos[i].y + pTile.y);
                if (tile != null && tile.hasUnits())
                {
                    WorldTile worldTile = tile;
                    Action<Actor> action;
                    if ((action = pActionActor) == null)
                    {
                        action = (pActionActor = delegate(Actor tActor)
                        {
                            if (!tActor.asset.canBeMovedByPowers)
                            {
                                return;
                            }
                            if (tActor.isInsideSomething())
                            {
                                return;
                            }
                            if (Main.savedSettings.multipleInputOptions["Magnet Level Filter"]["Min Level"].value == "-1" ||
                             Main.savedSettings.multipleInputOptions["Magnet Level Filter"]["Max Level"].value == "-1")
                            {
                            } else if (tActor.data.level < int.Parse(Main.savedSettings.multipleInputOptions["Magnet Level Filter"]["Min Level"].value) ||
                                  (tActor.data.level > int.Parse(Main.savedSettings.multipleInputOptions["Magnet Level Filter"]["Max Level"].value)))
                            {
                                return;
                            }
                            if (!this.units.Contains(tActor))
                            {
                                tActor.cancelAllBeh(null);
                                this.units.Add(tActor);
                                this.magnetUnits.Add(tActor);
                                tUnitsAdded = true;
                                tActor.is_in_magnet = true;
                                this.pickedup_mod = 2f;
                            }
                        });
                    }
                    worldTile.doUnits(action);
                }
            }
            if (tUnitsAdded)
            {
                this._hasUnits = true;
            }
        }

        public bool hasUnits()
        {
            return this._hasUnits;
        }
    }
}