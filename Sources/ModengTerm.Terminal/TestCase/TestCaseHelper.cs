using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Terminal.Session;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.TestCase
{
    public class TestCaseHelper
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TestCaseHelper");

        #endregion

        #region 实例变量

        private TestCase testCase;

        #endregion

        #region 属性

        public ShellSessionVM SessionVM { get; private set; }

        #endregion

        #region 公开接口

        public int Initialize(ShellSessionVM sessionVM)
        {
            this.SessionVM = sessionVM;

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 运行测试用例
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns>是否测试成功</returns>
        public int RunTestCase(TestCase testCase)
        {
            this.testCase = testCase;

            this.SessionVM.StatusChanged += SessionVM_StatusChanged;
            this.SessionVM.Open();

            return ResponseCode.SUCCESS;
        }

        #endregion

        #region 实例方法

        private void RunCaseSteps()
        {
            foreach (CaseStep caseStep in testCase.Steps)
            {
                if (!this.RunCaseStep(caseStep))
                {
                    logger.InfoFormat("CaseStep运行失败");
                    return;
                }
            }

            logger.InfoFormat("TestCase运行成功");
        }

        private bool RunCaseStep(CaseStep caseStep)
        {
            foreach (UserInput userInput in caseStep.UserInputs)
            {
                this.SessionVM.SendInput(userInput);
            }

            Thread.Sleep(2000); // 等ShellSessionVM处理完

            IVideoTerminal videoTerminal = this.SessionVM.VideoTerminal;
            VTDocument activeDocument = videoTerminal.ActiveDocument;

            #region 比对光标

            if (activeDocument.Cursor.Row != caseStep.CursorRow)
            {
                logger.ErrorFormat("");
                return false;
            }

            if (activeDocument.Cursor.Column != caseStep.CursorColumn)
            {
                logger.ErrorFormat("");
                return false;
            }

            #endregion

            #region 比对文本行

            VTextLine firstLine = activeDocument.FirstLine;
            VTextLine lastLine = activeDocument.LastLine;

            if (firstLine.PhysicsRow != caseStep.FirstLinePhysicsRow)
            {
                logger.ErrorFormat("");
                return false;
            }

            if (lastLine.PhysicsRow != caseStep.LastLinePhysicsRow)
            {
                logger.ErrorFormat("");
                return false;
            }

            foreach (CaseLine caseLine in caseStep.OutputLines)
            {
                VTextLine textLine = activeDocument.FindLine(caseLine.PhysicsRow);
                if (textLine == null)
                {
                    logger.ErrorFormat("");
                    return false;
                }

                if (!this.CompareLine(textLine, caseLine))
                {
                    logger.ErrorFormat("");
                    return false;
                }
            }

            #endregion

            return true;
        }

        private bool CompareLine(VTextLine textLine, CaseLine caseLine)
        {
            if (textLine.PhysicsRow != caseLine.PhysicsRow)
            {
                logger.ErrorFormat("");
                return false;
            }

            if (textLine.Characters.Count != caseLine.Characters.Count)
            {
                logger.ErrorFormat("");
                return false;
            }

            for (int i = 0; i < textLine.Characters.Count; i++)
            {
                VTCharacter character1 = textLine.Characters[i];
                VTCharacter character2 = caseLine.Characters[i];

                if (character1.Character != character2.Character)
                {
                    logger.ErrorFormat("");
                    return false;
                }

                if (character1.Attribute != character2.Attribute)
                {
                    logger.ErrorFormat("");
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region 事件处理器

        private void SessionVM_StatusChanged(OpenedSessionVM session, SessionStatusEnum status)
        {
            switch (status)
            {
                case SessionStatusEnum.Disconnected:
                    {
                        break;
                    }

                case SessionStatusEnum.Connected:
                    {
                        Task.Factory.StartNew(this.RunCaseSteps);
                        break;
                    }

                default:
                    break;
            }
        }

        #endregion
    }
}
