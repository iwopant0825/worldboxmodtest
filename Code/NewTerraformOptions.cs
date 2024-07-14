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
    class NewTerraformOptions : MonoBehaviour
    {
        public static void init()
        {
            loadOptions();
        }

        private static void loadOptions()
        {
            createAttributeTerraform(
                "FireAttributeTerraform",
                false, false, true, false, true, true
            );
            createAttributeTerraform(
                "WaterAttributeTerraform",
                false, true, false, false, false, false
            );
            createAttributeTerraform(
                "LightningAttributeTerraform",
                false, false, true, true, true, false
            );
            createAttributeTerraform(
                "EarthAttributeTerraform",
                true, true, true, false, false, false,
                0.15f
            );
            
        }

        private static void createAttributeTerraform(string attributeID,
        bool applyForce, bool removeFire, bool removeFrozen, bool lightningEffect, bool addBurned, bool setFire, float force_power = 0f)
        {
            for (int i = 1; i<=7; i++)
            {
                AssetManager.terraform.add(new TerraformOptions
                {
                    id = $"{attributeID}{i}",
                    flash = true,
                    explode_tile = true,
                    applyForce = applyForce,
                    removeFire = removeFire,
                    removeFrozen = removeFrozen,
                    lightningEffect = lightningEffect,
                    addBurned = addBurned,
                    setFire = setFire,
                    force_power = force_power
                });
            }
        }
    }
}