using GTA;

namespace Astros2.Core.Interfaces
{
    public interface IScriptEntityExtension<T> where T : Entity
    {
        int ID { get; }
        ScriptEntity<T> Entity { get; set; }
        void Update();
        void Dispose();
    }
}
