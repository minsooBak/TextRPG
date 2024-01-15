using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    internal class QuestManager : IListener
    {

        public QuestManager()
        {
            EventManager.Instance.AddListener(EventType.Quest, this);
        }
        public void OnEvent(EventType type, object data)
        {
            if (type == EventType.Quest)
            {
                var a = (KeyValuePair<eQuestType, string>)data;
                switch (a.Key)
                {
                    case eQuestType.Item:
                        {

                            break;
                        }
                    case eQuestType.Dungeon:
                        {

                            break;
                        }
                    case eQuestType.Monster:
                        {

                            break;
                        }
                    case eQuestType.Stats:
                        {

                            break;
                        }
                }
            }
        }
    }
}
