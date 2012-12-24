using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeEmsTest
{
    class DataField
    {
        public DataField(String shortname, String description, int offset, int size, double div, double addoffset = 0, double min = 0, double max = 0, bool isFlags = false, int bit = 0)
        {
            m_offset = offset;
            m_size = size;
            m_div = div;
            m_name = shortname;
            m_description = description;
            m_min = min;
            m_max = max;
            m_addoffset = addoffset;
            m_isFlags = isFlags;
            m_bit = bit;
        }
        public String description() { return m_description; }
        public String name() { return m_name; }
        public bool isFlag() { return m_isFlags; }
        public bool flagValue(List<byte> payload)
        {
            if (!m_isFlags)
            {
                return false;
            }
            if (payload.Count > m_offset + m_size)
            {
                uint val = 0;
                for (int i = 0; i < m_size; i++)
                {
                    val += ((uint)payload[m_offset + i]) << (8 * (m_size - (i + 1)));
                }
                return ((m_bit & val) != 0);
            }
            return false;
        }

        public double getValue(List<byte> payload)
        {
            if (payload.Count > m_offset + m_size)
            {
                float val = 0;
                for (int i = 0; i < m_size; i++)
                {
                    val += ((uint)payload[m_offset + i]) << (8 * (m_size - (i + 1)));
                }
                return (val / m_div) + m_addoffset;
            }
            return 0;
        }
        String m_name;
        String m_description;
        bool m_isFlags;
        int m_bit;
        int m_offset;
        int m_size;
        double m_div;
        double m_addoffset;
        double m_min;
        double m_max;
    }
}
