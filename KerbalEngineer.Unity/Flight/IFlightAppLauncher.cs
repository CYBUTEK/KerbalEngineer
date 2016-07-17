// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Unity.Flight
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface IFlightAppLauncher
    {
        bool IsControlBarVisible { get; set; }

        bool IsDisplayStackVisible { get; set; }

        bool IsOn { get; }

        void ApplyTheme(GameObject gameObject);

        void ClampToScreen(RectTransform rectTransform);

        Vector3 GetAnchor();

        IList<ISectionModule> GetCustomSections();

        IList<ISectionModule> GetStockSections();

        ISectionModule NewCustomSection();
    }
}