using AngleSharp.Parser.Html;
using bDiscord.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bDiscord.Classes
{
    public class GameFetcher
    {
        public static async Task<List<LiigaGame>> GetLiigaGames()
        {
            try
            {
                var gamesList = new List<LiigaGame>();
                using (WebClient web = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    var response = await web.DownloadStringTaskAsync("http://liiga.fi/");
                    var parser = new HtmlParser();
                    var document = parser.Parse(response);
                    var games = document.All.Where(c => c.ClassName == "game");
                    foreach (var game in games)
                    {
                        var gameLink = game.Children.Where(v => v.ClassName == "game_link").First();
                        var gameTime = game.Children.Where(v => v.ClassName == "status-short").First();
                        string[] splitString = gameTime.InnerHtml.Split(' ');
                        var homeTeamLogo = gameLink.Children.Where(c => c.ClassName == "scores-container")
                            .First().Children.Where(v => v.ClassName == "home")
                            .First().Children.Where(b => b.ClassName == "team-score")
                            .First().Children.Where(n => n.ClassName == "logo-container")
                            .First().Children.Where(t => t.ClassName == "logo")
                            .First();

                        var team1Name = ResolveTeamName(homeTeamLogo.Attributes["src"].Value);

                        var awayTeamLogo = gameLink.Children.Where(c => c.ClassName == "scores-container")
                            .First().Children.Where(v => v.ClassName == "away")
                            .First().Children.Where(b => b.ClassName == "team-score")
                            .First().Children.Where(n => n.ClassName == "logo-container")
                            .First().Children.Where(t => t.ClassName == "logo")
                            .First();

                        var team2Name = ResolveTeamName(awayTeamLogo.Attributes["src"].Value);
                        gamesList.Add(new LiigaGame() { StartTime = splitString[3], HomeTeamName = team1Name, AwayTeamName = team2Name });
                    }
                }
                return gamesList;
            }
            catch (Exception ex)
            {
                Printer.PrintTag("Exception", ex.Message);
                return null;
            }
        }

        private static string ResolveTeamName(string imageSource)
        {
            string[] teamNames = new string[] { "HIFK", "HPK", "Ilves", "Jukurit", "JYP", "KalPa", "KooKoo", "Karpat", "Lukko", "Pelicans", "SaiPa", "Sport", "Tappara", "TPS", "Assat" };
            string searchResult = teamNames.FirstOrDefault<string>(x => Regex.IsMatch(imageSource, x, RegexOptions.IgnoreCase));
            switch (searchResult)
            {
                case "Karpat":
                    searchResult = "Kärpät";
                    break;

                case "Assat":
                    searchResult = "Ässät";
                    break;
            }
            return searchResult;
        }
    }
}