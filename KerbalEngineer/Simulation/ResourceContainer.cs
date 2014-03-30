// Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

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
                if (this.resources.ContainsKey(type))
                {
                    return (double)this.resources[type];
                }
                else
                {
                    return 0d;
                }
            }
            set
            {
                if (this.resources.ContainsKey(type))
                {
                    this.resources[type] = value;
                }
                else
                {
                    this.resources.Add(type, value);
                }
            }
        }

        public bool HasType(int type)
        {
            return this.resources.ContainsKey(type);
        }

        public List<int> Types
        {
            get
            {
                List<int> types = new List<int>();

                foreach (int key in this.resources.Keys)
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

                foreach (double resource in this.resources.Values)
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
                foreach (int type in this.resources.Keys)
                {
                    if ((double)this.resources[type] > 1d)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void Add(int type, double amount)
        {
            if (this.resources.ContainsKey(type))
            {
                this.resources[type] = (double)this.resources[type] + amount;
            }
            else
            {
                this.resources.Add(type, amount);
            }
        }

        public void Reset()
        {
            this.resources = new Hashtable();
        }

        public void Debug()
        {
            foreach (int key in this.resources.Keys)
            {
                UnityEngine.MonoBehaviour.print(" -> " + GetResourceName(key) + " = " + this.resources[key]);
            }
        }

        public double GetResourceMass(int type)
        {
            return (double)this.resources[type] * GetResourceDensity(type);
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
