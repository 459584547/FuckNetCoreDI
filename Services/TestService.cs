using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using WebApplicationTest.Extension;

namespace WebApplicationTest.Services
{
    [Authorized]
    public class TestService
    {

        public TestService2 testService2 { get; set; }
        public ILog _logger { set; get; }
        public object x()
        {
            
            _logger.Error("xxx");
           return testService2.x2();
        }
        public void x2()
        {
            _logger.Error("xxx22");
        }
    }
}
