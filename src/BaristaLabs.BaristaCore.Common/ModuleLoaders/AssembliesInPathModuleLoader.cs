﻿namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module loader that scans assemblies in a specified folder for all IBaristaModule implementations.
    /// </summary>
    public class AssembliesInPathModuleLoader : IBaristaModuleLoader, IDisposable
    {
        private readonly string m_moduleFolderFullPath;
        private readonly ServiceCollection m_serviceCollection;
        private readonly Dictionary<string, Type> m_loadedModules;

        public AssembliesInPathModuleLoader(string modulePath = null)
        {
            if (string.IsNullOrWhiteSpace(modulePath))
                modulePath = "barista_modules";

            if (Path.IsPathRooted(modulePath))
                m_moduleFolderFullPath = modulePath;
            else
                m_moduleFolderFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modulePath);

            m_loadedModules = new Dictionary<string, Type>();
            m_serviceCollection = new ServiceCollection();
        }

        public Task<IBaristaModule> GetModule(string name)
        {
            //Look for any already loaded modules
            if (TryGetModuleByName(name, out IBaristaModule baristaModule))
            {
                return Task.FromResult(baristaModule);
            }

            //If not found scan the configured folder.
            ScanModuleFolder();

            //Try locating it again.
            if (TryGetModuleByName(name, out IBaristaModule newBaristaModule))
            {
                return Task.FromResult(newBaristaModule);
            }

            //Couldn't find one? Get outta Dodge.
            return null;
        }

        /// <summary>
        /// Scans the configured folder for assemblies.
        /// </summary>
        private void ScanModuleFolder()
        {
            var moduleFolderPathInfo = new DirectoryInfo(m_moduleFolderFullPath);
            if (!moduleFolderPathInfo.Exists)
                return;

            //Add our custom assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += ModuleAssemblyLoader;
            try
            {
                foreach (var assemblyFileInfo in moduleFolderPathInfo.GetFileSystemInfos("*.dll"))
                {
                    foreach(var moduleType in BaristaModuleTypeLoader.LoadBaristaModulesFromAssembly(assemblyFileInfo.FullName))
                    {
                        var baristaModuleAttribute = BaristaModuleAttribute.GetBaristaModuleAttributeFromType(moduleType);

                        if (baristaModuleAttribute.IsDiscoverable == false)
                            continue;

                        string targetModuleName = baristaModuleAttribute.Name;

                        if (string.IsNullOrWhiteSpace(targetModuleName))
                            throw new InvalidOperationException($"The specfied module ({moduleType}) must indicate a name.");

                        m_serviceCollection.AddTransient(typeof(IBaristaModule), moduleType);

                        if (m_loadedModules.ContainsKey(targetModuleName))
                            throw new InvalidOperationException($"A module with the specified name ({targetModuleName}) has already been registered. ({m_loadedModules[targetModuleName]})");

                        m_loadedModules.Add(targetModuleName, moduleType);
                    }
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= ModuleAssemblyLoader;
            }
        }

        private bool TryGetModuleByName(string moduleName, out IBaristaModule baristaModule)
        {
            baristaModule = null;
            if (m_loadedModules.ContainsKey(moduleName))
            {
                var moduleType = m_loadedModules[moduleName];
                using (var serviceProvider = m_serviceCollection.BuildServiceProvider())
                {
                    var modules = serviceProvider.GetServices<IBaristaModule>();
                    baristaModule = modules.Where(s => s.GetType() == moduleType).FirstOrDefault();
                }
            }

            return baristaModule != null;
        }

        /// <summary>
        /// Callback when assembly load events are fired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly ModuleAssemblyLoader(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.FullName == args.Name);

            if (assembly != null)
                return assembly;

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            var filename = args.Name.Split(',')[0] + ".dll".ToLower();

            var moduleAssemblyPath = Path.Combine(m_moduleFolderFullPath, filename);

            try
            {
                return Assembly.LoadFile(moduleAssemblyPath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region IDisposable Support
        private bool m_isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    //Do Nothing.
                }

                m_loadedModules.Clear();
                m_serviceCollection.Clear();
                m_isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
