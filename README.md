# Moleculer for Net


.NET Core 2.1 implementation of the [Moleculer microservices framework](http://moleculer.services/).
This is a work in progress aimed to be completely compatible with the NodeJS-based Moleculer (see Status below).


## Status

* Discovery, Heartbeat, Request-Reply and Response implemented
* Can join other moleculer nodes and answer for request calls made by any other services
* Can execute actions on other services via Request (similar to call in ServiceBroker)
* Has NATS protocol working, other protocols to be implemented

## Download

Clone or download this repository and open moleculer-net.sln solution. 
Packaging will be available soon


## Usage

This implementation is based on a .NET Core Generic Host (HostBuilder). The moleculer-net solution has a project 
named **HostApp** which is the startup project.

A service is any class with **ServiceAction** attribute. the Host scans all assemblies in the *plugin* folder to locate those classes.
By default the class name will be the service name and the method name will be the action name. The project **DummyActions** contains 
an example class **HelloService** to be used as example. It has a post build event to copy the assembly to HostApp's plugin folder so
an easy way to try is just to create another classes inside DummyActions project, rebuild and run.

So, for example

```csharp
[ServiceAction]
public class HelloService {

    public int Method1(int a, int b){
            return a + b;
    }
}
```

Will create a service named *HelloService.Method1* that can be called from a nodejs service as

```javascript
ctx.call("HelloService.Method1",{a:1,b:2});
```
where *ctx* is the reference to Service Broker.

## Call other services from .NET service class

Moleculer-NET has a *Context* class which has a method *call*. This context is created by the Host and 
injected in the services and cannot be created by the service itself. 

So, in order to call another service from a .NET service, the service class has to implement a constructor
that receives a Context object as parameter, as the example bellow.

```csharp
[ServiceAction]
public class HelloService {

   private Context _context;

   public HelloService(Context ctx){
      _context = ctx;
   }

   public async Task<int> Method2() {
      var result = await _context.Call("service2.hello", null, null);
      // do something
   }
}
```

# Documentation
Developer documentation is in progress. 

# License
Moleculer-NET is available under the [MIT license](https://tldrlegal.com/license/mit-license).