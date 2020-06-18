using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing
{
    public class PriceConveter
    {
        #region DecToChina2 ���������ߡ�2009��1313��

        private static readonly string[] numstr = { "", "��", "Ҽ", "��", "��", "��", "��", "½", "��", "��", "��", "ʰ" };
        private static readonly string[] units = new string[] { "Ǫ", "��", "ʰ", "" };

        /// <summary>
        /// ���������ߡ�2009��1313��
        /// </summary>
        /// <param name="number"></param>
        /// <param name="cntype">����</param>
        /// <returns></returns>
        public static string DecToChina2(decimal number, int cntype)
        {
            string integerPart = "", fractionPart = "";
            string strMinus = CheckMinus(ref number);
            if (Convert.ToString(number) == null)
            {
                return "0";
            }
            else
            {
                SqliteNumber(number, cntype, ref integerPart, ref fractionPart);

                string integerResult = DealWithIntegerPart(integerPart, true, 10);

                string fractionResult = DealWithFractionPart(fractionPart);

                if (fractionPart == null)
                {
                    return string.Format("{0}{1}Ԫ", strMinus, integerResult);
                }
                else
                {
                    if (integerResult == "��")
                    {
                        return fractionResult;
                    }
                    else
                    {
                        return string.Format("{0}{1}Ԫ{2}", strMinus, integerResult, fractionResult);
                    }
                }
            }
        }
        #endregion

        #region DecToChina3 ���������ߡ�2009��1049��

        /// <summary>
        /// ���������ߡ�2009��1049��
        /// </summary>
        /// <param name="number"></param>
        /// <param name="cntype">����</param>
        /// <returns></returns>
        public static string DecToChina3(decimal number, int cntype)
        {
            string integerPart = "", fractionPart = "";
            string strMinus = CheckMinus(ref number);
            if (Convert.ToString(number) == null)
            {
                return "0";
            }
            else
            {
                SqliteNumber(number, cntype, ref integerPart, ref fractionPart);

                string integerResult = DealWithIntegerPart(integerPart, false, -1);

                string fractionResult = DealWithFractionPart(fractionPart);
                integerResult = strMinus + integerResult;
                if (string.IsNullOrEmpty(fractionPart))
                {
                    return integerResult + "Ԫ";
                }
                else
                {
                    if (integerResult == "��")
                    {
                        return fractionResult;
                    }
                    else
                    {
                        return integerResult + "Ԫ" + fractionResult;
                    }
                }
            }
        }

        private static string DealWithIntegerPart(string integerPart, bool fillZero, int fixLength)
        {
            string result = string.Empty;
            string hundredMillionString = string.Empty;
            string tenThousandString = string.Empty;
            string oneString = string.Empty;

            integerPart = integerPart.PadLeft(12, '0'); // ������λ

            string hundredMillionNumber = integerPart.Substring(0, integerPart.Length - 8);
            string tenThousandNumber = (integerPart.Substring(integerPart.Length - 8, 8)).Substring(0, 4);
            string oneNumber = integerPart.Substring(integerPart.Length - 4, 4);

            int fixIndex = integerPart.Length - fixLength;
            string[] hundredMillionSurrfix = fillZero ? new string[] { "(��)", "(��)", "(��)", "" } : new string[4];
            string[] tenThousandSurrfix = fillZero ? new string[] { "(��)", "(��)", "(��)", "" } : new string[4];

            hundredMillionString = DealFirstSection(hundredMillionNumber, fillZero, fixIndex, hundredMillionSurrfix);
            tenThousandString = DealWithLaterSection(tenThousandNumber, fillZero, tenThousandSurrfix);
            oneString = FullyDealWithLaterSection(oneNumber);

            if (string.IsNullOrEmpty(hundredMillionString) || hundredMillionString == "��")
            {
                hundredMillionString = string.Empty;
            }
            else
            {
                hundredMillionString += "��";
            }

            if ((string.IsNullOrEmpty(tenThousandString) || tenThousandString == "��") &&
                string.IsNullOrEmpty(hundredMillionString))
            {
                tenThousandString = string.Empty;
            }
            else
            {
                if (!fillZero && tenThousandString.EndsWith("��"))
                    tenThousandString = tenThousandString.TrimEnd('��');

                tenThousandString += "��";
            }

            result = hundredMillionString + tenThousandString + oneString;
            result = result.TrimStart('��').TrimEnd('��');

            return result;
        }

        private static string DealWithFractionPart(string fractionPart)
        {
            string fractionResult = string.Empty;
            string[] n = new string[100];
            if (fractionPart != null)//ת��С������
            {
                if (fractionPart.Length >= 2)
                {
                    fractionPart = Convert.ToString(Math.Round(Convert.ToDouble(("0" + "." + fractionPart)), 2));
                    fractionPart = fractionPart + "0";
                    if (fractionPart.Substring(2, 1) == "0")
                    {
                        n[1] = "��" + "��";
                    }
                    else
                    {
                        n[1] = numstr[(Convert.ToInt16(fractionPart.Substring(2, 1))) + 1] + "��";
                    }
                    if (fractionPart.Substring(3, 1) == "0")
                    {
                        n[2] = "��" + "��";
                    }
                    else
                    {
                        n[2] = numstr[(Convert.ToInt16(fractionPart.Substring(3, 1))) + 1] + "��";
                    }

                }
                else
                {
                    if (fractionPart.Substring(0, 1) == "0")
                    {
                        n[1] = "��" + "��";
                    }
                    else
                    {
                        n[1] = numstr[(Convert.ToInt16(fractionPart.Substring(0, 1))) + 1] + "��";
                        n[2] = null;
                    }
                    if (fractionPart.Length > 1)
                    {
                        if (fractionPart.Substring(1, 1) == "0")
                        {
                            n[2] = null;
                        }
                        else
                        {
                            n[2] = numstr[(Convert.ToInt16(fractionPart.Substring(1, 1))) + 1] + "��";
                        }
                    }
                }
                fractionResult = n[1] + n[2];
            }

            return fractionResult;
        }

        private static void SqliteNumber(decimal number, int cntype, ref string integerPart, ref string fractionPart)
        {
            //TODO �̶�ֻȡ��λ?Bug?
            number = (Math.Round(number, cntype != 0 ? 2 : 0));

            string numberString = Convert.ToString(number);

            if (numberString == null)
            {
                throw new OverflowException("���:ת��Ϊ����Ҵ�д����������λ�����17λ");
            }

            int pointpos = numberString.LastIndexOf(".");
            if (pointpos < 0)
            {
                integerPart = numberString;
                fractionPart = null;
            }
            else
            {
                integerPart = numberString.Substring(0, pointpos);
                fractionPart = numberString.Substring(pointpos + 1, numberString.Length - (pointpos + 1));

                if (Convert.ToDecimal(fractionPart) == 0)
                {
                    fractionPart = null;
                }
            }
        }

        private static string DealFirstSection(string curSection, bool fillZero, int fixIndex, string[] surrfix)
        {
            string res = string.Empty;
            bool foundChar = false;
            char lastChar = '\0';

            for (int i = 0; i < curSection.Length; i++)
            {
                char curChar = curSection[i];

                System.Diagnostics.Debug.Assert(Char.IsDigit(curChar), "�Ƿ��ַ�");

                int index = (int)(curChar - 48);
                if (curChar != '0' || (fillZero && (foundChar || i >= fixIndex)))
                {
                    foundChar |= curChar != '0';
                    res += numstr[index + 1] + units[i] + surrfix[i];
                }
                else
                {
                    if (lastChar != '0' && foundChar /* �ҵ������㣬����һ����Ϊ�� */)
                        res += numstr[index + 1];
                }
                lastChar = curChar;
            }
            return res;
        }

        private static string FullyDealWithLaterSection(string curSection)
        {
            string res = string.Empty;
            System.Diagnostics.Debug.Assert(curSection.Length == 4, "ÿһ�ڳ��ȱ���Ϊ4");

            for (int i = 0; i < curSection.Length; i++)
            {
                char curChar = curSection[i];
                System.Diagnostics.Debug.Assert(Char.IsDigit(curChar), "�Ƿ��ַ�");

                int index = (int)(curChar - 48);
                if (index == 0)
                {
                    if (!res.EndsWith("��"))
                        res += numstr[index + 1];
                }
                else
                    res += numstr[index + 1] + units[i];
            }

            return res;
        }

        private static string DealWithLaterSection(string curSection, bool fillZero, string[] surrfix)
        {
            string res = string.Empty;
            System.Diagnostics.Debug.Assert(curSection.Length == 4, "ÿһ�ڳ��ȱ���Ϊ4");
            System.Diagnostics.Debug.Assert(surrfix.Length == 4, "��׺���ȱ���Ϊ4");

            char lastChar = '\0';

            for (int i = 0; i < curSection.Length; i++)
            {
                char curChar = curSection[i];
                System.Diagnostics.Debug.Assert(Char.IsDigit(curChar), "�Ƿ��ַ�");

                int index = (int)(curChar - 48);
                if (curChar != '0' || fillZero)
                    res += numstr[index + 1] + units[i] + surrfix[i];
                else if (lastChar != '0')
                    res += numstr[index + 1]; // ������λ��ʾһ����
                lastChar = curChar;
            }

            return res;
        }

        #endregion ���������ߡ�2009��1049��
        private static string CheckMinus(ref decimal number)
        {
            if (number < 0)
            {
                number = Math.Abs(number);
                return "��";
            }
            return "";
        }
        #region ��ֲPB�Ľ��ת�����벻����� DecToChina
        /// <summary>
        /// �����д��ʽ
        /// </summary>
        /// <param name="number"></param>
        /// <param name="cntype">����</param>
        /// <returns></returns>
        public static string DecToChina(decimal number, int cntype)
        {
            string strMinus = CheckMinus(ref number);
            string str1 = "", str2 = "", intstr = "", decstr = "";
            string[] num = new string[8];
            string[] n = new string[100];
            string number1, number2, number3;
            int pointpos;
            bool check = true;
            if (Convert.ToString(number) == null)
            {
                number = 0;
                return Convert.ToString(number);
            }
            else
            {
                if (check)
                {
                    if (cntype == 0)
                    {
                        number = (Math.Round(number, 0));
                    }
                    else
                    {
                        number = (Math.Round(number, 2));
                    }
                    {
                        if (Convert.ToString(number) == null)
                        {
                            //Function.Alert("ת��Ϊ����Ҵ�д����������λ�����17λ!", "����");
                            return "���";
                        }
                        else
                        {
                            //pointpos = Convert.ToString(number).Substring((Convert.ToString(number)).IndexOf('.')).Length - 1;
                            pointpos = Convert.ToString(number).LastIndexOf(".");
                            if (pointpos < 0)
                            {
                                str1 = Convert.ToString(number);
                                str2 = null;
                            }
                            else
                            {
                                str1 = (Convert.ToString(number)).Substring(0, pointpos);
                                str2 = (Convert.ToString(number)).Substring(pointpos + 1, (Convert.ToString(number)).Length - (pointpos + 1));
                                if (Convert.ToDecimal(str2) == 0)
                                {
                                    str2 = null;
                                }
                            }
                        }
                    }
                }
                else//ת��Ϊ���ֱ��ʽ
                {
                    for (int decpos = 1; decpos <= Convert.ToString(str2).Length; )
                    {
                        n[decpos] = numstr[(Convert.ToInt16(str2.Substring(decpos, 1))) + 1];
                        decstr = decstr + n[decpos];
                    }
                }
                if (str1.Length > 8)//����ת��
                {
                    num[1] = str1.Substring(0, str1.Length - 8);
                    num[2] = (str1.Substring(str1.Length - 8, 8)).Substring(0, 4);
                    num[3] = str1.Substring(str1.Length - 4, 4);
                    switch (num[1].Length)
                    {
                        case 1:
                            num[1] = "000" + num[1];
                            break;
                        case 2:
                            num[1] = "00" + num[1];
                            break;
                        case 3:
                            num[1] = "0" + num[1];
                            break;
                    }
                    //if(num[1].Length=3)
                    //{num[1]=0+num[1];}
                    //if(num[1].Length=2)
                    //{num[1]=00+num[1];}
                    //if(num[1].Length=1)
                    //{num[1]=000+num[1];}
                    number1 = null;
                    number2 = null;
                    if (num[1].Substring(0, 1) == "0")
                    {
                        n[1] = null;
                    }
                    else
                    {
                        n[1] = numstr[(Convert.ToInt16(num[1].Substring(0, 1))) + 1] + "Ǫ";
                    }
                    if (num[1].Substring(1, 1) == "0")
                    {
                        if (num[1].Substring(0, 1) == "0")
                        {
                            n[2] = null;
                        }
                        else
                        {
                            n[2] = numstr[1];
                        }
                    }
                    else
                    {
                        n[2] = numstr[(Convert.ToInt16(num[1].Substring(1, 1))) + 1] + "��";
                    }
                    if (num[1].Substring(2, 1) == "0")
                    {
                        if (num[1].Substring(1, 1) == "0")
                        {
                            n[3] = null;
                        }
                        else
                        {
                            n[3] = numstr[1];
                        }
                    }
                    else
                    {
                        n[3] = numstr[(Convert.ToInt16(num[1].Substring(2, 1))) + 1] + "ʰ";
                    }
                    if (num[1].Substring(3, 1) == "0")
                    {
                        n[4] = null;
                    }
                    else
                    {
                        n[4] = numstr[(Convert.ToInt16(num[1].Substring(3, 1))) + 1];
                    }
                    number3 = n[1] + n[2] + n[3] + n[4];
                    intstr = number1 + number2 + number3;
                    number1 = intstr + "��";
                    if (num[2].Substring(0, 1) == "0")
                    { n[1] = null; }
                    else
                    {
                        n[1] = numstr[(Convert.ToInt16(num[2].Substring(0, 1))) + 1] + "Ǫ";
                    }
                    if (num[2].Substring(1, 1) == "0")
                    {
                        if (num[2].Substring(0, 1) == "0")
                        {
                            n[2] = null;
                        }
                        else if (num[2].Substring(2, 1) == "0" && (num[2].Substring(3, 1) == "0"))
                        {
                            n[2] = null;
                        }
                        else if (num[2].Substring(2, 1) != "0" || (num[2].Substring(3, 1) != "0"))
                        {
                            n[2] = numstr[1];
                        }
                    }
                    else
                    {
                        n[2] = numstr[(Convert.ToInt16(num[2].Substring(1, 1))) + 1] + "��";
                    }
                    if (num[2].Substring(2, 1) == "0")
                    {
                        if (num[2].Substring(1, 1) == "0" && (num[2].Substring(0, 1) == "0"))
                        {
                            n[3] = null;
                        }
                        else if (num[2].Substring(3, 1) == "0")
                        {
                            n[3] = null;
                        }
                        else if ((num[2].Substring(3, 1) != "0"))
                        {
                            n[3] = numstr[1];
                        }
                    }
                    else
                    {
                        n[3] = numstr[(Convert.ToInt16(num[2].Substring(2, 1))) + 1] + "ʰ";
                    }
                    if (num[2].Substring(3, 1) == "0")
                    {
                        if ((num[2].Substring(2, 1) == "0" && (num[2].Substring(1, 1) == "0") && (num[2].Substring(0, 1) == "0")))
                        {
                            n[4] = numstr[1];
                        }
                        else
                        {
                            n[4] = null;
                        }
                    }
                    else
                    {
                        n[4] = numstr[(Convert.ToInt16(num[2].Substring(3, 1))) + 1];
                    }
                    if (num[3].Substring(0, 1) == "0")
                    {
                        n[5] = numstr[1] + "Ǫ";
                    }
                    else
                    {
                        n[5] = numstr[(Convert.ToInt16(num[3].Substring(0, 1))) + 1] + "Ǫ";
                    }
                    if (num[3].Substring(1, 1) == "0")
                    {
                        if (num[3].Substring(0, 1) == "0")
                        {
                            n[6] = numstr[1] + "��";
                        }
                        else
                        {
                            n[6] = numstr[1] + "��";
                        }
                    }
                    else
                    {
                        n[6] = numstr[(Convert.ToInt16(num[3].Substring(1, 1))) + 1] + "��";
                    }
                    if (num[3].Substring(2, 1) == "0")
                    {
                        if (num[3].Substring(1, 1) == "0")
                        {
                            n[7] = numstr[1] + "ʰ";
                        }
                        else
                        {
                            n[7] = numstr[1] + "ʰ";
                        }

                    }
                    else
                    {
                        n[7] = numstr[(Convert.ToInt16(num[3].Substring(2, 1))) + 1] + "ʰ";
                    }
                    if (num[3].Substring(3, 1) == "0")
                    {
                        n[8] = numstr[1];
                    }
                    else
                    {
                        n[8] = numstr[(Convert.ToInt16(num[3].Substring(3, 1))) + 1];
                    }
                    number2 = n[1] + n[2] + n[3] + n[4] + "��";
                    number3 = n[5] + n[6] + n[7] + n[8];
                    intstr = number1 + number2 + number3;
                }
                else if (str1.Length >= 5 && str1.Length <= 8)
                {
                    //fractionPart = null; 
                    num[1] = str1.Substring(0, str1.Length - 4);
                    switch (num[1].Length)
                    {
                        case 1:
                            num[1] = "000" + num[1];
                            break;
                        case 2:
                            num[1] = "00" + num[1];
                            break;
                        case 3:
                            num[1] = "0" + num[1];
                            break;
                    }
                    num[2] = (str1.Substring(str1.Length - 4, 4)).Substring(0, 4);
                    number1 = null;
                    if (num[1].Substring(0, 1) == "0")
                    { n[1] = null; }
                    else
                    { n[1] = numstr[(Convert.ToInt16(num[1].Substring(0, 1))) + 1] + "Ǫ"; }
                    if (num[1].Substring(1, 1) == "0")
                    {
                        if (num[1].Substring(0, 1) == "0")
                        {
                            n[2] = null;
                        }
                        else
                        {
                            n[2] = numstr[1];
                        }
                    }
                    else
                    {
                        n[2] = numstr[(Convert.ToInt16(num[1].Substring(1, 1))) + 1] + "��";
                    }
                    if (num[1].Substring(2, 1) == "0")
                    {
                        if (num[1].Substring(1, 1) == "0")
                        {
                            n[3] = null;
                        }
                        else
                        {
                            n[3] = numstr[1];
                        }

                    }
                    else
                    {
                        n[3] = numstr[(Convert.ToInt16(num[1].Substring(2, 1))) + 1] + "ʰ";
                    }
                    if (num[1].Substring(3, 1) == "0")
                    {
                        n[4] = null;
                        if (n[3] == numstr[1])
                            n[3] = null;
                    }
                    else
                    {
                        n[4] = numstr[(Convert.ToInt16(num[1].Substring(3, 1))) + 1];
                    }
                    if (num[2].Substring(0, 1) == "0")
                    {
                        if (num[2].Substring(1, 1) != "0" || num[2].Substring(2, 1) != "0" || num[2].Substring(3, 1) != "0")
                        {
                            n[5] = numstr[1];
                        }
                        else
                        {
                            n[5] = null;
                        }
                    }
                    else
                    {
                        n[5] = numstr[(Convert.ToInt16(num[2].Substring(0, 1))) + 1] + "Ǫ";
                    }
                    if (num[2].Substring(1, 1) == "0")
                    {
                        if (num[2].Substring(0, 1) == "0")
                        {
                            n[6] = null;
                        }
                        else
                        {
                            if (num[2].Substring(2, 1) == "0" && num[2].Substring(3, 1) == "0")
                            {
                                n[6] = null;
                            }
                            else
                            {
                                n[6] = numstr[1];
                            }
                        }
                    }
                    else
                    {
                        n[6] = numstr[(Convert.ToInt16(num[2].Substring(1, 1))) + 1] + "��";
                    }
                    if (num[2].Substring(2, 1) == "0")
                    {
                        if (num[2].Substring(1, 1) == "0")
                        {
                            n[7] = null;
                        }
                        else
                        {
                            if (num[2].Substring(3, 1) == "0")
                            {
                                n[7] = null;
                            }
                            else
                            {
                                n[7] = numstr[1];
                            }
                        }
                    }
                    else
                    {
                        n[7] = numstr[(Convert.ToInt16(num[2].Substring(2, 1))) + 1] + "ʰ";
                    }
                    if (num[2].Substring(3, 1) == "0")
                    {
                        n[8] = null; ;
                    }
                    else
                    {
                        n[8] = numstr[(Convert.ToInt16(num[2].Substring(3, 1))) + 1];
                    }
                    number3 = n[5] + n[6] + n[7] + n[8];
                    number2 = n[1] + n[2] + n[3] + n[4] + "��";
                    intstr = number1 + number2 + number3;
                }
                else
                {
                    num[1] = str1;
                    switch (num[1].Length)
                    {
                        case 1:
                            num[1] = "000" + num[1];
                            break;
                        case 2:
                            num[1] = "00" + num[1];
                            break;
                        case 3:
                            num[1] = "0" + num[1];
                            break;
                    }
                    number1 = null;
                    number2 = null;
                    if (num[1].Substring(0, 1) == "0")
                    {
                        //n[1]=numstr[1]+"Ǫ";
                        n[1] = null;
                    }
                    else
                    {
                        n[1] = numstr[(Convert.ToInt16(num[1].Substring(0, 1))) + 1] + "Ǫ";
                    }
                    if (num[1].Substring(1, 1) == "0")
                    {
                        if (num[1].Substring(0, 1) == "0")
                        {
                            n[2] = null;
                        }
                        else
                        {
                            if (num[1].Substring(2, 1) == "0" && num[1].Substring(3, 1) == "0")
                            {
                                n[2] = null;
                            }
                            else
                            {
                                n[2] = numstr[1];
                            }
                        }
                    }
                    else
                    {
                        n[2] = numstr[(Convert.ToInt16(num[1].Substring(1, 1))) + 1] + "��";
                    }

                    if (num[1].Substring(2, 1) == "0")
                    {
                        if (num[1].Substring(1, 1) == "0")
                        {
                            n[3] = null;
                        }
                        else
                        {
                            if (num[1].Substring(3, 1) == "0")
                            {
                                n[3] = null;
                            }
                            else
                            {
                                n[3] = numstr[1];
                            }
                        }
                    }
                    else
                    {
                        n[3] = numstr[(Convert.ToInt16(num[1].Substring(2, 1))) + 1] + "ʰ";
                    }
                    if (num[1].Substring(3, 1) == "0")
                    {
                        if (num[1].Substring(1, 1) == "0" || num[1].Substring(2, 1) == "0")
                        {
                            n[4] = null;
                        }
                    }
                    else
                    {
                        n[4] = numstr[(Convert.ToInt16(num[1].Substring(3, 1))) + 1];
                    }
                    number3 = n[1] + n[2] + n[3] + n[4];
                    intstr = number1 + number2 + number3;
                }
                if (str1 == "0")
                {
                    intstr = numstr[1];
                }
                if (str2 != null)//ת��С������
                {
                    if (str2.Length >= 2)
                    {
                        str2 = Convert.ToString(Math.Round(Convert.ToDouble(("0" + "." + str2)), 2));
                        //if (fractionPart.Substring(1, 1) == "0")
                        str2 = str2 + "0";
                        if (str2.Substring(2, 1) == "0")
                        {
                            //n[1] = "��";
                            n[1] = null;
                        }
                        else
                        {
                            //n[1] = numstr[(Convert.ToInt16(fractionPart.Substring(1, 1))) + 1] + "��";
                            n[1] = numstr[(Convert.ToInt16(str2.Substring(2, 1))) + 1] + "��";
                        }
                        //if (fractionPart.Substring(2, 1) == "0")
                        if (str2.Substring(3, 1) == "0")
                        {
                            n[2] = null;
                        }
                        else
                        {//n[2] = numstr[(Convert.ToInt16(fractionPart.Substring(1, 1))) + 1] + "��";
                            n[2] = numstr[(Convert.ToInt16(str2.Substring(3, 1))) + 1] + "��";
                        }

                    }
                    else
                    {
                        if (str2.Substring(0, 1) == "0")
                        {
                            n[1] = "��";
                        }
                        else
                        {
                            n[1] = numstr[(Convert.ToInt16(str2.Substring(0, 1))) + 1] + "��";
                            //if (fractionPart.Substring(1, 1) == "0")
                            //{
                            n[2] = null;
                            //}
                            //else
                            //{
                            //  n[2] = numstr[(Convert.ToInt16(fractionPart.Substring(1, 1))) + 1] + "��";
                            //}
                        }
                    }
                    decstr = n[1] + n[2];
                }
                intstr = strMinus + intstr;
                if (check)
                {
                    if (str2 == null)
                    {
                        return intstr + "Ԫ";
                    }
                    else
                    {
                        if (intstr == "��")
                        {
                            return decstr;
                        }
                        else
                        {
                            return intstr + "Ԫ" + decstr;
                        }
                    }
                }
                else
                {
                    if (str2 == null)
                    {
                        return intstr;
                    }
                    else
                    {
                        return intstr + "��" + decstr;
                    }
                }



            }

        }
        #endregion
    }
}
