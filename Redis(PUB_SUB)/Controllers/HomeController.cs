using Microsoft.AspNetCore.Mvc;
using Redis_PUB_SUB_.Models;
using StackExchange.Redis;
using System.Diagnostics;

namespace Redis_PUB_SUB_.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISubscriber _subscriber;
        private readonly IDatabase _database;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration; var muxer = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
            _database = muxer.GetDatabase(); 
            _subscriber = muxer.GetSubscriber();
        }
        public IActionResult Index() 
        {
            var channels = GetChannels();
            return View(channels); 
        }
        [HttpPost]
        public IActionResult CreateChannel(string channelName)
        {
            if (!string.IsNullOrEmpty(channelName)) 
            {
                _database.ListRightPush("channels", channelName); 
            }
            return RedirectToAction("Index"); }
        [HttpGet] 
        public IActionResult GetMessages(string channelName) 
        {
            var messages = _database.ListRange(channelName).Select(m => m.ToString()).ToList(); 
            ViewBag.ChannelName = channelName; 
            return View(messages);
        }
        [HttpPost] 
        public async Task<IActionResult> PostMessage(string channelName, string message) 
        {
            if (!string.IsNullOrEmpty(message)) 
            {
                await _database.ListRightPushAsync(channelName, message);
                await _subscriber.PublishAsync(channelName, message); }
            return RedirectToAction("GetMessages", new { channelName = channelName });
        }
        private List<string> GetChannels() 
        {
            var channels = _database.ListRange("channels").Select(c => c.ToString()).ToList(); 
            return channels;
        }
    }
}