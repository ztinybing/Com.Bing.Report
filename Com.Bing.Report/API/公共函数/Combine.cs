using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.API
{
    public class Combine
    {
        public delegate bool DealCombineDelegate(int[] state);
        public static void Do(int n, int m, DealCombineDelegate dele)
        {
            bool done = false;
            int[] state = new int[m];
            for (int i = 0; i < n; i++)
            {
                state[i] = 1;
            }
            if (dele != null)
            {
                done = dele(state);
            }
            bool swaped = true;
            while (swaped && !done)
            {
                for (int index = 1; index < state.Length; index++)
                {
                    swaped = false;
                    if (state[index - 1] == 1 && state[index] == 0)
                    {
                        swaped = true;
                        state[index - 1] = 0;
                        state[index] = 1;
                        int oneCount = 0;
                        for (int i = 0; i < index; i++)
                        {
                            oneCount += state[i];
                        }
                        for (int i = 0; i < oneCount; i++)
                        {
                            state[i] = 1;
                        }
                        for (int i = oneCount; i < index; i++)
                        {
                            state[i] = 0;
                        }
                        if (dele != null)
                        {
                            done = dele(state);
                        }
                        break;
                    }
                }
            }
        }


        public static int[] BestGroup(double[] blockPercent, double[] columnPercent)
        {
            int[] bestGroup = null;
            double lowerResult = 0;

            List<int> selectArray = new List<int>();
            for (int i = 0; i < columnPercent.Length; i++)
            {
                if (i < blockPercent.Length)
                {
                    selectArray.Add(1);
                }
                else
                {
                    selectArray.Add(0);
                }
            }
            bestGroup = selectArray.ToArray();
            lowerResult = CalcAbs(blockPercent, columnPercent, selectArray);

            double tempResult = 0.0;
            bool flag = true;
            while (flag)
            {
                for (int index = 1; index < selectArray.Count; index++)
                {
                    if (selectArray[index - 1] == 1 && selectArray[index] == 0)
                    {
                        selectArray[index - 1] = 0;
                        selectArray[index] = 1;
                        RePreInit(selectArray, index - 1);
                        if (Verify(selectArray))
                        {
                            flag = false;
                            break;
                        }
                        if (selectArray[0] != 0)
                        {
                            tempResult = CalcAbs(blockPercent, columnPercent, selectArray);
                            if (lowerResult > tempResult)
                            {
                                bestGroup = selectArray.ToArray();
                                lowerResult = tempResult;
                            }
                        }
                        break;
                    }
                }
            }
            return bestGroup;
        }
        private static double CalcAbs(double[] blockPercent, double[] columnPercent, List<int> selectArray)
        {
            double result = 0.0;
            double tempblockPercent = 0.0;

            int blockIndex = blockPercent.Length - 1;
            for (int i = columnPercent.Length - 1; i >= 0; i--)
            {
                if (selectArray[i] == 1)
                {
                    tempblockPercent += columnPercent[i];
                    result += Math.Abs(tempblockPercent - blockPercent[blockIndex]);
                    tempblockPercent = 0.0;
                    blockIndex--;
                }
                else
                {
                    tempblockPercent += columnPercent[i];
                }
            }
            return result;
        }
        private static void RePreInit(List<int> selectArray, int changeLoction)
        {
            int zeroIndex = -1;
            for (int index = 0; index < changeLoction; index++)
            {
                if (selectArray[index] == 0)
                {
                    zeroIndex = index;
                }
                else
                {
                    break;
                }
            }
            if (zeroIndex != -1)
            {
                selectArray.RemoveRange(0, zeroIndex + 1);
                int firstZeroIndex = -1;
                for (int index = 0; index < selectArray.Count; index++)
                {
                    if (selectArray[index] == 0)
                    {
                        firstZeroIndex = index;
                        break;
                    }
                }
                for (int index = 0; index < zeroIndex + 1; index++)
                {
                    selectArray.Insert(firstZeroIndex, 0);
                }

            }
        }
        private static bool Verify(List<int> selectArray)
        {
            bool verifyFlag = true;
            for (int index = 0; index < selectArray.Count - 1; index++)
            {
                if (selectArray[index] == 1 && selectArray[index + 1] == 0)
                {
                    verifyFlag = false;
                    break;
                }
            }
            return verifyFlag;
        }

    }

}
