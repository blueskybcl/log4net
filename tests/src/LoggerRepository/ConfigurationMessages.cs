/*
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
*/

using System;
using System.Collections;
using System.Xml;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Util;
using NUnit.Framework;

namespace log4net.Tests.LoggerRepository
{
    [TestFixture]
    public class ConfigurationMessages
    {
        [Test]
        public void ConfigurationMessagesTest()
        {
            try {
                LogLog.EmitInternalMessages = false;
                LogLog.InternalDebugging = true;

                XmlDocument log4netConfig = new XmlDocument();
                log4netConfig.LoadXml(@"
                <log4net>
    <!-- OFF, FATAL, ERROR, WARN, INFO, DEBUG, ALL -->
    <!-- Set root logger level to ERROR and its appenders -->
    <root>
      <level value=""INFO,ERROR"" />
      <appender-ref ref=""ErrorAppender"" />
      <appender-ref ref=""InfoAppender"" />
    </root>
    <appender name=""ErrorAppender"" type=""log4net.Appender.RollingFileAppender,log4net"">
      <param name=""StaticLogFileName"" value=""false"" />
      <param name=""File"" value=""App_Data/Error_Log/"" />
      <param name=""AppendToFile"" value=""true"" />
      <!--日志文件命名规则-->
      <param name=""DatePattern"" value=""&quot;error_log_&quot;yyyy-MM-dd&quot;.txt&quot;"" />
      <!--按照文件的大小进行变换日志文件-->
      <param name=""RollingStyle"" value=""Date"" />
      <!--单个文件最大数量-->
      <param name=""MaximumFileSize"" value=""10240KB"" />
      <!--保留的log文件数量 超过此数量后 自动删除之前的  -->
      <param name=""MaxSizeRollBackups"" value=""100"" />
      <!--最小锁定模型以允许多个进程可以写入同一个文件-->
      <param name=""lockingModel"" type=""log4net.Appender.FileAppender+MinimalLock"" />
      <layout type=""log4net.Layout.PatternLayout,log4net"">
        <param name=""ConversionPattern"" value=""%d [%t] %-5p %c - %m%n"" />
        <param name=""Header"" value=""&#xA;----------------------header--------------------------&#xA;"" />
        <param name=""Footer"" value=""&#xA;----------------------footer--------------------------&#xA;"" />
      </layout>
      <!--过滤器，只需要min和max之间的级别-->
      <filter type=""log4net.Filter.LevelRangeFilter"">
        <levelMin value=""WARN"" />
        <levelMax value=""FATAL"" />
      </filter>
    </appender>
    <appender name=""InfoAppender"" type=""log4net.Appender.RollingFileAppender,log4net"">
      <param name=""StaticLogFileName"" value=""false"" />
      <param name=""File"" value=""App_Data/Info_Log/"" />
      <param name=""AppendToFile"" value=""true"" />
      <!--日志文件命名规则-->
      <param name=""DatePattern"" value=""&quot;info_log_&quot;yyyy-MM-dd&quot;.txt&quot;"" />
      <!--按照文件的大小进行变换日志文件-->
      <param name=""RollingStyle"" value=""Date"" />
      <!--单个文件最大数量-->
      <param name=""MaximumFileSize"" value=""10240KB"" />
      <!--保留的log文件数量 超过此数量后 自动删除之前的  -->
      <param name=""MaxSizeRollBackups"" value=""100"" />
      <!--最小锁定模型以允许多个进程可以写入同一个文件-->
      <param name=""lockingModel"" type=""log4net.Appender.FileAppender+MinimalLock"" />
      <layout type=""log4net.Layout.PatternLayout,log4net"">
        <param name=""ConversionPattern"" value=""%d [%t] %-5p %c - %m%n"" />
        <param name=""Header"" value=""&#xA;----------------------header--------------------------&#xA;"" />
        <param name=""Footer"" value=""&#xA;----------------------footer--------------------------&#xA;"" />
      </layout>
      <filter type=""log4net.Filter.LevelRangeFilter"">
        <levelMin value=""INFO"" />
        <levelMax value=""ERROR"" />
      </filter>
    </appender>
  </log4net>");

                ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
                rep.ConfigurationChanged += new LoggerRepositoryConfigurationChangedEventHandler(rep_ConfigurationChanged);

                ICollection configurationMessages = XmlConfigurator.Configure(rep, log4netConfig["log4net"]);//根据配置生成相应的实例

                log4net.Config.XmlConfigurator.Configure();
                log4net.ILog log = log4net.LogManager.GetLogger("运行信息");
                if (log.IsInfoEnabled)
                {
                    log.Info("测试信息");
                }
                log = null;
                Assert.IsTrue(configurationMessages.Count > 0);
            }
            finally {
                LogLog.EmitInternalMessages = true;
                LogLog.InternalDebugging = false;
            }
        }

        [Test]
        public void TestLogInfo()
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ILog log = log4net.LogManager.GetLogger("运行信息");
            if (log.IsInfoEnabled)
            {
                log.Info("测试", "InfoAppender22",null);

                log.Info("测试全部");
            }
            log = null;

            Assert.IsTrue(1 > 0);
        }

        static void rep_ConfigurationChanged(object sender, EventArgs e)
        {
            ConfigurationChangedEventArgs configChanged = (ConfigurationChangedEventArgs)e;

            Assert.IsTrue(configChanged.ConfigurationMessages.Count > 0);
        }
    }

    public class LogLogAppender : AppenderSkeleton
    {
        private readonly static Type declaringType = typeof(LogLogAppender);

        public override void ActivateOptions()
        {
            LogLog.Debug(declaringType, "Debug - Activating options...");
            LogLog.Warn(declaringType, "Warn - Activating options...");
            LogLog.Error(declaringType, "Error - Activating options...");

            base.ActivateOptions();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            LogLog.Debug(declaringType, "Debug - Appending...");
            LogLog.Warn(declaringType, "Warn - Appending...");
            LogLog.Error(declaringType, "Error - Appending...");
        }
    }
}
