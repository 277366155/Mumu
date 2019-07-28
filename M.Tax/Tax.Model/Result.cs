using System;
using System.Collections.Generic;
using System.Text;

namespace Tax.Model
{
    public class BaseResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessed { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }
    }

    public class Success : BaseResult
    {
         
        public Success()
        {
            IsSuccessed = true;
            Msg = "成功";
        }
        public Success(string msg)
        {
            IsSuccessed = true;
            Msg = msg;
        }
    }

    public class Fail : BaseResult
    {      
        public Fail()
        {
            Msg = "失败";
        }
        public Fail(string msg)
        {
            Msg = msg;
        }
    }
    public class Result<T> : BaseResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
    }

    public class SuccessResult<T> : Result<T>
    {
        public SuccessResult(T data)
        {
            IsSuccessed = true;
            Data = data;
            Msg = "成功";
        }
        public SuccessResult(T data, string msg)
        {
            IsSuccessed = true;
            Data = data;
            Msg = msg;
        }
    }

    public class FailResult<T> : Result<T>
    {
        public new bool IsSuccessed { get { return false; } }
        public new T Data { get { return default(T); } }
        public FailResult()
        {
            Msg = "失败";
        }
        public FailResult(string msg)
        {
            Msg = msg;
        }
    }
}
