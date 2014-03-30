using System;

namespace KerbalEngineer.Simulation
{
    class AttachNodeSim
    {
        public PartSim attachedPartSim;
        public AttachNode.NodeType nodeType;
        public String id;

        public AttachNodeSim(PartSim partSim, String newId, AttachNode.NodeType newNodeType)
        {
            this.attachedPartSim = partSim;
            this.nodeType = newNodeType;
            this.id = newId;
        }

#if LOG
        public void DumpToBuffer(StringBuilder buffer)
        {
            if (attachedPartSim == null)
            {
                buffer.Append("<staged>:<n>");
            }
            else
            {
                buffer.Append(attachedPartSim.name);
                buffer.Append(":");
                buffer.Append(attachedPartSim.partId);
            }
            buffer.Append("#");
            buffer.Append(nodeType);
            buffer.Append(":");
            buffer.Append(id);
        }
#endif
    }
}
