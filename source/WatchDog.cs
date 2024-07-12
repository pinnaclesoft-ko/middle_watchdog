using NLog;
using System.IO.Compression;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Middle_WathchDog
{
    public class WatchDog : IDisposable
    {
        private Logger? m_log = null;

        private Thread? m_thMain = null;

        private bool m_bRun = true;

        private INI_Mgr? m_ini = null;

        TcpClient? m_client = null;
        private string? m_strIP = "";
        private string? m_strPort = "";
        private string? m_strUse = "";
        private string? m_strFile = "";

        private bool m_bAlive = false;

        public WatchDog(Logger log)
        {
            string strBasePath = AppContext.BaseDirectory;
            string strINI_Path = Path.Combine(strBasePath, Defien.DEF_INI_FILE_NAME);
            //INI 모듈 Load
            m_ini = new INI_Mgr(strINI_Path);
            Load_INI();

            m_log = log;

            if (bool.Parse(m_strUse))
            {
                m_thMain = new Thread(new ThreadStart(Run_Thread));
                m_thMain.Start();
            }
        }

        public void Load_INI()
        {
            m_strIP = m_ini!.GetValue(Defien.DEF_MIDDLE_SEC, Defien.DEF_MIDDLE_KEY_SERVER);
            m_strPort = m_ini!.GetValue(Defien.DEF_MIDDLE_SEC, Defien.DEF_MIDDLE_KEY_PORT);
            m_strUse = m_ini!.GetValue(Defien.DEF_MIDDLE_SEC, Defien.DEF_MIDDLE_KEY_USE);
            m_strFile = m_ini!.GetValue(Defien.DEF_MIDDLE_SEC, Defien.DEF_MIDDLE_KEY_FILE);
        }

        private void ConnectToServer()
        {
            if (m_client != null && m_client.Connected)
                return;

            try
            {
                m_client = new TcpClient();
                m_client.Connect(m_strIP!, int.Parse(m_strPort!));
                m_log?.Info($"Connected to server {m_strIP}:{m_strPort}");

                m_bAlive = true;

                // 간단한 데이터 송수신 예제
                using (NetworkStream stream = m_client.GetStream())
                {
                    byte[] dataToSend = System.Text.Encoding.ASCII.GetBytes("Ping");
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (response.Equals("Pong"))
                    {
                        m_bAlive = true;
                    }

                    m_log?.Info($"Received from server: {response}");
                }
            }

            catch (Exception ex)
            {
                m_log?.Error($"Failed to connect to server: {ex.Message}");
                m_client?.Close();
                m_client = null;
                m_bAlive = false;
            }
        }
        public void Run_Thread()
        {
            while (m_bRun)
            {
                ConnectToServer();

                if (m_bAlive)
                {
                    //실행중
                    m_client?.Close();
                    m_client = null;
                    Thread.Sleep(1000);
                }
                else
                {
                    //종료
                    //프로그램 재시작
                    //실행파일 경로 m_strFile
                    // 서버와 연결되지 않음
                    m_log?.Warn("Server connection lost, restarting application...");

                    // 프로그램 재시작
                    try
                    {
                        Process.Start(m_strFile!);
                        m_log?.Info("Application restarted.");
                    }
                    catch (Exception ex)
                    {
                        m_log?.Error($"Failed to restart application: {ex.Message}");
                    }
                    Thread.Sleep(5000);
                }
            }
        }

        public void Dispose()
        {
            m_bRun = false;
        }
    }
}