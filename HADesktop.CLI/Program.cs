using HADesktop;
using Splat;

var app = new HaDesktopApp();
await app.Startup();
ManualResetEvent mre = new(false);
Console.CancelKeyPress += (s, e) => { mre.Set(); e.Cancel = true; };
AppDomain.CurrentDomain.DomainUnload += (s, e) => { mre.Set(); while (!mre.WaitOne(1000)) { Thread.Sleep(500); } };

mre.WaitOne();
await app.Shutdown();
mre.Reset();