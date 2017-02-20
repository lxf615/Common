using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generic
{
    /// <summary>
    ///   公民身份号码是特征组合码,由十七位数字本体码和一位数字校验码组成.排列顺序从左至右依次为:
    /// 六位数字地址码,八位数字出生日期码,三位数字顺序码和一位数字校验码。
    /// 1、地址码：表示编码对象常住户口所在县（市、旗、区）的行政区划代码，按 GB/T 2260 的规定执行。
    /// 2、出生日期码：表示编码对象出生的年、月、日，按/// GB/T 7408 的规定执行。年、月、日代码之间不用分隔符。
    /// 例：某人出生日期为 1966年10月26日，其出生日期码为 19661026。
    /// 3、顺序码：表示在同一地址码所标识的区域范围内，
    /// 对同年、同月、同日出生的人编定的顺序号，顺序码的奇数分配给男性，偶数千分配给女性。
    /// 4、校验码：校验码采用ISO 7064：1983，MOD 11-2 校验码系统。
    /// （1）十七位数字本体码加权求和公式
    /// S = Sum(Ai/// Wi), i =/// 0, ... , 16 ，先对前17位数字的权求和
    /// Ai:表示第i位置上的身份证号码数字值
    /// Wi:表示第i位置上的加权因子
    /// Wi: 7 9 10 5 8 4 2 1 6 3 7 9 10 5 8 4 2 1
    /// （2）计算模 Y = mod(S, 11)
    /// （3）通过模得到对应的校验码
    /// Y: 0 1 2 3 4 5 6 7 8 9 10
    /// 校验码: 1 0 X 9 8 7 6 5 4 3 2
    /// </summary>
    public class IDCard
    {
        // 加权因子
        private static List<int> weight = new List<int>() { 7, 9, 10, 5, 8, 4, 2, 1, 6,
            3, 7, 9, 10, 5, 8, 4, 2, 1 };
        // 校验码
        private static List<int> checkDigit = new List<int>() { 1, 0, 'X', 9, 8, 7, 6,
            5, 4, 3, 2 };

        public IDCard()
        {
        }
        /**
         * 验证身份证是否符合格式
         * @param idcard
         * @return
         */
        public bool Verify(string idcard)
        {
            if (idcard.Length == 15)
            {
                idcard = this.Update2Eighteen(idcard);
            }
            if (idcard.Length != 18)
            {
                return false;
            }
            //获取输入身份证上的最后一位，它是校验码
            String checkDigit = idcard.Substring(17, 1);
            //比较获取的校验码与本方法生成的校验码是否相等
            if (checkDigit.Equals(this.GetCheckDigit(idcard)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证身份证是否符合格式及出生日期是否正确
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="birthDay"></param>
        /// <returns></returns>
        public bool Verify(string idcard, DateTime? birthDay)
        {
            if (!Verify(idcard))
            {
                return false;
            }


            if (birthDay != null)
            {
                //检测出生日期
                if (!CheckBirthDay(idcard, birthDay.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckBirthDay(string idcard, DateTime birthDay)
        {
            string date = idcard.Substring(6, 8);
            if (date.Equals(birthDay.ToString("yyyyMMdd")))
            {
                return true;
            }
            return false;
        }


        /**
         * 计算18位身份证的校验码
         * @param eighteenCardID    18位身份证
         * @return
         */
        private String GetCheckDigit(String eighteenCardID)
        {
            int remaining = 0;
            if (eighteenCardID.Length == 18)
            {
                eighteenCardID = eighteenCardID.Substring(0, 17);
            }

            if (eighteenCardID.Length == 17)
            {
                int sum = 0;
                int[] a = new int[17];
                //先对前17位数字的权求和
                for (int i = 0; i < 17; i++)
                {
                    String k = eighteenCardID.Substring(i, 1);
                    a[i] = k.ToInt();
                }
                for (int i = 0; i < 17; i++)
                {
                    sum = sum + weight[i] * a[i];
                }
                //再与11取模
                remaining = sum % 11;
            }
            return remaining == 2 ? "X" : checkDigit[remaining].ToString();
        }

        /**
         * 将15位身份证升级成18位身份证号码
         * @param fifteenCardID
         * @return
         */
        private String Update2Eighteen(String fifteenCardID)
        {
            //15位身份证上的生日中的年份没有19，要加上
            String eighteenCardID = fifteenCardID.Substring(0, 6);
            eighteenCardID = eighteenCardID + "19";
            eighteenCardID = eighteenCardID + fifteenCardID.Substring(6, 9);
            eighteenCardID = eighteenCardID + this.GetCheckDigit(eighteenCardID);
            return eighteenCardID;
        }

    }
}