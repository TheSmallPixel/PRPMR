using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
using ScottPlot.Plottable;

namespace PatternFromRandom
{
    public class SymStats
    {
        private string Label { get; set; }
        public List<int> BetAmount { get; set; } = new();
        public List<int> WonAmount { get; set; } = new();
        public List<int> WalletStatus { get; set; } = new();
        public List<int> FailedSequence { get; set; } = new();
        private Random random;
        private ScottPlot.Plot plot = new(400, 300);
        private ScottPlot.Plot Moneyplot = new(400, 300);

        private SignalPlot p1pp, p2pp, p3pp, p4pp;
        private ScottPlot.FormsPlotViewer form, walletForm;

        public SymStats(string label, Random random)
        {
            Label = label;
            this.random = random;
            BetAmount.Add(0);
            WonAmount.Add(0);
            WalletStatus.Add(0);
            FailedSequence.Add(0);
        }
        protected void RegisterBet(int betAmount, int wonAmount, int walletStatus, int failedSequence)
        {
            BetAmount.Add(betAmount);
            WonAmount.Add(wonAmount);
            WalletStatus.Add(walletStatus);
            FailedSequence.Add(failedSequence);
        }

        public async Task RunPlots()
        {
            ScottPlot.FormHelp.CheckForIllegalCrossThreadCalls = false;
            form = new ScottPlot.FormsPlotViewer(plot);
            walletForm = new ScottPlot.FormsPlotViewer(Moneyplot);

            p1pp = plot.AddSignal(WonAmount.Select(x => (double)x).ToArray(), 1D, GerRandomColor(), label: $"{Label}-WonAmount");
            p2pp = plot.AddSignal(BetAmount.Select(x => (double)x).ToArray(), 1D, GerRandomColor(), label: $"{Label}-BetAmount");
            p3pp = Moneyplot.AddSignal(WalletStatus.Select(x => (double)x).ToArray(), 1D, GerRandomColor(), label: $"{Label}-WalletStatus");
            p4pp = plot.AddSignal(FailedSequence.Select(x => (double)x).ToArray(), 1D, GerRandomColor(), label: $"{Label}-FailedSequence");

            await Task.WhenAll(
                Task.Run(() => Application.Run(form)),
                Task.Run(() => Application.Run(walletForm)),
                Task.Run(() => UpdateValues(500)));
        }


        private void UpdatePP(SignalPlot plot, List<int> data)
        {
            plot.Ys = new double[data.Count];
            plot.MaxRenderIndex = data.Count - 1;
            plot.Update(data.Select(x => (double)x).ToArray());
        }

        public async Task UpdateValues(int delay)
        {
            while (true)
            {
                try
                {
                    if (p1pp == null || p2pp == null || p3pp == null || p4pp == null) return;
                    if (form == null || plot == null) return;
                    plot.RenderLock();
                    UpdatePP(p1pp, WonAmount);
                    UpdatePP(p2pp, BetAmount);  
                    UpdatePP(p4pp, FailedSequence);
                    plot.RenderUnlock();
                    Moneyplot.RenderLock();
                    UpdatePP(p3pp, WalletStatus);
                    Moneyplot.RenderUnlock();
                    plot.AxisAuto();
                    Moneyplot.AxisAuto();
                    form.formsPlot1.Refresh();
                    walletForm.formsPlot1.Refresh();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                await Task.Delay(delay);
            }
        }

        Color GerRandomColor()
        {
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
    }

}
