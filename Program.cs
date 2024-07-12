

using Microsoft.VisualBasic.FileIO;

using System;
using NLog;

namespace Middle_WathchDog
{
    public class Program
    {
        private static readonly Logger m_log = LogManager.GetCurrentClassLogger();

        private static WatchDog? m_Main = null;

        public static void Main(string[] args)
        {
            GC.Collect();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            m_log.Info("Program Start");

            try
            {
                using (m_Main = new WatchDog(m_log))
                {
                    RunProgram();
                }
            }
            catch (Exception ex)
            {
                string strLog = $"Main Exception : ${ex.Message}";
                m_log.Error(strLog);
            }
        }

        private static void OnProcessExit(object? sender, EventArgs e)
        {
            if (m_Main != null)
            {

            }
        }

        public static void RunProgram()
        {
            while (true)
            {


                Thread.Sleep(100);
            }
            // Console.WriteLine("Exit");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 비정상 종료 시 처리할 코드
            string strLog = $"Unhandled exception: ${e.ExceptionObject}";
            m_log.Error(strLog);
            // 추가로 로그 저장 또는 정리 작업 수행 가능
        }
    }
}