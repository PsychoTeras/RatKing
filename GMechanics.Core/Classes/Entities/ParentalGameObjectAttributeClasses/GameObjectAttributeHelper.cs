using System.Collections.Generic;

namespace GMechanics.Core.Classes.Entities.ParentalGameObjectAttributeClasses
{
    public class AtributeValuesListMatchingMap : Dictionary<ParentalGameObjectAttributeValue, 
                                                 ParentalGameObjectAttributeValue> {}

    public static class GameObjectAttributeHelper
    {
        private static void GetMatchingForAtributeValue(AtributeValuesListMatchingMap map,
                                                        ParentalGameObjectAttributeValue parentValue,
                                                        ParentalGameObjectAttributeValue clonedValue)
        {
            if (!map.ContainsKey(clonedValue))
            {
                map.Add(clonedValue, parentValue);
                if (parentValue.Values != null)
                {
                    for (int i = 0; i < parentValue.Values.Count; i++)
                    {
                        GetMatchingForAtributeValue(map, parentValue.Values[i],
                                                    clonedValue.Values[i]);
                    }
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

        private static void ApplyMatchingsForAtributeValue(AtributeValuesListMatchingMap matchingMap,
                                                           ParentalGameObjectAttributeValuesList workList,
                                                           ParentalGameObjectAttributeValue value)
        {
            if (matchingMap.ContainsKey(value))
            {
                ParentalGameObjectAttributeValue baseValue = matchingMap[value];
                baseValue.Assign(value);
                workList[value] = baseValue;
                matchingMap.Remove(value);
                if (value.Values != null)
                {
                    for (int i = 0; i < value.Values.Count; i++)
                    {
                        ParentalGameObjectAttributeValue subValue = value.Values[i];
                        ApplyMatchingsForAtributeValue(matchingMap, workList, subValue);
                    }
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
                ApplyMatchingsForAtributeValue(matchingMap, workList, value);
            }

            // Destroy old unused
            foreach (ParentalGameObjectAttributeValue unusedValue in matchingMap.Values)
            {
                unusedValue.Destroy();
            }

            // Return result
            return workList;
        }
    }
}