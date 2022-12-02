using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sandbox.ModAPI.Ingame;
using System;
using Moq;
using static IngameScript.Program;

namespace Tests.Generic.AutoRun
{
    /// <summary>
    /// Unit Test <see cref="RunManager"/>
    /// </summary>
    [TestClass]
    public class RunManagerTest
    {
        /// <summary>
        /// Test <see cref="RunManager.AnalyzeUpdateType(UpdateType)"/>,
        /// when the running is paused, 
        /// then the Current Frequency should always be <see cref="UpdateFrequency.None"/>
        /// </summary>
        [TestMethod]
        public void Test_AnalyzeUpdateType_Paused()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManager = new RunManager(mockRunTime.Object);
            runManager.Paused = true;

            //Act 
            UpdateFrequency resultFrequency1 = runManager.AnalyzeUpdateType(UpdateType.Update1);
            UpdateFrequency resultFrequency10 = runManager.AnalyzeUpdateType(UpdateType.Update10);
            UpdateFrequency resultFrequency100 = runManager.AnalyzeUpdateType(UpdateType.Update100);
            UpdateFrequency resultFrequencyOnce = runManager.AnalyzeUpdateType(UpdateType.Once);
            UpdateFrequency resultFrequencyNone = runManager.AnalyzeUpdateType(UpdateType.None);
            UpdateFrequency resultFrequencyTerminal = runManager.AnalyzeUpdateType(UpdateType.Terminal);

            //Assert
            Assert.AreEqual(UpdateFrequency.None, resultFrequency1);
            Assert.AreEqual(UpdateFrequency.None, resultFrequency10);
            Assert.AreEqual(UpdateFrequency.None, resultFrequency100);
            Assert.AreEqual(UpdateFrequency.None, resultFrequencyOnce);
            Assert.AreEqual(UpdateFrequency.None, resultFrequencyNone);
            Assert.AreEqual(UpdateFrequency.None, resultFrequencyTerminal);
        }


        /// <summary>
        /// Test <see cref="RunManager.AnalyzeUpdateType(UpdateType)"/>,
        /// when the updateType is <see cref="UpdateType.Once"/>,
        /// then the currentFrequency should have the <see cref="UpdateFrequency.Once"/> flag.
        /// </summary>
        [TestMethod]
        public void Test_AnalyzeUpdateType_UpdateType1()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManager = new RunManager(mockRunTime.Object);

            //Act
            UpdateFrequency resultFrequency = runManager.AnalyzeUpdateType(UpdateType.Update1);

            //Assert
            Assert.IsTrue(resultFrequency.HasFlag(UpdateFrequency.Update1));
        }

        /// <summary>
        /// Test <see cref="RunManager.AnalyzeUpdateType(UpdateType)"/>,
        /// when the updateType is <see cref="UpdateType.Update1"/>,
        /// then the currentFrequency should have the <see cref="UpdateFrequency.Update1"/> flag.
        /// </summary>
        [TestMethod]
        public void Test_AnalyzeUpdateType_UpdateTypeOnce()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManager = new RunManager(mockRunTime.Object);

            //Act
            UpdateFrequency resultFrequency = runManager.AnalyzeUpdateType(UpdateType.Once);

            //Assert
            Assert.IsTrue(resultFrequency.HasFlag(UpdateFrequency.Once));
        }


        /// <summary>
        /// Test <see cref="RunManager.AnalyzeUpdateType(UpdateType)"/>,
        /// Test <see cref="RunManager.AnalyzeUpdateType(UpdateType)"/>,
        /// when the updateType is <see cref="UpdateType.Update10"/>
        /// or the updateType is <see cref="UpdateType.Once"/> ten times,
        /// or the updateType is <see cref="UpdateType.Update1"/> ten times,
        /// then the currentFrequency should have the <see cref="UpdateFrequency.Update10"/> flag.
        /// </summary>
        [TestMethod]
        public void Test_AnalyzeUpdateType_UpdateType10()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManagerSimple = new RunManager(mockRunTime.Object);
            RunManager runManagerManyTimes = new RunManager(mockRunTime.Object);

            UpdateFrequency updateFrequency5Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency10Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency19Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency20Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency21Times = UpdateFrequency.None;

            //Act
            UpdateFrequency updateFrequencySimple = runManagerSimple.AnalyzeUpdateType(UpdateType.Update10);

            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            updateFrequency5Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);

            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            updateFrequency10Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update1);

            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);
            updateFrequency19Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);

            updateFrequency20Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);

            updateFrequency21Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Once);

            //Assert
            Assert.IsTrue(updateFrequencySimple.HasFlag(UpdateFrequency.Update10));

            Assert.IsFalse(updateFrequency5Times.HasFlag(UpdateFrequency.Update10));
            Assert.IsTrue(updateFrequency10Times.HasFlag(UpdateFrequency.Update10));
            Assert.IsFalse(updateFrequency19Times.HasFlag(UpdateFrequency.Update10));
            Assert.IsTrue(updateFrequency20Times.HasFlag(UpdateFrequency.Update10));
            Assert.IsFalse(updateFrequency21Times.HasFlag(UpdateFrequency.Update10));
        }

        /// <summary>
        /// Test <see cref="RunManager.AnalyzeUpdateType(UpdateType)"/>,
        /// Test <see cref="RunManager.AnalyzeUpdateType(UpdateType)"/>,
        /// when the updateType is <see cref="UpdateType.Update100"/>
        /// or the updateType is <see cref="UpdateType.Update10"/> ten times,
        /// or the updateType is <see cref="UpdateType.Update100"/> a hundred times,
        /// then the currentFrequency should have the <see cref="UpdateFrequency.Update10"/> flag.
        /// </summary>
        [TestMethod]
        public void Test_AnalyzeUpdateType_UpdateType100()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManagerSimple = new RunManager(mockRunTime.Object);
            RunManager runManagerManyTimes = new RunManager(mockRunTime.Object);
            RunManager runManagerManyManyTimes = new RunManager(mockRunTime.Object);

            //Update 10 Checks
            UpdateFrequency updateFrequency5Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency10Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency19Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency20Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency21Times = UpdateFrequency.None;

            //Update 1 Checks
            UpdateFrequency updateFrequency99Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency100Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency101Times = UpdateFrequency.None;
            UpdateFrequency updateFrequency200Times = UpdateFrequency.None;

            //Act
            UpdateFrequency updateFrequencySimple = runManagerSimple.AnalyzeUpdateType(UpdateType.Update100);

            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            updateFrequency5Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);

            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            updateFrequency10Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);

            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);
            updateFrequency19Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);

            updateFrequency20Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);

            updateFrequency21Times = runManagerManyTimes.AnalyzeUpdateType(UpdateType.Update10);

            for (int i = 0; i < 98; i++)
            {
                runManagerManyManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            }
            updateFrequency99Times = runManagerManyManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            updateFrequency100Times = runManagerManyManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            updateFrequency101Times = runManagerManyManyTimes.AnalyzeUpdateType(UpdateType.Update1);

            for (int i = 101; i < 199; i++)
            {
                runManagerManyManyTimes.AnalyzeUpdateType(UpdateType.Update1);
            }
            updateFrequency200Times = runManagerManyManyTimes.AnalyzeUpdateType(UpdateType.Update1);

            //Assert
            Assert.IsTrue(updateFrequencySimple.HasFlag(UpdateFrequency.Update100));

            Assert.IsFalse(updateFrequency5Times.HasFlag(UpdateFrequency.Update100));
            Assert.IsTrue(updateFrequency10Times.HasFlag(UpdateFrequency.Update100));
            Assert.IsFalse(updateFrequency19Times.HasFlag(UpdateFrequency.Update100));
            Assert.IsTrue(updateFrequency20Times.HasFlag(UpdateFrequency.Update100));
            Assert.IsFalse(updateFrequency21Times.HasFlag(UpdateFrequency.Update100));

            Assert.IsFalse(updateFrequency99Times.HasFlag(UpdateFrequency.Update100));
            Assert.IsTrue(updateFrequency100Times.HasFlag(UpdateFrequency.Update100));
            Assert.IsFalse(updateFrequency101Times.HasFlag(UpdateFrequency.Update100));
            Assert.IsTrue(updateFrequency200Times.HasFlag(UpdateFrequency.Update100));
        }

        /// <summary>
        /// Test <see cref="RunManager.CheckForFrequency(UpdateFrequency)"/>,
        /// when after a frequency was evaualted, then the same frequency is provided,
        /// then it should return true, otherwise false.
        /// </summary>
        [TestMethod]
        public void Test_CheckForFrequency_Valid()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManager1 = new RunManager(mockRunTime.Object);
            RunManager runManager10 = new RunManager(mockRunTime.Object);
            RunManager runManager100 = new RunManager(mockRunTime.Object);
            RunManager runManagerOnce = new RunManager(mockRunTime.Object);
            RunManager runManagerNone = new RunManager(mockRunTime.Object);

            //Act
            runManager1.AnalyzeUpdateType(UpdateType.Update1);
            bool check1valid = runManager1.CheckForFrequency(UpdateFrequency.Update1);
            bool check1inValid = runManager1.CheckForFrequency(UpdateFrequency.Update10);

            runManager10.AnalyzeUpdateType(UpdateType.Update10);
            bool check10valid = runManager10.CheckForFrequency(UpdateFrequency.Update10);
            bool check10inValid = runManager10.CheckForFrequency(UpdateFrequency.Update100);

            runManager100.AnalyzeUpdateType(UpdateType.Update100);
            bool check100valid = runManager100.CheckForFrequency(UpdateFrequency.Update100);
            bool check100inValid = runManager100.CheckForFrequency(UpdateFrequency.Update1);

            runManagerOnce.AnalyzeUpdateType(UpdateType.Once);
            bool checkOnceValid = runManagerOnce.CheckForFrequency(UpdateFrequency.Once);
            bool checkOnceInValid = runManagerOnce.CheckForFrequency(UpdateFrequency.Update1);

            runManagerNone.AnalyzeUpdateType(UpdateType.None);
            bool checkNoneValid = runManagerNone.CheckForFrequency(UpdateFrequency.None);
            bool checkNoneInValid = runManagerNone.CheckForFrequency(UpdateFrequency.Update100);

            //Assert
            Assert.IsTrue(check1valid);
            Assert.IsFalse(check1inValid);

            Assert.IsTrue(check10valid);
            Assert.IsFalse(check10inValid);

            Assert.IsTrue(check100valid);
            Assert.IsFalse(check100inValid);

            Assert.IsTrue(checkOnceValid);
            Assert.IsFalse(checkOnceInValid);

            Assert.IsTrue(checkNoneValid);
            Assert.IsFalse(checkNoneInValid);
        }

        /// <summary>
        /// Test <see cref="RunManager.CheckForFrequency(UpdateFrequency)"/>,
        /// when after a frequency was evaualted, then the same frequency is provided,
        /// then it should return true, otherwise false.
        /// </summary>
        [TestMethod]
        public void Test_CheckForFrequency_Multiple()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManager1 = new RunManager(mockRunTime.Object);
            RunManager runManager10 = new RunManager(mockRunTime.Object);
            RunManager runManager100 = new RunManager(mockRunTime.Object);
            RunManager runManagerOnce = new RunManager(mockRunTime.Object);
            RunManager runManagerNone = new RunManager(mockRunTime.Object);

            //Act
            runManager1.AnalyzeUpdateType(UpdateType.Update1);
            bool check1valid = runManager1.CheckForFrequency(UpdateFrequency.Update1, UpdateFrequency.Update100);
            bool check1inValid = runManager1.CheckForFrequency(UpdateFrequency.Update10, UpdateFrequency.Update100);

            runManager10.AnalyzeUpdateType(UpdateType.Update10);
            bool check10valid = runManager10.CheckForFrequency(UpdateFrequency.Update10, UpdateFrequency.Update1);
            bool check10inValid = runManager10.CheckForFrequency(UpdateFrequency.Update100, UpdateFrequency.Update1);

            runManager100.AnalyzeUpdateType(UpdateType.Update100);
            bool check100valid = runManager100.CheckForFrequency(UpdateFrequency.Update100, UpdateFrequency.Update10);
            bool check100inValid = runManager100.CheckForFrequency(UpdateFrequency.Update1, UpdateFrequency.Update10);

            runManagerOnce.AnalyzeUpdateType(UpdateType.Once);
            bool checkOnceValid = runManagerOnce.CheckForFrequency(UpdateFrequency.Once, UpdateFrequency.Update100);
            bool checkOnceInValid = runManagerOnce.CheckForFrequency(UpdateFrequency.Update1, UpdateFrequency.Update100);

            runManagerNone.AnalyzeUpdateType(UpdateType.None);
            bool checkNoneValid = runManagerNone.CheckForFrequency(UpdateFrequency.None, UpdateFrequency.Update1);
            bool checkNoneInValid = runManagerNone.CheckForFrequency(UpdateFrequency.Update100, UpdateFrequency.Update1);

            //Assert
            Assert.IsTrue(check1valid);
            Assert.IsFalse(check1inValid);

            Assert.IsTrue(check10valid);
            Assert.IsFalse(check10inValid);

            Assert.IsTrue(check100valid);
            Assert.IsFalse(check100inValid);

            Assert.IsTrue(checkOnceValid);
            Assert.IsFalse(checkOnceInValid);

            Assert.IsTrue(checkNoneValid);
            Assert.IsFalse(checkNoneInValid);
        }


        /// <summary>
        /// Test <see cref="RunManager.ScheduleRunFrequency(UpdateFrequency)"/>,
        /// when a frequency is provided,
        /// then it is added to the sheduled frequencies.
        /// </summary>
        [TestMethod]
        public void Test_ScheduleRunFrequency_OnlyOne()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManager = new RunManager(mockRunTime.Object);

            //Act
            runManager.ScheduleRunFrequency(UpdateFrequency.Update1);

            //Assert
            Assert.AreEqual(UpdateFrequency.Update1, runManager.ScheduledFrequency);
        }

        /// <summary>
        /// Test <see cref="RunManager.ScheduleRunFrequency(UpdateFrequency)"/>,
        /// when multiple frequencies are provided,
        /// then all is added to the sheduled frequencies, but only once.
        /// </summary>
        [TestMethod]
        public void Test_ScheduleRunFrequency_Multiple()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManager = new RunManager(mockRunTime.Object);

            byte expectedByteFlag = (byte)UpdateFrequency.Update1 + (byte)UpdateFrequency.Update10;
            //Act
            runManager.ScheduleRunFrequency(UpdateFrequency.Update1);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update10);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update10);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update10);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update10);

            //Assert
            Assert.AreEqual(expectedByteFlag, (byte)runManager.ScheduledFrequency);
            Assert.IsFalse(runManager.ScheduledFrequency.HasFlag(UpdateFrequency.Update100));
        }


        /// <summary>
        /// Test <see cref="RunManager.IsAutoRun"/>,
        /// when the evaualted frequency suggests an automatic run,
        /// then it should return true, otherwise false.
        /// </summary>
        [TestMethod]
        public void Test_IsAutoRun_Valid()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            RunManager runManagerNone = new RunManager(mockRunTime.Object);
            RunManager runManagerTerminal = new RunManager(mockRunTime.Object);
            RunManager runManagerTrigger = new RunManager(mockRunTime.Object);
            RunManager runManagerMod = new RunManager(mockRunTime.Object);
            RunManager runManagerScipt = new RunManager(mockRunTime.Object);
            RunManager runManagerIGC = new RunManager(mockRunTime.Object);

            RunManager runManagerOnce = new RunManager(mockRunTime.Object);
            RunManager runManager1 = new RunManager(mockRunTime.Object);
            RunManager runManager10 = new RunManager(mockRunTime.Object);
            RunManager runManager100 = new RunManager(mockRunTime.Object);

            //Act
            runManagerNone.AnalyzeUpdateType(UpdateType.None);
            runManagerTerminal.AnalyzeUpdateType(UpdateType.Terminal);
            runManagerTrigger.AnalyzeUpdateType(UpdateType.Trigger);
            runManagerMod.AnalyzeUpdateType(UpdateType.Mod);
            runManagerScipt.AnalyzeUpdateType(UpdateType.Script);
            runManagerIGC.AnalyzeUpdateType(UpdateType.IGC);

            runManagerOnce.AnalyzeUpdateType(UpdateType.Once);
            runManager1.AnalyzeUpdateType(UpdateType.Update1);
            runManager10.AnalyzeUpdateType(UpdateType.Update10);
            runManager100.AnalyzeUpdateType(UpdateType.Update100);

            //Assert
            Assert.IsFalse(runManagerNone.IsAutoRun());
            Assert.IsFalse(runManagerTerminal.IsAutoRun());
            Assert.IsFalse(runManagerTrigger.IsAutoRun());
            Assert.IsFalse(runManagerMod.IsAutoRun());
            Assert.IsFalse(runManagerScipt.IsAutoRun());
            Assert.IsFalse(runManagerIGC.IsAutoRun());

            Assert.IsTrue(runManagerOnce.IsAutoRun());
            Assert.IsTrue(runManager1.IsAutoRun());
            Assert.IsTrue(runManager10.IsAutoRun());
            Assert.IsTrue(runManager100.IsAutoRun());
        }


        /// <summary>
        /// Test <see cref="RunManager.ApplySchedule"/>,
        /// when the running is paused,
        /// then the runtime is set to <see cref="UpdateFrequency.None"/>,
        /// and the <see cref="RunManager.ScheduledFrequency"/> is not changed.
        /// </summary>
        [TestMethod]
        public void Test_ApplySchedule_Valid()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            mockRunTime.SetupProperty(p => p.UpdateFrequency);

            RunManager runManager = new RunManager(mockRunTime.Object);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update1);

            Mock<IMyGridProgramRuntimeInfo> mockRunTimeMultiple = new Mock<IMyGridProgramRuntimeInfo>();
            mockRunTimeMultiple.SetupProperty(p => p.UpdateFrequency);

            RunManager runManagerMultiple = new RunManager(mockRunTimeMultiple.Object);
            runManagerMultiple.ScheduleRunFrequency(UpdateFrequency.Update1);
            runManagerMultiple.ScheduleRunFrequency(UpdateFrequency.Update10);
            runManagerMultiple.ScheduleRunFrequency(UpdateFrequency.Update100);


            //Act
            runManager.ApplySchedule();
            runManagerMultiple.ApplySchedule();

            //Assert
            Assert.AreEqual(UpdateFrequency.Update1, mockRunTime.Object.UpdateFrequency);

            Assert.IsTrue(mockRunTimeMultiple.Object.UpdateFrequency.HasFlag(UpdateFrequency.Update1));
            Assert.IsTrue(mockRunTimeMultiple.Object.UpdateFrequency.HasFlag(UpdateFrequency.Update10));
            Assert.IsTrue(mockRunTimeMultiple.Object.UpdateFrequency.HasFlag(UpdateFrequency.Update100));
            Assert.IsFalse(mockRunTimeMultiple.Object.UpdateFrequency.HasFlag(UpdateFrequency.Once));
        }

        /// <summary>
        /// Test <see cref="RunManager.ApplySchedule"/>,
        /// when the running is paused,
        /// then the runtime is set to <see cref="UpdateFrequency.None"/>,
        /// and the <see cref="RunManager.ScheduledFrequency"/> is not changed.
        /// </summary>
        [TestMethod]
        public void Test_ApplySchedule_Paused()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            mockRunTime.SetupProperty(p => p.UpdateFrequency);

            RunManager runManager = new RunManager(mockRunTime.Object)
            {
                Paused = true
            };

            runManager.ScheduleRunFrequency(UpdateFrequency.Update1);

            //Act
            runManager.ApplySchedule();

            //Assert
            Assert.AreEqual(UpdateFrequency.None, mockRunTime.Object.UpdateFrequency);
        }

        /// <summary>
        /// Test <see cref="RunManager.ApplySchedule"/>,
        /// when a frequence was sheduled, then the running was paused,
        /// then the runtime is set to <see cref="UpdateFrequency.None"/>,
        /// and the <see cref="RunManager.ScheduledFrequency"/> is not changed,
        /// and after enabling the running again, 
        /// then schedule continues in the last state.
        /// </summary>
        [TestMethod]
        public void Test_ApplySchedule_UnPause()
        {
            //Arrange
            Mock<IMyGridProgramRuntimeInfo> mockRunTime = new Mock<IMyGridProgramRuntimeInfo>();
            mockRunTime.SetupProperty(p => p.UpdateFrequency);

            RunManager runManager = new RunManager(mockRunTime.Object);

            //Act
            runManager.ScheduleRunFrequency(UpdateFrequency.Update1);
            runManager.Paused = true;
            runManager.ApplySchedule();
            runManager.ScheduleRunFrequency(UpdateFrequency.Update10);
            runManager.Paused = false;
            runManager.ApplySchedule();

            //Assert
            Assert.IsTrue(mockRunTime.Object.UpdateFrequency.HasFlag(UpdateFrequency.Update1));
            Assert.IsTrue(mockRunTime.Object.UpdateFrequency.HasFlag(UpdateFrequency.Update10));
        }
    }
}
