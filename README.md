# FuckNetCoreDI
.net5 +Autofac  属性注入，干掉万恶的构造方法注入，集成redis+log4net+日志/异常中间件

Controller 和service中再也 不需要构造函数了
日志记录 直接添加一个属性就可以 public ILog _logger { set; get; }
