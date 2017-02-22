using System.Collections.Generic;
using System.Threading.Tasks;

// WIP
namespace bDiscord.Source.Interfaces
{
    internal interface IStaticCommand
    {
        Task HandleCommand(List<string> p);
    }
}