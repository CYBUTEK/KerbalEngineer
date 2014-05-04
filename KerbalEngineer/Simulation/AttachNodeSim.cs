using System;
using System.Text;

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

#if LOG || true
        public void DumpToBuffer(StringBuilder buffer)
        {
            if (this.attachedPartSim == null)
            {
                buffer.Append("<staged>:<n>");
            }
            else
            {
                buffer.Append(this.attachedPartSim.name);
                buffer.Append(":");
                buffer.Append(this.attachedPartSim.partId);
            }
            buffer.Append("#");
            buffer.Append(this.nodeType);
            buffer.Append(":");
            buffer.Append(this.id);
        }
#endif
    }
}
