﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

namespace aProof {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class SimulationSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static SimulationSettings defaultInstance = ((SimulationSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new SimulationSettings())));
        
        public static SimulationSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public uint NUMBER_OF_AGENTS {
            get {
                return ((uint)(this["NUMBER_OF_AGENTS"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8")]
        public uint MAX_REPEATS_FOR_DRAW {
            get {
                return ((uint)(this["MAX_REPEATS_FOR_DRAW"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public uint MAX_PROOF_SEARCH_ATTEMPTS {
            get {
                return ((uint)(this["MAX_PROOF_SEARCH_ATTEMPTS"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public uint MAX_PROOF_SEARCH_TIME {
            get {
                return ((uint)(this["MAX_PROOF_SEARCH_TIME"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public uint MAX_ASSUMPTIONS_DURING_INIT {
            get {
                return ((uint)(this["MAX_ASSUMPTIONS_DURING_INIT"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public uint MAX_WORDS_COUNT_FOR_ASSUMPTION {
            get {
                return ((uint)(this["MAX_WORDS_COUNT_FOR_ASSUMPTION"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8")]
        public uint MAX_GOALS_DURING_INIT {
            get {
                return ((uint)(this["MAX_GOALS_DURING_INIT"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public uint MAX_WORDS_COUNT_FOR_GOAL {
            get {
                return ((uint)(this["MAX_WORDS_COUNT_FOR_GOAL"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8")]
        public uint DEFAULT_THINKING_ITERATIONS {
            get {
                return ((uint)(this["DEFAULT_THINKING_ITERATIONS"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IS_IN_DEBUG_MODE {
            get {
                return ((bool)(this["IS_IN_DEBUG_MODE"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("debug.log")]
        public string DEBUG_FILE_PATH {
            get {
                return ((string)(this["DEBUG_FILE_PATH"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\dictionary.csv")]
        public string DICTIONARY_FILE_PATH {
            get {
                return ((string)(this["DICTIONARY_FILE_PATH"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\known_facts.json")]
        public string KNOWN_FACTS_FILE_PATH {
            get {
                return ((string)(this["KNOWN_FACTS_FILE_PATH"]));
            }
        }
    }
}
