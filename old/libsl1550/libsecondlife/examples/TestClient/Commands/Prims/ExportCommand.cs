using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using libsecondlife;
using libsecondlife.StructuredData;

namespace libsecondlife.TestClient
{
    public class ExportCommand : Command
    {
        AutoResetEvent GotPermissionsEvent = new AutoResetEvent(false);
        LLObject.ObjectPropertiesFamily Properties;
        bool GotPermissions = false;
        LLUUID SelectedObject = LLUUID.Zero;

        Dictionary<LLUUID, Primitive> PrimsWaiting = new Dictionary<LLUUID, Primitive>();
        AutoResetEvent AllPropertiesReceived = new AutoResetEvent(false);

        public ExportCommand(TestClient testClient)
        {
            testClient.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            testClient.Objects.OnObjectProperties += new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
            testClient.Avatars.OnPointAt += new AvatarManager.PointAtCallback(Avatars_OnPointAt);

            Name = "export";
            Description = "Exports an object to an xml file. Usage: export uuid outputfile.xml";
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            if (args.Length != 2 && !(args.Length == 1 && SelectedObject != LLUUID.Zero))
                return "Usage: export uuid outputfile.xml";

            LLUUID id;
            uint localid;
            string file;

            if (args.Length == 2)
            {
                file = args[1];
                if (!LLUUID.TryParse(args[0], out id))
                    return "Usage: export uuid outputfile.xml";
            }
            else
            {
                file = args[0];
                id = SelectedObject;
            }

            Primitive exportPrim;

            exportPrim = Client.Network.CurrentSim.Objects.Find(
                delegate(Primitive prim) { return prim.ID == id; }
            );

            if (exportPrim != null)
            {
                if (exportPrim.ParentID != 0)
                    localid = exportPrim.ParentID;
                else
                    localid = exportPrim.LocalID;

                // Check for export permission first
                Client.Objects.RequestObjectPropertiesFamily(Client.Network.CurrentSim, id);
                GotPermissionsEvent.WaitOne(1000 * 10, false);

                if (!GotPermissions)
                {
                    return "Couldn't fetch permissions for the requested object, try again";
                }
                else
                {
                    GotPermissions = false;
                    if (Properties.OwnerID != Client.Self.AgentID && 
                        Properties.OwnerID != Client.MasterKey && 
                        Client.Self.AgentID != Client.Self.AgentID)
                    {
                        return "That object is owned by " + Properties.OwnerID + ", we don't have permission " +
                            "to export it";
                    }
                }

                List<Primitive> prims = Client.Network.CurrentSim.Objects.FindAll(
                    delegate(Primitive prim)
                    {
                        return (prim.LocalID == localid || prim.ParentID == localid);
                    }
                );

                bool complete = RequestObjectProperties(prims, 250);

                if (!complete)
                {
                    Console.WriteLine("Warning: Unable to retrieve full properties for:");
                    foreach (LLUUID uuid in PrimsWaiting.Keys)
                        Console.WriteLine(uuid);
                }

                string output = LLSDParser.SerializeXmlString(Helpers.PrimListToLLSD(prims));
                try { File.WriteAllText(file, output); }
                catch (Exception e) { return e.Message; }

                return "Exported " + prims.Count + " prims to " + file;
            }
            else
            {
                return "Couldn't find UUID " + id.ToString() + " in the " + 
                    Client.Network.CurrentSim.Objects.PrimCount + 
                    "objects currently indexed in the current simulator";
            }
        }

        private bool RequestObjectProperties(List<Primitive> objects, int msPerRequest)
        {
            // Create an array of the local IDs of all the prims we are requesting properties for
            uint[] localids = new uint[objects.Count];
            
            lock (PrimsWaiting)
            {
                PrimsWaiting.Clear();

                for (int i = 0; i < objects.Count; ++i)
                {
                    localids[i] = objects[i].LocalID;
                    PrimsWaiting.Add(objects[i].ID, objects[i]);
                }
            }

            Client.Objects.SelectObjects(Client.Network.CurrentSim, localids);

            return AllPropertiesReceived.WaitOne(2000 + msPerRequest * objects.Count, false);
        }

        void Avatars_OnPointAt(LLUUID sourceID, LLUUID targetID, LLVector3d targetPos, 
            PointAtType pointType, float duration, LLUUID id)
        {
            if (sourceID == Client.MasterKey)
            {
                //Client.DebugLog("Master is now selecting " + targetID.ToString());
                SelectedObject = targetID;
            }
        }

        void Objects_OnObjectPropertiesFamily(Simulator simulator, LLObject.ObjectPropertiesFamily properties)
        {
            Properties = properties;
            GotPermissions = true;
            GotPermissionsEvent.Set();
        }

        void Objects_OnObjectProperties(Simulator simulator, LLObject.ObjectProperties properties)
        {
            lock (PrimsWaiting)
            {
                PrimsWaiting.Remove(properties.ObjectID);

                if (PrimsWaiting.Count == 0)
                    AllPropertiesReceived.Set();
            }
        }
    }
}
