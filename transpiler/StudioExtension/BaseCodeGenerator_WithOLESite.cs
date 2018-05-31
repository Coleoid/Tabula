using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using VSLangProj;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace Tabula
{
    /// <summary> Base code generator with OLE site implementation </summary>
    public abstract class BaseCodeGenerator_WithOLESite : BaseCodeGenerator, VSOLE.IObjectWithSite
    {

        #region IObjectWithSite Members

        /// <summary> SetSite method of IOleObjectWithSite </summary>
        /// <param name="pUnkSite">site for this object to use</param>
        void VSOLE.IObjectWithSite.SetSite(object pUnkSite)
        {
            site = pUnkSite;
            codeDomProvider = null;
            serviceProvider = null;
        }
        private object site = null;

        /// <summary>GetSite method of IOleObjectWithSite</summary>
        /// <param name="riid">interface to get</param>
        /// <param name="ppvSite">IntPtr in which to stuff return value</param>
        void VSOLE.IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (site == null)
                throw new COMException("object is not sited", VSConstants.E_FAIL);

            IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(site);
            IntPtr intPointer = IntPtr.Zero;
            Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);

            if (intPointer == IntPtr.Zero)
                throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);

            ppvSite = intPointer;
        }

        #endregion

        /// <summary>Demand-creates a ServiceProvider</summary>
        private ServiceProvider SiteServiceProvider
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
                if (serviceProvider == null)
                {
                    serviceProvider = new ServiceProvider(site as VSOLE.IServiceProvider);
                    Debug.Assert(serviceProvider != null, "Unable to get ServiceProvider from site object.");
                }
                return serviceProvider;
            }
        }
        private ServiceProvider serviceProvider = null;

        /// <summary>Get a service by its GUID</summary>
        /// <param name="serviceGuid">GUID of service to retrieve</param>
        /// <returns>An object that implements the requested service</returns>
        protected object GetService(Guid serviceGuid)
        {
            ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
            return SiteServiceProvider.GetService(serviceGuid);
        }

        /// <summary>
        /// Pass-through to the SiteServiceProvider
        /// </summary>
        /// <param name="serviceType">Type of service to retrieve</param>
        /// <returns>An implementation of the requested service</returns>
        protected object GetService(Type serviceType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
            return SiteServiceProvider.GetService(serviceType);
        }

        /// <summary> Lazy init a C# CodeDomProvider </summary>
        /// <returns> This code generator's CodeDomProvider instance </returns>
        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (codeDomProvider == null)
                codeDomProvider = CodeDomProvider.CreateProvider("C#");

            return codeDomProvider;
        }
        private CodeDomProvider codeDomProvider = null;

        /// <summary>
        /// Gets the default extension of the output file from the CodeDomProvider
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            CodeDomProvider codeDom = GetCodeProvider();
            string extension = codeDom.FileExtension;
            if (extension != null && extension.Length > 0)
            {
                extension = "." + extension.TrimStart(".".ToCharArray());
            }
            return extension;
        }

        /// <summary>
        /// Returns the EnvDTE.ProjectItem object that corresponds to the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>The EnvDTE.ProjectItem of the project item the code generator was called on</returns>
        protected ProjectItem GetProjectItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
            object p = GetService(typeof(ProjectItem));
            Debug.Assert(p != null, "Unable to get Project Item.");
            return (ProjectItem)p;
        }

        /// <summary>
        /// Returns the EnvDTE.Project object of the project containing the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>
        /// The EnvDTE.Project object of the project containing the project item the code generator was called on
        /// </returns>
        protected Project GetProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
            return GetProjectItem().ContainingProject;
        }

        /// <summary>
        /// Returns the VSLangProj.VSProjectItem object that corresponds to the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>The VSLangProj.VSProjectItem of the project item the code generator was called on</returns>
        protected VSProjectItem GetVSProjectItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
            return (VSProjectItem)GetProjectItem().Object;
        }

        /// <summary>
        /// Returns the VSLangProj.VSProject object of the project containing the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>
        /// The VSLangProj.VSProject object of the project containing the project item 
        /// the code generator was called on
        /// </returns>
        protected VSProject GetVSProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
            return (VSProject)GetProject().Object;
        }
    }
}