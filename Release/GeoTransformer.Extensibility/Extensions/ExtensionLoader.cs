/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using MessageBoxDefaultButton = System.Windows.Forms.MessageBoxDefaultButton;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using MessageBoxOptions = System.Windows.Forms.MessageBoxOptions;

namespace GeoTransformer.Extensions
{
    /// <summary>
    /// Class that handles the loading of all extensions. Also holds the singleton instances of any loaded extensions.
    /// </summary>
    public static class ExtensionLoader
    {
        #region [ Helper methods ]

        /// <summary>
        /// The locking primitive for the ExtensionLoader class.
        /// </summary>
        private static object SyncRoot = new object();

        /// <summary>
        /// Returns a collection of all types that are currently loaded from the given assemblies. This catches <see cref="System.Reflection.ReflectionTypeLoadException"/> and returns only types that can be loaded.
        /// </summary>
        private static IEnumerable<Type> ReadAllLoadedTypes(this IEnumerable<System.Reflection.Assembly> assemblies)
        {
            if (assemblies == null)
                yield break;

            foreach (var ass in assemblies)
            {
                Type[] types;
                try
                {
                    types = ass.GetTypes();
                }
                catch (System.Reflection.ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }
                foreach (var t in types)
                    if (t != null)
                        yield return t;
            }
        }

        private static IEnumerable<System.Reflection.Assembly> OpenExtensionAssemblies()
        {
            // return the GeoTransformer.Extensibility assembly
            yield return typeof(IExtension).Assembly;

            var path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Extensions");
            if (!System.IO.Directory.Exists(path))
                yield break;

            foreach (var folder in System.IO.Directory.EnumerateDirectories(path, "*", System.IO.SearchOption.TopDirectoryOnly))
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(folder, "pending_uninstall")))
                {
                    try
                    {
                        System.IO.Directory.Delete(folder, true);
                    }
                    catch (System.IO.IOException)
                    {
                    }

                    continue;
                }

                foreach (var dll in System.IO.Directory.EnumerateFiles(folder, "*.dll", System.IO.SearchOption.TopDirectoryOnly))
                {
                    System.Reflection.Assembly assembly = null;
                    try
                    {
                        assembly = AppDomain.CurrentDomain.Load(System.Reflection.AssemblyName.GetAssemblyName(dll));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to load the extension from " + System.IO.Path.GetFileName(dll) + "." + Environment.NewLine + Environment.NewLine + ex.Message);
                    }

                    if (assembly != null)
                    {
                        yield return assembly;
                    }
                }
            }
        }

        private static IEnumerable<Type> RetrieveExtensionTypes(Type extensionBase)
        {
            return OpenExtensionAssemblies()
                .ReadAllLoadedTypes()
                .Where(o => !o.IsAbstract && !o.IsGenericTypeDefinition && extensionBase.IsAssignableFrom(o))
                .Where(o => !typeof(ISpecial).IsAssignableFrom(o))
                .ToList();
        }

        /// <summary>
        /// Gets the local storage path for a given extension. If the path does not exist, creates it.
        /// </summary>
        /// <param name="extension">The extension type that requires local storage.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">when <paramref name="extension"/> is <c>null</c></exception>
        /// <remarks><see cref="ILocalStorage"/> interface should be used when possible. This method is created to
        /// allow special extensions (marked with <see cref="ISpecial"/>) to use local storage since for them
        /// the interface property is not initialized.</remarks>
        public static string GetLocalStoragePath(Type extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            var path = System.IO.Path.Combine(new System.IO.FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName, "ExtensionData", extension.Namespace);
            System.IO.Directory.CreateDirectory(path);
            return path;
        }

        #endregion

        #region [ Application service ]

        /// <summary>
        /// Gets the application service object that can be used to interact with the main application form.
        /// </summary>
        public static IApplicationService ApplicationService { get; internal set; }

        #endregion

        #region [ Extensions ]

        private static SynchronizedReadOnlyCollection<IExtension> _Extensions;

        /// <summary>
        /// Gets a collection of all currently loaded extensions.
        /// </summary>
        public static SynchronizedReadOnlyCollection<IExtension> Extensions
        {
            get
            {
                if (_Extensions == null)
                    lock (SyncRoot)
                        if (_Extensions == null)
                            InitializeExtensions();

                return _Extensions;
            }
        }

        /// <summary>
        /// Retrieves the currently loaded extensions of the specified type.
        /// </summary>
        /// <typeparam name="TExtension">The type of the extension.</typeparam>
        /// <returns>Currently loaded extensions that can be assigned to the <typeparamref name="TExtension"/>.</returns>
        public static IEnumerable<TExtension> RetrieveExtensions<TExtension>()
            where TExtension : class
        {
            foreach (var x in Extensions)
            {
                var t = x as TExtension;
                if (t != null)
                    yield return t;
            }
        }

        private static void InitializeExtensions()
        {
            var types = RetrieveExtensionTypes(typeof(IExtension));

            var result = new List<IExtension>(types.Count());
            foreach (var t in types)
            {
                try
                {
                    var instance = (IExtension)System.Activator.CreateInstance(t);

                    var localStorage = instance as ILocalStorage;
                    if (localStorage != null)
                        localStorage.LocalStoragePath = GetLocalStoragePath(t);

                    result.Add(instance);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The extension '" + t.FullName + "' cannot be loaded." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Extension error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _Extensions = new SynchronizedReadOnlyCollection<IExtension>(new object(), result);

            // force the DB schema to be updated
            Data.TransformerSchema.TransformerSchema.Instance.GetType();
        }

        internal static void PersistExtensionConfiguration()
        {
            var table = Extensions.OfType<Extensions.ExtensionConfigurationTable>().Single();
            foreach (var instance in Extensions)
            {
                var configurable = instance as IConfigurable;
                if (configurable == null)
                    continue;

                var q = table.Replace();
                q.Value(o => o.ClassName, instance.GetType().FullName);
                q.Value(o => o.Configuration, configurable.SerializeConfiguration());
                q.Execute();
            }
        }

        #endregion
    }
}
