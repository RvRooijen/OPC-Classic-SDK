namespace Movares
{

    using System;
    using Softing.OPCToolbox.Client;
    using Softing.OPCToolbox;

    namespace DaConsole
    {
        public class OpcClient
        {
            private MyDaSession m_daSession;
            private MyDaSubscription m_daSubscription;
            private MyDaItem[] m_items;
            private ExecutionOptions m_executionOptions;

            public OpcClient()
            {
                m_items = new MyDaItem[6];
                m_executionOptions = new ExecutionOptions
                {
                    ExecutionType = EnumExecutionType.ASYNCHRONOUS,
                    ExecutionContext = 0
                };
            }

            public int Initialize()
            {
                int result = Application.Instance.Initialize();
                if (ResultCode.SUCCEEDED(result))
                {
                    Application.Instance.EnableTracing(
                        EnumTraceGroup.ALL,
                        EnumTraceGroup.ALL,
                        EnumTraceGroup.ALL,
                        EnumTraceGroup.ALL,
                        "Trace.txt",
                        1000000,
                        0);
                }

                return result;
            }

            public void Update()
            {
                ReadItems();
                // WriteItems(_values);
                // GetServerStatus();
            }
            
            public void Terminate()
            {
                m_daSubscription?.Disconnect(new ExecutionOptions());
                m_daSession?.Disconnect(new ExecutionOptions());
                Application.Instance.Terminate();
            }

            public int InitializeDaObjects(string url)
            {
                m_daSession = new MyDaSession(url);
                if (!m_daSession.Valid) return (int)EnumResultCode.E_FAIL;

                m_daSubscription = new MyDaSubscription(1000, m_daSession);
                if (!m_daSubscription.Valid) return (int)EnumResultCode.E_FAIL;

                string[] itemIds =
                {
                    "maths.sin", "time.local.second", "increment.UI1 array", "increment.BSTR", "secure.I2",
                    "IOP.static.I1 array"
                };
                for (int i = 0; i < itemIds.Length; i++)
                {
                    m_items[i] = new MyDaItem(itemIds[i], m_daSubscription);
                    if (!m_items[i].Valid) return (int)EnumResultCode.E_FAIL;
                }

                return m_daSession.Connect(true, false, m_executionOptions);
            }

            public void ReadItems()
            {
                ValueQT[] values;
                int[] results;
                m_daSubscription.Read(0, m_items, out values, out results, m_executionOptions);
            }

            public void WriteItems(ValueQT[] values)
            {
                int[] results;
                m_daSubscription.Write(m_items, values, out results, m_executionOptions);
            }

            public void GetServerStatus()
            {
                ServerStatus serverStatus;
                m_daSession.GetStatus(out serverStatus, m_executionOptions);
            }

            public void LogInSecure(string username, string password)
            {
                m_daSession.Logon(username, password, m_executionOptions);
            }

            public void LogOffSecure()
            {
                m_daSession.Logoff(m_executionOptions);
            }
        }
    }
}