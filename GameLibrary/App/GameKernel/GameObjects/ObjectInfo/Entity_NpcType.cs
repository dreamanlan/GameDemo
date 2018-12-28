using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary
{
    public partial class EntityInfo
    {
        public bool IsPlayerType
        {
            get { return m_EntityType == (int)EntityTypeEnum.Player; }
        }
        public bool IsNpcType
        {
            get { return m_EntityType == (int)EntityTypeEnum.Npc; }
        }
    }
}
