using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetAppointment.Lib.App.Model
{
    public class ReturnModel<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string RedirectURL { get; set; }
        public bool PageRefresh { get; set; }
        public string[] Errors { get; set; }

        public static ReturnModel<T> SuccessResponse(string message,T data,string redirectUrl = null, bool pageRefresh = false)
        {
            ReturnModel<T> returnModel = new ReturnModel<T>();
            returnModel.IsSuccess = true;
            returnModel.Message = message;
            returnModel.Data = data;
            returnModel.RedirectURL = redirectUrl;
            returnModel.PageRefresh = pageRefresh;
            return returnModel;
        }
        
        public static ReturnModel<T> FailureResponse(string message, T data = default, string redirectUrl = null, bool pageRefresh = false)
        {
            ReturnModel<T> returnModel = new ReturnModel<T>();
            returnModel.IsSuccess = false;
            returnModel.Message = message;
            returnModel.Data = data;
            returnModel.RedirectURL = redirectUrl;
            returnModel.PageRefresh = pageRefresh;
            return returnModel;
        }
    }
    public class ReturnModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public string RedirectURL { get; set; }
        public bool PageRefresh { get; set; }
        public string[] Errors { get; set; }

        public static ReturnModel SuccessResponse(string message, object data = null, string redirectUrl = null, bool pageRefresh = false)
        {
            ReturnModel returnModel = new ReturnModel();
            returnModel.IsSuccess = true;
            returnModel.Message = message;
            returnModel.Data = data;
            returnModel.RedirectURL = redirectUrl;
            returnModel.PageRefresh = pageRefresh;
            return returnModel;
        }

        public static ReturnModel FailureResponse(string message,object data = null, string redirectUrl = null, bool pageRefresh = false)
        {
            ReturnModel returnModel = new ReturnModel();
            returnModel.IsSuccess = false;
            returnModel.Message = message;
            returnModel.Data = data;
            returnModel.RedirectURL = redirectUrl;
            returnModel.PageRefresh = pageRefresh;
            return returnModel;
        }
    }


}
