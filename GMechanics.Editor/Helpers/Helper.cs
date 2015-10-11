using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses;
using GMechanics.Core.Classes.Storages;

namespace GMechanics.Editor.Helpers
{
    public class AtributeValuesListMatchingMap : Dictionary<ParentalGameObjectAttributeValue,
                                                 ParentalGameObjectAttributeValue> { }

    public static class Helper
    {
        public delegate void SetObjectTreeNodeIconHandler(Atom atom, TreeNode rootNode, 
                                                          TreeNode node);

        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam,
                                                IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmd);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int BringWindowToTop(IntPtr hwndParent);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x,
                                               int y, int cx, int cy, uint uFlags);

        public static void LockUpdate(Control parentCtrl)
        {
            SendMessage(parentCtrl.Handle, 0x000B, (IntPtr)0, (IntPtr)0);
        }

        public static void UnlockUpdate(Control parentCtrl)
        {
            SendMessage(parentCtrl.Handle, 0x000B, (IntPtr)1, (IntPtr)0);
            parentCtrl.Invalidate(true);
        }

        public static char GetCharFromKeys(Keys keyData)
        {
            char keyValue;
            switch (keyData)
            {
                case Keys.Add:
                case Keys.Oemplus:
                    keyValue = '+';
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    keyValue = '-';
                    break;
                case Keys.OemQuestion | Keys.Shift:
                    keyValue = '?';
                    break;
                case Keys.OemQuestion:
                case Keys.Divide:
                    keyValue = '/';
                    break;
                default:
                    if ((0x60 <= (int) keyData) && (0x69 >= (int) keyData))
                    {
                        keyValue = (char) ((int) keyData - 0x30);
                    }
                    else
                    {
                        keyValue = (char) keyData;
                    }
                    break;
            }
            return keyValue;
        }

        public static void BringWindowToFront(IntPtr windowHandle)
        {
            ShowWindow(windowHandle, 1);
            BringWindowToTop(windowHandle);
            SetForegroundWindow(windowHandle);
            SetWindowPos(windowHandle, 0, 0, 0, 0, 0, 0x0001 | 0x0002);
        }

        public static TreeNode AddGameEntitiesTreeViewNode(Atom atom, TreeNode rootNode)
        {
            return AddGameEntitiesTreeViewNode(atom, rootNode, null);
        }

        public static TreeNode AddGameEntitiesTreeViewNode(Atom atom, TreeNode rootNode,
            SetObjectTreeNodeIconHandler handler)
        {
            TreeNode node = new TreeNode(atom.ShortDisplayName) { Name = atom.Name, Tag = atom };
            if (handler == null)
            {
                node.ImageIndex = node.SelectedImageIndex = rootNode.Level + 1;
            }
            else
            {
                handler(atom, rootNode, node);
            }
            rootNode.Nodes.Add(node);
            return node;
        }

        public static bool IsGameObjectPropertyClassNameExists(string className)
        {
            return GlobalStorage.Instance.GameObjectPropertiesClasses.ContainsKey(className);
        }

        public static bool IsGameObjectPropertyNameExists(string propertyName)
        {
            return GlobalStorage.Instance.GameObjectProperties.ContainsKey(propertyName);
        }

        public static bool IsGameObjectFeatureClassNameExists(string className)
        {
            return GlobalStorage.Instance.GameObjectFeaturesClasses.ContainsKey(className);
        }

        public static bool IsGameObjectFeatureNameExists(string featureName)
        {
            return GlobalStorage.Instance.GameObjectFeatures.ContainsKey(featureName);
        }

        public static bool IsGameObjectAttributeNameExists(string attributeName)
        {
            return GlobalStorage.Instance.GameObjectAttributes.ContainsKey(attributeName);
        }

        public static bool IsElementaryGameObjectGroupNameExists(string groupPath)
        {
            return GlobalStorage.Instance.ElementaryGameObjectGroups.ContainsKey(groupPath);
        }

        public static bool IsGameObjectGroupNameExists(string groupPath)
        {
            return GlobalStorage.Instance.GameObjectGroups.ContainsKey(groupPath);
        }

        public static bool IsElementaryGameObjectNameExists(string gameObjectName)
        {
            return GlobalStorage.Instance.ElementaryGameObjectGroups.ContainsKey(gameObjectName);
        }

        public static bool IsGameObjectNameExists(string gameObjectName)
        {
            return GlobalStorage.Instance.GameObjectGroups.ContainsKey(gameObjectName);
        }

        private static void GetMatchingForAtributeValue(AtributeValuesListMatchingMap map,
                                                ParentalGameObjectAttributeValue parentValue,
                                                ParentalGameObjectAttributeValue clonedValue)
        {
            if (!map.ContainsKey(clonedValue))
            {
                map.Add(clonedValue, parentValue);
                for (int i = 0; i < parentValue.Values.Count; i++)
                {
                    GetMatchingForAtributeValue(map, parentValue.Values[i], clonedValue.Values[i]);
                }
            }
        }

        public static AtributeValuesListMatchingMap CreateMatchingMap(
            ParentalGameObjectAttributeValuesList parentList,
            ParentalGameObjectAttributeValuesList clonnedList)
        {
            AtributeValuesListMatchingMap valuesMap = new AtributeValuesListMatchingMap();
            if (parentList != null)
            {
                for (int i = 0; i < parentList.Count; i++)
                {
                    GetMatchingForAtributeValue(valuesMap, parentList[i], clonnedList[i]);
                }
            }
            return valuesMap;
        }

        private static void ApplyMatchingsForAttributeValue(AtributeValuesListMatchingMap matchingMap,
                                                           ParentalGameObjectAttributeValuesList workList,
                                                           ParentalGameObjectAttributeValue value)
        {
            if (matchingMap.ContainsKey(value))
            {
                ParentalGameObjectAttributeValue baseValue = matchingMap[value];
                baseValue.Assign(value);
                workList[value] = baseValue;
                matchingMap.Remove(value);
                foreach (ParentalGameObjectAttributeValue subValue in value.Values)
                {
                    ApplyMatchingsForAttributeValue(matchingMap, workList, subValue);
                }
            }
        }

        public static ParentalGameObjectAttributeValuesList ApplyMatchingMap(
            AtributeValuesListMatchingMap matchingMap,
            ParentalGameObjectAttributeValuesList workList)
        {
            // Apply matching
            for (int i = 0; i < workList.Count; i++)
            {
                ParentalGameObjectAttributeValue value = workList[i];
                ApplyMatchingsForAttributeValue(matchingMap, workList, value);
            }

            // Destroy old unused
            foreach (ParentalGameObjectAttributeValue unusedValue in matchingMap.Values)
            {
                unusedValue.Destroy();
            }

            // Return result
            return workList;
        }

        public static Atom GetAttributeValueParent(Atom attributeValue) //!!!
        {
            GameObjectAttributes attributes = GlobalStorage.Instance.GameObjectAttributes;
            foreach (ParentalGameObjectAttribute attribute in attributes.Values)
            {
                if (attribute.Values.GetValue(attributeValue.Name, true) == attributeValue)
                {
                    return attribute;
                }
            }
            return null;
        }

        public static string GetMD5Hash(string str)
        {
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in data)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public static string EncodeToBase64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeFromBase64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return Encoding.ASCII.GetString(encodedDataAsBytes);
        }
    }
}