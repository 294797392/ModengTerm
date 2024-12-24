using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Base
{
    public class ResponseCode
    {
        public const int FAILED = -1;
        public const int SUCCESS = 0;
        public const int TIMEOUT = 1;
        public const int NOT_SUPPORTED = 2;
        public const int PRIVATE_KEY_NOT_FOUND = 3;
        public const int OPERATION_CANCEL = 4;

        public static string GetMessage(int code)
        {
            switch (code)
            {
                case ResponseCode.FAILED: return "失败";
                case ResponseCode.SUCCESS: return "成功";
                case ResponseCode.TIMEOUT: return "超时";
                case ResponseCode.NOT_SUPPORTED: return "不支持的操作";
                case ResponseCode.PRIVATE_KEY_NOT_FOUND: return "密钥不存在";
                case ResponseCode.OPERATION_CANCEL: return "操作被取消";

                default:
                    return "未知错误";
            }
        }
    }
}