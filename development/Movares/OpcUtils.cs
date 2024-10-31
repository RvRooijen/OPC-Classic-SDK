using Newtonsoft.Json;

namespace Movares
{
    public static class OpcUtils
    {
        public static MyDaSession CreateSession(string url)
        {
            var session = new MyDaSession(url);
            if(session.Valid)
            {
                System.Console.WriteLine("Session created");
                return session;
            }

            throw new Exception("Failed to create session");
        }
    
        public static MyDaSubscription AddSubscription(this MyDaSession session, uint updateRate)
        {
            var sub = new MyDaSubscription(updateRate, session);
            if(sub.Valid)
            {
                System.Console.WriteLine("Subscription created");
                return sub;
            }

            throw new Exception("Failed to create subscription");
        }
    
        public static Configuration LoadConfiguration(string configFilePath)
        {
            var configJson = File.ReadAllText(configFilePath);
            System.Console.WriteLine($"Configuration loaded {configJson}");
            return JsonConvert.DeserializeObject<Configuration>(configJson);
        }
    }
}

