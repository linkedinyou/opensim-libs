using System;
using libsecondlife;

namespace libsecondlife.TestClient
{
    public class PrimCountCommand: Command
    {
        public PrimCountCommand(TestClient testClient)
		{
			Name = "primcount";
			Description = "Shows the number of objects currently being tracked.";
		}

        public override string Execute(string[] args, LLUUID fromAgentID)
		{
            int count = 0;

            lock (Client.Network.Simulators)
            {
                for (int i = 0; i < Client.Network.Simulators.Count; i++)
                {
                    int avcount = Client.Network.Simulators[i].Objects.AvatarCount;
                    int primcount = Client.Network.Simulators[i].Objects.PrimCount;

                    Console.WriteLine("{0} (Avatars: {1} Primitives: {2})", 
                        Client.Network.Simulators[i].Name, avcount, primcount);

                    count += avcount;
                    count += primcount;
                }
            }

			return "Tracking a total of " + count + " objects";
		}
    }
}
