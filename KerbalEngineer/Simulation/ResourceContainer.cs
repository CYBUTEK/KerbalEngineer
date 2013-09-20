// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using System.Collections;
using System.Collections.Generic;

namespace KerbalEngineer.Simulation
{
    public class ResourceContainer
    {
        Hashtable resources = new Hashtable();

        public double this[int type]
        {
            get
            {
                if (resources.ContainsKey(type))
                {
                    return (double)resources[type];
                }
                else
                {
                    return 0d;
                }
            }
            set
            {
                if (resources.ContainsKey(type))
                {
                    resources[type] = value;
                }
                else
                {
                    resources.Add(type, value);
                }
            }
        }

        public List<int> Types
        {
            get
            {
                List<int> types = new List<int>();

                foreach (int key in resources.Keys)
                {
                    types.Add(key);
                }

                return types;
            }
        }

        public double Mass
        {
            get
            {
                double mass = 0d;

                foreach (double resource in resources.Values)
                {
                    mass += resource;
                }

                return mass;
            }
        }

        public bool Empty
        {
            get
            {
                foreach (int type in resources.Keys)
                {
                    if ((double)resources[type] > 1d)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void Add(int type, double amount)
        {
            if (resources.ContainsKey(type))
            {
                resources[type] = (double)resources[type] + amount;
            }
            else
            {
                resources.Add(type, amount);
            }
        }

        public void Reset()
        {
            resources = new Hashtable();
        }

        public void Debug()
        {
            foreach (int key in resources.Keys)
            {
                UnityEngine.MonoBehaviour.print(" -> " + GetResourceName(key) + " = " + resources[key]);
            }
        }

        public double GetResourceMass(int type)
        {
            return (double)resources[type] * GetResourceDensity(type);
        }

        public static ResourceFlowMode GetResourceFlowMode(int type)
        {
            return PartResourceLibrary.Instance.GetDefinition(type).resourceFlowMode;
        }

        public static ResourceTransferMode GetResourceTransferMode(int type)
        {
            return PartResourceLibrary.Instance.GetDefinition(type).resourceTransferMode;
        }

        public static float GetResourceDensity(int type)
        {
            return PartResourceLibrary.Instance.GetDefinition(type).density;
        }

        public static string GetResourceName(int type)
        {
            return PartResourceLibrary.Instance.GetDefinition(type).name;
        }
    }
}
