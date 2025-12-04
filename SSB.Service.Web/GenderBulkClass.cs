using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using SSB.Helpers;
using SSB.Service.Web.GenderBulk;
using System.Data;
using SSB.Service.Web.GenderBulkPG;

namespace SSB.Service.Web
{
    public class GenderBulkClass
    {
            private string UserName = System.Configuration.ConfigurationManager.AppSettings["UserNameArmaghan"];
            private string Passwrod = System.Configuration.ConfigurationManager.AppSettings["PasswordArmaghan"];
        private string UserNamePG = System.Configuration.ConfigurationManager.AppSettings["UserNameArmaghanPG"];
        private string PasswrodPG = System.Configuration.ConfigurationManager.AppSettings["PasswordArmaghanPG"];
        private GenderBulk.BulkService gender = new BulkService();
        private GenderBulkPG.smscity genderPG = new smscity();
        public int getCountForGenderBulk(int selectedOperator, int ProvinceCode, int cityCode, int type, string PreFix, int Gender, int StartAge, int EndAge)
        {
            int count = 0;
            bool CountSp = false;
            try
            {
                switch (selectedOperator)
                {
                    case (int)Utility.Operator.Armaghan:
                        gender.getCountForExtendedBulk(UserName, Passwrod, ProvinceCode, cityCode, PreFix, type, Gender,
                            StartAge, EndAge, out count, out CountSp);
                        break;
                    case (int)Utility.Operator.PayamGostaran:
                        long prefixType = type;
                        long genderType = long.Parse(Gender.ToString());
                        count =
                            int.Parse(genderPG.CellCount(ProvinceCode, cityCode, ref prefixType, ref genderType,
                                StartAge.ToString(), EndAge.ToString(),
                                PreFix).ToString());
                        break;
                }
            }
            catch (Exception)
            {
                return -1;

            }
            return count;
        }
        public long SaveRequstForGenderBulk(int selectedOperator, long BulkNumber, string orig, string Content, int ProvincyCode, int CityCode, int Type, string PreFix, int Gender,
            int startAge, int EndAge, string dateTime, int StartIndex, int Count, string FirstNumber, string LastNumber, bool SortNumber)
        {
            long RequestNumber = -1;
            bool RequstSp;
            try
            {
                switch (selectedOperator)
                {
                    case (int)Utility.Operator.Armaghan:
                        gender.requestExtendedBulk(UserName, Passwrod, ProvincyCode, CityCode, PreFix, Type, Gender,
                            startAge, EndAge, StartIndex, Count, Content, orig, dateTime, FirstNumber,
                            LastNumber, SortNumber, BulkNumber, out RequestNumber, out RequstSp);
                        break;
                    case (int)Utility.Operator.PayamGostaran:
                        string result = genderPG.doSendCityBulk(UserNamePG, PasswrodPG, orig, Content, dateTime, StartIndex,
                            Count, SortNumber, Type, ProvincyCode, CityCode, PreFix, Gender, startAge, EndAge,
                            FirstNumber, LastNumber);
                        if (result.Contains("CHECK_OK"))
                        {
                            result = result.Replace("CHECK_OK ", "");
                            XmlDocument xm = new XmlDocument();
                            xm.LoadXml(result);
                            XmlNode anode = xm.SelectSingleNode("ReturnIDs");
                            result = anode.InnerText;
                        }
                        long errorCode = 0;
                        if (result.Contains("Error!"))
                        {
                            if (result.Contains("User is empty"))
                                errorCode = 11000101;
                            if (result.Contains("Password is empty"))
                                errorCode = 11000102;
                            if (result.Contains("Cellphone is empty"))
                                errorCode = 11000103;
                            if (result.Contains("Message is empty"))
                                errorCode = 11000104;
                            if (result.Contains("Expired account date"))
                                errorCode = 11000105;
                            if (result.Contains("Credit is not enough"))
                                errorCode = 11000106;
                            if (result.Contains("Authentication Failed"))
                                errorCode = 11000107;
                            if (result.Contains("Access denied"))
                                errorCode = 11000108;
                            if (result.Contains("Inactive user account"))
                                errorCode = 11000109;
                            if (result.Contains("Invalid orig Address"))
                                errorCode = 11000110;
                            if (result.Contains("Invalid arguments"))
                                errorCode = 11000111;
                            if (result.Contains("Arguments are empty"))
                                errorCode = 11000112;
                            if (result.Contains("Maximum amount of SMS in one batch exeeded"))
                                errorCode = 11000113;
                        }
                        if (errorCode != 0)
                            result = "-" + errorCode.ToString();
                        RequestNumber = long.Parse(result);
                        break;
                }
            }
            catch (Exception ex)
            {

                return -1;
            }
            return RequestNumber;
        }
        public DataTable GetCityGenderBulk(int selectedOperator, int ProvinceCode, out int ErrorCode)
        {
            ErrorCode = 0;
            bool SpProvince;
            city[] cities = null;


            DataTable dt = new DataTable();
            dt.Columns.Add("name");
            dt.Columns.Add("code");
            try
            {
                switch (selectedOperator)
                {
                    case (int)Utility.Operator.Armaghan:
                        gender.getCitiesOfProvinceForExtendedBulk(UserName, Passwrod, ProvinceCode, out ErrorCode, out SpProvince, out cities);
                        int count = 0;

                        foreach (city pro in cities)
                        {
                            DataRow dataRow;
                            dataRow = dt.NewRow();
                            dataRow["name"] = pro.name.ToString();
                            dataRow["code"] = pro.code.ToString();
                            dt.Rows.Add(dataRow);
                        }
                        break;
                    case (int)Utility.Operator.PayamGostaran:
                        string cityResult = genderPG.City(ProvinceCode);
                        cityResult = "<Cities>" + cityResult + "</Cities>";
                        string _ID = "";
                        string _Name = "";
                        using (XmlReader reader = XmlReader.Create(new StringReader(cityResult)))
                        {
                            while (reader.Read())
                            {
                                using (var fragmentReader = reader.ReadSubtree())
                                {
                                    if (fragmentReader.Read())
                                    {
                                        var fragment = XNode.ReadFrom(fragmentReader) as XElement;
                                        if (fragment != null)
                                        {
                                            var elements = fragment.Elements();
                                            foreach (var subFragment in elements)
                                            {
                                                var subElements = subFragment.Elements();
                                                if (subElements.Count() == 2)
                                                {
                                                    _ID = subElements.First().Value;
                                                    _Name = subElements.Last().Value;

                                                    if (_ID != "" && _Name != "")
                                                    {
                                                        DataRow dataRow;
                                                        dataRow = dt.NewRow();
                                                        dataRow["code"] = _ID;
                                                        dataRow["name"] = _Name;
                                                        dt.Rows.Add(dataRow);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                ErrorCode = -1;

            }
            return dt;
        }
        public DataTable GetPeovinceGenderBulk(int selectedOperator, out int ErrorCode)
        {
            ErrorCode = 0;
            bool SpProvince;
            province[] Province = null;

            DataTable dt = new DataTable();
            dt.Columns.Add("name");
            dt.Columns.Add("code");

            try
            {
                switch (selectedOperator)
                {
                    case (int)Utility.Operator.Armaghan:
                        gender.getProvincesForExtendedBulk(UserName, Passwrod, out ErrorCode, out SpProvince, out Province);
                        foreach (province pro in Province)
                        {
                            DataRow dataRow;
                            dataRow = dt.NewRow();
                            dataRow["name"] = pro.name.ToString();
                            dataRow["code"] = pro.code.ToString();
                            dt.Rows.Add(dataRow);
                        }
                        break;
                    case (int)Utility.Operator.PayamGostaran:
                        string provinceResult = genderPG.Province();
                        provinceResult = "<Provinces>" + provinceResult + "</Provinces>";
                        string _ID = "";
                        string _Name = "";
                        using (XmlReader reader = XmlReader.Create(new StringReader(provinceResult)))
                        {
                            while (reader.Read())
                            {
                                using (var fragmentReader = reader.ReadSubtree())
                                {
                                    if (fragmentReader.Read())
                                    {
                                        var fragment = XNode.ReadFrom(fragmentReader) as XElement;
                                        if (fragment != null)
                                        {
                                            var elements = fragment.Elements();
                                            foreach (var subFragment in elements)
                                            {
                                                var subElements = subFragment.Elements();
                                                if (subElements.Count() == 2)
                                                {
                                                    _ID = subElements.First().Value;
                                                    _Name = subElements.Last().Value;

                                                    if (_ID != "" && _Name != "")
                                                    {
                                                        DataRow dataRow;
                                                        dataRow = dt.NewRow();
                                                        dataRow["code"] = _ID;
                                                        dataRow["name"] = _Name;
                                                        dt.Rows.Add(dataRow);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                ErrorCode = -1;
            }
            return dt;
        }
        public DataTable GetBulkStatus(int selectedOperator, long RefId)
        {
            bulkStatus bulkStatusVar;
            DataTable dt = new DataTable();
            dt.Columns.Add("ErrorCode");
            dt.Columns.Add("totalSend");
            dt.Columns.Add("TotalDelivered");
            dt.Columns.Add("Status");
            long SentCount = 0;
            long DeliveryCount = 0;
            string ErroPG = "";
            string ResultPG = "";
            try
            {
                switch (selectedOperator)
                {
                    case (int)Utility.Operator.Armaghan:
                        bulkStatusVar = gender.getBulkStatus(UserName, Passwrod, RefId);
                        DataRow dr = dt.NewRow();
                        dr["ErrorCode"] = bulkStatusVar.errorCode;
                        dr["totalSend"] = bulkStatusVar.totalSent;
                        dr["TotalDelivered"] = bulkStatusVar.totalDelivered;
                        dr["Status"] = bulkStatusVar.bulkStatus1;
                        dt.Rows.Add(dr);
                        break;
                    case (int)Utility.Operator.PayamGostaran:

                        ErroPG = genderPG.doGetDelivery2(UserNamePG, RefId, ref SentCount, ref DeliveryCount);
                        ResultPG = genderPG.doGetDelivery(UserNamePG, RefId);
                        DataRow dr2 = dt.NewRow();
                        dr2["ErrorCode"] = ErroPG;
                        dr2["totalSend"] = SentCount;
                        dr2["TotalDelivered"] = DeliveryCount;
                        dr2["Status"] = ResultPG;
                        dt.Rows.Add(dr2);
                        break;
                }

            }
            catch (Exception)
            {

                dt = null;
                return dt;
            }
            return dt;
        }
        public void confirmBulkRequest(int selectedOperator, long?[] refId, out int errorCode, out int?[] result)
        {
            bool chk = true;
            switch (selectedOperator)
            {
                case (int)Utility.Operator.Armaghan:
                    gender.confirmBulkRequest(UserName, Passwrod, refId, out errorCode, out chk, out result);
                    break;
                case (int)Utility.Operator.PayamGostaran:
                    genderPG.doApprove(UserNamePG, refId[0].Value, true);
                    break;
            }

            errorCode = 0;
            result = new int?[] { };
        }
        public void getRequestPrice(long RefId, out int errorcode, out double Price)
        {
            bool chk = true;
            gender.getRequestPrice(UserName, Passwrod, RefId, out errorcode, out chk, out Price, out chk);
        }
    }
}
