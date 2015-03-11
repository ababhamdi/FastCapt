using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using FastCapt.ApplicationModel;

namespace FastCapt
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var compositionContainer = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly())))
            {
                var application = compositionContainer.GetExportedValue<IApplication>();
                application.Start();
            }
        }
    }
}
