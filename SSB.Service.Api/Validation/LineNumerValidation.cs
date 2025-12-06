using SSB.Service.Core;
using SSB.Service.SSBApi.Models.SendFromUrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSB.Service.SSBApi.Validation
{
    public class LineNumerValidation
    {
        #region props
        SMSService _service;
        #endregion
        #region ctors
        public LineNumerValidation()
        {
            _service = new SMSService();
        }
        #endregion
        #region public methods
        public string LineValidation(string[] fromNumber,string username) {
            if (!_service.CheckIsLinenumberOwner(fromNumber , username))
                return "20";
            return "";
        }
        #endregion
    }
}