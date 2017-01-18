using System.Collections.Generic;
using Astros2.Core.Interfaces;

namespace Astros2.Core.Types
{
    public class ScriptEntityExtensionPool<T> : 
        Dictionary<int, IScriptEntityExtension<T>> where T : GTA.Entity
    { }
}
