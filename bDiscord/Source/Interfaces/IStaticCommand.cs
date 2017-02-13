using System.Collections.Generic;
using System.Threading.Tasks;

// WIP
namespace bDiscord.Classes.Interfaces
{
    internal interface IStaticCommand
    {
        Task HandleCommand(List<string> p);
    }
}