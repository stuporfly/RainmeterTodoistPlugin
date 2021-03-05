/*
  Copyright (C) 2014 Birunthan Mohanathas

  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainmeter;
using TodoistPersister;

// Overview: This example demonstrates the basic concept of Rainmeter C# plugins.

// Sample skin:
/*
    [Rainmeter]
    Update=1000
    BackgroundMode=2
    SolidColor=000000

    [mString]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=String

    [mMajor]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Major

    [mMinor]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Minor

    [mNumber]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Number

    [Text1]
    Meter=STRING
    MeasureName=mString
    MeasureName2=mMajor
    MeasureName3=mMinor
    MeasureName4=mNumber
    X=5
    Y=5
    W=300
    H=70
    FontColor=FFFFFF
    Text="String: %1#CRLF#Major: %2#CRLF#Minor: %3#CRLF#Number: %4#CRLF#"

    [Text2]
    Meter=STRING
    MeasureName=mString
    MeasureName2=mMajor
    MeasureName3=mMinor
    MeasureName4=mNumber
    NumOfDecimals=1
    X=5
    Y=5R
    W=300
    H=70
    FontColor=FFFFFF
    Text="String: %1#CRLF#Major: %2#CRLF#Minor: %3#CRLF#Number: %4#CRLF#"
*/

namespace PluginTodoist
{
    internal class Measure
    {

        private string filterName;
        private string projectName;
        private string type;
        private int itemIndex;
        private string token;
        internal Measure()
        {
        }

        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
            type = api.ReadString("Type", "Filter");
            filterName = api.ReadString("FilterName", "");
            projectName = api.ReadString("ProjectName", "");
            itemIndex = api.ReadInt("itemIndex", -1);

            TodoistPersister.TodoistPersister.instance.Token = api.ReadString("Token", "");


        }

        internal double Update()
        {

            return 0.0;
        }

        internal string GetString()
        {
            try
            {

                if (type == "Project")
                {
                    if (itemIndex == -1)
                        return TodoistPersister.TodoistPersister.instance.getProjectAsString(projectName);
                    else
                        return TodoistPersister.TodoistPersister.instance.getProjectAsString(projectName, itemIndex);

                }
                if (type == "Filter")
                {
                    if (itemIndex == -1)

                        return TodoistPersister.TodoistPersister.instance.getFilterAsString(filterName);
                    else
                        return TodoistPersister.TodoistPersister.instance.getFilterAsString(filterName,itemIndex);



                }
                return "settings incorrect";

            }
            catch (Exception)
            {
                return "Something went wrong";

            }
        }
    }

    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();

            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }
    }
}
