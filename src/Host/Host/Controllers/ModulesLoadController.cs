using CommonModule.DllCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesLoadController : ControllerBase
    {


        private readonly ILogger<ModulesLoadController> _logger;

        public ModulesLoadController(ILogger<ModulesLoadController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "uploadAssembly")]
        public List<string> Get()
        {
            List<string> result = new List<string>();

            foreach (var item in GlobalConfiguration.Modules)
            {
                if (string.IsNullOrEmpty(item.EntryClass) == true) continue;
                Type type = Type.GetType($"{item.Id}.{item.EntryClass},{item.Id}");
                object obj = Activator.CreateInstance(type); //创建此类型实例
                                                             
                //4.获取所有方法并调用
                MethodInfo[] mInfo = type.GetMethods(); //获取当前方法
                for (int i = 0; i < mInfo.Length; i++)
                {
                    result.Add($"程序集{item.Id}中包含方法{mInfo[i].Name}");
                }
            }


            // Debug.Log("调用AAA()方法: " + mInfo[0].Invoke(obj, new object[0]));
            // Debug.Log("调用BBB(int a,int b)方法: " + mInfo[1].Invoke(obj, new object[] { 10, 5 }));
            return result;

        }
    }
}
