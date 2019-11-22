using System.Collections.Generic;

namespace SharpCmd
{
    public class HistoryBrowser
    {
        private readonly List<string> m_history = new List<string>();
        private int m_index = -1;

        public void Add(string item)
        {
            m_history.Add(item);
            m_index = m_history.Count;
        }
        public string Prev() => InsideBounds(m_index - 1, m_history) ? m_history[--m_index] : null;
        public string Next() => InsideBounds(m_index + 1, m_history) ? m_history[++m_index] : null;
        
        private static bool InsideBounds(int index, ICollection<string> array)
        {
            return index >= 0 && index < array.Count;
        }
    }
}