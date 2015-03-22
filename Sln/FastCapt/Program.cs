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
            using (var compositionContainer = Container.Current)
            {
                var application = compositionContainer.GetExportedValue<IApplication>();
                application.Start();
            }
        }
    }

    internal class Container
    {
        private static CompositionContainer _current;

        public static CompositionContainer Current
        {
            get { return _current ?? (_current = InitCompositionContainer()); }
        }

        private static CompositionContainer InitCompositionContainer()
        {
            return new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
        }
    }
}
