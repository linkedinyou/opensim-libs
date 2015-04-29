using System;
using libsecondlife;

namespace libsecondlife.TestClient
{
    public class PrimInfoCommand : Command
    {
        public PrimInfoCommand(TestClient testClient)
        {
            Name = "priminfo";
            Description = "Dumps information about a specified prim. " + "Usage: priminfo [prim-uuid]";
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            LLUUID primID;

            if (args.Length != 1)
                return "Usage: priminfo [prim-uuid]";

            if (LLUUID.TryParse(args[0], out primID))
            {
                Primitive target = Client.Network.CurrentSim.Objects.Find(
                    delegate(Primitive prim) { return prim.ID == primID; }
                );

                if (target != null)
                {
                    Client.Log("Light: " + target.Light.ToString(), Helpers.LogLevel.Info);

                    if (target.ParticleSys.CRC != 0)
                        Client.Log("Particles: " + target.ParticleSys.ToString(), Helpers.LogLevel.Info);

                    Client.Log("TextureEntry:", Helpers.LogLevel.Info);
                    if (target.Textures != null)
                    {
                        for (int i = 0; i < target.Textures.FaceTextures.Length; i++)
                        {
                            if (target.Textures.FaceTextures[i] != null)
                            {
                                Client.Log(String.Format("Face {0}: {1}", i,
                                    target.Textures.FaceTextures[i].TextureID.ToString()),
                                    Helpers.LogLevel.Info);
                            }
                        }
                    }
                    else
                    {
                        Client.Log("null", Helpers.LogLevel.Info);
                    }

                    return "Done.";
                }
                else
                {
                    return "Could not find prim " + primID.ToString();
                }
            }
            else
            {
                return "Usage: priminfo [prim-uuid]";
            }
        }
    }
}
