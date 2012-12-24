using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeEmsTest
{
    class DataPacketDecoder
    {


        List<DataField> m_dataFieldList = new List<DataField>();
        public void decodePayload(List<byte> payload)
        {
            Dictionary<string, string> m_valueMap = new Dictionary<string, string>();
            for (int i = 0; i < m_dataFieldList.Count; i++)
            {
                if (m_dataFieldList[i].isFlag())
                {
                    bool value = m_dataFieldList[i].flagValue(payload);
                    //qDebug() << "Decoding flag:" << m_dataFieldList[i].name() << "Value:" << value;
                    m_valueMap[m_dataFieldList[i].name()] = value.ToString();
                }
                else
                {
                    double value = m_dataFieldList[i].getValue(payload);
                    //qDebug() << "Decoding value:" << m_dataFieldList[i].name() << "Value:" << value;
                    m_valueMap[m_dataFieldList[i].name()] = value.ToString();
                }
                //ui.tableWidget->item(i,1)->setText(QString::number(value));
            }
            PacketDecoded(m_valueMap);
        }
        public delegate void PacketDecodedDelegate(Dictionary<string, string> valuemap);
        public event PacketDecodedDelegate PacketDecoded;

        public DataPacketDecoder()
        {
            m_dataFieldList.Add(new DataField("IAT", "Intake Air Temperature", 0, 2, 100, -273.15));
            m_dataFieldList.Add(new DataField("CHT", "Coolant/Head Temperature", 2, 2, 100, -273.15));
            m_dataFieldList.Add(new DataField("TPS", "Throttle Position Sensor", 4, 2, 640.0));
            m_dataFieldList.Add(new DataField("EGO", "Exhaust Gas Oxygen", 6, 2, 32768.0));
            m_dataFieldList.Add(new DataField("MAP", "Manifold Air Pressure", 8, 2, 100.0));
            m_dataFieldList.Add(new DataField("AAP", "Ambient Atmosphere Pressure", 10, 2, 100.0));
            m_dataFieldList.Add(new DataField("BRV", "Battery Reference Voltage", 12, 2, 1000.0));
            m_dataFieldList.Add(new DataField("MAT", "Manifold Air Temperature", 14, 2, 100.0, -273.15));
            m_dataFieldList.Add(new DataField("EGO2", "Exhaust Gas Oxygen 2", 16, 2, 32768.0));
            m_dataFieldList.Add(new DataField("IAP", "Intercooler Absolute Pressure", 18, 2, 100.0));
            m_dataFieldList.Add(new DataField("MAF", "Mass Air Flow", 20, 2, 1.0));
            m_dataFieldList.Add(new DataField("DMAP", "Delta Map", 22, 2, 1.0));
            m_dataFieldList.Add(new DataField("DTPS", "Delta TPS", 24, 2, 1.0));
            m_dataFieldList.Add(new DataField("RPM", "Engine Speed", 26, 2, 2.0));
            m_dataFieldList.Add(new DataField("DRPM", "Delta RPM", 28, 2, 1.0));
            m_dataFieldList.Add(new DataField("DDRPM", "Delta Delta RPM", 30, 2, 1.0));

            // DerivedVars
            m_dataFieldList.Add(new DataField("LoadMain", "Configurable Unit of Load", 32, 2, 512.0));
            m_dataFieldList.Add(new DataField("VEMain", "Volumentric Efficiency", 34, 2, 512.0));
            m_dataFieldList.Add(new DataField("Lambda", "Integral Lambda", 36, 2, 32768.0));
            m_dataFieldList.Add(new DataField("AirFlow", "Raw Intermediate", 38, 2, 1.0));
            m_dataFieldList.Add(new DataField("densityAndFuel", "raw Intermediate", 40, 2, 1.0));
            m_dataFieldList.Add(new DataField("BasePW", "Raw PW Before corrections", 42, 2, 1250));
            m_dataFieldList.Add(new DataField("ETE", "Engine Temp Enrichment Percentage", 44, 2, 16384.0 / 100.0));
            m_dataFieldList.Add(new DataField("TFCTotal", "Total Transient Fuel Corrections", 46, 2, 1250)); // Needs to be signed short / int16
            m_dataFieldList.Add(new DataField("EffectivePW", "Actual PW of fuel delivery", 48, 2, 1250.0));
            m_dataFieldList.Add(new DataField("IDT", "PW duration before fuel flow begins", 50, 2, 1250.0));
            m_dataFieldList.Add(new DataField("RefPW", "Reference electrical PW", 52, 2, 1250.0));
            m_dataFieldList.Add(new DataField("Advance", "Ignition advance", 54, 2, 50.0));
            m_dataFieldList.Add(new DataField("Dwell", "Dwell period", 56, 2, 1250.0));

            // KeyUserDebug
            m_dataFieldList.Add(new DataField("tempClock", "Once per datalog message", 58, 1, 1.0));
            m_dataFieldList.Add(new DataField("spareChar", "Unused at this time", 59, 1, 1.0));

            // TODO bits:
            //m_dataFieldList.Add(new DataField("coreStatusA", "Duplicate", 60, 1, 1.0, 0, true)); // Needs flags
            //m_dataFieldList.Add(new DataField("decoderFlags", "Various decoder state flags", 61, 1, 1.0, 0, true)); // Needs flags

            //m_dataFieldList.append(DataField("flaggableFlags","Flags to go with flaggables",62,2,1.0,0,true)); Listed below per bit:
            // Flaggable flags
            m_dataFieldList.Add(new DataField("callsToUISRs", "to ensure we aren't accidentally triggering unused ISRs.", 62, 2, 1.0, 0, 0, 0, true, 0));
            m_dataFieldList.Add(new DataField("lowVoltageConditions", "low voltage conditions.", 62, 2, 1.0, 0, 0, 0, true, 1));
            m_dataFieldList.Add(new DataField("decoderSyncLosses", "Number of times cam, crank or combustion sync is lost.", 62, 2, 1.0, 0, 0, 0, true, 2));
            m_dataFieldList.Add(new DataField("spare", "spare", 62, 2, 1.0, 0, 0, 0, true, 3));
            m_dataFieldList.Add(new DataField("decoderSyncStateClears", "Sync loss called when not synced yet, thus discarding data and preventing sync.", 62, 2, 1.0, 0, 0, 0, true, 4));
            m_dataFieldList.Add(new DataField("serialNoiseErrors", "Incremented when noise is detected", 62, 2, 1.0, 0, 0, 0, true, 5));
            m_dataFieldList.Add(new DataField("serialFramingErrors", "Incremented when a framing error occurs", 62, 2, 1.0, 0, 0, 0, true, 6));
            m_dataFieldList.Add(new DataField("serialParityErrors", "Incremented when a parity error occurs", 62, 2, 1.0, 0, 0, 0, true, 7));
            m_dataFieldList.Add(new DataField("serialOverrunErrors", "Incremented when overrun occurs (count available in KeyUserDebug below", 62, 2, 1.0, 0, 0, 0, true, 8));
            m_dataFieldList.Add(new DataField("serialEscapePairMismatches", "Incremented when an escape is found but not followed by an escapee", 62, 2, 1.0, 0, 0, 0, true, 9));
            m_dataFieldList.Add(new DataField("serialStartsInsideAPacket", "Incremented when a start byte is found inside a packet", 62, 2, 1.0, 0, 0, 0, true, 10));
            m_dataFieldList.Add(new DataField("serialPacketsOverLength", "Incremented when the buffer fills up before the end", 62, 2, 1.0, 0, 0, 0, true, 11));
            m_dataFieldList.Add(new DataField("serialChecksumMismatches", "Incremented when calculated checksum did not match the received one", 62, 2, 1.0, 0, 0, 0, true, 12));
            m_dataFieldList.Add(new DataField("serialPacketsUnderLength", "Incremented when a packet is found that is too short", 62, 2, 1.0, 0, 0, 0, true, 13));
            m_dataFieldList.Add(new DataField("commsDebugMessagesNotSent", "Incremented when a debug message can't be sent due to the TX buffer", 62, 2, 1.0, 0, 0, 0, true, 14));
            m_dataFieldList.Add(new DataField("commsErrorMessagesNotSent", "Incremented when an error message can't be sent due to the TX buffer", 62, 2, 1.0, 0, 0, 0, true, 15));

            m_dataFieldList.Add(new DataField("currentEvent", "Which input event was last", 64, 1, 1.0));
            m_dataFieldList.Add(new DataField("syncLostWithThisID", "UID for reason beind loss of sync", 65, 1, 1.0));
            m_dataFieldList.Add(new DataField("syncLostOnThisEvent", "Where in the input pattern sync was lost", 66, 1, 1.0));
            m_dataFieldList.Add(new DataField("syncCaughtOnThisevent", "Where in the input pattern sync was recovered", 67, 1, 1.0));
            m_dataFieldList.Add(new DataField("syncResetCalls", "Sum of losses, corrections, and state clears", 68, 1, 1.0));
            m_dataFieldList.Add(new DataField("primaryTeethSeen", "", 69, 1, 1.0));
            m_dataFieldList.Add(new DataField("secondaryTeethSeen", "", 70, 1, 1.0));
            m_dataFieldList.Add(new DataField("serialOverrunErrorsCount", "", 71, 1, 1.0)); //These three have "Count" added to the end of the name to avoid conflict with flags above.
            m_dataFieldList.Add(new DataField("serialHardwareErrorsCount", "", 72, 1, 1.0));
            m_dataFieldList.Add(new DataField("serialAndCommsCodeErrorsCount", "", 73, 1, 1.0));
            m_dataFieldList.Add(new DataField("inputEventTimeTolerance", "", 74, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp10", "", 76, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp9", "", 78, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp8", "", 80, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp7", "", 82, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp6", "", 84, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp5", "", 86, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp4", "", 88, 2, 1.0));
            m_dataFieldList.Add(new DataField("zsp3", "", 90, 2, 1.0));
            m_dataFieldList.Add(new DataField("clockInMilliSeconds", "Clock in milliseconds", 92, 2, 1.0));
            m_dataFieldList.Add(new DataField("clock8thMssInMillis", "Clock in 8th milliseconds", 94, 2, 8.0));
            m_dataFieldList.Add(new DataField("ignitionLimiterFlags", "", 96, 1, 1.0)); // Needs flags
            m_dataFieldList.Add(new DataField("injectionLimiterFlags", "", 97, 1, 1.0)); // Needs flags

        }
    }
}
