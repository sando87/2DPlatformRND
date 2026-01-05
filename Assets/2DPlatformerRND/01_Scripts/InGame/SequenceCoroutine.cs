using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    public class SequenceCoroutine
    {
        #region 내부 Step 정의

        abstract class Step
        {
            public abstract IEnumerator Execute();
        }

        class ActionStep : Step
        {
            Action action;

            public ActionStep(Action action)
            {
                this.action = action;
            }

            public override IEnumerator Execute()
            {
                action?.Invoke();
                yield break;
            }
        }

        class WaitStep : Step
        {
            float time;

            public WaitStep(float time)
            {
                this.time = time;
            }

            public override IEnumerator Execute()
            {
                yield return new WaitForSeconds(time);
            }
        }

        #endregion

        List<Step> steps = new List<Step>();
        Coroutine runningCoroutine;
        MonoBehaviour runner;

        bool loop;

        #region Public Fluent API

        public SequenceCoroutine Do(Action action)
        {
            steps.Add(new ActionStep(action));
            return this;
        }

        public SequenceCoroutine Wait(float time)
        {
            steps.Add(new WaitStep(time));
            return this;
        }

        public SequenceCoroutine DoLoop()
        {
            loop = true;
            return this;
        }

        public void Start(MonoBehaviour runner = null)
        {
            if (IsRunning())
                return;

            this.runner = runner ?? SequenceCoroutineRunner.Instance;
            runningCoroutine = this.runner.StartCoroutine(Run());
        }

        public void Stop()
        {
            if (runningCoroutine != null && runner != null)
            {
                runner.StopCoroutine(runningCoroutine);
            }

            runningCoroutine = null;
            runner = null;
        }

        public void ClearSequence()
        {
            Stop();

            steps.Clear();
            loop = false;
        }

        public bool IsRunning()
        {
            return runningCoroutine != null;
        }

        #endregion

        #region Coroutine Logic

        IEnumerator Run()
        {
            do
            {
                for (int i = 0; i < steps.Count; i++)
                {
                    yield return steps[i].Execute();
                }
            }
            while (loop);

            runningCoroutine = null;
        }

        #endregion
    }

    /// <summary>
    /// SequenceCoroutine가 MonoBehaviour 없이도 동작하게 해주는 글로벌 러너
    /// </summary>
    public class SequenceCoroutineRunner : MonoBehaviour
    {
        static SequenceCoroutineRunner instance;
        public static SequenceCoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("[SequenceCoroutineRunner]");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<SequenceCoroutineRunner>();
                }
                return instance;
            }
        }
    }
}
