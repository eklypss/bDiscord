using bDiscord.Source.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace bDiscord.Source.Commands.StaticCommands
{
    public class FollowAge : IStaticCommand
    {
        public async Task<string> HandleCommand(List<string> p)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string link = string.Format("https://api.rtainc.co/twitch/channels/{1}/followers/{0}?format=[2]", p[1], p[2]);
                    var response = await webClient.DownloadStringTaskAsync(link);
                    if (response.Contains("isn't following"))
                    {
                        return string.Format("{0} is not following {1}.", p[0], p[1]);
                    }
                    else
                    {
                        return string.Format("{0} has been following {1} for {2}!", p[1], p[2], response);
                    }
                }
            }
            catch (Exception ex)
            {
                Printer.PrintTag("Exception", ex.Message);
                return ex.Message;
            }
        }
    }
}
