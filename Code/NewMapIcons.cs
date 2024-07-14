using System;
using System.IO;
using System.Text;
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
    class NewMapIcons : MonoBehaviour
    {
        public static void init()
        {
            MapIconAsset whisperAssetOne = AssetManager.map_icons.add(new MapIconAsset
            {
                id = "whisper_of_alliance_dej",
                id_prefab = "p_mapZone",
                base_scale = 1f,
                draw_call = new MapIconUpdater(drawWhisperOfAlliance),
                render_on_map = true,
                add_camera_zoom_mod = false,
                color = new Color(1f, 1f, 1f, 0.4f),
                color_2 = new Color(1f, 0.1f, 0.1f, 0.2f)
            });
            MapIconGroupSystem whisperGroupOne = new GameObject().AddComponent<MapIconGroupSystem>();
			whisperGroupOne.create(whisperAssetOne);
			whisperAssetOne.group_system = whisperGroupOne;

            MapIconAsset whisperAssetTwo = AssetManager.map_icons.add(new MapIconAsset
            {
                id = "whisper_of_alliance_dej_line",
                id_prefab = "p_mapArrow_line",
                base_scale = 0.5f,
                draw_call = new MapIconUpdater(drawWhisperOfAllianceLine),
                render_on_map = true,
                render_in_game = true,
                color = new Color(0.4f, 0.4f, 1f, 0.9f)
            });
            MapIconGroupSystem whisperGroupTwo = new GameObject().AddComponent<MapIconGroupSystem>();
			whisperGroupTwo.create(whisperAssetTwo);
			whisperAssetTwo.group_system = whisperGroupTwo;

            MapIconAsset convertAssetOne = AssetManager.map_icons.add(new MapIconAsset
            {
                id = "city_convert_dej",
                id_prefab = "p_mapZone",
                base_scale = 1f,
                draw_call = new MapIconUpdater(drawCityConvert),
                render_on_map = true,
                add_camera_zoom_mod = false,
                color = new Color(1f, 1f, 1f, 0.4f),
                color_2 = new Color(1f, 0.1f, 0.1f, 0.2f)
            });
            MapIconGroupSystem convertGroupOne = new GameObject().AddComponent<MapIconGroupSystem>();
			convertGroupOne.create(convertAssetOne);
			convertAssetOne.group_system = convertGroupOne;

            MapIconAsset convertAssetTwo = AssetManager.map_icons.add(new MapIconAsset
            {
                id = "city_convert_dej_line",
                id_prefab = "p_mapArrow_line",
                base_scale = 0.5f,
                draw_call = new MapIconUpdater(drawCityConvertLine),
                render_on_map = true,
                render_in_game = true,
                color = new Color(0.4f, 0.4f, 1f, 0.9f)
            });
            MapIconGroupSystem convertGroupTwo = new GameObject().AddComponent<MapIconGroupSystem>();
			convertGroupTwo.create(convertAssetTwo);
			convertAssetTwo.group_system = convertGroupTwo;

            MapIconAsset expandAsset = AssetManager.map_icons.add(new MapIconAsset
            {
                id = "expand_zone_dej_line",
                id_prefab = "p_mapArrow_line",
                base_scale = 0.5f,
                draw_call = new MapIconUpdater(drawExpandZoneLine),
                render_on_map = true,
                render_in_game = true,
                color = new Color(0.4f, 0.4f, 1f, 0.9f)
            });
            MapIconGroupSystem expandGroup = new GameObject().AddComponent<MapIconGroupSystem>();
			expandGroup.create(expandAsset);
			expandAsset.group_system = expandGroup;

            MapIconAsset expandCultureAsset = AssetManager.map_icons.add(new MapIconAsset
            {
                id = "expand_culture_zone_dej_line",
                id_prefab = "p_mapArrow_line",
                base_scale = 0.5f,
                draw_call = new MapIconUpdater(drawCultureExpandZoneLine),
                render_on_map = true,
                render_in_game = true,
                color = new Color(0.4f, 0.4f, 1f, 0.9f)
            });
            MapIconGroupSystem expandCultureGroup = new GameObject().AddComponent<MapIconGroupSystem>();
			expandCultureGroup.create(expandCultureAsset);
			expandCultureAsset.group_system = expandCultureGroup;

            MapIconAsset filteredMagnetUnits = AssetManager.map_icons.add(new MapIconAsset
            {
                id = "filtered_magnet_units_dej",
                id_prefab = "p_mapSprite",
                render_on_map = true,
                render_in_game = true,
                draw_call = new MapIconUpdater(drawFilteredMagnetUnits)
            });
            MapIconGroupSystem filteredMagnetUnitsGroup = new GameObject().AddComponent<MapIconGroupSystem>();
			filteredMagnetUnitsGroup.create(filteredMagnetUnits);
			filteredMagnetUnits.group_system = filteredMagnetUnitsGroup;
        }

        private static void drawFilteredMagnetUnits(MapIconAsset pAsset)
        {
            if (!Main.filteredMagnet.hasUnits())
            {
                return;
            }
            foreach (Actor actor in Main.filteredMagnet.magnetUnits)
            {
                if (!(actor == null))
                {
                    MapMark mapMark = MapIconLibrary.drawMark(pAsset, actor.currentPosition, null, actor.kingdom, null, null, 1f, false);
                    mapMark.transform.localScale = actor.transform.localScale;
                    mapMark.spriteRenderer.sprite = actor.getSpriteToRender();
                }
            }
        }

        private static void drawCultureExpandZoneLine(MapIconAsset pAsset)
        {
            if (!Input.mousePresent)
            {
                return;
            }
            if (World.world.isBusyWithUI())
            {
                return;
            }
            if (!World.world.isSelectedPower("expand_culture_zone_dej") && !World.world.isSelectedPower("remove_culture_zone_dej"))
            {
                return;
            }
            Culture whisperA = NewActions.cultureToBeExpanded;
            if (whisperA == null)
            {
                return;
            }
            Vector2 mousePos = World.world.getMousePos();
            Color color = whisperA.getColor().getColorMain2();
            if (getCultureTile(whisperA) == null)
            {
                return;
            }
            Reflection.CallStaticMethod(
                typeof(MapIconLibrary), 
                "drawArrowMark", pAsset, 
                getCultureTile(whisperA).posV, 
                new Vector3(mousePos.x, mousePos.y, 1), 
                color, 
                null
            );
        }

        private static WorldTile getCultureTile(Culture culture)
        {
            if (culture.zones.Count == 0)
            {
                culture.titleCenter = Globals.POINT_IN_VOID;
                return null;
            }
            float num = 0f;
            float num2 = 0f;
            float num3 = 0f;
            TileZone tileZone = null;
            foreach (TileZone tileZone2 in culture.zones)
            {
                num += tileZone2.centerTile.posV3.x;
                num2 += tileZone2.centerTile.posV3.y;
            }
            culture.titleCenter.x = num / (float)culture.zones.Count;
            culture.titleCenter.y = num2 / (float)culture.zones.Count;
            foreach (TileZone tileZone3 in culture.zones)
            {
                float num4 = Toolbox.Dist((float)tileZone3.centerTile.x, (float)tileZone3.centerTile.y, culture.titleCenter.x, culture.titleCenter.y);
                if (tileZone == null || num4 < num3)
                {
                    tileZone = tileZone3;
                    num3 = num4;
                }
            }
            culture.titleCenter.x = tileZone.centerTile.posV3.x;
	        culture.titleCenter.y = tileZone.centerTile.posV3.y + 2f; 
            return tileZone.centerTile;
        }

        private static void drawExpandZoneLine(MapIconAsset pAsset)
        {
            if (!Input.mousePresent)
            {
                return;
            }
            if (World.world.isBusyWithUI())
            {
                return;
            }
            if (!World.world.isSelectedPower("expand_zone_dej") && !World.world.isSelectedPower("remove_zone_dej"))
            {
                return;
            }
            City whisperA = NewActions.cityToBeExpanded;
            if (whisperA == null)
            {
                return;
            }
            Vector2 mousePos = World.world.getMousePos();
            Kingdom pKingdom = (Kingdom)Reflection.GetField(typeof(City), whisperA, "kingdom");
            Color color = pKingdom.getColor().getColorMain2();
            if (whisperA.getTile(false) == null)
            {
                return;
            }
            Reflection.CallStaticMethod(
                typeof(MapIconLibrary), 
                "drawArrowMark", pAsset, 
                whisperA.getTile(false).posV, 
                new Vector3(mousePos.x, mousePos.y, 1), 
                color, 
                null
            );
        }

        private static void drawWhisperOfAlliance(MapIconAsset pAsset)
        {
            if (World.world.isBusyWithUI())
            {
                return;
            }
            if (!World.world.isSelectedPower("alliance_dej"))
            {
                return;
            }
            WorldTile mouseTilePos = World.world.getMouseTilePos();
            City city = ((mouseTilePos != null) ? mouseTilePos.zone.city : null);
            Kingdom kingdom;
            if (Config.whisperA == null)
            {
                if (city == null)
                {
                    return;
                }
                kingdom = (Kingdom)Reflection.GetField(typeof(City), city, "kingdom");
            }
            else
            {
                kingdom = Config.whisperA;
            }
            for (int i = 0; i < kingdom.cities.Count; i++)
            {
                City city2 = kingdom.cities[i];

                List<TileZone> pZones = (List<TileZone>)Reflection.GetField(typeof(City), city2, "zones");
                for (int j = 0; j < pZones.Count; j++)
                {
                    TileZone tileZone = pZones[j];
                    MapMark mark = (MapMark)drawMark( pAsset, tileZone.centerTile.posV, null, null, null, null, 1f, false);
                    
                    mark.spriteRenderer.color = pAsset.color;
                }
            }
            Reflection.CallStaticMethod(typeof(MapIconLibrary), "colorEnemies", pAsset, kingdom);
        }

        private static void drawWhisperOfAllianceLine(MapIconAsset pAsset)
        {
            if (!Input.mousePresent)
            {
                return;
            }
            if (World.world.isBusyWithUI())
            {
                return;
            }
            if (!World.world.isSelectedPower("alliance_dej"))
            {
                return;
            }
            Kingdom whisperA = Config.whisperA;
            if (whisperA == null)
            {
                return;
            }
            Vector2 mousePos = World.world.getMousePos();
            foreach (City city in whisperA.cities)
            {
                Color color = whisperA.getColor().getColorMain2();
                Reflection.CallStaticMethod(
                    typeof(MapIconLibrary), 
                    "drawArrowMark", pAsset, 
                    city.getTile(false).posV, 
                    new Vector3(mousePos.x, mousePos.y, 1), 
                    color, 
                    null
                );
            }
        }

        private static void drawCityConvert(MapIconAsset pAsset)
        {
            if (World.world.isBusyWithUI())
            {
                return;
            }
            if (!World.world.isSelectedPower("city_convert_dej"))
            {
                return;
            }
            WorldTile mouseTilePos = World.world.getMouseTilePos();
            City city = ((mouseTilePos != null) ? mouseTilePos.zone.city : null);
            if (NewActions.convertCityA != null)
            {
                city = NewActions.convertCityA;
            }
            if(city == null)
            {
                return;
            }

            List<TileZone> pZones = (List<TileZone>)Reflection.GetField(typeof(City), city, "zones");
            for (int j = 0; j < pZones.Count; j++)
            {
                TileZone tileZone = pZones[j];
                MapMark mark = (MapMark)drawMark( pAsset, tileZone.centerTile.posV, null, null, null, null, 1f, false);
                
                mark.spriteRenderer.color = pAsset.color;
            }
            Reflection.CallStaticMethod(typeof(MapIconLibrary), "colorEnemies", pAsset, (Kingdom)Reflection.GetField(typeof(City), city, "kingdom"));
        }

        private static void drawCityConvertLine(MapIconAsset pAsset)
        {
            if (!Input.mousePresent)
            {
                return;
            }
            if (World.world.isBusyWithUI())
            {
                return;
            }
            if (!World.world.isSelectedPower("city_convert_dej"))
            {
                return;
            }
            City whisperA = NewActions.convertCityA;
            if (whisperA == null)
            {
                return;
            }
            Vector2 mousePos = World.world.getMousePos();
            Kingdom pKingdom = (Kingdom)Reflection.GetField(typeof(City), whisperA, "kingdom");
            Color color = pKingdom.getColor().getColorMain2();
            Reflection.CallStaticMethod(
                typeof(MapIconLibrary), 
                "drawArrowMark", pAsset, 
                whisperA.getTile(false).posV, 
                new Vector3(mousePos.x, mousePos.y, 1), 
                color, 
                null
            );
        }

        private static MapMark drawMark(MapIconAsset pAsset, Vector3 pVec, WorldTile pTileTarget = null, Kingdom pKingdom = null, City pCity = null, BattleContainer pBattle = null, float pModScale = 1f, bool pSetColor = false)
        {
            MapMark next = (MapMark)pAsset.group_system.CallMethod("getNext");
            if (pSetColor)
            {
                next.spriteRenderer.color = Toolbox.color_white;
            }
            float num = pAsset.base_scale * pModScale;
            if (pKingdom != null && pAsset.flag_kingdom_color)
            {
                next.spriteRenderer.color = ((ColorAsset)Reflection.GetField(typeof(Kingdom), pKingdom, "kingdomColor")).getColorMain2();
            }
            if (pAsset.flag_battle)
            {
                num = num * pBattle.timer * 0.2f;
            }
            if (pAsset.add_camera_zoom_mod)
            {
                num *= (float)Reflection.CallStaticMethod(typeof(MapIconLibrary), "getCameraScaleZoom", pAsset);
            }
            if (pAsset.selected_city_scale)
            {
                if (pCity != null)
                {
                    num *= pCity.mark_scale_effect;
                }
                else
                {
                    num *= 0.5f;
                }
            }
            next.transform.localPosition = pVec;
            next.transform.localScale = new Vector3(num, num, 1f);
            return next;
        }
    }
}