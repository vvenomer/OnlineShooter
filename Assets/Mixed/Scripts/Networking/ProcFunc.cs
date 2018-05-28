using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class ProcFunc
{

    public delegate void Proc();
    Proc proc;
    int delay;
    public ProcFunc(Proc proc, int delay = 0)
    {
        this.proc = proc;
        this.delay = delay;
        new Thread(Update).Start();
    }
    private void Update()
    {
        while (true)
        {
            proc();
            if(delay != 0)
                Thread.Sleep(delay);
        }
    }
}