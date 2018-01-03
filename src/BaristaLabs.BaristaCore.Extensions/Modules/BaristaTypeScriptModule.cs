﻿namespace BaristaLabs.BaristaCore.Modules
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module that returns a script that will be parsed when imported.
    /// </summary>
    [BaristaModule("TypeScriptModule", "Built-in module that allows for specifing a string based typescript module")]
    public sealed class BaristaTypeScriptModule : IBaristaScriptModule
    {
        private readonly string m_name;
        private readonly string m_description;
        private readonly string m_filename;

        public BaristaTypeScriptModule(string name, string description = null, string filename = "script.ts")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            m_name = name;
            m_description = description;
            m_filename = filename;
        }

        /// <summary>
        /// Gets the name of the script module.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Getsthe description of the script module.
        /// </summary>
        public string Description
        {
            get { return m_description; }
        }

        /// <summary>
        /// Gets the filename associated with the script
        /// </summary>
        public string Filename
        {
            get { return m_filename; }
        }

        /// <summary>
        /// Gets or sets the script that will be executed.
        /// </summary>
        public string Script
        {
            get;
            set;
        }

        public async Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var transpiledScript = await TypeScriptTranspiler.Default.Transpile(new TypeScriptTranspilerOptions()
            {
                ScriptToTranspile = Script,
                Filename = m_filename
            });

            return transpiledScript;
        }
    }
}